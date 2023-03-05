/**
 *    Name: Player.cs
 *  Coders: Bohong Liu
 * Purpose: This class is a datacontracted class.
 *  This class represents an individual player in the game.
 */
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ConnectFour
{
    [DataContract]
    public class Player
    {
        // Data Members
        [DataMember]
        public int playerNumber;
        [DataMember]
        public int Wins;
        [DataMember]
        public string Colour;
        
        internal List<KeyValuePair<int, int>> Pieces;
        
        public Player(int number, string colour = "default")
        {
            this.playerNumber = number;
            this.Colour = colour;
            this.Wins = 0;
            this.Pieces = new List<KeyValuePair<int, int>>();
        }

        /// <summary>
        /// This resets the internal board reference
        /// </summary>
        internal void Reset()
        {
            Pieces = new List<KeyValuePair<int, int>>();
        }

        /// <summary>
        /// This method checks the internal board reference to see if the player has won the game
        /// </summary>
        /// <returns>If the player has won</returns>
        internal bool CheckWin()
        {
            // Can't win with less than 4 pieces on the board, so don't bother checkin yet
            if (Pieces.Count < 4)
            {
                return false;
            }

            // Perform a vertical, horizontal, and diagonal check for the player's pieces
            for (int i = 0; i < Pieces.Count; i++)
            {
                int veritcalCheck = 0;
                int horizontalCheck = 0;
                int diagonalCheck = 0;
                int backwardsDiagonalCheck = 0;
                for (int j = 0; j < Pieces.Count; j++)
                {
                    int currentColumn = Pieces[i].Key;
                    int currentRow = Pieces[i].Value;
                    int checkingColumn = Pieces[j].Key;
                    int checkingRow = Pieces[j].Value;

                    //Checks if the are 4 pieces in a column of this player
                    veritcalCheck += (checkingColumn == currentColumn && checkingRow == currentRow + 1) ? 1 : 0;
                    veritcalCheck += (checkingColumn == currentColumn && checkingRow == currentRow + 2) ? 1 : 0;
                    veritcalCheck += (checkingColumn == currentColumn && checkingRow == currentRow + 3) ? 1 : 0;

                    //Checks if the are 4 pieces in a row of this player
                    horizontalCheck += (checkingColumn == currentColumn + 1 && checkingRow == currentRow) ? 1 : 0;
                    horizontalCheck += (checkingColumn == currentColumn + 2 && checkingRow == currentRow) ? 1 : 0;
                    horizontalCheck += (checkingColumn == currentColumn + 3 && checkingRow == currentRow) ? 1 : 0;

                    //Checks if the are 4 pieces in a diagonal of this player
                    diagonalCheck += (checkingColumn == currentColumn + 1 && checkingRow == currentRow + 1) ? 1 : 0;
                    diagonalCheck += (checkingColumn == currentColumn + 2 && checkingRow == currentRow + 2) ? 1 : 0;
                    diagonalCheck += (checkingColumn == currentColumn + 3 && checkingRow == currentRow + 3) ? 1 : 0;

                    //Checks if the are 4 pieces in a diagonal backwards of this player
                    backwardsDiagonalCheck += (checkingColumn == currentColumn - 1 && checkingRow == currentRow + 1) ? 1 : 0;
                    backwardsDiagonalCheck += (checkingColumn == currentColumn - 2 && checkingRow == currentRow + 2) ? 1 : 0;
                    backwardsDiagonalCheck += (checkingColumn == currentColumn - 3 && checkingRow == currentRow + 3) ? 1 : 0;
                }
                if (veritcalCheck >= 3 || horizontalCheck >= 3 || diagonalCheck >= 3 || backwardsDiagonalCheck >= 3)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
