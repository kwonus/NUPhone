# Introducing NUPhone
The International Phonetic Alphabet (IPA) is a highly embraced standard representation for sounds. Professional linguists often use it to provide clear phonetic representation for any language. However, when used for fuzzy matching, it can actually be too precise. While diacritics provide the extra precision for linguists, they can be considered noise in comparison logic that expects high recall on fuzzy comparisons. For that reason, when normalizing full-fidelity IPA into NUPhone, diacritics found in standard IPA are indiscriminately removed. To be clear, NUPhone is not optimized for linguists. Instead it is optimized to streamline phonetic comparison logic, phoneme-by-phoneme.

NUPhone is concerned about more than just normalization, it also represents uncertainty. For example, \<read\> has two pronunciations. In IPA, we might represent the past and present tenses of this verb in this manner:

[rɛd] or [rid]

In NUPhone, we represent the variation (i.e. uncertainty) using a conflated expression. While we utilize IPA characters in NUPhone, our notation is in direct conflict with standard IPA. Yet, it streamlines phoneme-by-phoneme comparison logic.

Quite simply, /rɛd/ or /rid/ becomes conflated in NUPhone as:

r{ɛ|i}d

A secondary benefit of this notation is that table-driven IPA hash-lookups can be efficiently represented. Accordingly, NUPhone expressions remain intuitive. For OOV words where a table-driven lookup is a concatenation of smaller segmented lookups [segmented lookups are used to generate the whole, by composition]. Uncertainty points, where a segment-lookup yields multiple values, are easily discerned in NUPhone representation.

If a user chooses to further decorate the NUPhone example depicted above as [r{ɛ|i}d], /r{ɛ|i}d/, or even \\r{ɛ|i}d\\, we would **not** object. Quite possibly, however, others might ;-)

NUPhone is very opinionated about eliminating redundancy in expressions. We strongly discourage this type of expression for [rɛd] or [rid]:

\* {rɛd|rid}

While the above representation might be easier to generate from lookup tables, comparison logic would be more expensive. The parts that are similar among the variant phonetic representations are expected to be outside of the squiggly braces.
