using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlServerCe;

namespace Tecan_Quote_Generator
{
    public partial class ContactsForm : Form
    {
        Boolean updateMode;
        int[] currentContacts = new int[10];
        SqlCeConnection ContactsDatabase = null;
        public ContactsForm()
        {
            InitializeComponent();
        }

        private void Contacts_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'customersDataSet.Accounts' table. You can move, or remove it, as needed.
            this.accountsTableAdapter.Fill(this.customersDataSet.Accounts);
        }

        private void SetContactDisplay(object sender, EventArgs e)
        {
            if(AccountsComboBox.SelectedValue != null) 
                loadContactsList();
        }

        private void loadContactsList()
        {
            ContactsListBox.Items.Clear();
            short selectedAccount;
            selectedAccount = (short)Convert.ToInt16(AccountsComboBox.SelectedValue);

            openDB();
            SqlCeCommand cmd = ContactsDatabase.CreateCommand();
            cmd.CommandText = "SELECT First, Last, ContactID FROM Contacts WHERE AccountID = " + selectedAccount + " ORDER BY Last";
            SqlCeDataReader reader = cmd.ExecuteReader();

            int contactCount = 0;
            while (reader.Read())
            {
                ContactsListBox.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
                currentContacts[contactCount] = (short)Convert.ToInt16(reader[2].ToString());
                contactCount++;
            }
            reader.Dispose();
            ContactsDatabase.Close();

            if (ContactsListBox.Items.Count == 0)
            {
                ContactsListBox.Items.Add("Seleced account has no contacts.");
            }
            else
            {
                ContactsListBox.SelectedIndex = 0;
                displayContact();
            }

        }

