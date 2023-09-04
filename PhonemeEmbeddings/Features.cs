namespace PhonemeEmbeddings
{
    using Microsoft.VisualBasic;
    using System.Collections.Generic;
    using System.Text;

    public abstract class Features
    {
        public static sbyte GetXaxis(byte embeddings)
        {
            byte bits = (byte)((PHONEME_FEATURES_X_AXIS & embeddings) >> PHONEME_FEATURES_X_AXIS_BIT_SHIFT);

            sbyte sbits = (sbyte)(bits & TWO_BITS);

            if (sbits != 0 && bits > TWO_BITS)
                sbits *= (-1);

            return sbits;
        }
        public static sbyte GetYaxis(byte embeddings)
        {
            byte bits = (byte)(PHONEME_FEATURES_X_AXIS & embeddings);

            sbyte sbits = (sbyte)(bits & TWO_BITS);

            if (sbits != 0 && bits > TWO_BITS)
                sbits *= (-1);

            return sbits;
        }
        public static bool IsVowel(byte embeddings)
        {
            return (PHONEME_TYPE_MASK & embeddings) == PHONEME_TYPE_VOWEL;
        }
        public static bool IsConsonant(byte embeddings)
        {
            var type = (PHONEME_TYPE_MASK & embeddings);

            return (type == PHONEME_TYPE_CONSONANT_VOICED) || (type == PHONEME_TYPE_CONSONANT_VOICELESS);
        }
        public static bool IsVoicedConsonant(byte embeddings)
        {
            var type = (PHONEME_TYPE_MASK & embeddings);

            return (type == PHONEME_TYPE_CONSONANT_VOICELESS);
        }
        // PHONEME TYPES:
        //
        public const byte PHONEME_TYPE_MASK = 0xC0;
        public const byte PHONEME_TYPE_VOWEL = 0x00;
        public const byte PHONEME_TYPE_CONSONANT = 0x80;
        public const byte PHONEME_TYPE_CONSONANT_VOICING_BIT = 0x40;
        public const byte PHONEME_TYPE_CONSONANT_VOICELESS = PHONEME_TYPE_CONSONANT;
        public const byte PHONEME_TYPE_CONSONANT_VOICED = PHONEME_TYPE_CONSONANT | PHONEME_TYPE_CONSONANT_VOICING_BIT;

        public const byte PHONEME_FEATURES = 0x3F;
        public const byte PHONEME_FEATURES_X_AXIS_BIT_SHIFT = 3;
        public const byte PHONEME_FEATURES_X_AXIS = 0x38; // 0x7 << PHONEME_FEATURES_X_AXIS_BIT_SHIFT
        public const byte PHONEME_FEATURES_Y_AXIS = 0x07;

        public const byte THREE_BIT_NUM_NEGATIVE_BIT = 0x4;
        public const byte TWO_BITS = 0x3; // When combined with negative bit, range is -3, -2, -1, 0, 1, 2, 3

        // VOWEL FEATURES: CENTRALITY
        //
        public const sbyte VOWEL_FRONT  = -1;
        public const sbyte VOWEL_CENTER =  0;
        public const sbyte VOWEL_BACK   =  1;

        public const byte VOWEL_CENTRALITY = PHONEME_FEATURES_X_AXIS;
        public const byte VOWEL_CENTRALITY_FRONT  = 0x5 << 3; // -1 (0x5)
        public const byte VOWEL_CENTRALITY_CENTER = 0x4 << 3; // -0 (0x4)
        public const byte VOWEL_CENTRALITY_BACK   = 0x1 << 3; //  1 (0x1)

        public static sbyte? GetVowelCentrality(byte embeddings)
        {
            return Features.IsVowel(embeddings) ? GetXaxis(embeddings) : null;
        }

        // VOWEL FEATURES: HEIGHT
        //
        public const sbyte VOWEL_CLOSE     =  2;
        public const sbyte VOWEL_CLOSE_MID =  1;
        public const sbyte VOWEL_MID       =  0;
        public const sbyte VOWEL_OPEN_MID  = -1;
        public const sbyte VOWEL_OPEN      = -2;

        public const byte VOWEL_HEIGHT           = PHONEME_FEATURES_Y_AXIS;
        public const byte VOWEL_HEIGHT_CLOSE     = 0x2; //  2
        public const byte VOWEL_HEIGHT_CLOSE_MID = 0x1; //  1
        public const byte VOWEL_HEIGHT_MIDDLE    = 0x4; // -0
        public const byte VOWEL_HEIGHT_OPEN_MID  = 0x5; // -1
        public const byte VOWEL_HEIGHT_OPEN      = 0x6; // -2

        public static sbyte? GetVowelHeight(byte embeddings)
        {
            return Features.IsVowel(embeddings) ? GetYaxis(embeddings) : null;
        }

        // CONSONANT FEATURES ON Y-AXIS
        //

        // CONSONANT FEATURES: PLACE OF ARTICULATION
        //
        public const byte CONSONANT_PLACE = PHONEME_FEATURES_X_AXIS;

        public const sbyte CONSONANT_BILABIAL    = -3;
        public const sbyte CONSONANT_LABIODENTAL = -2;
        public const sbyte CONSONANT_DENTAL      = -1;
        public const sbyte CONSONANT_ALVEOLAR    =  0;
        public const sbyte CONSONANT_PALATAL     =  1;
        public const sbyte CONSONANT_VELAR       =  2;
        public const sbyte CONSONANT_GLOTTAL     =  3;

        public const byte CONSONANT_PLACE_BILABIAL    = 0x7 << 3; // -3
        public const byte CONSONANT_PLACE_LABIODENTAL = 0x6 << 3; // -2
        public const byte CONSONANT_PLACE_DENTAL      = 0x5 << 3; // -1
        public const byte CONSONANT_PLACE_ALVEOLAR    = 0x4 << 3; // -0
        public const byte CONSONANT_PLACE_PALATAL     = 0x1 << 3; //  1
        public const byte CONSONANT_PLACE_VELAR       = 0x2 << 3; //  2
        public const byte CONSONANT_PLACE_GLOTTAL     = 0x3 << 3; //  3

        // (ignored in English NUPhone)
        //
        public const sbyte CONSONANT_POSTALVEOLAR = CONSONANT_PALATAL;
        public const sbyte CONSONANT_RETROFLEX    = CONSONANT_PALATAL;
        public const sbyte CONSONANT_UVULAR       = CONSONANT_VELAR;
        public const sbyte CONSONANT_PHARYNGEAL   = CONSONANT_GLOTTAL;

        public const byte CONSONANT_PLACE_POSTALVEOLAR = CONSONANT_PLACE_PALATAL;
        public const byte CONSONANT_PLACE_RETROFLEX    = CONSONANT_PLACE_PALATAL;
        public const byte CONSONANT_PLACE_UVULAR       = CONSONANT_PLACE_VELAR;
        public const byte CONSONANT_PLACE_PHARYNGEAL   = CONSONANT_PLACE_GLOTTAL;
        //
        public static sbyte? GetConsonantPlace(byte embeddings)
        {
            return Features.IsConsonant(embeddings) ? GetXaxis(embeddings) : null;
        }

        // CONSONANT FEATURES: MANOR OF ARTICULATION
        //
        public const byte CONSONANT_MANOR = PHONEME_FEATURES_Y_AXIS;

        public const sbyte CONSONANT_PLOSIVE   =  2;
        public const sbyte CONSONANT_NASAL     =  1;
        public const sbyte CONSONANT_FRICATIVE =  0;
        public const sbyte CONSONANT_GLIDE     = -1;
        public const sbyte CONSONANT_LIQUID    = -2;

        public const byte CONSONANT_MANOR_PLOSIVE   = 0x2; //  2
        public const byte CONSONANT_MANOR_NASAL     = 0x1; //  1
        public const byte CONSONANT_MANOR_FRICATIVE = 0x4; // -0
        public const byte CONSONANT_MANOR_GLIDE     = 0x5; // -1
        public const byte CONSONANT_MANOR_LIQUID    = 0x6; // -2

        // (other manors of articulation are ignored in English NUPhone)
        //
        public const sbyte CONSONANT_TAP_OR_FLAP = CONSONANT_FRICATIVE;
        public const sbyte CONSONANT_TRILL       = CONSONANT_FRICATIVE;
        public const sbyte CONSONANT_AFFRICATE   = CONSONANT_FRICATIVE;

        public const byte CONSONANT_MANOR_TAP_OR_FLAP = CONSONANT_MANOR_FRICATIVE;
        public const byte CONSONANT_MANOR_TRILL       = CONSONANT_MANOR_FRICATIVE;
        public const byte CONSONANT_MANOR_AFFRICATE   = CONSONANT_MANOR_FRICATIVE;

        public static sbyte? GetConsonantManor(byte embeddings)
        {
            return Features.IsConsonant(embeddings) ? GetYaxis(embeddings) : null;
        }

        public static readonly Dictionary<char, byte> nuphone_inventory = new()
        {
            // consonants ...
            { 'p', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_BILABIAL   | CONSONANT_MANOR_PLOSIVE     },
            { 'b', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_BILABIAL   | CONSONANT_MANOR_PLOSIVE     | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 't', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_ALVEOLAR   | CONSONANT_MANOR_PLOSIVE     },
            { 'd', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_ALVEOLAR   | CONSONANT_MANOR_PLOSIVE     | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 'ʈ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_RETROFLEX  | CONSONANT_MANOR_PLOSIVE     },
            { 'ɖ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_RETROFLEX  | CONSONANT_MANOR_PLOSIVE     | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 'c', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_PALATAL    | CONSONANT_MANOR_PLOSIVE     },
            { 'ɟ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_PALATAL    | CONSONANT_MANOR_PLOSIVE     | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 'k', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_VELAR      | CONSONANT_MANOR_PLOSIVE     },
            { 'ɡ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_VELAR      | CONSONANT_MANOR_PLOSIVE     | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 'q', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_UVULAR     | CONSONANT_MANOR_PLOSIVE     },
            { 'ɢ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_UVULAR     | CONSONANT_MANOR_PLOSIVE     | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 'ʔ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_GLOTTAL    | CONSONANT_MANOR_PLOSIVE     },
            { 'm', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_BILABIAL   | CONSONANT_MANOR_NASAL       },
            { 'ɱ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_LABIODENTAL| CONSONANT_MANOR_NASAL       | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 'n', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_ALVEOLAR   | CONSONANT_MANOR_NASAL       | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 'ɳ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_RETROFLEX  | CONSONANT_MANOR_NASAL       | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 'ɲ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_PALATAL    | CONSONANT_MANOR_NASAL       | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 'ŋ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_VELAR      | CONSONANT_MANOR_NASAL       | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 'ɴ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_UVULAR     | CONSONANT_MANOR_NASAL       | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 'ʙ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_BILABIAL   | CONSONANT_MANOR_TRILL       | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 'r', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_ALVEOLAR   | CONSONANT_MANOR_TRILL       | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 'ʀ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_UVULAR     | CONSONANT_MANOR_TRILL       | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { '?', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_LABIODENTAL| CONSONANT_MANOR_TAP_OR_FLAP | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 'ɾ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_ALVEOLAR   | CONSONANT_MANOR_TAP_OR_FLAP | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 'ɽ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_RETROFLEX  | CONSONANT_MANOR_TAP_OR_FLAP | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 'ɸ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_BILABIAL   | CONSONANT_MANOR_FRICATIVE   },
            { 'β', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_BILABIAL   | CONSONANT_MANOR_FRICATIVE   | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 'f', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_LABIODENTAL| CONSONANT_MANOR_FRICATIVE   },
            { 'v', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_LABIODENTAL| CONSONANT_MANOR_FRICATIVE   | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 'θ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_DENTAL     | CONSONANT_MANOR_FRICATIVE   },
            { 'ð', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_DENTAL     | CONSONANT_MANOR_FRICATIVE   | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 's', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_ALVEOLAR   | CONSONANT_MANOR_FRICATIVE   },
            { 'z', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_ALVEOLAR   | CONSONANT_MANOR_FRICATIVE   | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 'ʃ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_POSTALVEOLAR|CONSONANT_MANOR_FRICATIVE   },
            { 'ʒ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_POSTALVEOLAR|CONSONANT_MANOR_FRICATIVE   | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 'ʂ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_RETROFLEX  | CONSONANT_MANOR_FRICATIVE   },
            { 'ʐ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_RETROFLEX  | CONSONANT_MANOR_FRICATIVE   | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 'ç', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_PALATAL    | CONSONANT_MANOR_FRICATIVE   },
            { 'ʝ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_PALATAL    | CONSONANT_MANOR_FRICATIVE   | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 'x', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_VELAR      | CONSONANT_MANOR_FRICATIVE   },
            { 'ɣ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_VELAR      | CONSONANT_MANOR_FRICATIVE   | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 'χ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_UVULAR     | CONSONANT_MANOR_FRICATIVE   },
            { 'ʁ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_UVULAR     | CONSONANT_MANOR_FRICATIVE   | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 'ħ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_PHARYNGEAL | CONSONANT_MANOR_FRICATIVE   },
            { 'ʕ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_PHARYNGEAL | CONSONANT_MANOR_FRICATIVE   | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 'h', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_GLOTTAL    | CONSONANT_MANOR_FRICATIVE   },
            { 'ɦ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_GLOTTAL    | CONSONANT_MANOR_FRICATIVE   | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 'ɬ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_ALVEOLAR   | CONSONANT_MANOR_AFFRICATE   },
            { 'ɮ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_ALVEOLAR   | CONSONANT_MANOR_AFFRICATE   | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 'ʋ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_LABIODENTAL| CONSONANT_MANOR_GLIDE       | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 'ɹ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_ALVEOLAR   | CONSONANT_MANOR_GLIDE       | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 'ɻ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_RETROFLEX  | CONSONANT_MANOR_GLIDE       | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 'j', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_PALATAL    | CONSONANT_MANOR_GLIDE       | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 'ɰ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_VELAR      | CONSONANT_MANOR_GLIDE       | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 'l', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_ALVEOLAR   | CONSONANT_MANOR_LIQUID      | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 'ɭ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_RETROFLEX  | CONSONANT_MANOR_LIQUID      | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 'ʎ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_PALATAL    | CONSONANT_MANOR_LIQUID      | PHONEME_TYPE_CONSONANT_VOICING_BIT },
            { 'ʟ', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_VELAR      | CONSONANT_MANOR_LIQUID      | PHONEME_TYPE_CONSONANT_VOICING_BIT },
/*added*/   { 'w', PHONEME_TYPE_CONSONANT | CONSONANT_PLACE_VELAR      | CONSONANT_MANOR_LIQUID      | PHONEME_TYPE_CONSONANT_VOICING_BIT }, // should also be bilabial, but this mechanism does not allow two places of articulation
            // vowels ...
            { 'i', PHONEME_TYPE_VOWEL | VOWEL_CENTRALITY_FRONT  | VOWEL_HEIGHT_CLOSE      },
            { 'y', PHONEME_TYPE_VOWEL | VOWEL_CENTRALITY_FRONT  | VOWEL_HEIGHT_CLOSE      },
            { 'ɨ', PHONEME_TYPE_VOWEL | VOWEL_CENTRALITY_CENTER | VOWEL_HEIGHT_CLOSE      },
            { 'ʉ', PHONEME_TYPE_VOWEL | VOWEL_CENTRALITY_CENTER | VOWEL_HEIGHT_CLOSE      },
            { 'ɯ', PHONEME_TYPE_VOWEL | VOWEL_CENTRALITY_BACK   | VOWEL_HEIGHT_CLOSE      },
            { 'u', PHONEME_TYPE_VOWEL | VOWEL_CENTRALITY_BACK   | VOWEL_HEIGHT_CLOSE      },
            { 'ɪ', PHONEME_TYPE_VOWEL | VOWEL_CENTRALITY_FRONT  | VOWEL_HEIGHT_CLOSE_MID  },
            { 'ʏ', PHONEME_TYPE_VOWEL | VOWEL_CENTRALITY_FRONT  | VOWEL_HEIGHT_CLOSE_MID  },
            { 'ʊ', PHONEME_TYPE_VOWEL | VOWEL_CENTRALITY_BACK   | VOWEL_HEIGHT_CLOSE_MID  },
            { 'e', PHONEME_TYPE_VOWEL | VOWEL_CENTRALITY_FRONT  | VOWEL_HEIGHT_CLOSE_MID  },
            { 'ø', PHONEME_TYPE_VOWEL | VOWEL_CENTRALITY_FRONT  | VOWEL_HEIGHT_CLOSE_MID  },
            { 'ɘ', PHONEME_TYPE_VOWEL | VOWEL_CENTRALITY_CENTER | VOWEL_HEIGHT_CLOSE_MID  },
            { 'ɵ', PHONEME_TYPE_VOWEL | VOWEL_CENTRALITY_CENTER | VOWEL_HEIGHT_CLOSE_MID  },
            { 'ɤ', PHONEME_TYPE_VOWEL | VOWEL_CENTRALITY_BACK   | VOWEL_HEIGHT_CLOSE_MID  },
            { 'o', PHONEME_TYPE_VOWEL | VOWEL_CENTRALITY_BACK   | VOWEL_HEIGHT_CLOSE_MID  },
            { 'ə', PHONEME_TYPE_VOWEL | VOWEL_CENTRALITY_CENTER | VOWEL_HEIGHT_MIDDLE     },
            { 'ɛ', PHONEME_TYPE_VOWEL | VOWEL_CENTRALITY_FRONT  | VOWEL_HEIGHT_OPEN_MID   },
            { 'œ', PHONEME_TYPE_VOWEL | VOWEL_CENTRALITY_FRONT  | VOWEL_HEIGHT_OPEN_MID   },
            { 'ɜ', PHONEME_TYPE_VOWEL | VOWEL_CENTRALITY_CENTER | VOWEL_HEIGHT_OPEN_MID   },
            { 'ɞ', PHONEME_TYPE_VOWEL | VOWEL_CENTRALITY_CENTER | VOWEL_HEIGHT_OPEN_MID   },
            { 'ʌ', PHONEME_TYPE_VOWEL | VOWEL_CENTRALITY_BACK   | VOWEL_HEIGHT_OPEN_MID   },
            { 'ɔ', PHONEME_TYPE_VOWEL | VOWEL_CENTRALITY_BACK   | VOWEL_HEIGHT_OPEN_MID   },
            { 'æ', PHONEME_TYPE_VOWEL | VOWEL_CENTRALITY_FRONT  | VOWEL_HEIGHT_OPEN_MID   },
            { 'ɐ', PHONEME_TYPE_VOWEL | VOWEL_CENTRALITY_CENTER | VOWEL_HEIGHT_OPEN_MID   },
            { 'a', PHONEME_TYPE_VOWEL | VOWEL_CENTRALITY_FRONT  | VOWEL_HEIGHT_OPEN       },
            { 'ɶ', PHONEME_TYPE_VOWEL | VOWEL_CENTRALITY_FRONT  | VOWEL_HEIGHT_OPEN       },
            { 'ɑ', PHONEME_TYPE_VOWEL | VOWEL_CENTRALITY_BACK   | VOWEL_HEIGHT_OPEN       },
            { 'ɒ', PHONEME_TYPE_VOWEL | VOWEL_CENTRALITY_BACK   | VOWEL_HEIGHT_OPEN       }
        };
        // this hashmap comes from:
        // https://www.dyslexia-reading-well.com/44-phonemes-in-english.html
        // With minor edits herein and an added field for nuphone
        //
        // It is used to generate nuphone for OOV words from other lookup data (it is refashioned here in ortho-centric hashmaps)
        //
        private static readonly Dictionary<string, (string nuphone, string[] orthography)> nuphone_primatives = new()
        {
            { "b",   ("b",      new string[] { "b", "bb" }) },                          //   bug, bubble
            { "d",   ("d",      new string[] { "d", "dd", "ed" }) },                    //   dad, add, milled
            { "f",   ("f",      new string[] { "f", "ff", "ph", "gh", "lf", "ft" }) },  //   fat, cliff, phone, enough, half, often
            { "g",   ("g",      new string[] { "g", "gg", "gh", "gu", "gue" }) },       //   gun, egg, ghost, guest, prologue
            { "h",   ("h",      new string[] { "h", "wh" }) },                          //   hop, who  
            { "dʒ",  ("d|ʒ",    new string[] { "j", "ge", "g", "dge", "di", "gg" }) },  //   jam, wage, giraffe, edge, soldier, exaggerate
            { "k",   ("k",      new string[] { "k", "c", "ch", "cc", /*"lk", "qu",*/ "q", "ck", /*"x"*/ }) },   //   kit, cat, chris, accent, folk, bouquet, queen, rack, box  
            { "l",   ("l",      new string[] { "l", "ll" }) },                          //   live, well
            { "m",   ("m",      new string[] { "m", "mm", "mb", "mn", "lm" }) },        //   man, summer, comb, column, palm
            { "n",   ("n",      new string[] { "n", "nn", "kn", "gn", "pn", "mn" }) },  //   net, funny, know, gnat, pneumonic, mnemonic
            { "p",   ("p",      new string[] { "p", "pp" }) },                          //   pin, dippy
            { "r",   ("r",      new string[] { "r", /*"rr",*/ "wr", "rh" }) },          //   run, carrot, wrench, rhyme
            { "s",   ("s",      new string[] { "s", "ss", "c", "sc", "ps", "st", "ce", "se" }) },   //   sit, less, circle, scene, psycho, listen, pace, course
            { "t",   ("t",      new string[] { "t", /*"tt",*/ "th", "ed" }) },          //   tip, matter, thomas, ripped
            { "v",   ("v",      new string[] { "v", "f", "ph", "ve" }) },               //   vine, of, stephen, five
            { "w",   ("w",      new string[] { "w", "wh", "u", "o" }) },                //   wit, why, quick, choir
            { "z",   ("z",      new string[] { "z", "zz", "s", "ss", "x", "ze", "se" }) },   //   zed, buzz, his, scissors, xylophone, craze
            { "ʒ",   ("ʒ",      new string[] { "s", "si", "z" }) },                     //   treasure, division, azure
            { "tʃ",  ("t|ʃ",    new string[] { "ch", "tch", "tu", "te" }) },            //   chip, watch, future, righteous
            { "ʃ",   ("ʃ",      new string[] { "sh", "ce", "s", "ci", "si", "ch", "sci", "ti" }) },   //   sham, ocean, sure, special, pension, machine, conscience, station 
            { "θ",   ("θ",      new string[] { "th" }) },                               //   thongs
            { "ð",   ("ð",      new string[] { "th" }) },                               //   leather
            { "ŋ",   ("ŋ",      new string[] { "ng", "n", "ngue" }) },                  //   ring, pink, tongue
            { "j",   ("j",      new string[] { "y", "i", "j" }) },                      //   you, onion, hallelujah
            { "æ",   ("æ",      new string[] { "a", "ai", "au" }) },                    //   cat, plaid, laugh
            { "eɪ",  ("e|ɪ",    new string[] { "a", "ai", "eigh", "aigh", "ay", "er", "et", "ei", "au", "ae", "ea", "ey" }) },  //   bay, maid, weigh, straight, pay, foyer, filet, eight, gauge, mate, break, they
            { "ɛ",   ("ɛ",      new string[] { "e", "ea", "u", "ie", "ai", "a", "eo", "ei", "ae" }) },              //   end, bread, bury, friend, said, many, leopard, heifer, aesthetic
            { "i:",  ("i",      new string[] { "e", "ee", "ea", "y", "ey", "oe", "ie", "i", "ei", "eo", "ay" }) },  //   be, bee, meat, lady, key, phoenix, grief, ski, deceive, people, quay
            { "ɪ",   ("ɪ",      new string[] { "i", "e", "o", "u", "ui", "y", "ie" }) },//   it, england, women, busy, guild, gym, sieve
            { "aɪ",  ("a|ɪ",    new string[] { "i", "y", "igh", "ie", "uy", "ye", "ai", "is", "eigh", "ie" }) },    //   spider, sky, night, pie, guy, stye, aisle, island, height, kite
            { "ɒ",   ("ɒ",      new string[] { "a", "ho", "au", "aw", "ough" }) },      //   swan, honest, maul, slaw, fought
            { "oʊ",  ("o|ʊ",    new string[] { "o", "oa", "oe", "oe", "ow", "ough", "eau", "oo", "ew" }) },        //   open, moat, bone, toe, sow, dough, beau, brooch, sew
            { "ʊ",   ("ʊ",      new string[] { "o", "oo", "u", "ou" }) },               //   wolf, look, bush, would
            { "ʌ",   ("ʌ",      new string[] { "u", "o", "oo", "ou" }) },               //   lug, monkey, blood, double
            { "u:",  ("u",      new string[] { "o", "oo", "ew", "ue", "ue", "oe", "ough", "ui", "oew", "ou" }) },   //   who, loon, dew, blue, flute, shoe, through, fruit, manoeuvre, group
            { "ɔɪ",  ("ɔ|ɪ",    new string[] { "oi", "oy", "uoy" }) },                  //   join, boy, buoy
            { "aʊ",  ("a|ʊ",    new string[] { "ow", "ou", "ough" }) },                 //   now, shout, bough
            { "ə",   ("ə",      new string[] { "a", "er", "i", "ar", "our", "ur" }) },  //   about, ladder, pencil, dollar, honour, augur
            { "eəʳ", ("{e|ə}r", new string[] { "air", "are", "ear", "ere", "eir", "ayer" }) },   //   chair, dare, pear, where, their, prayer
            { "ɑ:",  ("x",      new string[] { "a" }) },                                //   arm
            { "ɜ:ʳ", ("ɜr",     new string[] { "ir", "er", "ur", "ear", "or", "our", "yr" }) },  //   bird, term, burn, pearl, word, journey, myrtle
            { "ɔ:",  ("ɔ",      new string[] { "aw", "a", "or", "oor", "ore", "oar", "our", "augh", "ar", "ough", "au" }) },   //   paw, ball, fork, poor, fore, board, four, taught, war, bought, sauce
            { "ɪəʳ", ("{ɪ|ə}r", new string[] { "ear", "eer", "ere", "ier" }) },         //   ear, steer, here, tier
            { "ʊəʳ", ("{ʊ|ə}r", new string[] { "ure", "our" }) }                        //   cure, tourist
        };

        private static readonly Dictionary<string, (string nuphone, string actual)> nuphone_additional_graphemes = new()
        {
            { "à",  ("ɑ",       "ɑ:") },
            { "aa", ("ɑ",       "ɑ:") },
            { "bb", ("b",       "b" ) },
            { "cc", ("k",       "k" ) },
            { "dd", ("d",       "d" ) },
            { "ee", ("i",       "i:") },
//          { "ff", ("",         "" ) },
            { "gg", ("d|ʒ",     "dʒ") },
            { "hh", ("h",       "h" ) },
            { "ii", ("i{i|ɪ}",  "i:{i|ɪ}")},    // i.e. Skiing or skier
            { "jj", ("d|ʒ",     "dʒ") },
            { "kk", ("k",       "k" ) },
            { "lk", ("{lk|_k}", "{lk|k}" ) },
//          { "ll", ("",         "" ) },
//          { "mm", ("",         "" ) },
//          { "nn", ("",         "" ) },
            { "oo", ("{o|ʊ|ʌ|u}","{oʊ|ʊ|ʌ|u:}") },
//          { "pp", ("",         "" ) },
            { "q",  ("k",        "k") },
            { "qu", ("{kw|k}",   "{kw|k}") },
            { "rr", ("{ɹ|r}",    "{ɹ|r}") },
//          { "ss", ("",         "" ) },
            { "tt", ("{d|t}",    "{d|t}") },
            { "uu", ("{u|ʊ}",    "{u|ʊ}") },
            { "vv", ("v",        "v") },
            { "ww", ("ʊw",       "ʊw") },
            { "y",  ("j",         "j") },
//          { "zz", ("",         "" ) },
            { "xx", ("ks",       "ks")},
            { "x",  ("ks",       "ks")}
        };
        public struct NUPhoneRecord
        {
            public string nuphone;
            public string actual;
        }
        public static readonly Dictionary<byte, Dictionary<string, List<NUPhoneRecord>>> nuphone_grapheme_lookup;
        public static readonly Dictionary<byte, Dictionary<string, List<NUPhoneRecord>>> nuphone_lexicon_lookup;

        public static (string nuphone, string ipa) NormalizeIntoNUPhone(string[] variants)
        {
            // For now, we just pick the first one; Make a true nuphone representation later
            //
            foreach (var variant in variants)
            {
                var normalized = NormalizeIntoNUPhone(variant);
                if (normalized.Length > 0)
                {
                    return (normalized, variant.Trim());
                }
            }
            return (string.Empty, string.Empty);
        }
        public static HashSet<char> Removals = new();
        public static string NormalizeIntoNUPhone(string variant)
        {
            var trimmed = variant.Trim();   // this should be a NOOP, but just to be safe

            if (trimmed.Length > 0)
            {
                var result = new StringBuilder(trimmed.Length);
                var bruteforce = variant
                    .Replace('ɫ', 'ɭ')
                    .Replace('ɫ', 'l')
                    .Replace('ɝ', 'ɜ');
                /*
                if (variant == "həˈɫoʊ")
                {
                    ;
                }
                var normalized = bruteforce.Normalize(NormalizationForm.FormD);
                var chars = variant.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
                normalized = new string(chars).Normalize(NormalizationForm.FormC);
                */
                foreach (char c in bruteforce)
                {
                    if (Features.nuphone_inventory.ContainsKey(c))
                    {
                        result.Append(c);
                    }
                    // Keep a list of the IPA characters that we do not support
                    //
                    else if (!Features.Removals.Contains(c))
                    {
                        Features.Removals.Add(c);
                    }
                }
                if (result.Length > 0)
                    return result.ToString();
            }
            return string.Empty;
        }
#if FUTURE
        public static Dictionary<int, HashSet<string>> ConflateNUPhoneVariants(string[] variants)
        {
            Dictionary<int, HashSet<string>> conflatedVariants = new();

            bool postProcess = false;
            foreach (var variant in variants)
            {
                int len = Features.NUPhoneLen(variant);
                if (!conflatedVariants.ContainsKey(len))
                    conflatedVariants[len] = new();
                if (!conflatedVariants[len].Contains(variant))
                    conflatedVariants[len].Add(variant);
                if (postProcess)
                    continue;
                postProcess = conflatedVariants[len].Count > 1;
            }
            if (postProcess)
            {
                ;   // TODO: TO DO: we can do conflation later (it will happen in this block)
            }
            return conflatedVariants;
        }
#endif
        // static constructor
        //
        static Features()
        {
            var lexicon = new LexiconIPA("C:/src/NUPhone/PhonemeEmbeddings");

            Features.nuphone_lexicon_lookup = new();
            Features.nuphone_grapheme_lookup = new();

            for (byte len = 1; len <= 4; len++)
                Features.nuphone_grapheme_lookup[len] = new();

            foreach (var ipa in Features.nuphone_primatives.Keys)
            {
                var item = Features.nuphone_primatives[ipa];

                foreach (var text in item.orthography)
                {
                    NUPhoneRecord record = new();
                    record.nuphone = item.nuphone;
                    record.actual = ipa;

                    var len = text.Length;
                    if (len < 1 || len > 4)
                        continue;

                    var table = Features.nuphone_grapheme_lookup[(byte)len];

                    if (table.ContainsKey(text))
                    {
                        table[text].Add(record);
                    }
                    else
                    {
                        table[text] = new() { record };
                    }
                }
            }
            foreach (var grapheme in Features.nuphone_additional_graphemes.Keys)
            {
                var item = Features.nuphone_additional_graphemes[grapheme];
                var len = grapheme.Length;
                if (len < 1 || len > 4)
                    continue;

                NUPhoneRecord record = new();
                record.nuphone = item.nuphone;
                record.actual = item.actual;

                var table = Features.nuphone_grapheme_lookup[(byte)len];

                if (table.ContainsKey(grapheme))
                {
                    table[grapheme].Add(record);
                }
                else
                {
                    table[grapheme] = new() { record };
                }
            }
            foreach (var lex in lexicon.ipa_primatives.Keys)
            {
                if (lex.StartsWith('\''))
                    continue; // skip modern contractions
                if (lex.StartsWith('.'))
                    continue; // skip abberviations

                var len = lex.Length;
                if (len < 1)
                    continue;

                var variants = lexicon.ipa_primatives[lex];

                var entry = NormalizeIntoNUPhone(variants);

                if (entry.nuphone.Length > 0)
                {
                    NUPhoneRecord record = new();
                    record.nuphone = entry.nuphone;
                    record.actual = entry.ipa;

                    Dictionary<string, List<NUPhoneRecord>> table;

                    if (Features.nuphone_lexicon_lookup.ContainsKey((byte)len))
                    {
                        table = Features.nuphone_lexicon_lookup[(byte)len];
                    }
                    else
                    {
                        table = new();
                        Features.nuphone_lexicon_lookup[(byte)len] = table;
                    }
                    if (table.ContainsKey(lex))
                    {
                        table[lex].Add(record);
                    }
                    else
                    {
                        table[lex] = new() { record };
                    }
                }
            }
        }
        public static int NUPhoneLen(string nuphone)
        {
            int len = 0;

            bool ored = false;
            foreach (var c in nuphone)
            {
                switch(c)
                {
                    case '{': ored = true;         continue;
                    case '}': ored = false; len++; continue;
                    default:  if (!ored)    len++; continue;
                }
            }
            return len;
        }
        static internal (string nuphone, byte[] features) Generate(string word)
        {
            (string nuphone, byte[] features) generated = (string.Empty, new byte[0]);
            var ipa = new NUPhoneGen(word);
            if (ipa.Phonetic.Length > 0)
            {
                generated.nuphone = Features.NormalizeIntoNUPhone(ipa.Phonetic);
                var len = Features.NUPhoneLen(generated.nuphone);
                byte i;
                generated.features = new byte[len];
                for (i = 0; i < len; i++)
                    generated.features[i] = 0;
                i = 0;
                bool ored = false;
                foreach (var c in generated.nuphone)
                {
                    if (i >= len)   // fail-safety;
                    {
                        generated.features = new byte[0];
                        break;
                    }
                    switch (c)
                    {
                        case '{': ored = true;       continue;
                        case '}': ored = false; i++; continue;
                        case '|':                    continue;
                        default: break;
                    }
                    if (Features.nuphone_inventory.ContainsKey(c))
                        generated.features[i] |= Features.nuphone_inventory[c];
                    if (!ored)
                        i++;
                }
            }
            return generated;
        }
    }
}