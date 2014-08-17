using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothPhone.Bluetooth.Profile.Hfp
{
    class CIND : IHfpCommandParser
    {
        private Hfp hfp;

        public void ReciveCommand(string command, Hfp hfp)
        {
            this.hfp = hfp;

            if (command.IndexOf('(') >= 0)
            {
                parseInitCommand(command);
            }
            else
            {
                parseStatusCommand(command);
            }
        }

        private void parseInitCommand(string command)
        {
            int Index = 1;

            string[] commands = command.Split(new char[] { ',' });
            foreach (string value in commands)
            {
                if (value.Substring(0, 2) == "(\"")
                {
                    hfp.PhoneStatusValue.InitStatus(value.Substring(2, value.Length - 3), Index);
                    ++Index;
                }
            }
        }

        private void parseStatusCommand(string command)
        {
            int Index = 1;

            string[] commands = command.Split(new char[] { ',' });
            foreach (string value in commands)
            {
                hfp.PhoneStatusValue.SetStatus(Index, int.Parse(value));
                ++Index;
            }


            BluetoothPhone.Utils.CurrentDispatcher.Dispatch(new Action(() => hfp.DoPhoneStatusUpdate(0)));
        }

    }
}