        private void AccountsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadContactsList();
        }

        private void ContactsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(currentContacts != null)
                displayContact();
        }

        private void displayContact()
        {
            short selectedAccount;
            selectedAccount = (short)Convert.ToInt16(AccountsComboBox.SelectedValue);
            short selectedContact;
            selectedContact = (short)currentContacts[ContactsListBox.SelectedIndex];

            openDB();
            SqlCeCommand cmd = ContactsDatabase.CreateCommand();
            cmd.CommandText = "SELECT First, Last, Address, City, State, PostalCode,  WorkPhone, Fax, Email " +
                " FROM Contacts WHERE AccountID = " + selectedAccount + " AND ContactID = " + selectedContact;
            SqlCeDataReader reader = cmd.ExecuteReader();

            if(reader.Read())
            {
                FirstNameTextBox.Text = reader[0].ToString();
                LastNameTextBox.Text = reader[1].ToString();
                AddressTextBox.Text = reader[2].ToString();
                CityTextBox.Text = reader[3].ToString();
                StateTextBox.Text = reader[4].ToString();
                ZipTextBox.Text = reader[5].ToString();
                PhoneTextBox.Text = reader[6].ToString();
                FaxTextBox.Text = reader[7].ToString();
                EmailTextBox.Text = reader[8].ToString();
            }
            reader.Dispose();
            ContactsDatabase.Close();
            updateMode = true;
        }

        private void AddAccountButton_Click(object sender, EventArgs e)
        {
            AddAccountPanel.Visible = true;
            AddAccountTextBox.Focus();
        }

        private void AddAcountConfirmButton_Click(object sender, EventArgs e)
        {
            if (AddAccountTextBox.Text != "")
            {
                short newAccountID;
                newAccountID = getNewID("AccountID", "Accounts");
                openDB();
                SqlCeCommand cmd = ContactsDatabase.CreateCommand();
                cmd.CommandText = "INSERT INTO Accounts (AccountID, AccountName) Values (" + newAccountID + ", '" + AddAccountTextBox.Text + "')";
                cmd.ExecuteNonQuery();
                ContactsDatabase.Close();
                this.accountsTableAdapter.Fill(this.customersDataSet.Accounts);
                AccountsComboBox.SelectedIndex = AccountsComboBox.Items.Count - 1;

            }
            AddAccountPanel.Visible = false;
        }

        private void AddAccountCancelButton_Click(object sender, EventArgs e)
        {
            AddAccountTextBox.Text = "";
            AddAccountPanel.Visible = false;
        }

        private void AddContact_Click(object sender, EventArgs e)
        {
            updateMode = false;
            FirstNameTextBox.Text = "";
            LastNameTextBox.Text = "";
            AddressTextBox.Text = "";
            CityTextBox.Text = "";
            StateTextBox.Text = "";
            ZipTextBox.Text = "";
            PhoneTextBox.Text = "";
            FaxTextBox.Text = "";
            EmailTextBox.Text = "";
            FirstNameTextBox.Focus();
        }

        private void SaveContactButton_Click(object sender, EventArgs e)
        {
            openDB();
            SqlCeCommand cmd = ContactsDatabase.CreateCommand();
            short selectedAccount;
            short selectedContactID;
            short newContactID = 0;
            selectedAccount = (short)Convert.ToInt16(AccountsComboBox.SelectedValue);

            if (updateMode)
            {
                selectedContactID = (short)currentContacts[ContactsListBox.SelectedIndex];
                cmd.CommandText = "UPDATE Contacts SET First=@First, Last=@Last, Address=@Address, City=@City, State=@State, PostalCode=@PostalCode," +
                    " WorkPhone=@WorkPhone, Fax=@Fax, Email=@Email WHERE ContactID = " + selectedContactID + " AND AccountID = " + selectedAccount;
            }
            else
            {
                newContactID = getNewID("ContactID", "Contacts");
                cmd.CommandText = "INSERT INTO Contacts (First, Last, Address, City, State, PostalCode, WorkPhone, Fax, Email, ContactID, AccountID)" +
                    " Values " +
                    "(@First, @Last, @Address, @City, @State, @PostalCode, @WorkPhone, @Fax, @Email, @ContactID, @AccountID)";
            }

            cmd.Parameters.AddWithValue("@First", FirstNameTextBox.Text);
            cmd.Parameters.AddWithValue("@Last", LastNameTextBox.Text);
            cmd.Parameters.AddWithValue("@Address", AddressTextBox.Text);
            cmd.Parameters.AddWithValue("@City", CityTextBox.Text);
            cmd.Parameters.AddWithValue("@State", StateTextBox.Text);
            cmd.Parameters.AddWithValue("@PostalCode", ZipTextBox.Text);
            cmd.Parameters.AddWithValue("@WorkPhone", PhoneTextBox.Text);
            cmd.Parameters.AddWithValue("@Fax", FaxTextBox.Text);
            cmd.Parameters.AddWithValue("@Email", EmailTextBox.Text);
            if (!updateMode)
            {
                cmd.Parameters.AddWithValue("@ContactID", newContactID);
                cmd.Parameters.AddWithValue("@AccountID", selectedAccount);
            }
            cmd.ExecuteNonQuery();
            ContactsDatabase.Close();

        }

        private void openDB()
        {
            ContactsDatabase = new SqlCeConnection();
            String dataPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            ContactsDatabase.ConnectionString = "Data Source=|DataDirectory|\\Customers.sdf;Max Database Size=4000;Max Buffer Size=1024;Persist Security Info=False";
            ContactsDatabase.Open();
        }

        private short getNewID(string indexFieldName, string currentTable)
        {
            String newRowCount = "";
            int newRowCountNum = 0;
            openDB();
            SqlCeCommand cmd = ContactsDatabase.CreateCommand();
            if (indexFieldName == "Accounts")
            {
                cmd.CommandText = "SELECT " + indexFieldName + " FROM " + currentTable + " ORDER BY " + indexFieldName;
            }
            else
            {
                short selectedAccount;
                selectedAccount = (short)Convert.ToInt16(AccountsComboBox.SelectedValue);
                cmd.CommandText = "SELECT " + indexFieldName + " FROM " + currentTable + " WHERE AccountID = " + selectedAccount + " ORDER BY " + indexFieldName;
            }
            SqlCeDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                newRowCount = reader[0].ToString();
            }
            ContactsDatabase.Close();
            if (newRowCount != "")
            {
                newRowCountNum = Convert.ToInt16(newRowCount);
                newRowCountNum++;
            }
            else
            {
                newRowCountNum = 1;
            }
            return (short)newRowCountNum;
        }

    }
}
