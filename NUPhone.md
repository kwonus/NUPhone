# Normalized-Uncertainty Phonetic (NUPhone) Representation

##### Version 1.0.3.520

### Introducing NUPhone

The International Phonetic Alphabet (IPA) is a highly embraced standard representation for sounds. Professional linguists often use it to provide clear phonetic representation for any language. However, when used for fuzzy matching, it can actually be too precise. While diacritics provide the extra precision for linguists, they can be considered noise in comparison logic that expects high recall on fuzzy comparisons. For that reason, when normalizing full-fidelity IPA into NUPhone, diacritics found in standard IPA are indiscriminately removed. In fact, the normalization process only includes the standard IPA characters for vowels and consonants [including glides]. Pauses and syllable markers are eliminated along with the diacritics.

Still, NUPhone deals not only with normalization, but also with uncertainty. For example, \<read\> has two pronunciations. In IPA, we might represent the past and present tenses of this verb in this manner:

[rɛd] or [rid]

In NUPhone, we represent the variation (i.e. uncertainty) using a conflated expression. While we utilize IPA characters in NUPhone, our notation is in direct conflict with standard IPA. Yet, it streamlines phoneme-by-phoneme comparison logic.

Quite simply, /rɛd/ or /rid/ becomes conflated in NUPhone as:

r{ɛ|i}d

A secondary benefit of this notation is that table-driven IPA hash-lookups can be efficiently represented. Accordingly, NUPhone expressions remain intuitive. For OOV words where a table-driven lookup is a concatenation of smaller segmented lookups [segmented lookups are used to generate the whole, by composition]. Uncertainty points, where a segment-lookup yields multiple values, are easily discerned in NUPhone representation.

If a user chooses to further decorate the NUPhone example depicted above as [r{ɛ|i}d], /r{ɛ|i}d/, or even \\r{ɛ|i}d\\, we would **not** object. Quite possibly, however, others might ;-)

NUPhone is very opinionated about eliminating redundancy in expressions. We strongly discourage this type of expression for [rɛd] or [rid]:

{rɛd|rid}

While the above representation might be easier to generate from lookup tables, comparison logic would be more expensive. The parts that are similar among the variant phonetic representations are expected to be outside of the squiggly braces.

### Heads & Tails String-Matching

When performing fuzzy string comparisons (or even substring lookups), there are two prevailing paradigms: string-similarity algorithms, and edit-distance algorithms. Ratcliff/Obershelp pattern recognition is an example of the former; Levenshtein distance is an example of the latter. I fundamentally prefer similarity metrics over distance metrics. However, the Levenshtein algorithm is more intuitively understood and likely less cpu-intensive when compared to Ratcliff/Obershelp.

Incidentally, an example of fuzzy matching using Levenshtein distance on phonetic representations of English strings can be found here:
https://github.com/Microsoft/PhoneticMatching

The Heads & Tails (H&T) algorithm described herein is a dramatic simplification of Ratcliff/Obershelp (R/O) and should result in far fewer comparisons in most use-cases. I expect it to perform as well on English and NUPhone strings as does R/O. I am less certain about other orthographies. This document does not describe every difference between H&T and R/O, but it does identify some key differences.

R/O is looking for the longest common substring (LCS). H&T simplifies this by finding the largest common substring between just two candidates: the start of the two strings and the end of the two strings. This simple change dramatically simplifies calculating the remainders. This change also substantially reduces the frequency of character-by-character comparisons. Equally important, it makes the resulting H&T implementations far less complex than R/O. Simplification also makes H&T derivatives more intuitive. To be clear, Levenshtein is easier to understand than R/O. However, Levenshtein and H&T are on a more equal playing field, with respect to simplicity. In fact, the R/O algorithm almost demands recursion and scoring accumulators. By contrast, It's easy to make H&T iterative without resorting to recursion.

While this is my first public presentation of the H&T algorithm, I original conceived of it in 2016. While in pursuit of a Masters in Computational Linguistics, during my coursework at the University of Washington, I conceived of performing word alignment between bitexts using H&T. Specifically, I was aligning the words of an early French translation of the bible with the words of an early modern English version of the bible in conjunction with machine translation. To be completely honest, I do not recall the full details. However, I always ascribed merit to the H&T processing concept.

Fast-forward to now: I am resurrecting the H&T algorithm to be applied to the domain of fuzzy string comparisons. Furthermore, we will tweak the algorithm to compare phonetic features using an H&T-based NUPhone-generator. And finally we will create a version of H&T that operates upon phonemes instead of characters, replacing discrete Boolean comparisons with a percentage of similarity. We will utilize just two freely available tabular resources for English-to-NUPhone conversions:

1) [The 44 Phonemes in English (dyslexia-reading-well.com)](https://www.dyslexia-reading-well.com/44-phonemes-in-english.html)
2) [ipa-dict/en_US.txt at master · open-dict-data/ipa-dict · GitHub](https://github.com/open-dict-data/ipa-dict/blob/master/data/en_US.txt)

Sources found herein will utilize these two tables for NUPhone-generation from English strings. From here, it gets interesting for the trained linguist. Obviously, each NUPhone character (i.e. subset of IPA) can be expressed as a feature-vector. Moreover, these feature vectors can be utilized to mathematically calculate sound similarity. We will tweak the H&T algorithm to account for phonemic similarity: replace discrete Boolean character-by-character comparisons with phoneme-by-phoneme similarity metrics. This is not entirely unlike this open-source effort by Microsoft Research:

https://github.com/Microsoft/PhoneticMatching

Unlike the Levenshtein implementation cited above, we seek similarity instead of minimum distance. Perhaps this is part of why I prefer similarity metrics over difference metrics. Calculating difference metrics using similarity, just seems weird to me. Contrariwise, rolling up a similarity score for a word based upon the similarity of its components is altogether intuitive to me. Stay tuned for examples.

# 