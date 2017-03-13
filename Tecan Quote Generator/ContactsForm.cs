using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            this.accountsTableAdapter.FillBy(this.customersDataSet.Accounts);
        }

        // Run at form shown
        private void SetContactDisplay(object sender, EventArgs e)
        {
            // if(AccountsComboBox.SelectedValue != null)
            if (AccountsComboBox.Items.Count == 0)
            {
                this.Height = 100;
                editAccountNameButton.Visible = false;
            }
            else
            {
                AccountsComboBox.SelectedIndex = 0;
                this.Height = 175;
                loadContactsList();
            }
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
                // ContactsListBox.SelectedIndex = 0;
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
            if (ContactsListBox.SelectedIndex == -1)
            {
                selectedContact = 1;
            }
            else
            {
                selectedContact = (short)currentContacts[ContactsListBox.SelectedIndex];
            }

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
            if (AccountsComboBox.Items.Count == 0)
            {
                String importMessage = "You currently have no accounts and contacts.\n\n" +
                    "If you are planning on importing accounts, please do that before adding new accounts.\n\n" +
                    "If you choose to import accounts later, you will loose all saved account information.\n\n" +
                    "Do you want to go ahead and add a new account and contact?";

                if (MessageBox.Show(importMessage, "Confirm Adding Account", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    accountUpdateMode = false;
                    this.Height = 175;
                    editLabel.Visible = false;
                    AddAccountPanel.Visible = true;
                    AddAccountTextBox.Focus();
                    return;
                }
                else
                {
                    return;
                }
            }
            else
            {
                accountUpdateMode = false;
                this.Height = 175;
                editLabel.Visible = false;
                AddAccountPanel.Visible = true;
                AddAccountTextBox.Focus();
            }
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

            // Initialize Accounts Db if required
            if (AccountsComboBox.Items.Count == 0)
            {
                cmd.CommandText = "INSERT INTO Accounts (AccountID, AccountName) Values (0 , '- Please Select An Account -')";
                cmd.ExecuteNonQuery();
            }

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
            this.accountsTableAdapter.FillBy(this.customersDataSet.Accounts);
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
            int newIDNum = 0;
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
                newIDNum = Convert.ToInt32(reader[0]);
            }
            ContactsDatabase.Close();
            if (newIDNum == 0)
            {
                newIDNum = 1;
            }
            return (short)newIDNum;
        }

        private void listPrintContactsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create the temp directory if it does not exist
            String tempFilePath = @AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\temp";
            System.IO.Directory.CreateDirectory(tempFilePath);

            // If temp directory current contains any files, delete them
            System.IO.DirectoryInfo tempFiles = new DirectoryInfo(tempFilePath);

            foreach (FileInfo file in tempFiles.GetFiles())
            {
                file.Delete();
            }
            String fullFilePathName = @tempFilePath + "\\ContactsList.csv";

            string sData = "Account ID,Account Name,Contact ID,First,Last,Address,City,State,Postal Code,Work Phone,Fax,Email\n";

            openDB();
            SqlCeCommand cmd = ContactsDatabase.CreateCommand();

            cmd.CommandText = "SELECT A.AccountID, A.AccountName, C.ContactID, C.First, C.Last, C.Address, C.City, C.State, C.PostalCode,  C.WorkPhone, C.Fax, C.Email" +
                " FROM Accounts A LEFT OUTER JOIN Contacts C " +
                " ON A.AccountID = C.AccountID" +
                " ORDER BY A.AccountName, C.First";
            SqlCeDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                sData += reader[0].ToString() + "," + reader[1].ToString() + "," + reader[2].ToString() + "," + reader[3].ToString() +
                "," + reader[4].ToString() + "," + reader[5].ToString() + "," + reader[6].ToString() + "," + reader[7].ToString() +
                "," + reader[8].ToString() + "," + reader[9].ToString() + "," + reader[10].ToString() + "," + reader[11].ToString() + "\n";
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

        // todo Once Loaded no longer allow replace
        private void importContactsButton_Click(object sender, EventArgs e)
        {
            Int32 NewAccountID = 0;
            Int32 CurrentAccountID;
            Int32 NewContactID = 1;

            // Get the contacts cvs file
            String[] newContacts = getContactsFile();

            // test for invalid records
            String contactsErrorMessage = testNewContacts(newContacts);
            if (contactsErrorMessage != "")
            {
                MessageBox.Show(contactsErrorMessage);
                importContactsPanel.Visible = false;
                return;
            }

            String[] thisContact = null;
            List<Contact> alreadyAddedAccounts_Contacts = new List<Contact>();

            openDB();
            SqlCeCommand cmd = ContactsDatabase.CreateCommand();
            
            // No contacts in DB
            if (AccountsBindingSource.Count == 0)
            {
                // Initialize Accounts Db
                cmd.CommandText = "INSERT INTO Accounts (AccountID, AccountName) Values (0 , '- Please Select An Account -')";
                cmd.ExecuteNonQuery();
                NewAccountID = 1;
            }
            else
            {
                // Have accounts and contacts, put them in an array to test for existing contacts when adding 
                cmd.CommandText = "SELECT A.AccountName, C.First, C.Last, C.Address, C.City, C.State, C.PostalCode,  C.WorkPhone, C.Fax, C.Email, C.ContactID, A.AccountID " +
                " FROM Accounts A LEFT OUTER JOIN Contacts C " +
                " ON A.AccountID = C.AccountID" +
                " ORDER BY A.AccountName, C.First";
                SqlCeDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    // Add to alreadyAddedAccounts_Contacts list
                    if (reader[0].ToString() != "- Please Select An Account -")
                    {
                        alreadyAddedAccounts_Contacts.Add(new Contact
                        {
                            accountName = reader[0].ToString(),
                            first = reader[1].ToString(),
                            last = reader[2].ToString(),
                            address = reader[3].ToString(),
                            city = reader[4].ToString(),
                            state = reader[5].ToString(),
                            postalCode = reader[6].ToString(),
                            workPhone = reader[7].ToString(),
                            fax = reader[8].ToString(),
                            email = reader[9].ToString(),
                            fullName = reader[1].ToString() + " " + reader[2].ToString(),
                            contactID = Convert.ToInt32(reader[10]),
                            accountID = Convert.ToInt32(reader[11])
                        });
                    }
                }
                reader.Dispose();

                // Get Last AccountID 
                cmd.CommandText = "SELECT AccountID FROM Accounts ORDER BY AccountID";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    NewAccountID = Convert.ToInt16(reader[0]);
                }
                reader.Dispose();
                NewAccountID++;
            }

            // Loop through the contacts inported from the csv file 
            foreach (string newContact in newContacts)
            {
                thisContact = newContact.Split(',');
                // Accounts
                var theAccount = alreadyAddedAccounts_Contacts.FirstOrDefault(x => x.accountName == thisContact[0]);
                // Account exisits
                if (theAccount != null)
                {
                    CurrentAccountID = theAccount.accountID;
                    // Account found, test if contact for this account exists
                    var theContact = alreadyAddedAccounts_Contacts.SingleOrDefault(x => x.accountName == thisContact[0] && x.first == thisContact[1] && x.last == thisContact[2]);
                    // Contact for this account exists - do nothing
                    if (theContact == null)
                    // Account exist - Contact does not exist - update list add contact to table
                    {
                        var theContactData = alreadyAddedAccounts_Contacts.Last(x => x.accountName == thisContact[0]);
                        NewContactID = theContactData.contactID;
                        NewContactID++;
                        // Add contact to contact table
                        cmd.CommandText = "INSERT INTO Contacts (First, Last, Address, City, State, PostalCode, WorkPhone, Fax, Email, FullName, ContactID, AccountID)" +
                        " Values " +
                        "(@First, @Last, @Address, @City, @State, @PostalCode, @WorkPhone, @Fax, @Email, @FullName, @ContactID, @AccountID)";

                        cmd.Parameters.AddWithValue("@First", thisContact[1]);
                        cmd.Parameters.AddWithValue("@Last", thisContact[2]);
                        cmd.Parameters.AddWithValue("@Address", thisContact[3]);
                        cmd.Parameters.AddWithValue("@City", thisContact[4]);
                        cmd.Parameters.AddWithValue("@State", thisContact[5]);
                        cmd.Parameters.AddWithValue("@PostalCode", thisContact[6]);
                        cmd.Parameters.AddWithValue("@WorkPhone", thisContact[7]);
                        cmd.Parameters.AddWithValue("@Fax", thisContact[8]);
                        cmd.Parameters.AddWithValue("@Email", thisContact[9]);
                        cmd.Parameters.AddWithValue("@FullName", thisContact[1] + " " + thisContact[2]);
                        cmd.Parameters.AddWithValue("@ContactID", NewContactID);
                        cmd.Parameters.AddWithValue("@AccountID", CurrentAccountID);
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();

                        // Add to alreadyAddedAccounts_Contacts list
                        alreadyAddedAccounts_Contacts.Add(new Contact
                        {
                            accountName = thisContact[0],
                            first = thisContact[1],
                            last = thisContact[2],
                            address = thisContact[3],
                            city = thisContact[4],
                            state = thisContact[5],
                            postalCode = thisContact[6],
                            workPhone = thisContact[7],
                            fax = thisContact[8],
                            email = thisContact[9],
                            fullName = thisContact[1] + " " + thisContact[2],
                            contactID = NewContactID,
                            accountID = CurrentAccountID
                        });
                    }
                }
                else
                // Account does not exist - Add account and contact
                {
                    // Add account to accounts table
                    cmd.CommandText = "INSERT INTO Accounts (AccountID, AccountName) Values (" + NewAccountID + " , '" + thisContact[0] + "')";
                    cmd.ExecuteNonQuery();

                    // Add contact to contact table
                    cmd.CommandText = "INSERT INTO Contacts (First, Last, Address, City, State, PostalCode, WorkPhone, Fax, Email, FullName, ContactID, AccountID)" +
                    " Values " +
                    "(@First, @Last, @Address, @City, @State, @PostalCode, @WorkPhone, @Fax, @Email, @FullName, @ContactID, @AccountID)";

                    cmd.Parameters.AddWithValue("@First", thisContact[1]);
                    cmd.Parameters.AddWithValue("@Last", thisContact[2]);
                    cmd.Parameters.AddWithValue("@Address", thisContact[3]);
                    cmd.Parameters.AddWithValue("@City", thisContact[4]);
                    cmd.Parameters.AddWithValue("@State", thisContact[5]);
                    cmd.Parameters.AddWithValue("@PostalCode", thisContact[6]);
                    cmd.Parameters.AddWithValue("@WorkPhone", thisContact[7]);
                    cmd.Parameters.AddWithValue("@Fax", thisContact[8]);
                    cmd.Parameters.AddWithValue("@Email", thisContact[9]);
                    cmd.Parameters.AddWithValue("@FullName", thisContact[1] + " " + thisContact[2]);
                    cmd.Parameters.AddWithValue("@ContactID", 1);
                    cmd.Parameters.AddWithValue("@AccountID", NewAccountID);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    // Add to alreadyAddedAccounts_Contacts list
                    alreadyAddedAccounts_Contacts.Add(new Contact
                    {
                        accountName = thisContact[0],
                        first = thisContact[1],
                        last = thisContact[2],
                        address = thisContact[3],
                        city = thisContact[4],
                        state = thisContact[5],
                        postalCode = thisContact[6],
                        workPhone = thisContact[7],
                        fax = thisContact[8],
                        email = thisContact[9],
                        fullName = thisContact[1] + " " + thisContact[2],
                        contactID = 1,
                        accountID = NewAccountID
                    });
                    NewAccountID++;
                }

            }
            ContactsDatabase.Close();
            importContactsPanel.Visible = false;
            this.accountsTableAdapter.FillBy(this.customersDataSet.Accounts);
            SetContactDisplay(sender, e);
        }

        private String[] getContactsFile()
        {
            String filePath;
            String[] theContacts = null;
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
                    theContacts = File.ReadAllLines(filePath);
                }
                catch (Exception)
                {
                    MessageBox.Show("Please close the contacts file and try again");
                    importContactsPanel.Visible = false;
                    return theContacts;
                }
            }
            return theContacts;
        }

        private String testNewContacts(String[] newContacts)
        {
            Boolean hasError = false;
            RegexUtilities util = new RegexUtilities();
            Regex phoneNumpattern = new Regex(@"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}");
            Regex zipCodepattern = new Regex(@"^\d{5}(-\d{4})?$");

            String ErrorMessage = "Please correct items in your imported contact file.\n\n";
            ErrorMessage += "Account Name, First, and Last cannot be blank.\n";
            ErrorMessage += "Phone numbers and email address must be valid. State field can only conatin 2 characters.\n\n";
            String[] thisContact = null;
            foreach (string newContact in newContacts)
            {
                thisContact = newContact.Split(',');
                // Account Name, First, Last, Address, City, State, PostalCode, WorkPhone, Fax, Email
                if(thisContact[0] == "" && !ErrorMessage.Contains("Blank Account Name"))
                {
                    ErrorMessage += "Blank Account Name\n";
                    hasError = true;
                }
                if(thisContact[1] == "" && !ErrorMessage.Contains("Blank First Name"))
                {
                    ErrorMessage += "Blank First Name\n";
                    hasError = true;
                }
                if(thisContact[2] == "" && !ErrorMessage.Contains("Blank Last Name"))
                {
                    ErrorMessage += "Blank Last Name\n";
                    hasError = true;
                }
                if(thisContact[5].Length > 2 && thisContact[0] != "" && thisContact[1] != "" && thisContact[2] != "")
                {
                    ErrorMessage += thisContact[0] + ", " + thisContact[1] + " " + thisContact[2] + " has an invalid state\n";
                    hasError = true;
                }
                if (thisContact[6] != "" && !zipCodepattern.IsMatch(thisContact[6]) && thisContact[0] != "" && thisContact[1] != "" && thisContact[2] != "")
                {
                    ErrorMessage += thisContact[0] + ", " + thisContact[1] + " " + thisContact[2] + " has an invalid zipcode\n";
                    hasError = true;
                }
                if (thisContact[7] != "" && !phoneNumpattern.IsMatch(thisContact[7]) && thisContact[0] != "" && thisContact[1] != "" && thisContact[2] != "")
                {
                    ErrorMessage += thisContact[0] + ", " + thisContact[1] + " " + thisContact[2] + " has an invalid phone number\n";
                    hasError = true;
                }
                if (thisContact[8] != "" && !phoneNumpattern.IsMatch(thisContact[8]) && thisContact[0] != "" && thisContact[1] != "" && thisContact[2] != "")
                {
                    ErrorMessage += thisContact[0] + ", " + thisContact[1] + " " + thisContact[2] + " has an invalid fax number\n";
                    hasError = true;
                }
                if (thisContact[8] != "" && !util.IsValidEmail(thisContact[9]) && thisContact[0] != "" && thisContact[1] != "" && thisContact[2] != "")
                {
                    ErrorMessage += thisContact[0] + ", " + thisContact[1] + " " + thisContact[2] + " has an invalid email address\n";
                    hasError = true;
                }
            }
            if (hasError)
            {
                return ErrorMessage;
            }
            else
            {
                return "";
            }
        }

        private void CancelNewContactButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
