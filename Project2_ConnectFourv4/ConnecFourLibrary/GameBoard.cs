/**
 *    Name: GameBoard.cs
 *  Coders: Bohong Liu
 * Purpose: This class houses the main ServiceContract that the client will 
 *  interact with. This class contains the game logic.
 */
using System.Collections.Generic;
using System.ServiceModel;

namespace ConnectFour
{
    /// <summary>
    /// This is the callback the service can call on the clients
    /// </summary>
    public interface ICallback {
        [OperationContract(IsOneWay = true)]
        void UpdateGame(GameInfo info);
    }

    /// <summary>
    /// This is the what the client can call on the service
    /// </summary>
    [ServiceContract(CallbackContract = typeof(ICallback))]
    public interface IGameBoard
    {
        [OperationContract]
        bool PlayPiece(int playerNumber, int column);
        [OperationContract]
        Player JoinGame();
        [OperationContract]
        void ResetBoard();
    }

    /// <summary>
    /// This is the main service class
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class GameBoard : IGameBoard
    {
        private int COLUMNS;
        private int ROWS;
        private List<Player> players;
        // Colume<counts starts from the bottom of the board, piece info> 
        private List<List<Player>> Board;
        private List<ICallback> callbacks = new List<ICallback>();
        private string[] COLOURS = { "red", "blue", "purple", "green" };
        private bool isRunning = false;

        public GameBoard()
        {
            COLUMNS = 9;
            ROWS = 7;
            players = new List<Player>();
            Board = new List<List<Player>>();
            for (int i = 0; i < COLUMNS; i++)
            {
                Board.Add(new List<Player>());
            }
        }

        /// <summary>
        /// Connects the client to the game.
        /// Reasons why a player may not join:
        ///     A game is already running
        ///     the number of players is full
        /// </summary>
        /// <returns>A player object for the client to use. Will be null if the client is not allowed to join</returns>
        public Player JoinGame()
        {
            if (isRunning)
            {
                return null;
            }

            ICallback cb = OperationContext.Current.GetCallbackChannel<ICallback>();
            if (!callbacks.Contains(cb))
            {
                int number = players.Count;
                if (number == COLOURS.Length)
                {
                    return null;
                }
                Player p = new Player(number, COLOURS[number]);
                players.Add(p);

                //Updates the game for all clients (to indicate a new player has joined)
                callbacks.Add(cb);
                GameInfo info = new GameInfo(players, Board, -1, 0, true);
                foreach (ICallback callBacks in callbacks)
                {
                    callBacks.UpdateGame(info);
                }
                
                return p;
            }
            return null;
        }

        /// <summary>
        /// This function allows the client to play a piece
        /// </summary>
        /// <param name="playerNumber">Which player is playing the piece</param>
        /// <param name="column">Which column to place the piece</param>
        /// <returns>If the player can place a piece in the given column. (should be false if the column is full)</returns>
        public bool PlayPiece(int playerNumber, int column)
        {
            if(!isRunning)
            {
                isRunning = true;
            }

            int row = Board[column].Count + 1;
            //Returns false i the column is full
            if(row > ROWS)
            {
                return false;
            }

           //Places the peice onto the board
            Board[column].Add(players[playerNumber]);
            players[playerNumber].Pieces.Add(new KeyValuePair<int,int>(column, row));
            
            //Updates all the clients with the new information
            GameInfo info = new GameInfo(players, Board, players[playerNumber].CheckWin() ? playerNumber : -1, playerNumber + 1 == players.Count ? 0 : playerNumber + 1, false);
            foreach (ICallback cb in callbacks)
            {
                cb.UpdateGame(info);
            }

            return true;
        }

        /// <summary>
        /// This function resets the game. And tells all connected players that the game has been reset
        /// </summary>
        public void ResetBoard()
        {
            // Create a new board object
            Board = new List<List<Player>>();

            // Add the players to the new board
            for (int i = 0; i < COLUMNS; i++)
            {
                Board.Add(new List<Player>());
            }

            // Reset each player object
            foreach(Player player in players)
            {
                player.Reset();
            }

            // Update the game state
            isRunning = false;
            GameInfo info = new GameInfo(players, Board, -1, 0, true);
            foreach (ICallback cb in callbacks)
            {
                cb.UpdateGame(info);
            }
        }
    }
}
