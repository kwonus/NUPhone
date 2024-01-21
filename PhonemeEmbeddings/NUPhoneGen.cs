using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static PhonemeEmbeddings.Features;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PhonemeEmbeddings
{
    public class NUPhoneGen
    {
        public string Word { get; private set; }
        public string Phonetic { get; private set; }

        private byte[]? _Embeddings;
        public byte[] Embeddings
        {
            get
            {
                byte[] zero = new byte[0];
                if (this.Phonetic.Length == 0)
                {
                    return zero;
                }
                if (_Embeddings == null)
                { 
                    var len = Features.NUPhoneLen(this.Phonetic);
                    byte i;
                    byte[] features = new byte[len];
                    for (i = 0; i < len; i++)
                        features[i] = 0;
                    i = 0;
                    bool ored = false;
                    foreach (var c in this.Phonetic)
                    {
                        if (i >= len)   // fail-safety;
                        {
                            return zero;
                        }
                        switch (c)
                        {
                            case '{': ored = true; continue;
                            case '}': ored = false; i++; continue;
                            case '|': continue;
                            default: break;
                        }
                        if (Features.nuphone_inventory.ContainsKey(c))
                            features[i] |= Features.nuphone_inventory[c];
                        if (!ored)
                            i++;
                    }
                    this._Embeddings = features;
                }
                return this._Embeddings;
            }
        }

        public NUPhoneGen(string word, bool raw = false)
        {
            if (!raw)
            {
                this.Word = word.Trim().ToLower();
                this.Phonetic = this.Generate(word);
            }
            else
            {
                this.Word = string.Empty;
                this.Phonetic = word.Trim();
            }
        }
        private static byte[] SubBytes(byte[] input, int startIdx, int untilIdx)
        {
            if (untilIdx > startIdx)
            {
                var result = new byte[untilIdx - startIdx];
                for (int i = startIdx; i < untilIdx; i++)
                {
                    int idx = i - startIdx;
                    result[idx] = i < input.Length ? input[i] : (byte) 0;
                }
            }
            return new byte[0];
        }
        public UInt16? Compare(NUPhoneGen candidate, byte threshold) // 0 < threshold <= 100; returns between 0 and 10000 ... representring 0% to 100%
        {
            var self = this.Embeddings;
            var other = candidate.Embeddings;

            if (self.Length == 0 || other.Length == 0)
                return 0;

            if (threshold > 100)
                return null;

            int maxLen = self.Length > other.Length ? self.Length : other.Length;
            int minLen = self.Length < other.Length ? self.Length : other.Length;

            int diff = maxLen - minLen;
            int percent = ((maxLen-diff) * 100) / maxLen;

            return percent >= threshold ? Compare(candidate) : (UInt16?) null;  // null represents that comparison does not meet threshold for further comparison 
        }
        public UInt16 Compare(NUPhoneGen candidate) // returns between 0 and 10000 ... representring 0% to 100%
        {
            var self = this.Embeddings;
            var other = candidate.Embeddings;

            if (self.Length == 0 || other.Length == 0)
                return 0;

            int maxLen = self.Length > other.Length ? self.Length : other.Length;
            int minLen = self.Length < other.Length ? self.Length : other.Length;

            if (maxLen == 1)
            {
                return Compare(self[0], other[0]);
            }
            UInt64 score;
            UInt64 mean;
            if (minLen <= 3)
            {
                var first = Compare(self[0], other[0]);
                var bag = NUPhoneGen.BagCompare(self, other);

                score = bag.score + first;
                mean = score / (UInt64)(bag.cnt + 1);
                return (UInt16) mean;
            }
            if (minLen >= 4 && minLen <= 6)
            {
                var begin1 = NUPhoneGen.Compare(self[0], other[0]);
                var begin2 = NUPhoneGen.Compare(self[1], other[1]);

                var last2 = NUPhoneGen.Compare(self[self.Length-2], other[other.Length-2]);
                var last1 = NUPhoneGen.Compare(self[self.Length-1], other[other.Length-1]);

                var selfBag  = NUPhoneGen.SubBytes(self, 1, self.Length - 1);
                var otherBag = NUPhoneGen.SubBytes(other, 1, other.Length - 1);

                var bag = NUPhoneGen.BagCompare(self, other);

                score = bag.score + begin1 + begin2 + last2 + last1;
                mean = score / (UInt64)(bag.cnt + 4);
                return (UInt16)mean;
            }
            var begin = new UInt64[]
            {
                NUPhoneGen.Compare(self[0], other[0]),
                NUPhoneGen.Compare(self[1], other[1]),
                NUPhoneGen.Compare(self[2], other[2])
            };
            var end = new UInt64[]
            {
                NUPhoneGen.Compare(self[self.Length-3], other[other.Length-3]),
                NUPhoneGen.Compare(self[self.Length-2], other[other.Length-2]),
                NUPhoneGen.Compare(self[self.Length-1], other[other.Length-1])
            };
            var selfRemainer   = NUPhoneGen.SubBytes(self, 2, self.Length-2);
            var otherRemainder = NUPhoneGen.SubBytes(other, 2, other.Length-2);

            var remainder = NUPhoneGen.BagCompare(self, other);

            score = remainder.score;
            for (int i = 0; i < 3; i++)
            {
                score += begin[i];
                score += end[i];
            }
            mean = score / (UInt64)(remainder.cnt + 6);
            return (UInt16)mean;
        }
        public static (UInt64 score, byte cnt) BagCompare(byte[] a, byte[] b) // returns between 0 and 10000 ... representring 0% to 100%
        {
            var maxLen = a.Length > b.Length ? a.Length : b.Length;
            var minLen = a.Length < b.Length ? a.Length : b.Length;

            if (maxLen > byte.MaxValue)
                return (0, byte.MaxValue);

            if (a.Length == 0 || b.Length == 0)
                return (0, (byte) maxLen);

            var scores = new Dictionary<byte, Dictionary<byte, UInt16>>();

            for (int i = 0; i < a.Length; i++)
            {
                if (scores.ContainsKey(a[i]))
                    continue;

                var candidates = new Dictionary<byte, UInt16>();
                for (int j = 0; j < b.Length; j++)
                {
                    if (candidates.ContainsKey(b[j]))
                        continue;

                    var score = NUPhoneGen.Compare(a[i], b[j]);
                    candidates[b[j]] = score;
                }
                scores[a[i]] = candidates;
            }
            UInt64 total = 0;
            foreach (var key in scores.Keys)
            {
                var checks = scores[key];
                UInt64 maxScore = 0;
                foreach (UInt16 test in checks.Values)
                {
                    if (test > maxScore)
                        maxScore = test;
                }
                total += maxScore;
            }
            return (total, (byte)maxLen);
        }
        private static UInt16 Compare(byte a, byte b) // returns between 0 and 10000 ... representring 0% to 100%
        {
            var va = Features.IsVowel(a);
            var vb = Features.IsVowel(b);

            if (va != vb)
                return 0;

            if (va)
                return CompareVowel(a, b);
            else
                return CompareConsonant(a, b);
        }
        private static UInt16 CompareVowel(byte a, byte b) // returns between 0 and 10000 ... representring 0% to 100%
        {
            var xa = Features.GetXaxis(a);
            var xb = Features.GetXaxis(b);
            var xdiff = (xa > xb) ? xa - xb : xb - xa; // absolute value

            var ya = Features.GetYaxis(a);
            var yb = Features.GetYaxis(b);
            var ydiff = (ya > yb) ? ya - yb : yb - ya; // absolute value

            var diff = xdiff + ydiff;

            switch (diff)
            {
                case 0:  return 10000;
                case 1:  return  9900;
                case 2:  return  9700;
                case 3:  return  9300;
                default: return  8500;
            }
        }
        private static UInt16 CompareConsonant(byte a, byte b) // returns between 0 and 10000 ... representring 0% to 100%
        {
            var xa = Features.GetXaxis(a);
            var xb = Features.GetXaxis(b);
            var xdiff = (xa > xb) ? xa - xb : xb - xa; // absolute value

            var ya = Features.GetYaxis(a);
            var yb = Features.GetYaxis(b);
            var ydiff = (ya > yb) ? ya - yb : yb - ya; // absolute value

            var diff = xdiff + ydiff;

            UInt16 penalty = Features.IsVoicedConsonant(a) != Features.IsVoicedConsonant(b) ? (UInt16)1000: (UInt16)0;

            switch (diff)
            {
                case 0:  return (UInt16) (10000 - penalty);
                case 1:  return (UInt16) ( 9800 - penalty);
                case 2:  return (UInt16) ( 9400 - penalty);
                case 3:  return (UInt16) ( 8600 - penalty);
                case 4:  return (UInt16) ( 7000 - penalty);
                default: return (UInt16) ( 3800 - penalty);
            }
        }
        private static string GetShortestVariant(List<NUPhoneRecord> variants)
        {
            string? variant = null;

            foreach (var candidate in variants)
            {
                var normalized = Features.Instance.NormalizeIntoNUPhone(candidate.nuphone);
                if (variant == null || variant.Length > normalized.Length)
                    variant = normalized;
            }
            return variant != null ? variant : string.Empty;
        }
        private enum Position
        {
            Left,
            Right
        };
        private static Dictionary<byte, Dictionary<string, List<NUPhoneRecord>>>[] graphemeFirst = new Dictionary<byte, Dictionary<string, List<NUPhoneRecord>>>[]
        {
            Features.Instance.nuphone_grapheme_lookup,
            Features.Instance.nuphone_lexicon_lookup
        };
        private static Dictionary<byte, Dictionary<string, List<NUPhoneRecord>>>[] lexiconFirst = new Dictionary<byte, Dictionary<string, List<NUPhoneRecord>>>[]
        {
            Features.Instance.nuphone_lexicon_lookup,
            Features.Instance.nuphone_grapheme_lookup
        };
        private string Generate(string segment)
        {
            Dictionary<string, List<NUPhoneRecord>> table;

            var len = (byte)segment.Length;

            foreach (var lookup in graphemeFirst)
            {
                if (lookup.ContainsKey(len))
                {
                    table = lookup[len];

                    if (table.ContainsKey(segment))
                    {
                        List<NUPhoneRecord> records = table[segment];
                        return GetShortestVariant(records);
                    }
                }
            }
            var builder = new StringBuilder(segment.Length+3);

            var lookups = (len > 2) ? lexiconFirst : graphemeFirst;
            foreach (var lookup in lookups)
            {
                for (byte i = 1; i < len; i ++)
                {
                    byte sublen = (byte)(len - i);

                    if (lookup.ContainsKey(sublen))
                    {
                        table = lookup[sublen];
                        var left = segment.Substring(0, sublen);
                        var right = segment.Substring(sublen);

                        if (table.ContainsKey(left))
                        {
                            List<NUPhoneRecord> records = table[left];
                            var shortest = GetShortestVariant(records);
                            return shortest + this.Generate(right);
                        }
                        else if (table.ContainsKey(right))
                        {
                            List<NUPhoneRecord> records = table[right];
                            var shortest = GetShortestVariant(records);
                            return this.Generate(left) + shortest;
                        }
                    }
                }
            }
            return Features.Instance.NormalizeIntoNUPhone(segment);
        }
    }
}
