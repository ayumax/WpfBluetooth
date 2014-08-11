using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InTheHand;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Bluetooth.AttributeIds;
using InTheHand.Net.Ports;
using InTheHand.Net.Sockets;

namespace BluetoothPhone.Bluetooth
{
    public class Server
    {
        public Server()
        {
        }

        public void Listen(Guid service)
        {
            BluetoothListener listener = new BluetoothListener(BluetoothAddress.None, service);
            listener.Start(10);
            listener.BeginAcceptBluetoothClient(new AsyncCallback(AcceptConnection), listener);
        }

        void AcceptConnection(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                BluetoothClient remoteDevice = ((BluetoothListener)result.AsyncState).EndAcceptBluetoothClient(result);
            }
        }
    }
}
