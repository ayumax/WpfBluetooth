using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BluetoothPhone.Bluetooth;

using TestBluetoothPhone.Common;

namespace TestBluetoothPhone
{
    class PhoneManagerViewModel : BindableBase
    {
        public PhoneBookViewModel[] Books { get; set; }

        private BluetoothPhone.PhoneManager Model;

        private PhoneConnectWindow window;

        public PhoneManagerViewModel()
        {
            Model = new BluetoothPhone.PhoneManager();
            Model.OnRing += Model_OnRing;
            Model.OnUpdatePhoneStatus += Model_OnUpdatePhoneStatus;
            Model.InitAtBluetoothDialog();

            CreatePhoneBooks();
        }

        void Model_OnUpdatePhoneStatus(PhoneStatus.PhoneStatusKind Status)
        {
            switch (Status)
            {
                case PhoneStatus.PhoneStatusKind.phoneIncomingCalling:
                    {
                        window = new PhoneConnectWindow();
                        PhoneBook book = new PhoneBook();
                        book.Name = "着信中";
                        window.DataContext = new PhoneBookViewModel(book, this.Model);
                        window.Show();
                    }
                    break;

                case PhoneStatus.PhoneStatusKind.phoneOutgoingCalling:
                    {
                        window = new PhoneConnectWindow();
                        PhoneBook book = new PhoneBook();
                        book.Name = "発信中";
                        window.DataContext = new PhoneBookViewModel(book, this.Model);
                        window.Show();
                    }
                    break;

                case PhoneStatus.PhoneStatusKind.phoneNormal:
                    {
                        if (window != null)
                        {
                            window.Close();
                        }
                    }
                    break;
            }
        }

        void Model_OnRing(string phoneNumber, PhoneBook book)
        {
            if (window != null)
            {
                window.DataContext = new PhoneBookViewModel(book, this.Model);
            }
        }

        private void CreatePhoneBooks()
        {
            List<PhoneBookViewModel> bookList = new List<PhoneBookViewModel>();

            PhoneBook[] books = Model.GetBook();
            foreach (PhoneBook book in books)
            {
                bookList.Add(new PhoneBookViewModel(book, Model));
            }

            Books = bookList.ToArray();
        }

    }
}
