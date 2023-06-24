# Normalized-Uncertainty Phonetic (NUPhone) Representation

##### Version 1.0.3.624

### Introducing NUPhone

The International Phonetic Alphabet (IPA) is a highly embraced standard representation for sounds. Professional linguists often use it to provide clear phonetic representation for any language. However, when used for fuzzy matching, it can actually be too precise. While diacritics provide the extra precision for linguists, they can be considered noise in comparison logic that expects high recall on fuzzy comparisons. For that reason, when normalizing full-fidelity IPA into NUPhone, diacritics found in standard IPA can be removed.  To be clear, NUPhone is not optimized for linguists. Instead it is optimized to streamline phonetic comparison logic, phoneme-by-phoneme.

NUPhone is concerned about more than just normalization, it also represents uncertainty. For example, \<read\> has two pronunciations. In IPA, we might represent the past and present tenses of this verb in this manner:

[rɛd] or [rid]

In NUPhone, we represent the variation (i.e. uncertainty) using a conflated expression. While we utilize IPA characters in NUPhone, our notation is in direct conflict with standard IPA. Yet, it streamlines phoneme-by-phoneme comparison logic.

Quite simply, /rɛd/ or /rid/ becomes conflated in NUPhone as:

r{ɛ|i}d

A secondary benefit of this notation is that table-driven IPA hash-lookups can be efficiently represented. Accordingly, NUPhone expressions remain intuitive. For OOV words where a table-driven lookup is a concatenation of smaller segmented lookups [segmented lookups are used to generate the whole, by composition]. Uncertainty points, where a segment-lookup yields multiple values, are easily discerned in NUPhone representation.

If a user chooses to further decorate the NUPhone example depicted above as [r{ɛ|i}d], /r{ɛ|i}d/, or even \\r{ɛ|i}d\\, we would **not** object. Quite possibly, however, others might ;-)

NUPhone is very opinionated about eliminating redundancy in expressions. We strongly discourage this type of expression for [rɛd] or [rid]:

\* {rɛd|rid}

While the above representation might be generated with more simplistic logic, comparison logic would be more expensive. The parts that are similar among the variant phonetic representations are expected to be outside of the squiggly braces.

### Comparison processing via Cartesian Coordinates on NUPhone Representations

While there are numerous opportunities to pursue cross-orthography fuzzy string comparisons and search logic, this open-source effort is narrowly focused on Modern English and Early Modern English [as found in the King James Bible]. By narrowing the focus to a mostly singular language domain, the problem-space is greatly simplified. Large tech companies, like Microsoft, provide more sophisticated solutions for cross-language processing.

That said, let's look at ways to speed up similarity processing for English-language strings.

On 23 May 2023, I had this short conversation with Bing:
![phone-embedding](./bing.png)

I was rightly impressed with her response. Given her insight, let's also consider the value proposition of using phonemes for sounds-alike searching. We will not only look for ways ways to speed up similarity processing, but we will consciencely seek a design that provides a broad spectrum of results with high fidelity.

The following academic paper describes how phoneme embeddings and similarity measurements can be rolled-up into whole-token comparisons: https://arxiv.org/pdf/2109.14796.pdf   [<u>Phonetic Word Embeddings</u>, Rahul Sharma et al]

There are two prevailing paradigms for fuzzy string comparisons: string-similarity metrics, and edit-distance metrics. Ratcliff/Obershelp (R/O) pattern recognition is an example of the former; Levenshtein distance is an example of the latter.

For whatever reason [perhaps CPU costs], Levenshtein distance seems to be the de facto standard for similarity assessment. Even this open-source effort by Microsoft Research favors Levenshtein:
https://github.com/Microsoft/PhoneticMatching

I use what I call Heads & Tails (H&T) processing. Originally conceived in 2016, H&T is my invention. Still, from the outside looking in, one might characterize H&T as a simplification of R/O. However, I discovered R/O, five or so years after my initial H&T implementation. Yet, the similarity of H&T with R/O is notable. Under most use-cases, H&T should yield far fewer comparisons than R/O. This document does not describe every difference between H&T and R/O, but it does identify the primary difference.  R/O considers the longest common substring (LCS) between two strings, in order to drive comparison logic. H&T simplifies this, by favoring LCS candidates on the head of the string and on the tail of the string. That minor difference greatly simplifies remainder-processing. 

This is my first public release of the H&T algorithm, but it is not my first implementation.  While in pursuit of a Masters in Computational Linguistics at the University of Washington, as part of my coursework, I devised H&T to perform word alignment between bitexts. Specifically, I was aligning the words of an early French translation of the bible with the words of an early English version of the bible in conjunction with machine translation to align the bitexts. I always felt that I would revisit this processing concept, even though it has remained dormant until now. 

In this repo, I am providing an H&T implementation that compares NUPhone representations using a difference metric on phoneme embeddings. My difference metric treats manor and place of articulation as an X&Y axis. Vowels similarly use frontness/backness and open/close as X&Y. There are a number freely available tabular resources for IPA and ARPAbet. Some of these are incorporated into a general-purpose English-to-NUPhone generator:

