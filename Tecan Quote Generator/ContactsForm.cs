using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlServerCe;
using System.IO;
using System.Diagnostics;

namespace Tecan_Quote_Generator
{
    public partial class ContactsForm : Form
    {
        Boolean contactUpdateMode;
        Boolean accountUpdateMode;
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
            // this.contactsTableAdapter.FillByAccountID(this.customersDataSet.Contacts, selectedAccount);

            openDB();
            SqlCeCommand cmd = ContactsDatabase.CreateCommand();
            cmd.CommandText = "SELECT First, Last, ContactID FROM Contacts WHERE AccountID = " + selectedAccount + " ORDER BY First";
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

            setDisplayHeightListContacts();

        }

        private void AccountsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadContactsList();
        }

        private void ContactsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(currentContacts != null) displayContact();
        }

        private void setDisplayHeightListContacts()
        {
            if (ContactsListBox.Items.Count == 0)
            {
                ContactsListBox.Items.Add("Seleced account has no contacts.");
                clearContact();
                this.Height = 175;
                editLabel.Visible = false;
            }
            else
            {
                ContactsListBox.SelectedIndex = 0;
                displayContact();
                this.Height = 375;
                editLabel.Visible = true;
            }
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
            contactUpdateMode = true;
        }

        private void AddAccountButton_Click(object sender, EventArgs e)
        {
            accountUpdateMode = false;
            AddAccountPanel.Visible = true;
            AddAccountTextBox.Focus();
        }

        private void editAccountNameButton_Click(object sender, EventArgs e)
        {
            accountUpdateMode = true;
            this.Height = 175;
            editLabel.Visible = false;
            AddAccountPanel.Visible = true;
            AddAccountTextBox.Text = AccountsComboBox.Text;
            AddAccountTextBox.Focus();
        }

        private void AddAcountConfirmButton_Click(object sender, EventArgs e)
        {
            openDB();
            SqlCeCommand cmd = ContactsDatabase.CreateCommand();
            short selectedAccount;
            
            if (accountUpdateMode)
            {
                selectedAccount = (short)Convert.ToInt16(AccountsComboBox.SelectedValue);
                cmd.CommandText = "UPDATE Accounts SET AccountName='" + AddAccountTextBox.Text + "' WHERE AccountID = " + selectedAccount;
            }
            else
            {
                if (AddAccountTextBox.Text != "")
                {
                    short newAccountID;
                    newAccountID = getNewID("AccountID", "Accounts");
                    cmd.CommandText = "INSERT INTO Accounts (AccountID, AccountName) Values (" + newAccountID + ", '" + AddAccountTextBox.Text + "')";
                }
            }
            cmd.ExecuteNonQuery();
            ContactsDatabase.Close();
            this.accountsTableAdapter.Fill(this.customersDataSet.Accounts);
            if (!accountUpdateMode)
            {
                AccountsComboBox.SelectedIndex = AccountsComboBox.Items.Count - 1;
            }
            else
            {
                loadContactsList();
            }
            
            AddAccountPanel.Visible = false;
        }

        private void AddAccountCancelButton_Click(object sender, EventArgs e)
        {
            AddAccountTextBox.Text = "";
            AddAccountPanel.Visible = false;
            setDisplayHeightListContacts();
        }

        private void AddContact_Click(object sender, EventArgs e)
        {
            contactUpdateMode = false;
            clearContact();
            this.Height = 375;
            FirstNameTextBox.Focus();
        }

        private void SaveContactButton_Click(object sender, EventArgs e)
        {
            String errorMessage = "All fields must be entered!\n\nPlease enter \\ correct the following information.\n\n";
            Boolean contactError = false;
            
            if (FirstNameTextBox.Text == "")
            {
                contactError = true;
                errorMessage = errorMessage + "First Name.\n\n";
            }
            if (LastNameTextBox.Text == "")
            {
                contactError = true;
                errorMessage = errorMessage + "Last Name.\n\n";
            }
            if (AddressTextBox.Text == "")
            {
                contactError = true;
                errorMessage = errorMessage + "Address.\n\n";
            }
            if (CityTextBox.Text == "")
            {
                contactError = true;
                errorMessage = errorMessage + "City.\n\n";
            }
            if (StateTextBox.Text == "")
            {
                contactError = true;
                errorMessage = errorMessage + "State.\n\n";
            }
            if (ZipTextBox.Text == "")
            {
                contactError = true;
                errorMessage = errorMessage + "Zip Code.\n\n";
            }
            if (PhoneTextBox.Text == "" || PhoneTextBox.Text.Length < 12)
            {
                contactError = true;
                errorMessage = errorMessage + "Phone Number.\n\n";
            }
            RegexUtilities util = new RegexUtilities();
            if (EmailTextBox.Text == "" || !util.IsValidEmail(EmailTextBox.Text))
            {
                contactError = true;
                errorMessage = errorMessage + "Email Address.\n\n";
            }
            if (contactError)
            {
                MessageBox.Show(errorMessage);
                // return;
            }
            else
            {
                openDB();
                SqlCeCommand cmd = ContactsDatabase.CreateCommand();
                short selectedAccount;
                short selectedContactID;
                short newContactID = 0;
                selectedAccount = (short)Convert.ToInt16(AccountsComboBox.SelectedValue);

                if (contactUpdateMode)
                {
                    selectedContactID = (short)currentContacts[ContactsListBox.SelectedIndex];
                    cmd.CommandText = "UPDATE Contacts SET First=@First, Last=@Last, Address=@Address, City=@City, State=@State, PostalCode=@PostalCode," +
                        " WorkPhone=@WorkPhone, Fax=@Fax, Email=@Email FullName=@FullName WHERE ContactID = " + selectedContactID + " AND AccountID = " + selectedAccount;
                }
                else
                {
                    newContactID = getNewID("ContactID", "Contacts");
                    cmd.CommandText = "INSERT INTO Contacts (First, Last, Address, City, State, PostalCode, WorkPhone, Fax, Email, FullName, ContactID, AccountID)" +
                        " Values " +
                        "(@First, @Last, @Address, @City, @State, @PostalCode, @WorkPhone, @Fax, @Email, @FullName, @ContactID, @AccountID)";
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
                cmd.Parameters.AddWithValue("@FullName", FirstNameTextBox.Text + " " + LastNameTextBox.Text);
                if (!contactUpdateMode)
                {
                    cmd.Parameters.AddWithValue("@ContactID", newContactID);
                    cmd.Parameters.AddWithValue("@AccountID", selectedAccount);
                }
                cmd.ExecuteNonQuery();
                ContactsDatabase.Close();
                loadContactsList();
            }
        }

        private void clearContact()
        {
            FirstNameTextBox.Text = "";
            LastNameTextBox.Text = "";
            AddressTextBox.Text = "";
            CityTextBox.Text = "";
            StateTextBox.Text = "";
            ZipTextBox.Text = "";
            PhoneTextBox.Text = "";
            FaxTextBox.Text = "";
            EmailTextBox.Text = "";
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
            if (indexFieldName == "AccountID")
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

        private void listPrintContactsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create the new file in temp directory
            String tempFilePath = @AppDomain.CurrentDomain.BaseDirectory.ToString() + "temp";
            System.IO.Directory.CreateDirectory(tempFilePath);

            // If temp directory current contains any files, delete them
            System.IO.DirectoryInfo tempFiles = new DirectoryInfo(tempFilePath);

            foreach (FileInfo file in tempFiles.GetFiles())
            {
                file.Delete();
            }
            String fullFilePathName = @tempFilePath + "\\ContactsList.csv";

            string sData = "Account Name,First,Last,Address,City,State,Postal Code,Work Phone,Fax,Email\n";

            openDB();
            SqlCeCommand cmd = ContactsDatabase.CreateCommand();

            cmd.CommandText = "SELECT A.AccountName, C.First, C.Last, C.Address, C.City, C.State, C.PostalCode,  C.WorkPhone, C.Fax, C.Email " +
                " FROM Accounts A LEFT OUTER JOIN Contacts C " +
                " ON A.AccountID = C.AccountID" +
                " ORDER BY A.AccountName, C.First";
            SqlCeDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                sData += reader[0].ToString() + "," + reader[1].ToString() + "," + reader[2].ToString() + "," + reader[3].ToString() +
                "," + reader[4].ToString() + "," + reader[5].ToString() + "," + reader[6].ToString() + "," + reader[7].ToString() +
                "," + reader[8].ToString() + "," + reader[9].ToString() + "\n";
            }
            reader.Dispose();
            ContactsDatabase.Close();

            File.WriteAllText(fullFilePathName, sData);
            Process.Start(fullFilePathName);
        }

        private void importContactsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Height = 375;
            importContactsPanel.Visible = true;
        }

        private void label16_Click(object sender, EventArgs e)
        {
            Process.Start("https://support.office.com/en-US/article/Introduction-to-importing-and-exporting-data-08422593-42DD-4E73-BDF1-4C21FC3AA1B0");
        }

        private void label17_Click(object sender, EventArgs e)
        {
            Process.Start("https://support.office.com/en-us/article/Export-contacts-from-Outlook-10f09abd-643c-4495-bb80-543714eca73f");
        }

        private void label18_Click(object sender, EventArgs e)
        {
            Process.Start("https://support.office.com/en-us/article/Import-or-export-text-txt-or-csv-files-5250ac4c-663c-47ce-937b-339e391393ba");
        }

        private void label19_Click(object sender, EventArgs e)
        {
            Process.Start("http://support2.constantcontact.com/articles/FAQ/2411");
        }

        private void label20_Click(object sender, EventArgs e)
        {
            Process.Start("https://support.google.com/mail/answer/1069522?hl=en");
        }

        private void importContactsButton_Click(object sender, EventArgs e)
        {
            
            Boolean deleteDb = false;
            short newAccountID = 0;
            short newContactID = 0;
            // If replace is selected and contacts dB not empty, verify action
            if (replaceRadioButton.Checked == true && AccountsComboBox.Items.Count != 0)
            {
                if (MessageBox.Show("This will delete all current Accounts and Contacts and Replace them with your selected Contacts List.\r\n\r\nDo you want to proceed?", "Replace Contacts", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
                else
                {
                    deleteDb = true;
                }
            }
            
            openDB();
            SqlCeCommand cmd = ContactsDatabase.CreateCommand();
            SqlCeDataReader reader;

            if (deleteDb)
            {
                cmd.CommandText = "DELETE FROM Accounts";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "DELETE FROM Contacts";
                cmd.ExecuteNonQuery();
            }

            // Get the contacts cvs file
            String filePath;

            // Get csv Filename and Path
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Title = "Please select your contacts csv file";
            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "csv files (*.csv)|*.csv";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            // read csv file and place in array
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog1.FileName;
                try
                {
                    var newContacts = File.ReadAllLines(filePath).Select(x => x.Split(',')).ToArray();
                    foreach (string[] newContact in newContacts)
                    {
                        // Get Account Name, test if exists, add or use AccountID
                        cmd.CommandText = "SELECT AccountName, AccountID FROM Accounts WHERE AccountName = '" + newContact[0] + "'";
                        reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            newAccountID = (short)Convert.ToInt16(reader[1]);
                        }
                        else if (newContact[0] != "")
                        {
                            newAccountID = getNewID("AccountID", "Accounts");
                            cmd.CommandText = "INSERT INTO Accounts (AccountID, AccountName) Values (" + newAccountID + ", '" + newContact[0] + "')";
                            cmd.ExecuteNonQuery();
                        }

                        // Get Contact Name, test if exists, add if not found
                        cmd.CommandText = "SELECT AccountID, First, Last FROM Contacts WHERE AccountID = '" + newAccountID + "' AND First = '" +
                            newContact[1] + "' AND Last = '" + newContact[2] + "'";
                        reader = cmd.ExecuteReader();
                        if (!reader.Read() && (newContact[1] != "" || newContact[2] != ""))
                        {
                            newContactID = getNewID("ContactID", "Contacts");
                            cmd.CommandText = "INSERT INTO Contacts (First, Last, Address, City, State, PostalCode, WorkPhone, Fax, Email, FullName, ContactID, AccountID)" +
                            " Values " +
                            "(@First, @Last, @Address, @City, @State, @PostalCode, @WorkPhone, @Fax, @Email, @FullName, @ContactID, @AccountID)";

                            cmd.Parameters.AddWithValue("@First", newContact[1]);
                            cmd.Parameters.AddWithValue("@Last", newContact[2]);
                            cmd.Parameters.AddWithValue("@Address", newContact[3]);
                            cmd.Parameters.AddWithValue("@City", newContact[4]);
                            cmd.Parameters.AddWithValue("@State", newContact[5]);
                            cmd.Parameters.AddWithValue("@PostalCode", newContact[6]);
                            cmd.Parameters.AddWithValue("@WorkPhone", newContact[7]);
                            cmd.Parameters.AddWithValue("@Fax", newContact[8]);
                            cmd.Parameters.AddWithValue("@Email", newContact[9]);
                            cmd.Parameters.AddWithValue("@FullName", newContact[1] + " " + newContact[2]);
                            cmd.Parameters.AddWithValue("@ContactID", newContactID);
                            cmd.Parameters.AddWithValue("@AccountID", newAccountID);
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                        reader.Dispose();
                    }
                    ContactsDatabase.Close();
                    importContactsPanel.Visible = false;
                    this.accountsTableAdapter.Fill(this.customersDataSet.Accounts);
                    this.Show();
                }
                catch (Exception)
                {
                    MessageBox.Show("Please close the contacts file and try again");
                    importContactsPanel.Visible = false;
                    return;
                }
            }
        }

        private void CancelNewContactButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
