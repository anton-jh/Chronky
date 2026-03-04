# Chronky


## Introduction

Chronky is a simple time-keeping program for your terminal. It works by you recording points in time,
and then simply describing what you did inbetween.

The Chronky interface is like a limited text-editor with two modes, _entering_ and _cursor_.

In **entering mode** you can type in stuff. Pressing ENTER parses that line. You can type multiple commands
on the same line and Chronky will split them by whitespace.

Submitting an empty line takes us to **cursor mode**. Now we can move up/down with our arrow-keys.
We can even remove lines with BACKSPACE. Simply press ENTER again to return to **entering mode**
_on the selected line_.


## Timestamps

**Let's say we start at 8.**

> ... so we enter `08:00`.

```
08:00
```

That's a _timestamp_.

The following shorthands are available:

- `h:mm` => `0h:mm`
- `hh` => `hh:00`
- `h` => `0h:00`

... and the following QoL-commands are available:

- `.` => Records the current time when submitted
- `..` => Records the current time when submitted, rounded to the nearest whole 15 minutes




## Labels

**We start with breakfast.**

> ... so we enter `breakfast` to label the time.

```
08:00
    breakfast
```

That's _label_.
A label _labels_ the time between two timestamps.
There can only be **one** label between a pair of time entries.

Labels start with either a letter (едц is fine!) or an underscore (_),
after that everything goes (except spaces!).

Any label starting with an underscore (even just "_" by itself) is treated as a _discard_.
Discards are not counted towards the day's total worked time and are grouped separately
in the summary.


## Sub-segments

**When we get back we remember that we took a phone-call during breakfast,
but we don't remember _when_ exactly.**

> ... so we enter `5m=>phone_call` to subtract 5 minutes from _breakfast_ and move them to _phone_call_.

```
08:00
    breakfast
	-00:05=>phone_call
```

That's a sub-segment. In this case it subtracts 5 minutes from our _breakfast_ and adds them to _phone_call_.

Sub-segments are formatted like this: `-hh:mm=>label`.

The following shorthands are available:

- `1h=>label` => `01:00=>label`
- `1m=>label` => `00:01=>label`
- `1h1m=>label` => `01:01=>label`


## Closing spans

**We got back after our breakfast at 9.**

> ... and entered `.` to conveniently record the current time

```
08:00
    breakfast
	-00:05=>phone_call
09:01
```

This _span_ is now closed. 55 minutes are counted towards _breakfast_ and 5 minutes towards _phone_call_.


## Extra-segments

**Some time later we remember that we slept 8 hours during the night, but it's not relevant _when_ exactly.**

> ... so we enter `+8h=>sleep`

```
08:00
    breakfast
	-00:05=>phone_call
09:00
+08:00=>sleep
```

That's an extra-segment. It's very similar to sub-segments but it doesn't subtract time from anywhere,
and instead just adds it out of nowhere!
