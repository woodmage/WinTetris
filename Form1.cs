namespace WinTetris
{
    public partial class Form1 : Form
    {
        private readonly PieceType shape = new();
        private readonly PieceType[] shapearray = [ new(new int[,] { { 0, 0, 0, 0, 0 }, { 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1 }, { 0, 0, 0, 0, 0 } }),
                                                    new(new int[,] { { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 1, 1, 1, 0 }, { 0, 0, 0, 0, 0 } }),
                                                    new(new int[,] { { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 1, 1, 0 }, { 0, 1, 1, 0, 0 }, { 0, 0, 0, 0, 0 } }),
                                                    new(new int[,] { { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 1, 1, 0, 0 }, { 0, 0, 1, 1, 0 }, { 0, 0, 0, 0, 0 } }),
                                                    new(new int[,] { { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 1, 1, 1, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 } }),
                                                    new(new int[,] { { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 1, 0 }, { 0, 1, 1, 1, 0 }, { 0, 0, 0, 0, 0 } }),
                                                    new(new int[,] { { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 1, 1, 1, 0 }, { 0, 0, 1, 0, 0 }, { 0, 0, 0, 0, 0 } }),
                                                    new(new int[,] { { 0, 0, 0, 0, 0 }, { 0, 1, 1, 0, 0 }, { 0, 1, 1, 0, 0 }, { 0, 1, 1, 0, 0 }, { 0, 0, 0, 0, 0 } }),
                                                    new(new int[,] { { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 1, 1, 1, 1 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 } }),
                                                    new(new int[,] { { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 1, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 } }) ];
        private int score = 0, speed = Constant.InitialSpeed, count = 0, piecex, piecey, nextpiece;
        private bool isbusy = false, isgameover = false;
        private readonly System.Windows.Forms.Timer timer = new();
        private Bitmap board = new(1, 1);
        private Bitmap helpboard = new(1, 1);
        private Bitmap statusboard = new(1, 1);
        private Bitmap infoboard = new(1, 1);
        private readonly Panel backscreen;
        private readonly PictureBox help;
        private readonly PictureBox screen;
        private readonly PictureBox info;
        private readonly PictureBox status;
        private readonly Font myfont = new("Arial", 15);
        private readonly Dictionary<Keys, (string, Action)> keys;
        
        /// <summary>
        /// Form1 is the default constructor for the main class, so it is the entry point.
        /// </summary>
        public Form1()
        {
            keys = new Dictionary<Keys, (string, Action)>
            {
                { Keys.F1, ( "F1: gets help", GetHelp ) },
                { Keys.Up, ( "up: rotate piece", RotatePiece ) },
                { Keys.Left, ( "left: move piece left", MovePieceLeft ) },
                { Keys.Right, ( "right: move piece right", MovePieceRight ) },
                { Keys.Down, ( "down: move piece down", MovePieceDown ) },
                { Keys.Enter, ( "enter: drop piece", DropPiece ) },
                { Keys.Delete, ( "delete: delete current piece", DeletePiece ) },
                { Keys.Space, ( "space: pauses / unpauses game", PauseUnpause ) },
                { Keys.Back, ( "backspace: restarts game", RestartGame ) },
                { Keys.Escape, ( "escape: exits game", Close ) }
            };
            InitializeComponent(); //set up the main form
            KeyDown += WT_KeyDown; //set key down event handler
            Resize += WT_Resize; //set form resize event handler
            Shown += WT_Shown; //set form shown event handler
            FormClosing += WT_Closing; //set form closing event handler
            backscreen = new() { BackColor = Color.FromArgb(32, 0, 0) }; //very dark red background
            screen = new(); //make controls
            help = new();
            info = new();
            status = new();
            ResizePBs(); //resize to get controls sizes and locations fixed
            backscreen.Controls.Add(screen); //add screen to backscreen
            backscreen.Controls.Add(help); //add help to backscreen
            Controls.Add(backscreen); //add controls to form
            Controls.Add(info);
            Controls.Add(status);
        }
        
        /// <summary>
        /// This handler is called whenever the form is about to close.  It verifies with the user that they want to exit.  If they don't, it also offers to restart the game.
        /// </summary>
        private void WT_Closing(object? sender, FormClosingEventArgs e)
        {
            timer.Stop(); //stop the timer if it is running
            if (MessageBox.Show("Exit?", "Quit", MessageBoxButtons.YesNo) == DialogResult.Yes) //if user agrees to exit
            {
                e.Cancel = false; //we don't want to cancel
                return; //and we are done
            }
            isgameover = false; //game is not over now
            e.Cancel = true; //we don't want to close
            if (MessageBox.Show("Begin again?", "Restart?", MessageBoxButtons.YesNo) == DialogResult.Yes) //if user wants to restart
                RestartGame(); //restart game
            timer.Start(); //start the timer back up
        }
        
        /// <summary>
        /// This handler is called when the form is first shown.  It simply sets up the timer.
        /// </summary>
        private void WT_Shown(object? sender, EventArgs e)
        {
            timer.Interval = 1; //timer interval set to 1 to do immediately, it will get reset to speed in the event handler
            timer.Tick += WT_Time; //event handler for timer
            timer.Start(); //start the timer
        }
        
        /// <summary>
        /// This method sets the variables for a fresh start of the game.
        /// </summary>
        private void RestartGame()
        {
            timer.Stop(); //stop the timer
            score = 0; //reset variables
            speed = Constant.InitialSpeed;
            count = 0;
            GameBoard.SetBoard(0); //reset the game board
            timer.Interval = 1; //set timer interval to 1 to do immediately
            timer.Start(); //start the timer back up
        }
        
        /// <summary>
        /// This handler is called by the timer and is responsible for all the "automatic" game logic.  
        /// It also handles setting the speed on the timer it is attached to.
        /// </summary>
        private void WT_Time(object? sender, EventArgs e)
        {
            timer.Stop(); //stop the timer
            if (!GameBoard.IsMoving()) //if the piece is stopped
                NewPiece(); //make a new piece
            else //otherwise
                MovePieceDown(); //move the piece down
            DrawScreen(); //draw the screen
            if (isgameover) //if game over
            {
                MessageBox.Show("Game Over!"); //tell the user
                Close(); //close the window
            }
            else //otherwise
            {
                if (speed < Constant.MinimumSpeed)
                {
                    speed = Constant.MinimumSpeed; //if speed is less than minimum speed, set it to minimum speed
                }
                timer.Interval = speed; //set the timer interval
                timer.Start(); //start the timer back up
            }
        }
        
        /// <summary>
        /// This method copies the piece from nextpiece and generates a new nextpiece.
        /// </summary>
        private void NewPiece()
        {
            //int firstpiecehere;
            Random r = new(); //make a new random number generator
            GameBoard.ClearMoving(); //clear the piece from the game board
            int pi = r.Next(10); //get a piece index randomly (0 - 9)
            int pc = r.Next(9) + 1; //get a color for the piece (1 - 9)
            shape.CopyFrom(shapearray[nextpiece], 10 + pc); //copy the piece from nextpiece
            if (!shape.CanMoveTo(3, 0)) //if 3, 0 is not a valid spot for the piece
                isgameover = true;
            piecex = 3; //position the piece
            piecey = 0;
            nextpiece = pi; //set the next piece according to piece index
            AddPiece(); //add the piece to the game board
        }
        
        /// <summary>
        /// This method will keep moving a piece down until it stops.
        /// </summary>
        private void DropPiece()
        {
            while (GameBoard.IsMoving()) //while the piece is still moving
                MovePieceDown(); //move it down
        }
        
        /// <summary>
        /// This method moves the piece left.
        /// </summary>
        private void MovePieceLeft() => MovePiece(-1, 0); //move piece left
        
        /// <summary>
        /// This method moves the piece right.
        /// </summary>
        private void MovePieceRight() => MovePiece(1, 0); //move piece right
        
        /// <summary>
        /// This method moves the piece down.  It also handles the count for reducing the delay thus increasing the speed.
        /// </summary>
        private void MovePieceDown()
        {
            count++; //increment count
            if (count > Constant.CountMax) //if count is greater than 1000
            {
                count = 0; //reset count to 0
                score += Constant.ScoreAdd; //add 100 to score
                speed -= Constant.SpeedSub; //subtract 25 from speed
            }
            MovePiece(0, 1); //move piece down
            DrawScreen();
        }
        
        /// <summary>
        /// This method handles moving the piece.  It validates the move, checks for the piece hitting the bottom of the game board, and checks for lines filled.
        /// </summary>
        /// <param name="x"></param> Horizontal movement
        /// <param name="y"></param> Vertical movement
        private void MovePiece(int x, int y)
        {
            isbusy = true; //set busy flag
            GameBoard.ClearMoving(); //clear piece from game board
            if (shape.CanMoveTo(piecex + x, piecey + y)) //if move is valid
            {
                piecex += x; //move in x
                piecey += y; //move in y
            }
            AddPiece(); //add piece back to game board
            if (GameBoard.PieceAtBottom()) //if piece is at the bottom of the game board
                GameBoard.StopMoving(); //stop the piece there (glue it in place)
            CheckLines(); //check for lines filled
            isbusy = false; //reset busy flag
            DrawScreen(); //draw the screen
        }

        /// <summary>
        /// This method checks each line of the game board and if it is filled, it will remove the line.  It uses recursive logic to make sure it catches all filled lines.
        /// </summary>
        private void CheckLines()
        {
            int line;
            while ((line = GameBoard.FilledLine()) != -1) //while there are filled lines
            {
                GameBoard.RemoveLine(line); //remove filled line
                score += Constant.ScoreAdd; //add 100 to score
            }
        }
        
        /// <summary>
        /// This method adds the piece back onto the game board.  It also checks to see if the piece should stop and handles it appropriately.
        /// </summary>
        private void AddPiece()
        {
            GameBoard.AddPiece(shape, piecex, piecey);
        }
        
        /// <summary>
        /// This method rotates the piece.
        /// </summary>
        private void RotatePiece()
        {
            GameBoard.ClearMoving(); //clear the piece from the board
            shape.Rotate(); //rotate the piece
            AddPiece(); //add the piece to the game board
            DrawScreen(); //draw the screen
        }

        /// <summary>
        /// This method handles resizing the controls.
        /// </summary>
        private void ResizePBs()
        {
            Text = Constant.Version; //set titlebar text for window
            backscreen.Location = new(0, 0); //locate and size backscreen
            backscreen.Size = new(ClientSize.Width - 200, ClientSize.Height - 50);
            help.Location = new(0, 0); //location and size of help
            help.Size = new(backscreen.Width, backscreen.Height);
            help.Visible = false;
            int wide = backscreen.Width; //compute screen location and size such that screen.Width * 3 = screen.Height
            int high = wide * Constant.Rows / Constant.Cols;
            if (high > backscreen.Height)
            {
                high = backscreen.Height;
                wide = high * Constant.Cols / Constant.Rows;
            }
            high = high / Constant.Rows * Constant.Rows; //adjust high and wide so they are exact
            wide = wide / Constant.Cols * Constant.Cols;
            screen.Size = new(wide, high); //set the size of the screen
            int x = (backscreen.Width - wide) / 2;
            int y = (backscreen.Height - high) / 2;
            screen.Location = new(x, y); //set the location (centered) of the screen
            info.Location = new(ClientSize.Width - 200, 0); //set sizes and locations of other controls
            info.Size = new(200, ClientSize.Height);
            status.Location = new(0, ClientSize.Height - 50);
            status.Size = new(ClientSize.Width - 200, 50);
            board.Dispose(); //make new bitmaps for board, help board, status board, and info board
            board = new(screen.Width, screen.Height);
            helpboard.Dispose();
            helpboard = new(help.Width, help.Height);
            statusboard.Dispose();
            statusboard = new(status.Width, status.Height);
            infoboard.Dispose();
            infoboard = new(info.Width, info.Height);
            DrawScreen(); //draw the screen
        }
        
        /// <summary>
        /// This handler is called whenever the user resizes the form.
        /// </summary>
        private void WT_Resize(object? sender, EventArgs e) => ResizePBs(); //resize routine
        
        /// <summary>
        /// This handler is called whenever the user presses a key and calls the appropriate method to handle it.
        /// </summary>
        private void WT_KeyDown(object? sender, KeyEventArgs e)
        {
            if (!isbusy) //if we are not busy
            {
                if (keys.TryGetValue(e.KeyData, out var tupvar)) //if keystroke is in dictionary
                {
                    DoDebug.Write($">> {SeparateKeys(tupvar.Item1).Item1} pressed."); //write debugging message
                    tupvar.Item2(); //execute function (yeah, action)
                    DoDebug.Write($">> {SeparateKeys(tupvar.Item1).Item1} processed."); //write debugging message
                }
                else //otherwise (unknown keystroke)
                {
                    DoDebug.Write($">>>  Unknown key ({e.KeyData} pressed - passing to Windows.");
                    return; //just return so windows will handle it
                }
                e.Handled = true; //tell windows we handled that key
            }
        }

        /// <summary>
        /// This method provides help for the keys used by the game.
        /// </summary>
        private void GetHelp()
        {
            timer.Stop(); //stop the timer
            SizeF caret = new(0, 0); //set up a size control
            int yposition = 0; //set up a y position
            string key, message;
            using Graphics g = Graphics.FromImage(helpboard);
            {
                using SolidBrush white = new(Color.White);
                {
                    using SolidBrush yellow = new(Color.Yellow);
                    {
                        foreach (var keytuple in keys.Values)
                        {
                            int xposition = 0; //set up a x position
                            (key, message) = SeparateKeys(keytuple.Item1); //separate key and message
                            g.DrawString(key, myfont, yellow, 0, yposition); //output key in yellow
                            caret = g.MeasureString(key, myfont); //get size of string
                            xposition += (int)caret.Width; //add width to x position
                            g.DrawString($" - {message}", myfont, white, xposition, yposition); //output message in white
                            caret = g.MeasureString(message, myfont); //get size of string
                            yposition += (int)caret.Height; //add height to y position
                        }
                        yposition += (int)caret.Height; //add an extra line
                        g.DrawString("\nGame is paused.  Press space to continue.", myfont, white, 0, yposition); //give instructions
                    }
                }
            }
            screen.Visible = false; //stop showing screen
            help.Visible = true; //show help
            help.Image = helpboard; //set info area to use infoboard for its image
            help.Invalidate(); //tell windows to repaint info area
        }

        /// <summary>
        /// This method separates a string into the value before and after a ':'.
        /// </summary>
        /// <param name="keymsg"></param> The string to separate.
        /// <returns></returns> a tuple of (before, after) where each is a string.
        private static (string, string) SeparateKeys(string keymsg) => (keymsg[0..keymsg.IndexOf(':')], keymsg[(keymsg.IndexOf(':') + 1)..]);

        /// <summary>
        /// This method increases the speed and deletes the current piece.
        /// </summary>
        private void DeletePiece()
        {
            speed -= Constant.SpeedSub; //make it a bit faster
            NewPiece(); //replace piece
            DrawScreen(); //draw the screen
        }

        /// <summary>
        /// This method toggles the timer on / off.
        /// </summary>
        private void PauseUnpause()
        {
            if (timer.Enabled) //if timer is active
            {
                timer.Stop(); //stop timer
            }
            else //otherwise
            {
                timer.Start(); //start timer back up
            }
            DrawScreen(); //draw the screen
        }

        /// <summary>
        /// This method handles drawing the screen, drawing the status, and drawing the info.
        /// </summary>
        private void DrawScreen()
        {
            help.Visible = false; //turn off help
            screen.Visible = true; //turn on screen
            GameBoard.PaintBoard(board, Color.FromArgb(16, 16, 16));
            screen.Image = board; //set board to screen
            screen.Invalidate(); //tell windows to repaint screen
            using Graphics gstatus = Graphics.FromImage(statusboard); //get graphics for status board
            {
                gstatus.Clear(Color.FromArgb(32, 32, 32)); //clear to dark grey
                using SolidBrush brush = new(Color.White); //use white brush
                {
                    gstatus.DrawString("Piece: ", myfont, brush, 0, 0); //draw string "Piece: "
                    SizeF caret = gstatus.MeasureString("Piece: ", myfont); //get position after string
                    shape.PaintPiece(gstatus, (int)caret.Width, 0, 5, Color.White); //draw piece
                    caret.Width += 40; //add 40 to position
                    gstatus.DrawString("Next: ", myfont, brush, caret.Width, 0); //draw string "Next: "
                    SizeF caradd = gstatus.MeasureString("Next: ", myfont); //get size of "Next: "
                    caret.Width += caradd.Width; //add it to position
                    shapearray[nextpiece].PaintPiece(gstatus, (int)caret.Width, 0, 5, Color.White); //draw next piece
                    caret.Width += 50; //add 50 to position
                    gstatus.DrawString("Press F1 for help with keys.", myfont, brush, caret.Width, 0); //give instructions
                }
            }
            status.Image = statusboard; //set status board to status
            status.Invalidate(); //tell windows to repaint status
            using Graphics ginfo = Graphics.FromImage(infoboard); //get graphics for info board
            {
                ginfo.Clear(Color.Navy); //clear to navy
                using SolidBrush brush = new(Color.White); //make brush
                {
                    ginfo.DrawString($"Piece at {piecex}, {piecey}", myfont, brush, 0, 0); //show piece location
                    ginfo.DrawString($"Score: {score}", myfont, brush, 0, 50); //show score
                    ginfo.DrawString($"Speed: {speed}", myfont, brush, 0, 100); //show speed
                }
                DrawBars(ginfo, 0, 150, count, Constant.CountMax, 200, 30, $"Count: {count}"); //make bars for count
            }
            info.Image = infoboard; //set info board to info
            info.Invalidate(); //tell windows to repaint info
        }

        /// <summary>
        /// This method draws two bars, one on top of the other to show amount completed then draws text on top of it.
        /// </summary>
        /// <param name="g"></param> The graphics object to use
        /// <param name="x"></param> The starting x position to use
        /// <param name="y"></param> The starting y position to use
        /// <param name="count"></param> The count that we have reached
        /// <param name="total"></param> The total possible count
        /// <param name="wide"></param> The maximum width of the bars
        /// <param name="high"></param> How high to draw the bars
        /// <param name="text"></param> The text to draw over the bars
        private void DrawBars(Graphics g, int x, int y, int count, int total, int wide, int high, string text)
        {
            using SolidBrush brush = new(Color.White); //make brushes
            {
                using SolidBrush rbrush = new(Color.Red);
                {
                    using SolidBrush gbrush = new(Color.Green);
                    {
                        g.FillRectangle(rbrush, x, y, wide, high); //paint red all the way across
                        count = count * wide / total; //compute how far to paint for count
                        g.FillRectangle(gbrush, x, y, count, high); //paint green that far
                        g.DrawString(text, myfont, brush, x, y); //draw the text in white over the bars
                    }
                }
            }
        }
    }

    /// <summary>
    /// This class handles the individual pieces or shapes that are used in the game.  It contains several methods for working with them.
    /// </summary>
    public class PieceType
    {
        public int[,] Piece { get => _piece; set => _piece = value; } //the actual piece (yes, it is an int[,])
        private int[,] _piece = new int[Constant.PieceSize, Constant.PieceSize];

        /// <summary>
        /// This is the default constructor and simply provides a blank piece.
        /// </summary>
        public PieceType() => SetPiece(0);

        /// <summary>
        /// This is the constructor which takes a int[,] value and uses it to initialize the piece.
        /// </summary>
        /// <param name="piece"></param> This is the int[,] value used.
        public PieceType(int[,] piece) => CopyFrom(piece, 1);

        /// <summary>
        /// This method clears a piece to the specified color (usually 0 - black - as that is blank).
        /// </summary>
        /// <param name="c"></param> The color to clear the piece to.
        public void SetPiece(int c)
        {
            for (int y = 0; y < Constant.PieceSize; y++)
            {
                for (int x = 0; x < Constant.PieceSize; x++)
                {
                    _piece[y, x] = c;
                }
            }
        }
        
        /// <summary>
        /// This method copies a 2d integer array to the piece, multiplying each color in it by the specified color.
        /// The reason for the multiplication is because 1 is used where there should be a color and 0 is used where it should be blank.
        /// </summary>
        /// <param name="piece"></param> The 2d int array to copy.
        /// <param name="color"></param> The color to use.
        public void CopyFrom(int[,] piece, int color)
        {
            for (int y = 0; y < Constant.PieceSize; y++)
            {
                for (int x = 0; x < Constant.PieceSize; x++)
                {
                    _piece[y, x] = piece[y, x] * color;
                }
            }
        }

        /// <summary>
        /// This method copies a PieceType piece to the piece, multiplying each color in it by the specified color.
        /// </summary>
        /// <param name="piece"></param> The PieceType piece to copy.
        /// <param name="color"></param> The color to use.
        public void CopyFrom(PieceType piece, int color) => CopyFrom(piece.Piece, color);

        /// <summary>
        /// This method checks to see if the piece can be moved to the specified coordinates.
        /// </summary>
        /// <param name="x"></param> The horizontal position to check.
        /// <param name="y"></param> The vertical position to check.
        /// <returns></returns> true if the piece can move there, false otherwise.
        public bool CanMoveTo(int x, int y)
        {
            for (int yi = 0; yi < Constant.PieceSize; yi++)
            {
                for (int xi = 0; xi < Constant.PieceSize; xi++)
                {
                    if (_piece[yi, xi] != 0)
                    {
                        int xp = x + xi; //convenience variables
                        int yp = y + yi;
                        if (xp < 0 || xp >= Constant.Cols) //if the horzontal position is out of bounds
                        {
                            DoDebug.Write($"Can't move to {x}, {y} - out of bounds!"); //write debugging message
                            return false; //return false to say no, we can't move there
                        }
                        if (GameBoard.Board[yp, xp] != 0) //if the board is not empty there
                        {
                            DoDebug.Write($"Can't move to {x}, {y} - position occupied!"); //write debugging message
                            return false; //return false to say no, we can't move there
                        }
                    }
                }
            }
            DoDebug.Write($"Movement to {x}, {y} is legal."); //write debugging messagte
            return true; //return true to say yes, we can move there
        }

        /// <summary>
        /// This method rotates the piece.
        /// </summary>
        public void Rotate()
        {
            int[,] piece = new int[Constant.PieceSize, Constant.PieceSize]; //we will need a new piece to handle the rotation
            for (int y = 0; y < Constant.PieceSize; y++)
            {
                for (int x = 0; x < Constant.PieceSize; x++)
                {
                    piece[y, x] = _piece[(Constant.PieceSize - 1) - x, y]; //rotate the piece
                }
            }
            for (int y = 0; y < Constant.PieceSize; y++)
            {
                for (int x = 0; x < Constant.PieceSize; x++)
                {
                    _piece[y, x] = piece[y, x]; //copy the new piece over so the rotation happens in-place
                }
            }
        }

        /// <summary>
        /// This method paints a piece using the specified graphics object, the starting x and y position, the size of the positions, and the color to be used.
        /// </summary>
        /// <param name="g"></param> The graphics object to use.
        /// <param name="startx"></param> The starting horizontal position.
        /// <param name="starty"></param> The starting vertical position.
        /// <param name="size"></param> The size to use for each spot in the piece.
        /// <param name="color"></param> The color to paint the piece.
        public void PaintPiece(Graphics g, int startx, int starty, int size, Color color)
        {
            using SolidBrush brush = new(color);
            {
                for (int y = 0; y < Constant.PieceSize; y++)
                {
                    for (int x = 0; x < Constant.PieceSize; x++)
                    {
                        if (_piece[y, x] != 0) //if colored
                        {
                            g.FillRectangle(brush, x * size + startx, y * size + starty, size - 1, size - 1); //paint it
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// This class contains the game board and some methods for interacting with it.
    /// </summary>
    public static class GameBoard
    {
        public static int[,] Board { get => _board; set => _board = value; }  //the game board and its public interface
        private static int[,] _board = new int[Constant.Rows, Constant.Cols];

        /// <summary>
        /// This method simply fills the game board with a numeric color.  It will normally be used with 0 for black as that is considered the background color of the game board.
        /// </summary>
        /// <param name="color"></param>  The color to use - likely 0 (black).
        public static void SetBoard(int color)
        {
            for (int y = 0; y < Constant.Rows; y++)
            {
                for (int x = 0; x < Constant.Cols; x++)
                {
                    _board[y, x] = color;
                }
            }
        }

        /// <summary>
        /// This method looks through the game board for moving pieces.
        /// </summary>
        /// <returns></returns> true if there are moving pieces, false otherwise.
        public static bool IsMoving()
        {
            for (int y = 0; y < Constant.Rows; y++)
            {
                for (int x = 0; x < Constant.Cols; x++)
                {
                    if (_board[y, x] >= 10)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// This method adjusts any part of the board that is "moving" so that it no longer is.  A moving color is one that has 10 added to it.
        /// </summary>
        public static void StopMoving()
        {
            for (int y = 0; y < Constant.Rows; y++)
            {
                for (int x = 0; x < Constant.Cols; x++)
                {
                    if (_board[y, x] >= 10) //if spot is moving
                    {
                        _board[y, x] -= 10; //stop it
                    }
                }
            }
        }

        /// <summary>
        /// This method blanks any piece of the board that is moving, changing that spot to 0 (black - our background color).
        /// </summary>
        public static void ClearMoving()
        {
            for (int y = 0; y < Constant.Rows; y++)
            {
                for (int x = 0; x < Constant.Cols; x++)
                {
                    if (_board[y, x] >= 10) //if spot is moving
                    {
                        _board[y, x] = 0; //clear it
                    }
                }
            }
        }

        /// <summary>
        /// This method finds a filled line on the board and returns the line number.
        /// </summary>
        /// <returns></returns> the line number for a filled line or -1 if no filled line found
        public static int FilledLine()
        {
            for (int y = 0; y < Constant.Rows; y++)
            {
                if (LineAllFilled(y)) //check line for being filled
                {
                    return y; //return line number if it is
                }
            }
            return -1; //return -1 to show no filled lines found
        }

        /// <summary>
        /// This private method complements the FilledLine method.  It checks a particular line to see if it is filled.
        /// </summary>
        /// <param name="line"></param> The line to check.
        /// <returns></returns> true if line is filled, false otherwise.
        private static bool LineAllFilled(int line)
        {
            for (int x = 0; x < Constant.Cols; x++)
            {
                int spot = _board[line, x]; //convenience variable
                if (spot == 0 || spot >= 10) //if spot on line is blank (0) or a moving piece (greater than 9)
                {
                    return false; //return false to show line is not filled
                }
            }
            return true; //since we got to here after checking each spot, the line must be filled, so return true
        }

        /// <summary>
        /// This method removes or deletes a line from the game board, filling in the top of the board with a new blank line.
        /// </summary>
        /// <param name="line"></param> This is the line to remove.
        public static void RemoveLine(int line)
        {
            for (int i = line; i >= 0; i--)
            {
                for (int x = 0; x < Constant.Cols; x++)
                {
                    if (i == 0) //if we are at the top line
                    {
                        _board[i, x] = 0; //set it to 0s (blank)
                    }
                    else //otherwise
                    {
                        _board[i, x] = _board[i - 1, x]; //copy the line from the line above it
                    }
                }
            }
        }

        /// <summary>
        /// This method simply checks if there is a moving piece at the bottom of the game board.
        /// </summary>
        /// <returns></returns> true if there is a moving piece there, false otherwise
        public static bool PieceAtBottom()
        {
            for (int x = 0; x < Constant.Cols; x++)
            {
                if (_board[Constant.Rows - 1, x] >= 10) //if there is a moving piece on the bottom line
                {
                    return true; //return true to say yes, moving piece found
                }
            }
            return false; //fall through to say no, no moving piece found
        }

        /// <summary>
        /// This slightly complex method will add a piece to the game board.  It copies all the non-zero parts of the piece.
        /// It also handles if there is something below the piece, stopping the piece (glueing it in place) if so.
        /// </summary>
        /// <param name="piece"></param> The piece to add to the board.
        /// <param name="piecex"></param> The horizontal position to add the piece.
        /// <param name="piecey"></param> The vertical position to add the piece.
        public static void AddPiece(PieceType piece, int piecex, int piecey)
        {
            bool hitsomething = false;
            for (int y = 0; y < Constant.Rows; y++) //for each spot in game board
            {
                for (int x = 0; x < Constant.Cols; x++)
                {
                    for (int py = 0; py < Constant.PieceSize; py++) //for each position in piece
                    {
                        for (int px = 0; px < Constant.PieceSize; px++)
                        {
                            if (piecex + px == x && piecey + py == y) //if that piece position is the same as the game board spot
                            {
                                if (piece.Piece[py, px] != 0) //if piece position is not blank
                                {
                                    if (y < Constant.Rows - 1) //if it is before the bottom row
                                    {
                                        if (Board[y + 1, x] != 0 && piece.Piece[py, px] != 0) //if there is something solid below the piece
                                        {
                                            hitsomething = true; //set a flag so we can get the rest of the piece copied in
                                        }
                                    }
                                    Board[y, x] = piece.Piece[py, px]; //copy it to the gameboard
                                }
                            }
                        }
                    }
                }
            }
            if (hitsomething) //if we hit anything (other than the bottom of the board, naturally!)
            {
                StopMoving(); //stop moving piece (glue it in place)
            }
        }

        /// <summary>
        /// This method handles painting the game board to a bitmap.
        /// </summary>
        /// <param name="pbox"></param> This is the bitmap we are painting to.
        /// <param name="back"></param> This is the background color to use (it will be the small lines segmenting the individual spots).
        public static void PaintBoard(Image pbox, Color back)
        {
            int hsize = pbox.Width / Constant.Cols; //compute sizes
            int vsize = pbox.Height / Constant.Rows;
            using Graphics g = Graphics.FromImage(pbox);
            {
                g.Clear(back); //clear the board to background color
                for (int y = 0; y < Constant.Rows; y++)
                {
                    for (int x = 0; x < Constant.Cols; x++)
                    {
                        int gbspot = Board[y, x]; //convenience variable
                        if (gbspot >= 10) //if spot is moving
                        {
                            gbspot -= 10; //count it as not moving
                        }
                        using SolidBrush b = new(gbspot switch
                        {
                            1 => Color.Blue,
                            2 => Color.Aqua,
                            3 => Color.Green,
                            4 => Color.Yellow,
                            5 => Color.Orange,
                            6 => Color.Red,
                            7 => Color.Magenta,
                            8 => Color.Gray,
                            9 => Color.White,
                            _ => Color.Black
                        }); //make a brush out of the color there
                        {
                            g.FillRectangle(b, x * hsize, y * vsize, hsize - 1, vsize - 1); //draw filled rectangle leaving 1 pixel on right and bottom
                        }

                    }
                }
            }
        }
    }

    /// <summary>
    /// This class is responsible for writing out debugging messages - currently to the debug console in Visual Studio.
    /// </summary>
    public static class DoDebug
    {
        /// <summary>
        /// This message will optionally write a debugging message.
        /// </summary>
        /// <param name="message"></param> The message to write.
        public static void Write(string message)
        {
            if (Constant.IsDebug) //if debugging is turned on
            {
                System.Diagnostics.Debug.WriteLine(message); //write the message
            }
        }
    }

    /// <summary>
    /// This class contains publicly available constants for the program.
    /// </summary>
    public class Constant
    {
        public const int MajorVersion = 0; //major version #
        public const int MinorVersion = 1; //minor version #
        public const int Revision = 0; //revision #
        public const string ReleaseType = "alpha"; //release type (alpha, beta, final, etc)
        public const string ProgramName = "WinTetris"; //program name
        public const int InitialSpeed = 2000; //initial speed used by the timer (milliseconds)
        public const int MinimumSpeed = 1; //minimum speed to use with the timer
        public const int SpeedSub = 25; //amount to decrease the delay on count exceeding CountMax or when player deletes a piece
        public const int CountMax = 1000; //the value count should get to before speed is changed and count is reset
        public const int ScoreAdd = 100; //the amount to add to the score when a line is filled or when count exceeds CountMax
        public const int PieceSize = 5; //the size of our pieces
        public const int Rows = 30; //the number of rows in the game board
        public const int Cols = 10; //the number of columns in the game board
        public const bool IsDebug = true; //whether to do debugging messages

        public static string Version => $"{ProgramName} version {MajorVersion}.{MinorVersion}.{Revision}-{ReleaseType}";
    }
}
