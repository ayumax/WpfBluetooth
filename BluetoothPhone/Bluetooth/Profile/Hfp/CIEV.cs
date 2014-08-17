using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothPhone.Bluetooth.Profile.Hfp
{
    class CIEV : IHfpCommandParser
    {
        public void ReciveCommand(string command, Hfp hfp)
        {
            string[] commands = command.Split(new char[] { ',' });

            if (commands.Length >= 2)
            {
                int SetIndex = int.Parse(commands[0]);
                hfp.PhoneStatusValue.SetStatus(SetIndex, int.Parse(commands[1]));


                BluetoothPhone.Utils.CurrentDispatcher.Dispatch(new Action(() => hfp.DoPhoneStatusUpdate(SetIndex)));
            }
        }
    }
}
