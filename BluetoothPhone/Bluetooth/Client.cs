using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InTheHand;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Bluetooth.AttributeIds;
using InTheHand.Net.Ports;
using InTheHand.Net.Sockets;
using InTheHand.Windows.Forms;

using BluetoothPhone.Bluetooth.Profile;
using BluetoothPhone.Bluetooth.Profile.Pbap;
using BluetoothPhone.Bluetooth.Profile.Hfp;

namespace BluetoothPhone.Bluetooth
{
    public class Client
    {
        public event Action<string, PhoneBook> OnRing;
        public event Action<PhoneStatus.PhoneStatusKind> OnUpdatePhoneStatus;

        private string ConnectPhoneName;

        private Pbap ProfilePbap;
        private Hfp ProfileHfp;

        private PhoneStatus.PhoneStatusKind Status;

        public Client()
        {
            ProfilePbap = new Pbap();
            ProfileHfp = new Hfp();

            ProfileHfp.OnRing += ProfileHfp_OnRing;
            ProfileHfp.OnUpdatePhoneStatus += ProfileHfp_OnUpdatePhoneStatus;
        }

        public void InitAtBluetoothDialog()
        {
            SelectBluetoothDeviceDialog dlg = new SelectBluetoothDeviceDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                InitPhone(dlg.SelectedDevice.DeviceName);
            }
        }



 
        public bool InitPhone(string phoneName)
        {
            ConnectPhoneName = phoneName;

            BluetoothDeviceInfo[] devices = GetPairDevices();

            foreach (BluetoothDeviceInfo device in devices)
            {
                if (device.DeviceName == ConnectPhoneName)
                {
                    if (!Connect(device)) return false;

                    return true;
                }
            }

            return false;
        }

        public BluetoothDeviceInfo[] GetPairDevices()
        {
            BluetoothClient client = new BluetoothClient();
            return client.DiscoverDevices(10, false, true, false);
        }


        public bool Connect(BluetoothDeviceInfo device)
        {
            try
            {
                ProfilePbap.Connect(device);
                ProfileHfp.Connect(device);

                Console.WriteLine("OnConnected");
            }
            catch
            {
                Console.WriteLine("Bluetooth Connected Fail!");
                return false;
            }

            return true;
        }

        public void Tel(string PhoneNumber)
        {
            ProfileHfp.Dial(PhoneNumber);
        }

        public void Hook()
        {
            ProfileHfp.Hook();
        }

        public void Terminate()
        {
            ProfileHfp.Terminate();
        }

        

        public PhoneBook[] getPhoneBooks(PbapFolder Folder, int MaxNum = -1)
        {
            return ProfilePbap.GetPhoneBooks(Folder, MaxNum);
        }
        

        private void ProfileHfp_OnRing(string phoneNumber)
        {
            if (OnRing == null)
            {
                return;
            }

            PhoneBook book = ProfilePbap.GetPhoneBookFromPhoneNumber(PbapFolder.pb, phoneNumber);
            if (book == null)
            {
                book = new PhoneBook();
                book.PhoneNumbers.Add(phoneNumber);
            }

            OnRing(phoneNumber, book);   
        }

        private void ProfileHfp_OnUpdatePhoneStatus(string changedStatus)
        {
            if (OnUpdatePhoneStatus == null)
            {
                return;
            }


            switch (changedStatus)
            {
                case HfpPhoneStatus.STATUS_CALL:
                    OnHfpPhoneStatusCallChanged();
                    break;
                case HfpPhoneStatus.STATUS_CALLSETUP:
                    OnHfpPhoneStatusCallSetupChanged();
                    break;
            }

        }

        private void OnHfpPhoneStatusCallChanged()
        {
            switch(ProfileHfp.PhoneStatusValue.GetStatus(HfpPhoneStatus.STATUS_CALL))
            {
                case 0:
                    if (Status != PhoneStatus.PhoneStatusKind.phoneNormal)
                    {
                        Status = PhoneStatus.PhoneStatusKind.phoneNormal;
                        OnUpdatePhoneStatus(Status);
                    }
                    break;
                case 1:
                    Status = PhoneStatus.PhoneStatusKind.phoneConnect;
                    OnUpdatePhoneStatus(Status);
                    break;
            }
            
        }

        private void OnHfpPhoneStatusCallSetupChanged()
        {
            switch (ProfileHfp.PhoneStatusValue.GetStatus(HfpPhoneStatus.STATUS_CALLSETUP))
            {
                case 0:
                    if (Status != PhoneStatus.PhoneStatusKind.phoneNormal)
                    {
                        Status = PhoneStatus.PhoneStatusKind.phoneNormal;
                        OnUpdatePhoneStatus(Status);
                    }
                    break;
                case 1:
                   Status = PhoneStatus.PhoneStatusKind.phoneIncomingCalling;
                    OnUpdatePhoneStatus(Status);
                    break;
                case 2:
                    Status = PhoneStatus.PhoneStatusKind.phoneOutgoingCalling;
                    OnUpdatePhoneStatus(Status);
                    break;
                case 3:
                    //OnUpdatePhoneStatus(PhoneStatus.PhoneStatusKind.);
                    break;
            }
        }

    }
}
