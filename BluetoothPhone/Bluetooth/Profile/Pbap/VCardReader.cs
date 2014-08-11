﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothPhone.Bluetooth.Profile.Pbap
{
    class VCardReader
    {
        public static List<PhoneBook> ParseVCard(string VCardsString)
        {
            VCardsString = VCardsString.Replace("\r\n", "\n");
            VCardsString = VCardsString.Replace("\r", "\n");

            string[] tempStrs = VCardsString.Split(new char[] {'\n'});

            PhoneBook addBook = new PhoneBook();
            List<PhoneBook> phoneBookList = new List<PhoneBook>();

            foreach (string temp in tempStrs)
            {
                string vCardsStr = temp.ToUpper();

                if (vCardsStr.Equals("BEGIN:VCARD"))
                {
                    addBook = new PhoneBook();
                }
                else if (vCardsStr.Equals("END:VCARD"))
                {
                    phoneBookList.Add(addBook);
                }
                else
                {
                    ParseVCardString(vCardsStr, addBook);
                }
            }

            return phoneBookList;
        }

        private static void ParseVCardString(string vCardStr, PhoneBook phoneBook)
        {
            string[] strs = vCardStr.Split(new char[] { ':' });

            if (strs.Length < 2) return;

            string[] kinds = strs[0].Split(new char[] { ';' });

            switch (kinds[0])
            {
                case "FN":
                    phoneBook.Name = strs[1];
                    break;
                case "N":
                    if (String.IsNullOrWhiteSpace(phoneBook.Name))
                    {
                        phoneBook.Name = strs[1].Replace(';', ' ');
                    }
                    break;
                case "TEL":
                    phoneBook.PhoneNumber = strs[1];
                    break;
                case "EMAIL":
                    phoneBook.EmailAddress = strs[1];
                    break;
                case "X-IRMC-CALL-DATETIME":
                    string T = strs[1];
                    phoneBook.Time = new DateTime(int.Parse(T.Substring(0, 4)),
                        int.Parse(T.Substring(4, 2)),
                        int.Parse(T.Substring(6, 2)),
                        int.Parse(T.Substring(9, 2)),
                        int.Parse(T.Substring(11, 2)),
                        int.Parse(T.Substring(13, 2)));
                    break;
            }
        }
    }
}
