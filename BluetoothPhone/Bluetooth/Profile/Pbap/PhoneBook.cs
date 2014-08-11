using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothPhone.Bluetooth.Profile.Pbap
{
    class PhoneBook
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public Nullable<DateTime> Time { get; set; }

        public PhoneBook()
        {
            this.Name = "";
            this.PhoneNumber = "";
            this.EmailAddress = "";
            this.Time = null;
        }

        public PhoneBook(string Name, string PhoneNumber, string EmailAddress, DateTime Time)
        {
            this.Name = Name;
            this.PhoneNumber = PhoneNumber;
            this.EmailAddress = EmailAddress;
            this.Time = Time;
        }
    }
}
