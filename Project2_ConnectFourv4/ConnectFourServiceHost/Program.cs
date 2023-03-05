/**
 *    Name: Program.cs
 *  Coders: Bohong Liu
 * Purpose: This is the service host for the connect four library.
 */
using System;
using System.ServiceModel;
using ConnectFour;

namespace ConnectFourServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = null;
            try
            {
                //Creates and opens the service
                host = new ServiceHost(typeof(GameBoard), new Uri("net.tcp://localhost:13000/ConnectFourLibrary/"));
                host.AddServiceEndpoint(typeof(IGameBoard), new NetTcpBinding(), "ConnectFourService");
                host.Open();
                Console.WriteLine("Connect Four Service Started.");
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                //Waits to close the service
                Console.ReadKey();
                host?.Close();
            }
        }
    }
}
