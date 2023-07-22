# The Quine Language

Quine is a programming language for writing quines, or rather, one specific quine.

Quine is based on a virtual machine that contains a `buffer` holding the previous token, a `main queue` which can hold arbitrary tokens, and a `standard output` stream. The VM is manipulated by these machine language statements:

- `000` - (`e`) - raise `e`rror: dump the contents of the `main queue` to `standard output`, followed by 'e' to indicate that an error has occurred.
- `001` - (`i`) - disable copying of the previous token to the `main queue` (and `i`ndicate success) by writing current token to the `main queue`.
- `010` - (`n`) - copy previous a`n`d current token to the `main queue`.
- `011` - (`q`) - clear the main `q`ueue and commence normal operation.
- `100` - (`u`) - `u`ndertake continuous copying of the previous token `buffer` to the `main queue`.

There are no other symbols or operators permitted.

White space is significant: there must be none.

The nature of the parser, tokenizer, and the virtual machine architecture mean that for complex reasons it is a syntax error to place the high level tokens in any order other than:

1. `q`
2. `u`
3. `i`
4. `n`
5. `e`

The consequence of this is that the only valid program in the language Quine is this:

    quine

When executed it provides this output:

    quine