1) [The 44 Phonemes in English (dyslexia-reading-well.com)](https://www.dyslexia-reading-well.com/44-phonemes-in-english.html)
2) [ipa-dict/en_US.txt at master · open-dict-data/ipa-dict · GitHub](https://github.com/open-dict-data/ipa-dict/blob/master/data/en_US.txt)
3) [The CMU Pronouncing Dictionary](http://www.speech.cs.cmu.edu/cgi-bin/cmudict)
4) [arpabet-to-ipa/App.php at master · wwesantos/arpabet-to-ipa · GitHub](https://github.com/wwesantos/arpabet-to-ipa/blob/master/src/App.php)

The generated NUPhone representations are scored for similarity using a roll-up of similarities based upon the phoneme embeddings via the X&Y coordinate systems described above. As my initial implementation found herein is English-only, I am able to represent the full spectrum of phoneme embeddings in a single 8-bit integer. If this integer where expanded to 16-bits, scoring could easily accommodate additional languages.  However, in this repo Modern English and Early-Modern English are my sole focus. As such, broadening support for additional languages is not to be expected in this current repo.

### The "Bag of Phonemes" approach

Another type of fuzzy string matching that could be explored is what I call, the "Bag of Phonemes" (BoP) approach. In tandem with the BoP, I also introduce a "Bag of Phonetic Features" (BoPF). The BoPF is a compact, but lossy representation of the BoP for the entire token. Unlike a "Bag of Words" (BoW) algorithm, the feature dimensions for NUPhone is quite finite. This is unlike a lexicon of words that seems almost infinite. This is why old-fashioned BoW algorithms often employed TF-IDF [Term-Frequency, Inverse-Document-Frequency]. We won't explore BoW or TF-IDF, because these days, only the name construction(i.e. "Bag of X") is inspiring.  

My "Bag of Phonetic Features" transcription process behaves like a checksum on the NUPhone string. The presence of a bit means that at least one phoneme in the string had that feature. Comparison logic performs a bitwise-AND between the two feature vectors, and also a bitwise-OR between the two feature vectors. We have a global function that converts English strings to NUPhone, and we call it nuphone(). Likewise, we have a global function that counts-1-bits (all non-zero bits) and we call this cnt_bits().

For a given {str1, str2}. The score is a combination of simple integer math, and hash-map lookups:

```C#

uint get_bag(string phone)
{
    uint bag = 0;
    for (char c in phone)
    {
        uint vector = phoneme_embeddings[c];
        bag |= vector;
    }
    return bag;
}
string str1 = "foo";
string str2 = "bar";
string phone1 = nuphone(str1);
string phone2 = nuphone(str2);
uint bag1 = get_bag(phone1);
uint bag2 = get_bag(phone2);
uint and_ed = bag1 & bag2;
uint or_ed = bag1 | bag2;
uint and_cnt = cnt_bits(and_ed);
uint or_cnt = cnt_bits(or_ed);
uint score = (100 * and_cnt) / or_cnt;

// A score close to 100 is a high score (100% match). However, this a fuzzy score.
// It does not represent 100% similarity. Tuning will be required to figure out thresholds
// for appropriate levels of recall. Other algorithms, such as H&T will be used to filter
// out the outliers
```

##### Table 1 - Course-Fuzzy Scoring using BoPF

### Status of BoP and BoPF

At the moment, BoP and BoPF are being deferred for any further exploration.

### Combining H&T with NUPhone

1. NUPhone representations will be generated for both tokens being compared.
2. Phoneme embeddings will be generated for both NUPhone representations. Consider these two polymorphic methods:

   - embeddings(nuphone_str: utf8_char): returns byte

   - embeddings(nuphone_str: utf8_string): returns Array\<byte\>

   - Unlike most embeddings that have a discrete feature for each characteristic, my embeddings are a simple cartesian coordinate system. Well really, two distinct coordinate systems: one set of coordinates for Consonants and another set for Vowels. The value of an X+Y coordinate system is that calculating distance is trivial, using only bit-shifts and primitive integer calculations. Moreover, constraining ourselves to just English, the entire coordinate system is fully represented in a single byte. That's right! All phonetic features of any English NUPhone character is compactly represented in a single byte: two 3-bit integers for the X & Y axes. The remaining 2 bits tell us if it is a vowel or a voiced/unvoiced consonant.
3. Prescoring can preempt comparisons of differing phoneme lengths where the absolute maximum similarity cannot possibly meet threshold. (e.g. Comparing an 8 phonemes with a 12 phonemes can never exceed a 75% match). Prescoring can substantially reduce the number of comparisons required across an entire lexicon. For that reason, we partition our lexicon by length of the nuphone transcription.

H&T first finds the LCS at the head of the string, and the LCS at the tail of the string. If these are overlapping, the highest scoring string is chosen. If they are not overlapping, both are chosen. In either case, the remainder is processed recursively by H&T.
