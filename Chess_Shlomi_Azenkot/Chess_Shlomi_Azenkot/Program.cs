namespace Chess_Shlomi_Azenkot
{
    public class ChessGameLauncher
    {
        public static void Main()
        {
            new ChessGame().Play();


        }
    }

    class ChessGame
    {
        Piece[,] chessBoard;
        Rook[] rookPieces;
        Pawn[] pawnPieces;
        King[] kingPieces;
        string[] boardStates;
        int moveRuleCounter, turnCount, currentCol, currentRow, targetCol, targetRow;
        bool isWhiteInCheck, isBlackInCheck;

        public string LetterToDigitConverter(bool isBlackTurn)
        {
            //משתנים שמירת קלט,קלט מומר,אימות 
            string input = "", changedInput = "";
            bool verifyInput = false;

            //כל עוד הקלט אינו מאומת המשך
            while (!verifyInput)
            {
                //תורות

                PrintToScreen($"{(isBlackTurn ? "Black" : "White")} player's turn. Please enter your move:");
                //מהלך או הצעת תיקו
                PrintToScreen("Your move (e.g., E2E4) or 'draw' to offer a draw:");

                input = GetInputUser().Trim().ToUpper();
                //איפוס לשמירת קלט מומר
                changedInput = "";
                //איפוס המונים לאותיות ומספרים
                int letterCount = 0, numberCount = 0;

                if (input == "DRAW")
                {
                    PrintToScreen("Do you agree? Please respond with 'yes' or 'no':");
                    string userInput = GetInputUser().Trim().ToUpper();

                    string[] possibleResponses = { "YES", "AGREED" };
                    if (possibleResponses.Contains(userInput))
                    {
                        PrintToScreen("The draw has been confirmed by your opponent.");
                        return "DRAW";
                    }
                    else
                    {
                        PrintToScreen("Your draw request was not accepted. Please enter your move.");

                        continue;
                    }
                }
                else if (input.Length == 0)
                {
                    PrintToScreen("Oops! It looks like your entry was empty. Let's try again:");
                    continue;
                }
                foreach (char currentChar in input)
                {
                    if (currentChar >= 'A' && currentChar <= 'H')
                    {
                        changedInput += currentChar - 'A' + 1;
                        letterCount++;
                    }
                    else if (currentChar >= '1' && currentChar <= '8')
                    {
                        changedInput += currentChar;
                        numberCount++;
                    }
                    else
                    {
                        PrintToScreen("Your input is invalid. Please try again.");

                        verifyInput = false;
                        break;
                    }
                }

                // בדיקה אם הקלט מכיל בדיוק 2 אותיות ו-2 מספרים
                if (letterCount == 2 && numberCount == 2)
                {
                    verifyInput = true; // קלט תקין
                }
                else
                {
                    PrintToScreen("The entered pattern is invalid. It should consist of 2 letters and 2 numbers. Try again.");

                }
            }

            return changedInput;
        }

        public void PrintToScreen(string message)
        {
            Console.WriteLine(message);
        }
        public string GetInputUser()
        {
            return Console.ReadLine();
        }

        public void Play()
        {
            printBoard();
            bool isBlackTurn = false, isGameOver = false;
            while (!isGameOver)
            {
                bool canMakeMove = false, isSlotAvailableForMove = false, isPlayerTurnCorrect = false;
                while (!isGameOver && (!canMakeMove || !isSlotAvailableForMove || !isPlayerTurnCorrect))
                {
                    if (!isGameOver)
                    {
                        string input = LetterToDigitConverter(isBlackTurn);
                        if (input == "DRAW")
                        {
                            isGameOver = true; break;
                        }
                        ConvertInput(input);
                        string messege = "";
                        canMakeMove = CanMakeMove(currentCol, currentRow, targetCol, targetRow);
                        if (canMakeMove)
                        {
                            isSlotAvailableForMove = IsSlotAvailableForMove(isBlackTurn);
                            if (isSlotAvailableForMove)
                            {
                                isPlayerTurnCorrect = IsPlayerTurnCorrect(isBlackTurn);
                                messege += !isPlayerTurnCorrect ? "this is not your piece try again" : "";
                            }
                        }
                        messege += canMakeMove ? "" : "Illegal step ";
                        PrintToScreen(messege);
                        if (canMakeMove && isSlotAvailableForMove && isPlayerTurnCorrect)
                        {
                            UpdatePiecesStatus(isBlackTurn);
                            if ((!isBlackTurn && isWhiteInCheck) || (isBlackTurn && isBlackInCheck))
                            {
                                canMakeMove = false; isSlotAvailableForMove = false; isPlayerTurnCorrect = false;
                            }
                            if (isBlackInCheck || isWhiteInCheck)
                                PrintToScreen("this is Check");

                        }
                    }
                }
                if (isGameOver)
                    break;
                isGameOver = CheckDraw();
                bool isKingUnderThreat = IsKingUnderThreat(isBlackTurn);
                if (isKingUnderThreat)
                    isGameOver = isGameOver || IsCheckmateConditionMet(isBlackTurn);

                isKingUnderThreat = IsKingUnderThreat(!isBlackTurn);
                if (isKingUnderThreat)
                    isGameOver = isGameOver || IsCheckmateConditionMet(!isBlackTurn);

                IncreaseTurns();
                isBlackTurn = !isBlackTurn;
                printBoard();
                if (isGameOver)
                {
                    String message;
                    if (!isWhiteInCheck && !isBlackInCheck)
                    {
                        message = "GAME OVER!!!!";
                    }
                    else if (isWhiteInCheck)
                    {
                        message = "GAME OVER WHITE LOSE!!!!";
                    }
                    else
                    {
                        message = "GAME OVER BLACK LOSE!!!!";
                    }

                    PrintToScreen(message);
                    break;
                }

            }
        }
        public void ConvertInput(string input)
        {
            if (input == null || input.Length != 4)
            {
                PrintToScreen("Invalid input. Please provide a string with exactly 4 characters.");
                return;
            }

            currentCol = int.Parse("" + input[0]) - 1;
            currentRow = int.Parse("" + input[1]) - 1;
            targetCol = int.Parse("" + input[2]) - 1;
            targetRow = int.Parse("" + input[3]) - 1;
        }
        public int GetCurrentTurns()
        {
            return turnCount;
        }
        public void IncreaseTurns()
        {
            turnCount++;

        }
        public bool CanMakeMove(int currentColumn, int currentRow, int newColumn, int newRow)
        {
            if (chessBoard[currentRow, currentColumn] == null)
                return false;

            bool canMakeMove = ((chessBoard[currentRow, currentColumn]).CanMakeMove(currentRow, currentColumn, newRow, newColumn, chessBoard, this)) ? true : false;
            if (canMakeMove)
                return true;

            return false;
        }
        public bool IsSlotAvailableForMove(bool isBlack)
        {
            bool isSlotAvailableForMove = (chessBoard[targetRow, targetCol] == null || isBlack != chessBoard[targetRow, targetCol].getIsBlack());
            if (isSlotAvailableForMove)
                return true;
            else
            {

                return false;
            }
        }
        ///  This function changes the position of the players and clears the previous position

        public void MakeMove(int curCol, int curRow, int newCol, int newRow, Piece tempPiece)
        {
            chessBoard[newRow, newCol] = chessBoard[curRow, curCol];
            chessBoard[curRow, curCol] = null;
        }
        public void ReverseMove(int curCol, int curRow, int newCol, int newRow, Piece tempPiece)
        {
            chessBoard[curRow, curCol] = chessBoard[newRow, newCol];
            chessBoard[newRow, newCol] = tempPiece;
        }

        public bool UpdatePiecesStatus(bool isBlackTurn)
        {
            Piece temporaryPiece = chessBoard[targetRow, targetCol];
            MakeMove(currentCol, currentRow, targetCol, targetRow, temporaryPiece);
            //update kings Locations
            int position = isBlackTurn ? 1 : 0;
            if (chessBoard[targetRow, targetCol] is King)
                kingPieces[position].setPosition(targetRow, targetCol);

            CanPawnBePromoted(isBlackTurn);
            isBlackInCheck = IsKingUnderThreat(true);
            isWhiteInCheck = IsKingUnderThreat(false);
            ///in case the player does move that threating on his king the move will be cancelled
            if ((isBlackInCheck && GetCurrentTurns() % 2 == 1) || (isWhiteInCheck && GetCurrentTurns() % 2 == 0))
            {
                if (chessBoard[targetRow, targetCol] is King)
                    kingPieces[position].setPosition(currentRow, currentCol);

                ReverseMove(currentCol, currentRow, targetCol, targetRow, temporaryPiece);
                PrintToScreen("your move threating on your king ,try again");

                return false;
            }
            if (chessBoard[targetRow, targetCol] is Pawn)
            {
                if (((Pawn)chessBoard[targetRow, targetCol]).getNeverMoved() && currentRow - targetRow == -2 || currentRow - targetRow == 2)
                    ((Pawn)chessBoard[targetRow, targetCol]).SetDoubleJumpTurn(GetCurrentTurns());
                //enPassnt case
                if (temporaryPiece == null)
                {
                    int direction = targetCol - currentCol == -1 ? -1 : 1;
                    if (targetCol - currentCol == +direction && chessBoard[currentRow, currentCol + direction] != null)
                        chessBoard[currentRow, currentCol + direction] = null;
                }
            }
            chessBoard[targetRow, targetCol].MarkAsMoved();
            if (chessBoard[targetRow, targetCol] is Pawn || temporaryPiece != null)
                moveRuleCounter = 0;

            return true;
        }

        public bool CheckDraw()
        {
            bool isFiftyMoveRule = CheckFiftyMoveRule();
            bool isThreeFoldRepetition = CheckThreeFoldRepetition();
            bool isDeadPosition = CheckDeadPosition();
            bool isStalemateWithWhite = CheckStalemate(true);
            bool isStalemateWithBlack = CheckStalemate(false);

            return isFiftyMoveRule || isThreeFoldRepetition || isDeadPosition || isStalemateWithWhite || isStalemateWithBlack;
        }

        public bool CheckFiftyMoveRule()
        {
            moveRuleCounter++;
            if (moveRuleCounter == 50)
            {
                PrintToScreen("this is a Fifty Move Rule");
                return true;
            }
            return false;
        }


        public bool CheckThreeFoldRepetition()
        {
            string temporary = "";
            for (int row = 7, y = 8, i = 0; row != -1; row--, y--)
            {
                for (int column = 0; column < 8; column++, i++)
                {
                    if (chessBoard[row, column] == null)
                    {
                        temporary += " "; continue;
                    }
                    temporary += chessBoard[row, column].ToString();
                    if (i < 8)
                        temporary += "" + pawnPieces[column]?.getNeverMoved() + pawnPieces[column + 8]?.getNeverMoved();
                    if (i < 4)
                        temporary += rookPieces[i]?.HasNeverMoved();
                    if (i < 2)
                        temporary += "" + kingPieces[i]?.getNeverMovedStatus();
                }
            }
            boardStates[GetCurrentTurns()] = temporary;

            for (int i = 0, counter = 0; i < GetCurrentTurns() + 1; i++)
            {
                if (boardStates[i] == temporary)
                    counter++;
                if (counter == 3)
                {
                    Console.WriteLine("Three Fold Repetition");
                    return true;
                }
            }
            return false;
        }



        public bool CheckStalemate(bool isBlack)
        {
            bool isKingUnderThreat = IsKingUnderThreat(isBlack);
            bool isCheckmateConditionMet = IsCheckmateConditionMet(isBlack);
            if (isCheckmateConditionMet && !isKingUnderThreat)
            {
                PrintToScreen("this is a Stalemate");
                return true;
            }
            return false;
        }
        public ChessGame()
        {
            rookPieces = new Rook[4];
            pawnPieces = new Pawn[16];
            kingPieces = new King[2];
            boardStates = new string[200];
            chessBoard = new Piece[8, 8];



            //שורה 1
            chessBoard[1, 0] = new Pawn(false);
            chessBoard[1, 1] = new Pawn(false);

            chessBoard[1, 2] = new Pawn(false);
            chessBoard[1, 3] = new Pawn(false);
            chessBoard[1, 4] = new Pawn(false);
            chessBoard[1, 5] = new Pawn(false);
            chessBoard[1, 6] = new Pawn(false);
            chessBoard[1, 7] = new Pawn(false);


            // אתחול בשורה 6
            chessBoard[6, 0] = new Pawn(true);
            chessBoard[6, 1] = new Pawn(true);
            chessBoard[6, 2] = new Pawn(true);
            chessBoard[6, 3] = new Pawn(true);
            chessBoard[6, 4] = new Pawn(true);
            chessBoard[6, 5] = new Pawn(true);
            chessBoard[6, 6] = new Pawn(true);
            chessBoard[6, 7] = new Pawn(true);


            // Initialize rooks, knights, and bishops

            chessBoard[0, 0] = rookPieces[0] = new Rook(false);
            chessBoard[0, 7] = rookPieces[1] = new Rook(false);
            chessBoard[7, 0] = rookPieces[2] = new Rook(true);
            chessBoard[7, 7] = rookPieces[3] = new Rook(true);


            chessBoard[0, 1] = new Knight(false);
            chessBoard[0, 6] = new Knight(false);
            chessBoard[7, 1] = new Knight(true);
            chessBoard[7, 6] = new Knight(true);

            chessBoard[0, 2] = new Bishop(false);
            chessBoard[0, 5] = new Bishop(false);
            chessBoard[7, 2] = new Bishop(true);
            chessBoard[7, 5] = new Bishop(true);

            // Initialize queens

            chessBoard[0, 3] = new Queen(false);
            chessBoard[7, 3] = new Queen(true);


            // Initialize kings
            chessBoard[0, 4] = kingPieces[0] = new King(0, 4, false, false, kingPieces);
            chessBoard[7, 4] = kingPieces[1] = new King(7, 4, true, true, kingPieces);

            /*
          


            מקוצר
             * 
            // Initialize pawns
            
            for (int col = 0; col < 8; col+=4)
            {
                chessBoard[1, col] = pawnPieces[col] = new Pawn(false);
                chessBoard[6, col] = pawnPieces[col + 8] = new Pawn(true);
            }
            

            // Initialize rooks, knights, bishops, and rookPieces array
            int[] rookPositions = { 0, 7 };
            foreach (var pos in rookPositions)
            {
                chessBoard[0, pos] = rookPieces[pos / 4] = new Rook(false);
                chessBoard[7, pos] = rookPieces[2 + pos / 4] = new Rook(true);
                chessBoard[0, pos == 0 ? 1 : 6] = new Knight(false);
                chessBoard[7, pos == 0 ? 1 : 6] = new Knight(true);
                chessBoard[0, pos == 0 ? 2 : 5] = new Bishop(false);
                chessBoard[7, pos == 0 ? 2 : 5] = new Bishop(true);
            }

            // Initialize queens
            chessBoard[0, 3] = new Queen(false);
            chessBoard[7, 3] = new Queen(true);

            // Initialize kings
            chessBoard[0, 4] = kingPieces[0] = new King(0, 4, false, false, kingPieces);
            chessBoard[7, 4] = kingPieces[1] = new King(7, 4, true, true, kingPieces);
            */
        }

        public void printBoard()
        {
            string boardOutput = "   A   B   C   D   E   F   G   H\n";
            boardOutput += " +---+---+---+---+---+---+---+---+\n";
            for (int row = 7; row >= 0; row--)
            {
                boardOutput += (row + 1) + "|";
                for (int column = 0; column < 8; column++)
                {
                    boardOutput += " ";
                    if (chessBoard[row, column] == null)
                    {
                        boardOutput += "  ";
                    }
                    else
                    {
                        string piece = chessBoard[row, column].ToString().ToUpper();
                        if (piece.Length == 2)
                        {
                            boardOutput += piece;
                        }
                        else
                        {
                            boardOutput += piece + " ";
                        }
                    }
                    boardOutput += "|";
                }
                boardOutput += (row + 1); // Add row number on the right side
                boardOutput += "\n +---+---+---+---+---+---+---+---+\n";
            }
            boardOutput += "   A   B   C   D   E   F   G   H"; // Add letters at the bottom
            PrintToScreen(boardOutput);
        }
        public bool CanPawnBePromoted(bool isBlackTurn)
        {
            bool verifyInput = true;
            for (int column = 0; column < 8 && verifyInput; column++)
            {
                if (chessBoard[0, column] != null && chessBoard[0, column] is Pawn || chessBoard[7, column] != null && chessBoard[7, column] is Pawn)
                {
                    int Side = !isBlackTurn ? 7 : 0;
                    while (verifyInput)
                    {
                        PrintToScreen("Please type the character you want to promote the pawn to, press 1 for Rook, 2 for Bishop, 3 for Queen, or 4 for Knight.");

                        string input = GetInputUser().ToUpper();

                        if (input == "1")
                        {
                            chessBoard[Side, column] = new Rook(isBlackTurn);
                            verifyInput = false;
                        }
                        else if (input == "2")
                        {
                            chessBoard[Side, column] = new Bishop(isBlackTurn);
                            verifyInput = false;
                        }
                        else if (input == "3")
                        {
                            chessBoard[Side, column] = new Queen(isBlackTurn);
                            verifyInput = false;
                        }
                        else if (input == "4")
                        {
                            chessBoard[Side, column] = new Knight(isBlackTurn);
                            verifyInput = false;
                        }
                    }
                }
            }
            if (!verifyInput)
                return true;
            return false;
        }
        public bool CheckDeadPosition()
        {
            int Kings = 0, Knights = 0, Bishops = 0, AllPieces = 0;
            // סריקת כל הלוח לספירת הקומפוננטות
            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    if (chessBoard[row, column] != null) // בדיקה שהתא לא ריק
                    {
                        AllPieces++; // ספירת כל חלקי השחמט
                        if (chessBoard[row, column] is King) Kings++;
                        else if (chessBoard[row, column] is Knight) Knights++;
                        else if (chessBoard[row, column] is Bishop) Bishops++;
                    }
                }
            }

            // בדיקת תנאי מצב מת
            if ((Kings == 2 && AllPieces == 2) || // מלך מול מלך
                (Kings == 2 && AllPieces == 3 && Bishops == 1) || // מלך ורץ מול מלך
                (Kings == 2 && AllPieces == 3 && Knights == 1)) // מלך ופרש מול מלך
            {
                PrintToScreen("This is a dead position.");
                return true;
            }

            return false; // אם אף אחד מהתנאים לא התקיים, המשחק לא במצב מת
        }


        public bool IsPlayerTurnCorrect(bool isBlackTurn)
        {
            if (isBlackTurn == chessBoard[currentRow, currentCol]?.getIsBlack())
                return true;

            return false;
        }

        //בדיקת סכנה לפי סוג המלך
        public bool IsKingUnderThreat(bool isBlackTurn)
        {
            //0 ללבן
            int position = isBlackTurn ? 1 : 0;

            for (int row = 7, y = 8; row != -1; row--, y--)
                for (int column = 0; column < 8; column++)
                {
                    bool canMakeMove = false;
                    //אם יש כלי של היריב בדוק אם הוא יכול לזוז למיקום המלך
                    if (chessBoard[row, column] != null && isBlackTurn != chessBoard[row, column].getIsBlack())
                        canMakeMove = CanMakeMove(column, row, kingPieces[position].getColumn(), kingPieces[position].getRow());
                    //אם כן מלך באיום
                    if (canMakeMove)
                        return true;
                }
            return false;
        }
        //בדיקה שחמט
        public bool IsCheckmateConditionMet(bool isBlackTurn)
        {
            for (int row = 7; row != -1; row--)
                for (int column = 0; column < 8; column++)
                    // בדיקה אם המשבצת מכילה כלי של השחקן הנוכחי
                    if (chessBoard[row, column] != null && isBlackTurn == chessBoard[row, column].getIsBlack())
                        // ניסיון לבצע כל מהלך אפשרי עבור הכלי הנוכחי
                        for (int nextRow = 7; nextRow != -1; nextRow--)
                            for (int nextColumn = 0; nextColumn < 8; nextColumn++)
                            {
                                Piece temporaryPiece = chessBoard[nextRow, nextColumn];
                                // בדיקה אם המהלך חוקי (כולל לכלוך כלי יריב או הזזה למשבצת ריקה)
                                if (temporaryPiece == null || isBlackTurn != temporaryPiece.getIsBlack())
                                {
                                    bool LegalMove = CanMakeMove(column, row, nextColumn, nextRow);
                                    if (LegalMove)
                                    {
                                        // ביצוע המהלך ובדיקה אם המלך עדיין נמצא תחת איום לאחר המהלך
                                        MakeMove(column, row, nextColumn, nextRow, temporaryPiece);
                                        bool isKingUnderThreat = IsKingUnderThreat(isBlackTurn);
                                        // ביטול המהלך
                                        ReverseMove(column, row, nextColumn, nextRow, temporaryPiece);
                                        // אם המהלך מנע שחמט, אין מצב שחמט
                                        if ((chessBoard[row, column] is King) || (!isKingUnderThreat))
                                            return false;
                                    }
                                }
                            }
            // אם אין מהלך שמונע שחמט, שחמט
            return true;
        }
        //הצרחה קטנה אפשרית
        public bool CheckShortCastlingPossible(bool isBlack)
        {
            int position = isBlack ? 1 : 0;

            if (!isBlack && !(chessBoard[0, 7] != null && rookPieces[1]?.HasNeverMoved() == true && kingPieces[0].getNeverMovedStatus() && chessBoard[0, 5] == null && chessBoard[0, 6] == null))
                return false;
            if (isBlack && !(chessBoard[7, 7] != null && rookPieces[3]?.HasNeverMoved() == true && kingPieces[1].getNeverMovedStatus() && chessBoard[7, 5] == null && chessBoard[7, 6] == null))
                return false;

            for (int row = 7, y = 8; row != -1; row--, y--)
                for (int column = 0; column < 8; column++)
                    if (chessBoard[row, column] != null && isBlack != chessBoard[row, column].getIsBlack())
                        for (int x = 0; x < 3; x++)
                        {
                            bool canMakeMove = CanMakeMove(column, row, (kingPieces[position].getColumn() + x), kingPieces[position].getRow());
                            if (canMakeMove)
                                return false;
                        }
            return true;
        }
        public bool CheckLongCastlingPossible(bool isBlack)
        {
            int position = isBlack ? 1 : 0;

            if (isBlack && !(rookPieces[2]?.HasNeverMoved() == true && kingPieces[1].getNeverMovedStatus() && chessBoard[7, 1] == null && chessBoard[7, 2] == null && chessBoard[7, 3] == null))
                return false;

            if (!isBlack && !(rookPieces[0]?.HasNeverMoved() == true && kingPieces[0].getNeverMovedStatus() && chessBoard[0, 3] == null && chessBoard[0, 2] == null && chessBoard[0, 1] == null))
                return false;

            for (int row = 7, y = 8; row != -1; row--, y--)
                for (int column = 0; column < 8; column++)
                    if (chessBoard[row, column] != null && isBlack != chessBoard[row, column].getIsBlack())
                        for (int x = 0; x < 3; x++)
                        {
                            bool canMakeMove = CanMakeMove(column, row, (kingPieces[position].getColumn() - x), kingPieces[position].getRow());

                            if (canMakeMove)
                                return false;
                        }
            return true;
        }
    }
    class Piece
    {
        protected bool isBlack;

        public Piece(bool isBlack)
        {
            this.isBlack = isBlack;
        }
        public bool getIsBlack()
        {
            return isBlack;
        }
        //אם הכלים זזו
        public virtual void MarkAsMoved()
        {
        }
        public virtual bool CanMakeMove(int currentRow, int currentColumn, int newRow, int newColumn, Piece[,] board, ChessGame game)
        {
            return true;
        }
    }
    class King : Piece
    {
        bool neverMoved;
        int row;
        int column;
        King[] kings;
        public override bool CanMakeMove(int currentRow, int currentColumn, int newRow, int newColumn, Piece[,] board, ChessGame game)
        {
            //מיקום מלך בהתאם לצבע 
            int position = isBlack ? position = 1 : position = 0;
            //בדיקה אם התנועה מתאימה לאחת מתנועות המלך האפשריות
            if ((newRow == currentRow - 1 && newColumn == currentColumn + 1) || (newRow == currentRow - 1 && newColumn == currentColumn - 1)
                || (newRow == currentRow + 1 && newColumn == currentColumn + 1) || (newRow == currentRow && newColumn == currentColumn + 1)
                || (newRow == currentRow && newColumn == currentColumn - 1) || (newRow == currentRow - 1 && newColumn == currentColumn)
                   || (newRow == currentRow + 1 && newColumn == currentColumn) || (newRow == currentRow + 1 && newColumn == currentColumn - 1))
            {
                if (board[newRow, newColumn] != null && board[newRow, newColumn] is King)
                    return true;
                // ביצוע המהלך בפועל לצורך בדיקת האיום על המלך
                Piece temporaryPiece = board[newRow, newColumn];
                kings[position].setPosition(newRow, newColumn);
                game.MakeMove(currentColumn, currentRow, newColumn, newRow, temporaryPiece);
                bool isKingUnderThreat = game.IsKingUnderThreat(isBlack);
                kings[position].setPosition(currentRow, currentColumn);
                game.ReverseMove(currentColumn, currentRow, newColumn, newRow, temporaryPiece);
                // אם המהלך לא מאיים על המלך, המהלך תקין
                if (!isKingUnderThreat)
                    return true;
            }
            // בדיקה עבור צריחה (קצרה וארוכה), תחילה לשחקן השחור
            bool checkLongCastlingPossible = false;
            if (isBlack)
            {
                // אם מדובר בהצרחה ארוכה עבור השחור
                if (currentRow == 7 && currentColumn == 4 && newRow == 7 && newColumn == 2)
                {
                    checkLongCastlingPossible = game.CheckLongCastlingPossible(true);
                    if (checkLongCastlingPossible)
                    {
                        board[7, 3] = board[7, 0]; board[7, 0] = null;
                        return true;
                    }
                }
                // אם מדובר בהצרחה קצרה עבור השחור
                if (currentRow == 7 && currentColumn == 4 && newRow == 7 && newColumn == 6)
                {
                    checkLongCastlingPossible = game.CheckShortCastlingPossible(true);
                    if (checkLongCastlingPossible)
                    {
                        board[7, 5] = board[7, 7]; board[7, 7] = null;
                        return true;
                    }
                }
            }
            //אותו הדבר רק עבור לבן
            if (!isBlack)
            {
                // צריחה ארוכה ללבן
                if (currentRow == 0 && currentColumn == 4 && newRow == 0 && newColumn == 2)
                {
                    checkLongCastlingPossible = game.CheckLongCastlingPossible(false);
                    if (checkLongCastlingPossible)
                    {
                        board[0, 3] = board[0, 0]; board[0, 0] = null;
                        return true;
                    }
                }
                // הצרחה קטנה ללבן
                if (currentRow == 0 && currentColumn == 4 && newRow == 0 && newColumn == 6)
                {
                    checkLongCastlingPossible = game.CheckShortCastlingPossible(false);
                    if (checkLongCastlingPossible)
                    {
                        board[0, 5] = board[0, 7]; board[0, 7] = null;
                        return true;
                    }
                }
            }
            return false;
        }
        //מחרוזת שמייצגת מלך ואם הוא שחור או לבן
        public override string ToString()
        {
            return (isBlack ? "B" : "W") + "K";
        }
        //מיקום המלך, אם זז או לא, צבעו, ומערך המלכים 
        public King(int row, int column, bool neverMoved, bool isBlack, King[] kings) : base(isBlack)
        {
            this.kings = kings;
            this.row = row;
            this.column = column;
            this.neverMoved = true;
        }
        //מלך זז להצרחה
        public override void MarkAsMoved()
        {
            neverMoved = false;
        }
        //לא זז
        public bool getNeverMovedStatus()
        {
            return neverMoved;
        }
        //עדכון עמודות ושורות המלך
        public void setRow(int row)
        {
            this.row = row;
        }
        public void setColumn(int column)
        {
            this.column = column;
        }
        public void setPosition(int row, int column)
        {
            setRow(row);
            setColumn(column);
        }
        public int getRow()
        {
            return row;
        }
        public int getColumn()
        {
            return column;
        }
    }
    class Pawn : Piece
    {
        //מהלך ראשון לצורך תזוזה של שני משבצות בפעם הראשונה
        bool isFirstMove;
        //מספר התור שרגלי ביצע קפיצה לצורך הכהה ד"ה
        int turnWithDoubleJump;
        //מהלכים לרגלי
        public override bool CanMakeMove(int currentRow, int currentColumn, int newRow, int newColumn, Piece[,] board, ChessGame game)
        {
            if (!isBlack && currentRow - newRow == -1 && board[newRow, newColumn] == null)
            {

                //  בדיקה אם הרגלי זז קדימה משבצת אחת והיא ריקה והכאה לצד ימין
                if (currentColumn < 7 && board[currentRow, currentColumn + 1] != null && currentColumn - newColumn == -1)
                    if (board[currentRow, currentColumn + 1] is Pawn && (((Pawn)board[currentRow, currentColumn + 1]).GetDoubleJumpTurn() == game.GetCurrentTurns() - 1))
                        if (IsValidEnPassantMove(currentRow, currentColumn, newRow, newColumn, game, board, false, true))
                            return true;




                //הכאה לצד ימין לרגלי לבן
                if (currentColumn > 0 && board[currentRow, currentColumn - 1] != null && currentColumn - newColumn == 1)
                    if (board[currentRow, currentColumn - 1] is Pawn && (((Pawn)board[currentRow, currentColumn - 1]).GetDoubleJumpTurn() == game.GetCurrentTurns() - 1))
                        if (IsValidEnPassantMove(currentRow, currentColumn, newRow, newColumn, game, board, false, false))
                            return true;
            }
            //רגלי שחור אותו הבדיקה
            if (isBlack && currentRow - newRow == 1 && board[newRow, newColumn] == null)
            {


                // אנ-פסאנט לצד שמאל לרגלי השחור
                if (currentColumn < 7 && board[currentRow, currentColumn + 1] != null && currentColumn - newColumn == -1)
                    if (board[currentRow, currentColumn + 1] is Pawn && (((Pawn)board[currentRow, currentColumn + 1]).GetDoubleJumpTurn() == game.GetCurrentTurns() - 1))
                        if (IsValidEnPassantMove(currentRow, currentColumn, newRow, newColumn, game, board, true, false))
                            if (currentRow + 1 == 4)
                            {
                                return true;

                            }



                // אנ-פסאנט לצד שמאל לרגלי השחור
                if (currentColumn > 0 && board[currentRow, currentColumn - 1] != null && currentColumn - newColumn == 1)
                    if ((board[currentRow, currentColumn - 1] is Pawn) && (((Pawn)board[currentRow, currentColumn - 1]).GetDoubleJumpTurn() == game.GetCurrentTurns() - 1))
                        if (IsValidEnPassantMove(currentRow, currentColumn, newRow, newColumn, game, board, true, true))
                            if (currentRow + 1 == 4)
                                return true;
            }
            // בדיקה עבור כל רגלי אם יש לכידה אפשרית במשבצת היעד
            if (board[newRow, newColumn] != null && board[newRow, newColumn].getIsBlack() != board[currentRow, currentColumn].getIsBlack())
            {
                if (isBlack && ((currentColumn - newColumn == 1 && currentRow - newRow == 1) || (currentColumn - newColumn == -1 && currentRow - newRow == 1)))
                    return true;

                if (!isBlack && ((currentColumn - newColumn == -1 && currentRow - newRow == -1) || (currentColumn - newColumn == +1 && currentRow - newRow == -1)))
                    return true;
            }

            int firstDirction = !isBlack ? -1 : 1;
            int secondDirction = !isBlack ? -2 : 2;
            //בדיקה אם זה מהלך ראשון,ואם זז שני משבצות
            if (isFirstMove && currentRow - newRow == secondDirction && currentColumn == newColumn && board[newRow, newColumn] == null && board[newRow + firstDirction, newColumn] == null)
                return true;

            if (currentColumn == newColumn && currentRow - newRow == firstDirction && board[newRow, newColumn] == null)
                return true;
            // אם אף אחד מהתנאים לא מתקיים, הרגלי אינו יכול לבצע את המהלך
            return false;
        }
        public bool IsValidEnPassantMove(int currentRow, int currentColumn, int newRow, int newColumn, ChessGame game, Piece[,] board, bool isBlack, bool isRightSide)
        {
            // כיון ביצוע אנפסנט בהתאם עם נע לימין או שמאל
            int Dirction = (isBlack && !isRightSide) || (!isBlack && isRightSide) ? 1 : -1;
            //שמירה במידה והמהלך לא תקף
            Piece temporaryPiece = board[currentRow, currentColumn + Dirction];
            // מרוקן את המשבצת מהקלט כדי לבדוק את השפעת המהלך על מצב המלך.
            board[currentRow, currentColumn + Dirction] = null;
            // מבצע את המהלך אנ-פסאנט בלוח.
            game.MakeMove(currentColumn, currentRow, newColumn, newRow, null);
            // בודק אם לאחר ביצוע המהלך המלך נמצא בסכנת שח.
            bool isKingUnderThreat = game.IsKingUnderThreat(isBlack);
            // מחזיר את הלוח למצבו הקודם על ידי החזרת הקלט למשבצתו וביטול המהלך אנ-פסאנט.
            board[currentRow, currentColumn + Dirction] = temporaryPiece;
            game.ReverseMove(currentColumn, currentRow, newColumn, newRow, null);
            // אם המהלך משים את המלך בסכנת שח, הוא לא תקף והפונקציה מחזירה false.
            if (isKingUnderThreat)
                return false;
            // אם המהלך לא משים את המלך בסכנת שח, הוא תקף והפונקציה מחזירה true.
            return true;
        }
        public override string ToString()
        {
            return (isBlack ? "B" : "W") + "P";
        }
        //סימון שטרם זז
        public Pawn(bool isBlack) : base(isBlack)
        {
            isFirstMove = true;
        }
        //תור שבו ביצע קפיצה כפולה
        public void SetDoubleJumpTurn(int turn)
        {
            this.turnWithDoubleJump = turn;
        }

        public int GetDoubleJumpTurn()
        {
            return turnWithDoubleJump;
        }
        //זז ולא יוכל לקפוץ 2
        public override void MarkAsMoved()
        {
            isFirstMove = false;
        }

        public bool getNeverMoved()
        {
            return isFirstMove;
        }
    }
    class Rook : Piece
    {
        bool neverMoved;
        public override bool CanMakeMove(int currentRow, int currentColumn, int newRow, int newColumn, Piece[,] board, ChessGame game)
        {
            if (((newRow == currentRow + 1) && (newColumn == currentColumn)) || ((newRow == currentRow - 1) && (newColumn == currentColumn))
                 || (newRow == currentRow) && (newColumn == currentColumn - 1) || (newRow == currentRow) && (newColumn == currentColumn + 1))
                return true;
            for (int i = 0; i < 9; i++)
            {
                ///למעלה
                if ((newRow == currentRow + i) && (newColumn == currentColumn))
                {
                    for (int j = newRow - 1, totalBlackSquares = 0; j > currentRow; j--)
                    {
                        if (board[j, currentColumn] == null)
                            totalBlackSquares++;

                        if (totalBlackSquares == newRow - currentRow - 1)
                            return true;
                    }
                }
                ///למטה
                else if ((newRow == currentRow - i) && (newColumn == currentColumn))
                {
                    for (int j = currentRow - 1, totalBlackSquares = 0; j > newRow; j--)
                    {
                        if (board[j, currentColumn] == null)
                            totalBlackSquares++;

                        if (totalBlackSquares == currentRow - newRow - 1)
                            return true;
                    }
                }
                //שמאל
                else if ((newRow == currentRow) && (newColumn == currentColumn - i))
                {
                    for (int j = currentColumn - 1, totalBlackSquares = 0; j > newColumn; j--)
                    {
                        if (board[currentRow, j] == null)
                            totalBlackSquares++;

                        if (totalBlackSquares == currentColumn - newColumn - 1)
                            return true;
                    }
                }
                //ימין
                else if ((newRow == currentRow) && (newColumn == currentColumn + i))
                {
                    for (int j = newColumn - 1, totalBlackSquares = 0; j > currentColumn; j--)
                    {
                        if (board[currentRow, j] == null)
                            totalBlackSquares++;

                        if (totalBlackSquares == newColumn - currentColumn - 1)
                            return true;
                    }
                }
            }
            return false;
        }
        public override string ToString()
        {
            return (isBlack ? "B" : "W") + "R";
        }
        //הגדרה שלא זז בהתחלה
        public Rook(bool isBlack) : base(isBlack)
        {
            this.neverMoved = true;
        }
        //עדכון למצב שזז
        public override void MarkAsMoved()
        { neverMoved = false; }
        //מחזיר אם זז מתחילת המשחק
        public bool HasNeverMoved()
        { return neverMoved; }
    }
    class Bishop : Piece
    {
        public override bool CanMakeMove(int currentRow, int currentColumn, int newRow, int newColumn, Piece[,] board, ChessGame game)
        {
            //כל הזויות לתנועת אלכסון
            for (int i = 0; i < 9; i++)
            {
                // אלכסון ימין למעלה.
                if ((newRow == currentRow + i) && (newColumn == currentColumn + i))
                {
                    for (int j = newColumn - 1, x = newRow - 1, totalBlackSquares = 0; j >= currentColumn; j--, x--)
                    {
                        if (board[x, j] == null)
                            totalBlackSquares++;
                        if (totalBlackSquares == newColumn - currentColumn - 1)
                            return true;
                    }
                }
                // אלכסון שמאל למעלה.
                else if ((newRow == currentRow + i) && (newColumn == currentColumn - i))
                {
                    for (int j = newColumn + 1, x = newRow - 1, totalBlackSquares = 0; j <= currentColumn; j++, x--)
                    {
                        if (board[x, j] == null)
                            totalBlackSquares++;
                        if (totalBlackSquares == currentColumn - newColumn - 1)
                            return true;
                    }
                }
                // אלכסון ימין למטה.
                else if ((newRow == currentRow - i) && (newColumn == currentColumn + i))
                {
                    for (int j = newColumn - 1, x = newRow + 1, totalBlackSquares = 0; j >= currentColumn; j--, x++)
                    {
                        if (board[x, j] == null)
                            totalBlackSquares++;
                        if (totalBlackSquares == newColumn - currentColumn - 1)
                            return true;
                    }
                }
                // אלכסון שמאל למטה.
                else if ((newRow == currentRow - i) && (newColumn == currentColumn - i))
                {
                    for (int j = newColumn + 1, x = newRow + 1, totalBlackSquares = 0; j <= currentColumn; j++, x++)
                    {
                        if (board[x, j] == null)
                            totalBlackSquares++;
                        if (totalBlackSquares == currentColumn - newColumn - 1)
                            return true;
                    }
                }
            }
            return false;
        }
        //צבע הרץ
        public Bishop(bool isBlack) : base(isBlack)
        {
        }
        public override string ToString()
        {
            return (isBlack ? "B" : "W") + "B";
        }
    }
    class Knight : Piece
    {
        //בידקת חוקיות מהלך פרש
        public override bool CanMakeMove(int currentRow, int currentColumn, int newRow, int newColumn, Piece[,] board, ChessGame game)
        {
            //עליון ימיני עליון שמאלי
            if ((newRow == currentRow + 2 && newColumn == currentColumn + 1 || (newRow == currentRow + 2 && newColumn == currentColumn - 1)))
                return true;
            //תחתון ימיני תחתון שמאלי
            else if ((newRow == currentRow - 2 && newColumn == currentColumn + 1) || (newRow == currentRow - 2 && newColumn == currentColumn - 1))
                return true;
            //ימיני עליון ימיני תחתון
            else if ((newRow == currentRow + 1 && newColumn == currentColumn + 2) || (newRow == currentRow + 1 && newColumn == currentColumn - 2))
                return true;
            //שמאלי עליון שמאלי תחתון
            else if ((newRow == currentRow - 1 && newColumn == currentColumn + 2) || (newRow == currentRow - 1 && newColumn == currentColumn - 2))
                return true;
            //אחרת לא חוקי
            return false;

        }
        //צבע
        public Knight(bool isBlack) : base(isBlack)
        { }
        public override string ToString()
        { return (isBlack ? "B" : "W") + "N"; }
    }
    class Queen : Piece
    {
        public override bool CanMakeMove(int currentRow, int currentColumn, int newRow, int newColumn, Piece[,] board, ChessGame game)
        {
            //שחזור לתנועה המלכה
            Bishop queenAsBishop = new Bishop(false);
            Rook queenAsRook = new Rook(false);
            //בדיקה למהלך כמו רץ או טורה
            bool canMakeMove = queenAsBishop.CanMakeMove(currentRow, currentColumn, newRow, newColumn, board, game) || queenAsRook.CanMakeMove(currentRow, currentColumn, newRow, newColumn, board, game);
            //אם אפשרי
            if (canMakeMove)
                return true;

            return false;

        }
        //צבע
        public Queen(bool isBlack) : base(isBlack)
        { }
        public override string ToString()
        { return (isBlack ? "B" : "W") + "Q"; }
    }


}

