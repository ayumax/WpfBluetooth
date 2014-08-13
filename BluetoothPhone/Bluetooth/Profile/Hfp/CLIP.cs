using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothPhone.Bluetooth.Profile.Hfp
{
    class CLIP : IHfpCommandParser
    {
        public void ReciveCommand(string command, Hfp hfp)
        {
            string[] commands = command.Split(new char[] { ',' });
            if (commands.Length > 1)
            {
                string phoneNumber = commands[0].Trim(new char[] { '\"' });
                hfp.DoRing(phoneNumber);
            }
        }
    }
}
