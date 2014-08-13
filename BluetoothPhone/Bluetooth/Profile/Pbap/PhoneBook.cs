using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothPhone.Bluetooth.Profile.Pbap
{
    public class PhoneBook
    {
        public string Name { get; set; }
        public List<string> PhoneNumbers { get; set; }
        public List<string> EmailAddresss { get; set; }
        public Nullable<DateTime> Time { get; set; }

        public PhoneBook()
        {
            this.Name = "";
            this.PhoneNumbers = new List<string>();
            this.EmailAddresss = new List<string>();
            this.Time = null;
        }

        public PhoneBook(string Name, string PhoneNumber, string EmailAddress, DateTime Time)
        {
            this.Name = Name;
            this.PhoneNumbers = new List<string>();
            this.PhoneNumbers.Add(PhoneNumber);
            this.EmailAddresss = new List<string>();
            this.PhoneNumbers.Add(EmailAddress);

            this.Time = Time;
        }
    }
}
