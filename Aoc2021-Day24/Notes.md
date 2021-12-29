# Notes
The input consists of a sequence of 14 blocks of 18 instructions.
Each block is the same, but with some constants, `a`, `b` and `c` that are different in each block.

The program translated into C# looks like this:

    // 14 repetitions, with different values in a, b and c.

    var w = inputs.Read();
    var x = z % 26;
    z /= a;
    x = x + b == w ? 0 : 1;
    var y = 25 * x + 1;
    z *= y;
    y = (w + c) * x;
    z += y;

The constants `a`, `b` and `c` for each digit satisfy the following criteria:

* If `b` is greater than zero, `a` is one (otherwise, `a` is twenty six).
* If `b` is greater than zero, it is always greater than none.

This restriction on the constants allow some code paths to be eliminated, based on whether `b` is greater than zero:

    // 14 repetitions, with different values in a, b and c.

    var w = inputs.Read();
    if (b > 0)
    {
        z = z * 26 + w + c;
    }
    else
    {
        var x = z % 26;
        z /= 26;

        if (w != x + b)
        {
            z = z * 26 + w + c;
        }
    }

The variable `z` can be seen as acting like a stack of numbers between zero and 25.

The code `z = z * 26 + w + c` is effectively pushing `w + c` onto that stack.

The code `var x = z % 26;` and `z /= 26;` is effectively popping a value off of the stack into `x`.

The `z` variable can only end up at zero (i.e. indicating a valid model number) if the stack is empty after the last instruction completes.

The puzzle input is arranged such that `b > 0` for exactly half of the digits (for these digits, a value is always pushed to `z`).

For the remaining digits, a value is popped from `z`, and if another value is pushed, there is no way for `z` to end up at zero.
The additional push is prevented only when `w` (the digit) equals the sum of the value last pushed and `b`.

This establishes a relationship between the digit and another digit based on the constants.
The input digit is only valid if:
`current_digit - b` equals `related_digit + c`.

This means that half of the digits can be calculated directly from the other half.

Furthermore, because digits have to fall in the range 1-9, the other digits can be constrained to a range.
