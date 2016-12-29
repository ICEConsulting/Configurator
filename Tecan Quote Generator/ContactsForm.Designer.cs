namespace Tecan_Quote_Generator
{
    partial class ContactsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.AccountsComboBox = new System.Windows.Forms.ComboBox();
            this.AccountsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.customersDataSet = new Tecan_Quote_Generator.CustomersDataSet();
            this.accountsTableAdapter = new Tecan_Quote_Generator.CustomersDataSetTableAdapters.AccountsTableAdapter();
            this.label2 = new System.Windows.Forms.Label();
            this.ContactsListBox = new System.Windows.Forms.ListBox();
            this.AddAccountButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.FirstNameTextBox = new System.Windows.Forms.TextBox();
            this.LastNameTextBox = new System.Windows.Forms.TextBox();
            this.AddressTextBox = new System.Windows.Forms.TextBox();
            this.CityTextBox = new System.Windows.Forms.TextBox();
            this.StateTextBox = new System.Windows.Forms.TextBox();
            this.ZipTextBox = new System.Windows.Forms.TextBox();
            this.PhoneTextBox = new System.Windows.Forms.TextBox();
            this.FaxTextBox = new System.Windows.Forms.TextBox();
            this.EmailTextBox = new System.Windows.Forms.TextBox();
            this.AddContactButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.AddAccountPanel = new System.Windows.Forms.Panel();
            this.AddAccountCancelButton = new System.Windows.Forms.Button();
            this.AddAcountConfirmButton = new System.Windows.Forms.Button();
            this.AddAccountTextBox = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.SaveContactButton = new System.Windows.Forms.Button();
            this.CancelNewContactButton = new System.Windows.Forms.Button();
            this.editLabel = new System.Windows.Forms.Label();
            this.editAccountNameButton = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listPrintContactsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importContactsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.importContactsPanel = new System.Windows.Forms.Panel();
            this.importContactsButton = new System.Windows.Forms.Button();
            this.updateRadioButton = new System.Windows.Forms.RadioButton();
            this.replaceRadioButton = new System.Windows.Forms.RadioButton();
            this.label22 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.AccountsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.customersDataSet)).BeginInit();
            this.AddAccountPanel.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.importContactsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(23, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Accounts:";
            // 
            // AccountsComboBox
            // 
            this.AccountsComboBox.DataSource = this.AccountsBindingSource;
            this.AccountsComboBox.DisplayMember = "AccountName";
            this.AccountsComboBox.FormattingEnabled = true;
            this.AccountsComboBox.Location = new System.Drawing.Point(99, 33);
            this.AccountsComboBox.Name = "AccountsComboBox";
            this.AccountsComboBox.Size = new System.Drawing.Size(336, 21);
            this.AccountsComboBox.TabIndex = 1;
            this.AccountsComboBox.ValueMember = "AccountID";
            this.AccountsComboBox.SelectedIndexChanged += new System.EventHandler(this.AccountsComboBox_SelectedIndexChanged);
            // 
            // AccountsBindingSource
            // 
            this.AccountsBindingSource.DataMember = "Accounts";
            this.AccountsBindingSource.DataSource = this.customersDataSet;
            // 
            // customersDataSet
            // 
            this.customersDataSet.DataSetName = "CustomersDataSet";
            this.customersDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // accountsTableAdapter
            // 
            this.accountsTableAdapter.ClearBeforeFill = true;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(23, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 34);
            this.label2.TabIndex = 2;
            this.label2.Text = "Account Contacts:";
            // 
            // ContactsListBox
            // 
            this.ContactsListBox.FormattingEnabled = true;
            this.ContactsListBox.Location = new System.Drawing.Point(99, 69);
            this.ContactsListBox.Name = "ContactsListBox";
            this.ContactsListBox.Size = new System.Drawing.Size(336, 43);
            this.ContactsListBox.TabIndex = 3;
            this.ContactsListBox.SelectedIndexChanged += new System.EventHandler(this.ContactsListBox_SelectedIndexChanged);
            // 
            // AddAccountButton
            // 
            this.AddAccountButton.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.AddAccountButton.Location = new System.Drawing.Point(440, 30);
            this.AddAccountButton.Name = "AddAccountButton";
            this.AddAccountButton.Size = new System.Drawing.Size(38, 24);
            this.AddAccountButton.TabIndex = 4;
            this.AddAccountButton.Text = "Add";
            this.AddAccountButton.UseVisualStyleBackColor = false;
            this.AddAccountButton.Click += new System.EventHandler(this.AddAccountButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(53, 144);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 16);
            this.label3.TabIndex = 5;
            this.label3.Text = "First:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(31, 175);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 16);
            this.label5.TabIndex = 7;
            this.label5.Text = "Address:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(56, 206);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(36, 16);
            this.label6.TabIndex = 8;
            this.label6.Text = "City:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(339, 207);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(68, 16);
            this.label8.TabIndex = 10;
            this.label8.Text = "Zip Code:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(39, 237);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 16);
            this.label9.TabIndex = 11;
            this.label9.Text = "Phone:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(281, 237);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(36, 16);
            this.label10.TabIndex = 12;
            this.label10.Text = "Fax:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(44, 268);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(48, 16);
            this.label11.TabIndex = 13;
            this.label11.Text = "Email:";
            // 
            // FirstNameTextBox
            // 
            this.FirstNameTextBox.Location = new System.Drawing.Point(99, 143);
            this.FirstNameTextBox.Name = "FirstNameTextBox";
            this.FirstNameTextBox.Size = new System.Drawing.Size(155, 20);
            this.FirstNameTextBox.TabIndex = 14;
            // 
            // LastNameTextBox
            // 
            this.LastNameTextBox.Location = new System.Drawing.Point(323, 143);
            this.LastNameTextBox.Name = "LastNameTextBox";
            this.LastNameTextBox.Size = new System.Drawing.Size(155, 20);
            this.LastNameTextBox.TabIndex = 15;
            // 
            // AddressTextBox
            // 
            this.AddressTextBox.Location = new System.Drawing.Point(99, 174);
            this.AddressTextBox.Name = "AddressTextBox";
            this.AddressTextBox.Size = new System.Drawing.Size(380, 20);
            this.AddressTextBox.TabIndex = 16;
            // 
            // CityTextBox
            // 
            this.CityTextBox.Location = new System.Drawing.Point(99, 205);
            this.CityTextBox.Name = "CityTextBox";
            this.CityTextBox.Size = new System.Drawing.Size(155, 20);
            this.CityTextBox.TabIndex = 17;
            // 
            // StateTextBox
            // 
            this.StateTextBox.Location = new System.Drawing.Point(309, 206);
            this.StateTextBox.Name = "StateTextBox";
            this.StateTextBox.Size = new System.Drawing.Size(23, 20);
            this.StateTextBox.TabIndex = 18;
            // 
            // ZipTextBox
            // 
            this.ZipTextBox.Location = new System.Drawing.Point(413, 206);
            this.ZipTextBox.Name = "ZipTextBox";
            this.ZipTextBox.Size = new System.Drawing.Size(65, 20);
            this.ZipTextBox.TabIndex = 19;
            // 
            // PhoneTextBox
            // 
            this.PhoneTextBox.Location = new System.Drawing.Point(99, 236);
            this.PhoneTextBox.Name = "PhoneTextBox";
            this.PhoneTextBox.Size = new System.Drawing.Size(155, 20);
            this.PhoneTextBox.TabIndex = 20;
            // 
            // FaxTextBox
            // 
            this.FaxTextBox.Location = new System.Drawing.Point(323, 236);
            this.FaxTextBox.Name = "FaxTextBox";
            this.FaxTextBox.Size = new System.Drawing.Size(155, 20);
            this.FaxTextBox.TabIndex = 21;
            // 
            // EmailTextBox
            // 
            this.EmailTextBox.Location = new System.Drawing.Point(99, 267);
            this.EmailTextBox.Name = "EmailTextBox";
            this.EmailTextBox.Size = new System.Drawing.Size(380, 20);
            this.EmailTextBox.TabIndex = 22;
            // 
            // AddContactButton
            // 
            this.AddContactButton.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.AddContactButton.Location = new System.Drawing.Point(440, 79);
            this.AddContactButton.Name = "AddContactButton";
            this.AddContactButton.Size = new System.Drawing.Size(38, 24);
            this.AddContactButton.TabIndex = 23;
            this.AddContactButton.Text = "Add";
            this.AddContactButton.UseVisualStyleBackColor = false;
            this.AddContactButton.Click += new System.EventHandler(this.AddContact_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(279, 144);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 16);
            this.label4.TabIndex = 24;
            this.label4.Text = "Last:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(264, 207);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(45, 16);
            this.label7.TabIndex = 25;
            this.label7.Text = "State:";
            // 
            // AddAccountPanel
            // 
            this.AddAccountPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.AddAccountPanel.Controls.Add(this.AddAccountCancelButton);
            this.AddAccountPanel.Controls.Add(this.AddAcountConfirmButton);
            this.AddAccountPanel.Controls.Add(this.AddAccountTextBox);
            this.AddAccountPanel.Controls.Add(this.label12);
            this.AddAccountPanel.Location = new System.Drawing.Point(23, 30);
            this.AddAccountPanel.Name = "AddAccountPanel";
            this.AddAccountPanel.Size = new System.Drawing.Size(507, 98);
            this.AddAccountPanel.TabIndex = 26;
            this.AddAccountPanel.Visible = false;
            // 
            // AddAccountCancelButton
            // 
            this.AddAccountCancelButton.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.AddAccountCancelButton.Location = new System.Drawing.Point(283, 59);
            this.AddAccountCancelButton.Name = "AddAccountCancelButton";
            this.AddAccountCancelButton.Size = new System.Drawing.Size(53, 24);
            this.AddAccountCancelButton.TabIndex = 6;
            this.AddAccountCancelButton.Text = "Cancel";
            this.AddAccountCancelButton.UseVisualStyleBackColor = false;
            this.AddAccountCancelButton.Click += new System.EventHandler(this.AddAccountCancelButton_Click);
            // 
            // AddAcountConfirmButton
            // 
            this.AddAcountConfirmButton.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.AddAcountConfirmButton.Location = new System.Drawing.Point(180, 59);
            this.AddAcountConfirmButton.Name = "AddAcountConfirmButton";
            this.AddAcountConfirmButton.Size = new System.Drawing.Size(38, 24);
            this.AddAcountConfirmButton.TabIndex = 5;
            this.AddAcountConfirmButton.Text = "Add";
            this.AddAcountConfirmButton.UseVisualStyleBackColor = false;
            this.AddAcountConfirmButton.Click += new System.EventHandler(this.AddAcountConfirmButton_Click);
            // 
            // AddAccountTextBox
            // 
            this.AddAccountTextBox.Location = new System.Drawing.Point(120, 21);
            this.AddAccountTextBox.Name = "AddAccountTextBox";
            this.AddAccountTextBox.Size = new System.Drawing.Size(315, 20);
            this.AddAccountTextBox.TabIndex = 1;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(21, 23);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(93, 15);
            this.label12.TabIndex = 0;
            this.label12.Text = "Account Name:";
            // 
            // SaveContactButton
            // 
            this.SaveContactButton.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.SaveContactButton.Location = new System.Drawing.Point(205, 303);
            this.SaveContactButton.Name = "SaveContactButton";
            this.SaveContactButton.Size = new System.Drawing.Size(48, 24);
            this.SaveContactButton.TabIndex = 27;
            this.SaveContactButton.Text = "Save";
            this.SaveContactButton.UseVisualStyleBackColor = false;
            this.SaveContactButton.Click += new System.EventHandler(this.SaveContactButton_Click);
            // 
            // CancelNewContactButton
            // 
            this.CancelNewContactButton.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.CancelNewContactButton.Location = new System.Drawing.Point(284, 303);
            this.CancelNewContactButton.Name = "CancelNewContactButton";
            this.CancelNewContactButton.Size = new System.Drawing.Size(48, 24);
            this.CancelNewContactButton.TabIndex = 28;
            this.CancelNewContactButton.Text = "Cancel";
            this.CancelNewContactButton.UseVisualStyleBackColor = false;
            this.CancelNewContactButton.Click += new System.EventHandler(this.CancelNewContactButton_Click);
            // 
            // editLabel
            // 
            this.editLabel.AutoSize = true;
            this.editLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editLabel.Location = new System.Drawing.Point(96, 126);
            this.editLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.editLabel.Name = "editLabel";
            this.editLabel.Size = new System.Drawing.Size(296, 15);
            this.editLabel.TabIndex = 30;
            this.editLabel.Text = "To edit contact, simply make changes and click Save!";
            // 
            // editAccountNameButton
            // 
            this.editAccountNameButton.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.editAccountNameButton.Location = new System.Drawing.Point(483, 30);
            this.editAccountNameButton.Name = "editAccountNameButton";
            this.editAccountNameButton.Size = new System.Drawing.Size(38, 24);
            this.editAccountNameButton.TabIndex = 31;
            this.editAccountNameButton.Text = "Edit";
            this.editAccountNameButton.UseVisualStyleBackColor = false;
            this.editAccountNameButton.Click += new System.EventHandler(this.editAccountNameButton_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.PowderBlue;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 1, 0, 1);
            this.menuStrip1.Size = new System.Drawing.Size(537, 24);
            this.menuStrip1.TabIndex = 32;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuToolStripMenuItem
            // 
            this.menuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.listPrintContactsToolStripMenuItem,
            this.importContactsToolStripMenuItem});
            this.menuToolStripMenuItem.Name = "menuToolStripMenuItem";
            this.menuToolStripMenuItem.Size = new System.Drawing.Size(50, 22);
            this.menuToolStripMenuItem.Text = "Menu";
            // 
            // listPrintContactsToolStripMenuItem
            // 
            this.listPrintContactsToolStripMenuItem.Name = "listPrintContactsToolStripMenuItem";
            this.listPrintContactsToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.listPrintContactsToolStripMenuItem.Text = "List /Print Contacts";
            this.listPrintContactsToolStripMenuItem.Click += new System.EventHandler(this.listPrintContactsToolStripMenuItem_Click);
            // 
            // importContactsToolStripMenuItem
            // 
            this.importContactsToolStripMenuItem.Name = "importContactsToolStripMenuItem";
            this.importContactsToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.importContactsToolStripMenuItem.Text = "Import Contacts";
            this.importContactsToolStripMenuItem.Click += new System.EventHandler(this.importContactsToolStripMenuItem_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(12, 9);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(181, 16);
            this.label13.TabIndex = 0;
            this.label13.Text = "Import / Update Contacts";
            // 
            // label14
            // 
            this.label14.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(23, 31);
            this.label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(469, 49);
            this.label14.TabIndex = 1;
            this.label14.Text = "You can import contacts from any source that you can save (export) as a comma \r\ns" +
    "eperated file (csv). MS Access, MS Outlook, MS Excel, Thunderbird, Google and ot" +
    "hers.";
            // 
            // importContactsPanel
            // 
            this.importContactsPanel.Controls.Add(this.importContactsButton);
            this.importContactsPanel.Controls.Add(this.updateRadioButton);
            this.importContactsPanel.Controls.Add(this.replaceRadioButton);
            this.importContactsPanel.Controls.Add(this.label22);
            this.importContactsPanel.Controls.Add(this.label21);
            this.importContactsPanel.Controls.Add(this.label20);
            this.importContactsPanel.Controls.Add(this.label19);
            this.importContactsPanel.Controls.Add(this.label18);
            this.importContactsPanel.Controls.Add(this.label17);
            this.importContactsPanel.Controls.Add(this.label16);
            this.importContactsPanel.Controls.Add(this.label15);
            this.importContactsPanel.Controls.Add(this.label14);
            this.importContactsPanel.Controls.Add(this.label13);
            this.importContactsPanel.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.importContactsPanel.Location = new System.Drawing.Point(9, 30);
            this.importContactsPanel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.importContactsPanel.Name = "importContactsPanel";
            this.importContactsPanel.Size = new System.Drawing.Size(520, 297);
            this.importContactsPanel.TabIndex = 33;
            this.importContactsPanel.Visible = false;
            // 
            // importContactsButton
            // 
            this.importContactsButton.BackColor = System.Drawing.Color.PowderBlue;
            this.importContactsButton.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.importContactsButton.Location = new System.Drawing.Point(207, 259);
            this.importContactsButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.importContactsButton.Name = "importContactsButton";
            this.importContactsButton.Size = new System.Drawing.Size(101, 22);
            this.importContactsButton.TabIndex = 15;
            this.importContactsButton.Text = "Import Contacts";
            this.importContactsButton.UseVisualStyleBackColor = false;
            this.importContactsButton.Click += new System.EventHandler(this.importContactsButton_Click);
            // 
            // updateRadioButton
            // 
            this.updateRadioButton.AutoSize = true;
            this.updateRadioButton.Checked = true;
            this.updateRadioButton.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.updateRadioButton.Location = new System.Drawing.Point(276, 217);
            this.updateRadioButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.updateRadioButton.Name = "updateRadioButton";
            this.updateRadioButton.Size = new System.Drawing.Size(115, 18);
            this.updateRadioButton.TabIndex = 14;
            this.updateRadioButton.TabStop = true;
            this.updateRadioButton.Text = "Update Contacts";
            this.updateRadioButton.UseVisualStyleBackColor = true;
            // 
            // replaceRadioButton
            // 
            this.replaceRadioButton.AutoSize = true;
            this.replaceRadioButton.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.replaceRadioButton.Location = new System.Drawing.Point(131, 217);
            this.replaceRadioButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.replaceRadioButton.Name = "replaceRadioButton";
            this.replaceRadioButton.Size = new System.Drawing.Size(120, 18);
            this.replaceRadioButton.TabIndex = 13;
            this.replaceRadioButton.Text = "Replace Contacts";
            this.replaceRadioButton.UseVisualStyleBackColor = true;
            // 
            // label22
            // 
            this.label22.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.Location = new System.Drawing.Point(30, 161);
            this.label22.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(466, 47);
            this.label22.TabIndex = 10;
            this.label22.Text = "Account Name, First Name, Last Name, Street Address, City, State (2 characters), " +
    "Zip Code, Phone Number, Fax Number, Email Address";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.Location = new System.Drawing.Point(21, 144);
            this.label21.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(356, 14);
            this.label21.TabIndex = 9;
            this.label21.Text = "CSV file must have the following columns with no heading row.";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(384, 110);
            this.label20.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(47, 15);
            this.label20.TabIndex = 8;
            this.label20.Text = "Google";
            this.label20.Click += new System.EventHandler(this.label20_Click);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(294, 110);
            this.label19.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(74, 15);
            this.label19.TabIndex = 7;
            this.label19.Text = "Thunderbird";
            this.label19.Click += new System.EventHandler(this.label19_Click);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(215, 110);
            this.label18.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(56, 15);
            this.label18.TabIndex = 6;
            this.label18.Text = "MS Excel";
            this.label18.Click += new System.EventHandler(this.label18_Click);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(127, 110);
            this.label17.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(69, 15);
            this.label17.TabIndex = 5;
            this.label17.Text = "MS Outlook";
            this.label17.Click += new System.EventHandler(this.label17_Click);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(39, 110);
            this.label16.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(66, 15);
            this.label16.TabIndex = 4;
            this.label16.Text = "MS Access";
            this.label16.Click += new System.EventHandler(this.label16_Click);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(21, 92);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(180, 14);
            this.label15.TabIndex = 3;
            this.label15.Text = "Instructions to export contacts:";
            // 
            // ContactsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.BlanchedAlmond;
            this.ClientSize = new System.Drawing.Size(537, 357);
            this.Controls.Add(this.importContactsPanel);
            this.Controls.Add(this.editLabel);
            this.Controls.Add(this.CancelNewContactButton);
            this.Controls.Add(this.SaveContactButton);
            this.Controls.Add(this.AddAccountPanel);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.AddContactButton);
            this.Controls.Add(this.EmailTextBox);
            this.Controls.Add(this.FaxTextBox);
            this.Controls.Add(this.PhoneTextBox);
            this.Controls.Add(this.ZipTextBox);
            this.Controls.Add(this.StateTextBox);
            this.Controls.Add(this.CityTextBox);
            this.Controls.Add(this.AddressTextBox);
            this.Controls.Add(this.LastNameTextBox);
            this.Controls.Add(this.FirstNameTextBox);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.AddAccountButton);
            this.Controls.Add(this.ContactsListBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.AccountsComboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.editAccountNameButton);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ContactsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Contacts";
            this.Load += new System.EventHandler(this.Contacts_Load);
            this.Shown += new System.EventHandler(this.SetContactDisplay);
            ((System.ComponentModel.ISupportInitialize)(this.AccountsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.customersDataSet)).EndInit();
            this.AddAccountPanel.ResumeLayout(false);
            this.AddAccountPanel.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.importContactsPanel.ResumeLayout(false);
            this.importContactsPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox AccountsComboBox;
        private System.Windows.Forms.BindingSource AccountsBindingSource;
        private CustomersDataSet customersDataSet;
        private CustomersDataSetTableAdapters.AccountsTableAdapter accountsTableAdapter;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox ContactsListBox;
        private System.Windows.Forms.Button AddAccountButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox FirstNameTextBox;
        private System.Windows.Forms.TextBox LastNameTextBox;
        private System.Windows.Forms.TextBox AddressTextBox;
        private System.Windows.Forms.TextBox CityTextBox;
        private System.Windows.Forms.TextBox StateTextBox;
        private System.Windows.Forms.TextBox ZipTextBox;
        private System.Windows.Forms.TextBox PhoneTextBox;
        private System.Windows.Forms.TextBox FaxTextBox;
        private System.Windows.Forms.TextBox EmailTextBox;
        private System.Windows.Forms.Button AddContactButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel AddAccountPanel;
        private System.Windows.Forms.Button AddAccountCancelButton;
        private System.Windows.Forms.Button AddAcountConfirmButton;
        private System.Windows.Forms.TextBox AddAccountTextBox;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button SaveContactButton;
        private System.Windows.Forms.Button CancelNewContactButton;
        private System.Windows.Forms.Label editLabel;
        private System.Windows.Forms.Button editAccountNameButton;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem listPrintContactsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importContactsToolStripMenuItem;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Panel importContactsPanel;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.RadioButton updateRadioButton;
        private System.Windows.Forms.RadioButton replaceRadioButton;
        private System.Windows.Forms.Button importContactsButton;
    }
}