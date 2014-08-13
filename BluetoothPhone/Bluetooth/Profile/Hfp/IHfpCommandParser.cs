using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothPhone.Bluetooth.Profile.Hfp
{
    interface IHfpCommandParser
    {
        void ReciveCommand(string command, Hfp hfp);
    }
}
