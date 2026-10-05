using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SettingsClone;
using SettingsClone.Services;
using SettingsClone.Views;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SettingsClone.ViewModels
{

    public class NetworkPageViewModel : ViewModelBase
    {
        private readonly INavigationService _nav;
        public ObservableCollection<ScannerViewModel> Scanners { get; set; }
        private const int listenPort = 11000;


        #region CONSTRUCTOR

        public NetworkPageViewModel(INavigationService nav)
        {
            _nav = nav;
            //StartListener();
        }
        #endregion

        /*....................................................................
        @
        @
        @                            INITIALIZE
        @
        @
        @....................................................................*/
        public void Initialize(object parameter)
        {
            if (parameter is ScannerPageViewModel scanner)
            {
                Scanners = scanner.Scanners;
            }

        }
        /*....................................................................
        @   end of Initialize
        @....................................................................*/
        /*....................................................................
        @
        @
        @
        @
        @   Start UDP listener
        @
        @
        @
        @....................................................................*/
        private static void StartListener()
        {
            // This constructor arbitrarily assigns the local port number.
            UdpClient udpClient = new UdpClient();
            //UdpClient udpClientB = new UdpClient();

            IPAddress ipAddress = new IPAddress([11,200,0,41]);

            try
            {
                udpClient.Connect(ipAddress, 11000);

                // Sends a message to the host to which you have connected.
                Byte[] sendBytes = Encoding.ASCII.GetBytes("Is anybody there?");

                udpClient.Send(sendBytes, sendBytes.Length);

                // Sends a message to a different host using optional hostname and port parameters.
                //udpClientB.Send(sendBytes, sendBytes.Length, "AlternateHostMachineName", 11000);

                //IPEndPoint object will allow us to read datagrams sent from any source.
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

                // Blocks until a message returns on this socket from a remote host.
                Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
                string returnData = Encoding.ASCII.GetString(receiveBytes);

                // Uses the IPEndPoint object to determine which of these two hosts responded.
                Console.WriteLine("This is the message you received " +
                                             returnData.ToString());
                Console.WriteLine("This message was sent from " +
                                            RemoteIpEndPoint.Address.ToString() +
                                            " on their port number " +
                                            RemoteIpEndPoint.Port.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                udpClient.Close();
                //udpClientB.Close();
            }
        }
        /*....................................................................
        @   end of StartListener
        @....................................................................*/

    }
}
