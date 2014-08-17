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
        public event Action<string> OnUpdatePhoneStatus;

        private CancellationTokenSource TaskCancellation;
        private List<string> SendCmdList;

        private bool IsSendOK;

        public HfpPhoneStatus PhoneStatusValue;

        public Hfp()
            : base(BluetoothService.Handsfree)
        {
            IsSendOK = true;
            SendCmdList = new List<string>();
            PhoneStatusValue = new HfpPhoneStatus();
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
                SendCmdList.Add("AT+BRSF=185");

                //SendCmdList.Add("AT+BAC=1,2,12");

               

                SendCmdList.Add("AT+CIND=?");

                SendCmdList.Add("AT+CIND?");

                SendCmdList.Add("AT+CMER=3,0,0,1,0");

                //SendCmdList.Add("AT+CHLD=?");

          
                // 着信番号表示
                SendCmdList.Add("AT+CLIP=1");

                // Audio Connect
                //SendCmdList.Add("AT+BCC");

                //SendCmdList.Add("AT+BCS=1");
                
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

                            if (receiveValue == "OK" || receiveValue == "ERROR")
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

            
            Type commandtype = Type.GetType("BluetoothPhone.Bluetooth.Profile.Hfp." + className);
            if (commandtype != null)
            {
                IHfpCommandParser CommandParser = Activator.CreateInstance(commandtype) as IHfpCommandParser;
                if (CommandParser != null)
                {
                    string parameter = (values.Length > 1) ? values[1].Trim() : String.Empty;
                    CommandParser.ReciveCommand(parameter, this);
                }
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

                        Byte[] dcB = System.Text.Encoding.ASCII.GetBytes(SendCmdList.First() + "\r");
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
                SendCmdList.Add(String.Format("ATD{0}", PhoneNumber));
            }
        }

        public void Hook()
        {
            lock (SendCmdList)
            {
                //ATA
                SendCmdList.Add("ATA");
            }
        }

        public void Terminate()
        {
            lock (SendCmdList)
            {
                //ATH
                SendCmdList.Add("AT+CHUP");
            }
        }


        /* *************************************
         * For IHfpCommandParser
         * *************************************/

        public void DoRing(string PhoneNumber)
        {
            if (OnRing != null)
            {
                OnRing(PhoneNumber);
            }
        }

        public void DoPhoneStatusUpdate(int UpdateIndex)
        {
            if (OnUpdatePhoneStatus != null)
            {
                OnUpdatePhoneStatus(PhoneStatusValue.GetStatusName(UpdateIndex));
            }
        }
    }
}
