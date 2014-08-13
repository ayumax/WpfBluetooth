using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using InTheHand;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Bluetooth.AttributeIds;
using InTheHand.Net.Ports;
using InTheHand.Net.Sockets;
using Brecham.Obex;
using Brecham.Obex.Objects;

namespace BluetoothPhone.Bluetooth.Profile.Pbap
{
    public static class Extensions
    {
        public static PbapFolder Folder = PbapFolder.pb;
        public static string FullPath(this PbapFolder pbapFolder)
        {
            return String.Format("telecom/{0}.vcf", pbapFolder.ToString());
        }

        public static string Name(this PbapFolder pbapFolder)
        {
            return pbapFolder.ToString();
        }
    }

    public enum PbapFolder
    {
        pb,     // local phone book
        ich,    // Incoming Calls History
        och,    // Outgoing Calls History
        mch,    // Missed Calls History
        cch,    // Combined Calls History
    };

    class Pbap : BluetoothProfile
    {
        private ObexClientSession session;
        

        public Pbap()
            : base( BluetoothService.PhonebookAccess)
        {
        }

        protected override void OnConnected()
        {
            LocalClient.Encrypt = true;

            session = new ObexClientSession(LocalClient.GetStream(), UInt16.MaxValue);

            session.Connect(new byte[] { 0x79, 0x61, 0x35, 0xf0, 0xf0, 0xc5, 0x11, 0xd8, 0x09, 0x66, 0x08, 0x00, 0x20, 0x0c, 0x9a, 0x66 });
        }

        public int GetPhoneBookCount(PbapFolder Folder)
        {
            int length = 0;

            using (ObexGetStream Stream = PullPhoneBook(Folder, 0, 0))
            {
                byte[] lengsData = Stream.ResponseHeaders.GetByteSeq(ObexHeaderId.AppParameters);
                if (lengsData == null) return 0;
                if (lengsData.Length < 3) return 0;
                if (lengsData[0] != 8 || lengsData[1] != 2) return 0;

                length = (lengsData[2] << 8 | lengsData[3]);
            }

            return length;
        }

        public PhoneBook[] GetPhoneBooks(PbapFolder Folder)
        {
            List<PhoneBook> phoneBookList = new List<PhoneBook>();

            var t = Task.Factory.StartNew(() =>
            {
                int length = GetPhoneBookCount(Folder);

                for (int i = 0; i < length; i += 10)
                {
                    using (ObexGetStream Stream = PullPhoneBook(Folder, 10, i))
                    {
                        byte[] ba = new byte[UInt16.MaxValue];
                        int readSize = Stream.Read(ba, 0, UInt16.MaxValue);

                        phoneBookList.AddRange(VCardReader.ParseVCard(UTF8Encoding.UTF8.GetString(ba, 0, readSize)));
                    }

                }
            });

            t.Wait();

            return phoneBookList.ToArray();
        }

        public PhoneBook GetPhoneBookFromPhoneNumber(PbapFolder Folder, string PhoneNumber)
        {
           PhoneBook book = null;

           var t = Task.Factory.StartNew(() =>
           {
               string handle = String.Empty;

               using (ObexGetStream Stream = PullvCardListing(Folder, 1, 0, PhoneNumber))
               {
                   byte[] ba = new byte[UInt16.MaxValue];
                   int readSize = Stream.Read(ba, 0, UInt16.MaxValue);

                   handle = VCardReader.ParseVCardListenXML(UTF8Encoding.UTF8.GetString(ba, 0, readSize));
               }

               if (!String.IsNullOrWhiteSpace(handle))
               {
                   book = GetPhoneBook(Folder, handle);
               }
           });
        
            t.Wait();

            return book;
        }

        private PhoneBook GetPhoneBook(PbapFolder Folder, string handle)
        {
            using (ObexGetStream Stream = PullvCardEntry(Folder, handle))
            {
                byte[] ba = new byte[UInt16.MaxValue];
                int readSize = Stream.Read(ba, 0, UInt16.MaxValue);

                List<PhoneBook> phoneBookList = VCardReader.ParseVCard(UTF8Encoding.UTF8.GetString(ba, 0, readSize));
                if (phoneBookList.Count > 0)
                {
                    return phoneBookList[0];
                }
            }

            return null;
        }

        private ObexGetStream PullPhoneBook(PbapFolder Folder, int MaxListCount, int ListStartOffset)
        {
            ObexHeaderCollection headers = new ObexHeaderCollection();

            headers.AddType("x-bt/phonebook");
            headers.Add(ObexHeaderId.Name, Folder.FullPath());
            headers.Add(ObexHeaderId.AppParameters, new byte[] {  
                    0x06, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,                             // Filter
                    0x07, 0x01, 0x01,                                                                       // Format 0x00 = 2.1 0x01 = 3.0
                    0x04, 0x02, (byte)((MaxListCount >> 8) & 0xFF), (byte)(MaxListCount & 0xFF),            // MaxListCount
                    0x05, 0x02, (byte)((ListStartOffset >> 8) & 0xFF), (byte)(ListStartOffset & 0xFF) });   // ListStartOffset


            return session.Get(headers);
        }

        private ObexGetStream PullvCardListing(PbapFolder Folder, int MaxListCount, int ListStartOffset, string PhoneNumber = "")
        {
            session.SetPathReset();
            session.SetPath("telecom");

            ObexHeaderCollection headers = new ObexHeaderCollection();
            headers.AddType("x-bt/vcard-listing");
            headers.Add(ObexHeaderId.Name, Folder.Name());


            List<byte> paramList = new List<byte>();
            paramList.AddRange(new byte[] { 0x01, 0x01, 0x01 }); // Order

            byte[] searchValue = UTF8Encoding.UTF8.GetBytes(PhoneNumber);
            if (!String.IsNullOrWhiteSpace(PhoneNumber))
            {
                paramList.AddRange(new byte[] { 0x02, (byte)searchValue.Length });
                paramList.AddRange(searchValue);                                                                          // SearchValue
            }

            paramList.AddRange(new byte[] { 0x03, 0x01, 0x01,                                                             // SearchAttribute 0x00:Name 0x01:Number 0x02:Sound
                                      0x04, 0x02, (byte)((MaxListCount >> 8) & 0xFF), (byte)(MaxListCount & 0xFF),        // MaxListCount
                                      0x05, 0x02, (byte)((ListStartOffset >> 8) & 0xFF), (byte)(ListStartOffset & 0xFF)   // ListStartOffset
            });
            headers.Add(ObexHeaderId.AppParameters, paramList.ToArray());

            return session.Get(headers);
        }

        private ObexGetStream PullvCardEntry(PbapFolder Folder, string ObjectName)
        {
            session.SetPath(Folder.Name());

            ObexHeaderCollection headers = new ObexHeaderCollection();
            headers.AddType("x-bt/vcard");
            headers.Add(ObexHeaderId.Name, ObjectName);

            return session.Get(headers);
        }

    }
}