//בדיקות
/*
   string[] mate1 = { "f2f4", "e7e5", "f4e5", "d7d6", "e5d6", "f8d6", "g1f3", "g7g5", "h2h3", "d6g3" };
            string[] mate2 = { "d2d4", "f7f5","e2e4", "f5e4", "d1h5", "g7g6", "f1e2", "g6h5", "e2h5" };
            string[] mate3 = { "d2d4", "f7f5", "c1g5", "h7h6", "g5h4", "g7g5", "h4g3", "f5f4", "e2e3", "f4g3", "d1h5" };
            string[] mate4 = { "e2e4","e7e5", "g1f3", "f7f6", "f1c4", "c7c6", "f3e5", "f6e5", "d1h5", "e8e7", "h5e5" };
            string[] mate5 = { "e2e4", "d7d5", "h2h3", "d5e4", "f2f3", "g8f6", "f3e4", "f6e4", "f1c4", "e4d6", "c4b3", "e7e5", "d2d4", "d8h4", "e1f1", "d6e4", "g1f3", "h4f2"};
            string[] mate6 = { "f2f3", "e7e5", "g2g4", "d8h4"};
            string[] mate7 = { "E2E4", "E7E5", "G1F3", "F7F5", "E4F5", "D7D5", "F3E5", "C7C5", "E5C6", "B8C6", "D1H5", "G7G6", "F5G6",
                "D5D4", "G6H7", "E8E7", "H7G8", "H8G8", "H5E5", "C8E6", "F1B5", "D8D5", "E1G1", "D5E5", "B5C6", "G8H8", "C6B7", "E5H2" };
            string[] mate8 = { "g2g4", "d7d5", "g1f3", "b8c6", "f1h3", "d5d4", "e1g1", "e7e5", "c2c3", "d5d4", "e5e4", "f3g5", "d4c3", "b1c3", "d8g5", "c3e4", "g5c5", "e4c5", "f8c5", "d1a4", 
                "b7b6", "a4c6", "c8d7", "c6a8", "e8e7", "a8a7", "e2e4", "f7f5", "g4f5", "d7f5", "h3f5", "g8f6", "a7c7", "e7e8", "c7c8", "e8e7", "c8h8", "e7f7", "h8h7", "f7f8", "h7h8", "f8f7", 
                "d2d4", "f7e7", "d4c5", "f6h5", "f1d1", "e7f7", "d1d7", "f7f6", "h8g7", "h5g7", "c5c6", "f6e6", "f6g6", "f6g5", "f6f5", "c6c7", "g7e6", "c7c8", "f5g5", "f5g6", "c8f8", "g6h6", 
                "g6h5", "d7g7", "h5h4", "f8h8" };
            string[] staleMate1 = { "e2e3", "a7a5", "d1h5", "a8a6", "h5a5", "h7h5", "a5c7", "a6h6", "h2h4", "f7f6", "c7d7", "e8f7", "d7b7", "d8d3", "b7b8", "d3h7", "b8c8", "f7g6", "c8e6" };
            string[] staleMate2 = { "d2d4", "e7e5", "d1d2", "e5e4", "d2f4", "f7f5", "h2h3", "f8b4", "b1d2", "d7d6", "f4h2", "c8e6", "a2a4", "d8h4", "a1a3", "c7c5", "a3g3", "f5f4", 
                "f2f3", "e6b3", "d4d5", "b4a5", "c2c4", "e4e3"};
            string[] staleMate3 = { "f3f8", "g8f8"};
            string[] staleMate4 = { "d5d6", "c7d6" };
            string[] staleMate5 = { "d5d6" };
            string[] staleMate6 = { "a7f7"};
            string[] staleMate7 = { "a5a6", "b5b6", "a6b6" };
            string[] threefold1 = { "h1h2", "h8h7", "h2h1", "h7h8", "h1h2", "h8h7", "h2h1", "h7h8" };
            string[] threefold2 = { "h2h3", "h7h6", "h3h2", "h6h7", "h2h3", "h7h6", "h3h2", "h6h7", "h2h3", "h7h6", "h3h2", "h6h7" };
            //In those en_passant tests, you need to check if it allowes or not
            string[] en_passant_1 = { "B2B4", "F7F5", "B4B5", "H7H5", "D2D4", "F5F4", "D4D5", "C7C5", "B5C6" };
            string[] en_passant_2 = { "B2B4", "F7F5", "B4B5", "H7H5", "D2D4", "F5F4", "D4D5", "C7C5", "A2A3", "H5H4", "G2G4", "F4G3" };
            string[] en_passant_3 = { "B2B4", "F7F5", "B4B5", "F5F4", "A2A3", "A7A5", "E2E4", "C7C5", "B5C6", "G7G6", "G2G4", "F4G3" };
            //In those castling tests, you need to check if it allowes or not
            string[] atzracha_1 = { "B2B4", "B7B5", "C2C4", "C8A6", "C4C5", "B8C6", "A2A4", "F7F5", "A4B5", "G7G5", "B5B6", "H7H6", 
                "B6B7", "E7E5", "D1B3", "D8F6", "H2H3", "E8C8" };
            string[] atzracha_2 = { "B2B4", "B7B5", "C2C4", "C8A6", "C4C5", "B8C6", "A2A4", "F7F5", "A4B5", "G7G5", "B5B6", "H7H6", "B6B7", 
                "E7E5", "D1B3", "D8F6", "H2H3", "A6B7", "H3H4", "F8D6", "E2E3", "G8E7", "B4B5", "E8C8" };
            string[] atzracha_3 = { "B2B4", "B7B5", "C2C4", "C8A6", "C4C5", "B8C6", "A2A4", "F7F5", "A4B5", "G7G5", "B5B6", "H7H6", "B6B7", 
                "E7E5", "D1B3", "D8F6", "H2H3", "A6B7", "H3H4", "F8D6", "E2E3", "G8E7", "B4B5", "E7G6", "B3E6", "E8C8" };
            string[] atzracha_4 = { "B2B4", "B7B5", "C2C4", "C8A6", "C4C5", "B8C6", "A2A4", "F7F5", "A4B5", "G7G5", "B5B6", "H7H6", "B6B7", 
                "E7E5", "D1B3", "D8F6", "H2H3", "A6B7", "H3H4", "F8D6", "E2E3", "G8E7", "B4B5", "E7G6", "B3E6", "F6E7", "E6F7", "E8C8" };
*/