using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BluetoothPhone.Bluetooth;
using TestBluetoothPhone.Common;

namespace TestBluetoothPhone
{
    class PhoneBookViewModel : BindableBase
    {
        private PhoneBook Book;
        private BluetoothPhone.PhoneManager phoneManager;

        public string Name
        {
            get { return maskStr(Book.Name); }
        }

        public string PhoneNumbers
        {
            get
            {
                if (Book.PhoneNumbers.Count == 0)
                {
                    return String.Empty;
                }

                return maskStr(Book.PhoneNumbers[0]);
            }
        }

        public string EmailAddresss
        {
            get
            {
                if (Book.EmailAddresss.Count == 0)
                {
                    return String.Empty;
                }

                return maskStr(Book.EmailAddresss[0]);
            }
        }

        public PhoneBookViewModel(PhoneBook Book, BluetoothPhone.PhoneManager phoneManager)
        {
            this.Book = Book;
            this.phoneManager = phoneManager;
        }

       

        private string maskStr(string fromStr)
        {
            int length = fromStr.Length;

            string retStr = fromStr;
            if (length > 3)
            {
                retStr = fromStr.Substring(0, 3);

                int addCount = length - retStr.Length;

                for (int i = 0; i < addCount; ++i)
                {
                    retStr += "*";
                }

            }

           
            return retStr;
        }


        /////// Command ////////
        private RelayCommand callCommand;
        public RelayCommand CallCommand
        {
            get { return callCommand = callCommand ?? new RelayCommand(() => phoneManager.Tel(Book.PhoneNumbers[0])); }
        }

        private RelayCommand acceptCommand;
        public RelayCommand AcceptCommand
        {
            get { return acceptCommand = acceptCommand ?? new RelayCommand(() => phoneManager.Hook()); }
        }

        private RelayCommand terminateCommand;
        public RelayCommand TerminateCommand
        {
            get { return terminateCommand = terminateCommand ?? new RelayCommand(() => phoneManager.Terminate()); }
        }
    }
}
