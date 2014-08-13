using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using InTheHand;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Bluetooth.AttributeIds;
using InTheHand.Net.Ports;
using InTheHand.Net.Sockets;

namespace BluetoothPhone.Bluetooth.Profile.Hfp
{
    class Hfp : BluetoothProfile
    {
        private CancellationTokenSource TaskCancellation;
        private List<string> SendCmdList;

        public Hfp()
            : base(BluetoothService.Handsfree)
        {
            SendCmdList = new List<string>();
        }

        protected override void OnConnected()
        {
            NetworkStream peerStream = LocalClient.GetStream();


            TaskCancellation = new CancellationTokenSource();

            var t = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    //if (!LocalClient.Connected)
                    //{
                    //    Connect(ConnectedDevice);
                    //    break;
                    //}

                    if (TaskCancellation.IsCancellationRequested)
                    {
                        TaskCancellation.Token.ThrowIfCancellationRequested();
                    }

                    ReceiveAndSend();
                    Thread.Sleep(1000);
                }

            }, TaskCancellation.Token);


            
           ServiceLevelConnectionInitialization();
        }

        private void ServiceLevelConnectionInitialization()
        {
            //0 EC and/or NR function
            //1 Call waiting and 3-way calling
            //2 CLI presentation capability
            //3 Voice recognition activation
            //4 Remote volume control
            //5 Enhanced call status
            //6 Enhanced call control
            //7-31 Reserved for future definition

            lock (SendCmdList)
            {
           
                SendCmdList.Add("AT+BRSF=20\r");

                SendCmdList.Add("AT+CIND=?\r");

                SendCmdList.Add("AT+CIND?\r");

                SendCmdList.Add("AT+CMER=3,0,0,1,0\r");


                SendCmdList.Add("AT+CLIP=1\r");

                lock (SendCmdList)
                {
                    if (SendCmdList.Count > 0)
                    {
                        try
                        {
                            if (LocalClient.Connected)
                            {
                                NetworkStream stream = LocalClient.GetStream();

                                Byte[] dcB = System.Text.Encoding.ASCII.GetBytes(SendCmdList.First());
                                stream.Write(dcB, 0, dcB.Length);

                                Console.WriteLine("Send:" + SendCmdList.First());
                                stream.Flush();

                                SendCmdList.RemoveAt(0);
                            }

                        }
                        catch
                        {
                        }
                    }
                }
                
            }
        }

        private void ReceiveAndSend() 
        {
            NetworkStream stream = LocalClient.GetStream();

            while (stream.DataAvailable)
            {
                Byte[] sRes = new Byte[200];
                if (stream.Read(sRes, 0, 199) > 0)
                {
                    string ReceiveString = System.Text.Encoding.ASCII.GetString(sRes);

                    Console.WriteLine(ReceiveString);

                    lock (SendCmdList)
                    {
                        if (SendCmdList.Count > 0)
                        {
                            try
                            {
                                if (LocalClient.Connected)
                                {
                                    Byte[] dcB = System.Text.Encoding.ASCII.GetBytes(SendCmdList.First());
                                    stream.Write(dcB, 0, dcB.Length);

                                    Console.WriteLine("Send:" + SendCmdList.First());
                                    stream.Flush();

                                    SendCmdList.RemoveAt(0);
                                }

                            }
                            catch
                            {
                                Console.WriteLine("Send Fail:" + SendCmdList.First());
                            }
                        }
                    }
                }
            }

           
        }

        public void Dial()
        {
          
                //SendCmdList.Add("AT+CMER\r");
                //SendCmdList.Add("AT+CIND=?\r");


                //SendCmdList.Add("AT+CLIP=1\r");

                //"ATD117;\r";

            

        }

    }
}
