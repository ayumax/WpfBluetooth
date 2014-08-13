using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Threading;
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
        public event Action<string> OnRing;

        private CancellationTokenSource TaskCancellation;
        private List<string> SendCmdList;

        private bool IsSendOK;

        public Hfp()
            : base(BluetoothService.Handsfree)
        {
            IsSendOK = true;
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
                    if (TaskCancellation.IsCancellationRequested)
                    {
                        TaskCancellation.Token.ThrowIfCancellationRequested();
                    }

                    Receive();
                    Send();
                    Thread.Sleep(1);
                }

            }, TaskCancellation.Token);


            
           ServiceLevelConnectionInitialization();
        }

        private void ServiceLevelConnectionInitialization()
        {
            lock (SendCmdList)
            {
                SendCmdList.Add("AT+BRSF=0\r");

                SendCmdList.Add("AT+CIND=?\r");

                SendCmdList.Add("AT+CIND?\r");

                SendCmdList.Add("AT+CMER=3,0,0,1,0\r");


                SendCmdList.Add("AT+CLIP=1\r");
            }
        }

        private void Receive() 
        {
            NetworkStream stream = LocalClient.GetStream();

            while (stream.DataAvailable)
            {
                Byte[] sRes = new Byte[200];

                if (stream.Read(sRes, 0, 199) > 0)
                {
                    string[] ReceiveStrings = System.Text.Encoding.ASCII.GetString(sRes).Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string receiveString in ReceiveStrings)
                    {
                        string receiveValue = receiveString.Trim(new char[] { '\r', '\n', '\0' });

                        if (!String.IsNullOrWhiteSpace(receiveValue))
                        {
                            Console.WriteLine(receiveValue);

                            if (receiveValue == "OK" || receiveValue == "ReceiveString")
                            {
                                IsSendOK = true;
                                Thread.Sleep(100);  // "OK"のあとすぐに送信すると受け付けてくれないため100ms待つ
                            }
                            else
                            {
                                OnRecive(receiveValue);
                            }
                        }
                    }
                }
            }

        }

        private void OnRecive(string receiveValue)
        {
            string[] values = receiveValue.Split(new char[] { ':' });
            string className = values[0].TrimStart(new char[] { '+' }).Trim();

            try
            {
                IHfpCommandParser CommandParser = Activator.CreateInstance(Type.GetType("BluetoothPhone.Bluetooth.Profile.Hfp." + className)) as IHfpCommandParser;
                if (CommandParser != null)
                {
                    string parameter = (values.Length > 1) ? values[1].Trim() : String.Empty;
                    CommandParser.ReciveCommand(parameter, this);
                }
            }
            catch
            {
            }
        }

        private void Send()
        {
            lock (SendCmdList)
            {
                if (SendCmdList.Count > 0 && IsSendOK)
                {
                    try
                    {
                        IsSendOK = false;

                        NetworkStream stream = LocalClient.GetStream();

                        Byte[] dcB = System.Text.Encoding.ASCII.GetBytes(SendCmdList.First());
                        stream.Write(dcB, 0, dcB.Length);

                        Console.WriteLine("Send:" + SendCmdList.First());

                        SendCmdList.RemoveAt(0);

                    }
                    catch
                    {
                        Console.WriteLine("Send Fail:" + SendCmdList.First());
                    }
                }
            }
        }

        public void Dial(string PhoneNumber)
        {
            lock (SendCmdList)
            {
                SendCmdList.Add(String.Format("ATD{0}\r", PhoneNumber));
            }
        }

        public void DoRing(string PhoneNumber)
        {
            System.Windows.Threading.Dispatcher dispatcher = System.Windows.Application.Current.Dispatcher;
            if (dispatcher.CheckAccess())
            {
                if (OnRing != null)
                {
                    OnRing(PhoneNumber);
                }
            }
            else
            {
                dispatcher.Invoke(() => DoRing(PhoneNumber));
            }

           
        }
    }
}
