using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothPhone.Bluetooth.Profile.Hfp
{
    class HfpPhoneStatus
    {
// service: Service availability indication, where:
//      <value>=0 implies no service. No Home/Roam network available.
//      <value>=1 implies presence of service. Home/Roam network available.

// call: Standard call status indicator, where:
//      <value>=0 means there are no calls in progress
//      <value>=1 means at least one call is in progress

// callsetup: Bluetooth proprietary call set up status indicator4. Support for this indicator is optional for the HF. When supported, this indicator shall be used in conjunction with, and as an extension of the standard call indicator. Possible values are as follows:
//      <value>=0 means not currently in call set up.
//      <value>=1 means an incoming call process ongoing.
//      <value>=2 means an outgoing call set up is ongoing.
//      <value>=3 means remote party being alerted in an outgoing call.
//      See Section 8.9 in [2].

// callheld: Bluetooth proprietary call hold status indicator. Support for this indicator is mandatory for the AG, optional for the HF. Possible values are as follows:
//      0= No calls held
//      1= Call is placed on hold or active/held calls swapped
//      (The AG has both an active AND a held call)
//      2= Call on hold, no active call

// signal: Signal Strength indicator, where:
//      <value>= ranges from 0 to 5

// roam: Roaming status indicator, where:
//      <value>=0 means roaming is not active
//      <value>=1 means a roaming is active

// battchg: Battery Charge indicator of AG, where:
//      <value>=ranges from 0 to 5


        public const string STATUS_ALL          = "all";
        public const string STATUS_CALL         = "call";
        public const string STATUS_CALLSETUP    = "callsetup";
        public const string STATUS_SERVICE      = "service";
        public const string STATUS_SIGNAL       = "signal";
        public const string STATUS_ROAM         = "roam";
        public const string STATUS_BATTCHG      = "battchg";
        public const string STATUS_CALLHELD     = "callheld";

        public Dictionary<string, int> StatusPositionMap;
        public Dictionary<int, int> StatusMap;

        public HfpPhoneStatus()
        {
            StatusPositionMap = new Dictionary<string, int>();
            StatusMap = new Dictionary<int, int>();
        }

        public void InitStatus(string StatusName, int Index)
        {
            StatusPositionMap[StatusName] = Index;
        }

        public void SetStatus(int Index, int Status)
        {
            StatusMap[Index] = Status;
        }

        public int GetStatus(int Index)
        {
            return StatusMap[Index];
        }

        public int GetStatus(string StatusName)
        {
            return StatusMap[StatusPositionMap[StatusName]];
        }

        public string GetStatusName(int Index)
        {
            if (Index == 0)
            {
                return STATUS_ALL;
            }

            KeyValuePair<string, int>[] values = StatusPositionMap.ToArray();

            for (int i = 0; i < values.Length; ++i)
            {
                if (values[i].Value == Index)
                {
                    return values[i].Key;
                }
            }

            return String.Empty;
        }
    }
}
