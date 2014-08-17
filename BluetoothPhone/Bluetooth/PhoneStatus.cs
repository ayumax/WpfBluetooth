using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothPhone.Bluetooth
{
    public class PhoneStatus
    {
        public enum PhoneStatusKind
        {
            phoneNormal = 0,            // 待ち受け状態
            phoneIncomingCalling,       // 着信中
            phoneOutgoingCalling,       // 発信中
            phoneConnect,               // 通話中
        }

        public PhoneStatusKind Status { get; set; }
    }
}
