using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecan_Quote_Generator
{
    class Contact
    {
        private string AccountName;
        private string First;
        private string Last;
        private string Address;
        private string City;
        private string State;
        private string PostalCode;
        private string WorkPhone;
        private string Fax;
        private string Email;
        private string FullName;
        private int ContactID;
        private int AccountID;

        public Contact()
        { }

        public string accountName
        {
            get
            {
                return AccountName;
            }
            set
            {
                AccountName = value;
            }
        }

        public string first
        {
            get
            {
                return First;
            }
            set
            {
                First = value;
            }
        }

        public string last
        {
            get
            {
                return Last;
            }
            set
            {
                Last = value;
            }
        }

        public string address
        {
            get
            {
                return Address;
            }
            set
            {
                Address = value;
            }
        }

        public string city
        {
            get
            {
                return City;
            }
            set
            {
                City = value;
            }
        }

        public string state
        {
            get
            {
                return State;
            }
            set
            {
                State = value;
            }
        }

        public string postalCode
        {
            get
            {
                return PostalCode;
            }
            set
            {
                PostalCode = value;
            }
        }

        public string workPhone
        {
            get
            {
                return WorkPhone;
            }
            set
            {
                WorkPhone = value;
            }
        }

        public string fax
        {
            get
            {
                return Fax;
            }
            set
            {
                Fax = value;
            }
        }

        public string email
        {
            get
            {
                return Email;
            }
            set
            {
                Email = value;
            }
        }

        public string fullName
        {
            get
            {
                return this.first + " " + this.last;
            }
            set
            {
                FullName = value;
            }
        }

        public int contactID
        {
            get
            {
                return ContactID;
            }
            set
            {
                ContactID = value;
            }
        }

        public int accountID
        {
            get
            {
                return AccountID;
            }
            set
            {
                AccountID = value;
            }
        }

    }
}
