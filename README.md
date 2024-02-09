# WinTetris

This is, as you might surmise, a Tetris type game written for Windows.  It uses WinForms for simple windows and is written completely in C#.  As a Tetris game, the primary interface is with the keyboard.

| keyboard    | description                              |
|-------------|------------------------------------------|
| F1          | provides keyboard help                   |
| up arrow    | rotates the current piece                |
| left arrow  | moves the current piece left             |
| right arrow | moves the current piece right            |
| down arrow  | moves the current piece down one line    |
| Enter       | drops the current piece                  |
| delete      | deletes the current piece (costs speed)  |
| spacebar    | pauses and unpauses the game             |
| backspace   | restarts the game                        |
| escape      | quits the game                           |


The **F1** key provides keyboard help.  The **up arrow** rotates the current piece.  The **left arrow** moves it left.  The **right arrow** moves it right.  The **down arrow** moves it down one line (if you are impatient like me!).  The **enter** key drops it all the way down.  The **delete** key will delete the current piece.  The **spacebar** will pause and unpause the game.  The **backspace** key will start a new game.  And the **escape** key will quit the game.

Like any tetris, you will rotate and move your pieces to attempt to fit them into lines.  When you get a line filled, it will do a brief animation, add to the score, and remove that line.  If you get your lines too high, you will experience a game over condition.

The current and next piece are shown at the bottom of the window, while variables such as score and count are shown to the right.  If you adjust the size of the window, all the parts of the game will adjust to fit it correctly.

This game is copyright 2024 by John Worthington
  and uses the General Public License (version 3)
Feel free to use any part or all of this program for your own things.  While it is not necessary, I would appreciate knowing about anything you make of it.  You may contact me at woodmage@gmail.com and I would appreciate it if the title of your message included the name WinTetris.

Anyway, please enjoy!
