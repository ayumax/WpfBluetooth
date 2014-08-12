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
            peerStream.WriteTimeout = 5000;
            peerStream.ReadTimeout = 5000;

            TaskCancellation = new CancellationTokenSource();

            var t = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (TaskCancellation.IsCancellationRequested)
                    {
                        TaskCancellation.Token.ThrowIfCancellationRequested();
                    }

                    ReceiveAndSend();
                    Thread.Sleep(1);
                }

            }, TaskCancellation.Token);

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
                }
            }

            lock (SendCmdList)
            {
                while (SendCmdList.Count > 0)
                {
                    Byte[] dcB = System.Text.Encoding.ASCII.GetBytes(SendCmdList.First());
                    stream.Write(dcB, 0, dcB.Length);

                    SendCmdList.RemoveAt(0);
                }
            }
        }

        public void Dial()
        {
            lock (SendCmdList)
                {


                 SendCmdList.Add("AT+CMER\r");
                 SendCmdList.Add("AT+CIND=?\r");
                 SendCmdList.Add("AT+BRSF=\r");
                    SendCmdList.Add("AT+CLIP=1\r");

                    //String dialCmd1 = ;
                    //String dialCmd2 = ;
                    //String dialCmd3 = ;
                    //String dialCmd4 = "ATD117;\r";

                    //Byte[] dcB = System.Text.Encoding.ASCII.GetBytes(dialCmd1);
                    //peerStream.Write(dcB, 0, dcB.Length);

                    //Byte[] sRes = new Byte[200];
                    //peerStream.Read(sRes, 0, 199);
                    //Console.WriteLine("\n\r----------\n\r" + System.Text.Encoding.ASCII.GetString(sRes));

                    //dcB = System.Text.Encoding.ASCII.GetBytes(dialCmd2);
                    //peerStream.Write(dcB, 0, dcB.Length);

                    //peerStream.Read(sRes, 0, 199);
                    //Console.WriteLine("\n\r----------\n\r" + System.Text.Encoding.ASCII.GetString(sRes));

                    //dcB = System.Text.Encoding.ASCII.GetBytes(dialCmd3);
                    //peerStream.Write(dcB, 0, dcB.Length);

                    //peerStream.Read(sRes, 0, 199);
                    //Console.WriteLine("\n\r----------\n\r" + System.Text.Encoding.ASCII.GetString(sRes));

                    //dcB = System.Text.Encoding.ASCII.GetBytes(dialCmd4);
                    //peerStream.Write(dcB, 0, dcB.Length);
                }
            //});
        }

    }
}
