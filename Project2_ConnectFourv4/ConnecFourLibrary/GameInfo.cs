/**
 *    Name: GameInfo.cs
 *  Coders: Bohong Liu
 * Purpose: This class is the data that will be sent to the clients when a game update occurrs.
 */
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ConnectFour
{
    /// <summary>
    /// This is the DataContract that will be sent to the client with details of an update
    /// </summary>
    [DataContract]
    public class GameInfo
    {
        [DataMember]
        public List<Player> PLAYERS { get; set; }
        [DataMember]
        public List<List<Player>> BOARD { get; set; }
        [DataMember]
        public int WINNER { get; set; }
        [DataMember]
        public int NEXTPLAYER { get; set; }
        [DataMember]
        public bool RESET { get; set; }

        public GameInfo(List<Player> players, List<List<Player>> board, int winner, int nextPlayer, bool isNewGame)
        {
            PLAYERS = players;
            BOARD = board;
            WINNER = winner;
            NEXTPLAYER = nextPlayer;
            RESET = isNewGame;
        }
    }
}
