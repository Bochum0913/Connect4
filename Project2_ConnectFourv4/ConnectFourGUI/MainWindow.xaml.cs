/**
* Class Name:	MainWindow.xaml.cs
* Purpose:		Code side of GUI
* Coders:       Bohong Liu
* Date:			April 7, 2022
*/

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ConnectFour;
using System.ServiceModel;
using System.Threading;

namespace ConnectFourGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public partial class MainWindow : Window, ICallback
    {
        // 2D List representing the game board
        List<List<Ellipse>> gb = new List<List<Ellipse>>();

        // Player icons
        List<Border> playerIcons = new List<Border>();

        // Local variables
        IGameBoard board;
        private Player player;
        private int nextPlayer = 0;

        public MainWindow()
        {
            // Initialize the board and palyer
            DuplexChannelFactory<IGameBoard> channelFactory = new DuplexChannelFactory<IGameBoard>(this, new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:13000/ConnectFourLibrary/ConnectFourService"));
            board = channelFactory.CreateChannel();
            player = board.JoinGame();

            InitializeComponent();
            PlayerIndicator.Text = $"You Are:\nPlayer #{player.playerNumber + 1}";

            #region Initialize Grid
            List<Ellipse> col = new List<Ellipse>();
            col.Add(A0);
            col.Add(A1);
            col.Add(A2);
            col.Add(A3);
            col.Add(A4);
            col.Add(A5);
            col.Add(A6);
            gb.Add(col);
            col = new List<Ellipse>();
            col.Add(B0);
            col.Add(B1);
            col.Add(B2);
            col.Add(B3);
            col.Add(B4);
            col.Add(B5);
            col.Add(B6);
            gb.Add(col);
            col = new List<Ellipse>();
            col.Add(C0);
            col.Add(C1);
            col.Add(C2);
            col.Add(C3);
            col.Add(C4);
            col.Add(C5);
            col.Add(C6);
            gb.Add(col);
            col = new List<Ellipse>();
            col.Add(D0);
            col.Add(D1);
            col.Add(D2);
            col.Add(D3);
            col.Add(D4);
            col.Add(D5);
            col.Add(D6);
            gb.Add(col);
            col = new List<Ellipse>();
            col.Add(E0);
            col.Add(E1);
            col.Add(E2);
            col.Add(E3);
            col.Add(E4);
            col.Add(E5);
            col.Add(E6);
            gb.Add(col);
            col = new List<Ellipse>();
            col.Add(F0);
            col.Add(F1);
            col.Add(F2);
            col.Add(F3);
            col.Add(F4);
            col.Add(F5);
            col.Add(F6);
            gb.Add(col);
            col = new List<Ellipse>();
            col.Add(G0);
            col.Add(G1);
            col.Add(G2);
            col.Add(G3);
            col.Add(G4);
            col.Add(G5);
            col.Add(G6);
            gb.Add(col);
            col = new List<Ellipse>();
            col.Add(H0);
            col.Add(H1);
            col.Add(H2);
            col.Add(H3);
            col.Add(H4);
            col.Add(H5);
            col.Add(H6);
            gb.Add(col);
            col = new List<Ellipse>();
            col.Add(I0);
            col.Add(I1);
            col.Add(I2);
            col.Add(I3);
            col.Add(I4);
            col.Add(I5);
            col.Add(I6);
            gb.Add(col);
            playerIcons.Add(Player1Icon);
            playerIcons.Add(Player2Icon);
            playerIcons.Add(Player3Icon);
            playerIcons.Add(Player4Icon);
            #endregion
        }

        /// <summary>
        /// This method updates the GUI with the given GameInfo state
        /// </summary>
        /// <param name="info">The current state</param>
        public void UpdateGame(GameInfo info)
        {
            if(Thread.CurrentThread == Dispatcher.Thread)
            {
                var converter = new System.Windows.Media.BrushConverter();
                TextBlock childText = null;
                nextPlayer = info.NEXTPLAYER;
                //Sets all of the player icons to the default colour scheme
                for(int i = 0; i < info.PLAYERS.Count; i++)
                {
                    playerIcons[i].Background = (Brush)converter.ConvertFromString("#FFB8B8B8");
                    childText = playerIcons[i].Child as TextBlock;
                    childText.Foreground = (Brush)converter.ConvertFromString(info.PLAYERS[i].Colour);
                }
                //Highlights the playerIcons for the player whose turn it is
                playerIcons[nextPlayer].Background = (Brush)converter.ConvertFromString(info.PLAYERS[nextPlayer].Colour);
                childText = playerIcons[nextPlayer].Child as TextBlock;
                childText.Foreground = (Brush)converter.ConvertFromString("white");

                //This checks if the game has been reset
                if (info.RESET)
                {
                    //Shows only the number of player icons for the number of players connected
                    for(int i = 0; i < info.PLAYERS.Count; i++)
                    {
                        playerIcons[i].Visibility = Visibility.Visible;
                    }

                    //Sets each token in the grid to the colour white
                    foreach (List<Ellipse> column in gb)
                    {
                        foreach (Ellipse ellipse in column)
                        {
                            ellipse.Fill = (Brush)converter.ConvertFromString("white");
                        }
                    }
                    return;
                }

                //Fills in the token colours with the given board information
                for(int i = 0; i < info.BOARD.Count; i++)
                {
                    for(int r = 0; r < info.BOARD[i].Count; r++)
                    {
                        gb[i][r].Fill = (Brush)converter.ConvertFromString(info.BOARD[i][r].Colour);
                    }
                }

                //Displays if there was a winner
                if(info.WINNER > -1)
                {
                    MessageBox.Show(info.WINNER == player.playerNumber ? "You have won." : $"You lost. Player #{info.WINNER + 1} has won.");
                    nextPlayer = -1;
                }
            }
            else
            {
                //Sends to the dispatcher if this is not the correct thread
                Action<GameInfo> action = UpdateGame;
                Dispatcher.BeginInvoke(action, info);
            }
        }

        /// <summary>
        /// This button places the player's token into a specific column
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(player.playerNumber == nextPlayer)
            {
                FrameworkElement element = sender as FrameworkElement;
                int col = Int32.Parse(element.Uid);
                if(!board.PlayPiece(player.playerNumber, col))
                {
                    MessageBox.Show("Column Full. Place somewhere else.");
                }
            }
        }

        /// <summary>
        /// This methods tells the service that the game need to be reset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            board.ResetBoard();
        }
    }
}
