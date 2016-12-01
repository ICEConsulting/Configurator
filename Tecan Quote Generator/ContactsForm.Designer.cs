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
            ((System.ComponentModel.ISupportInitialize)(this.AccountsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.customersDataSet)).BeginInit();
            this.AddAccountPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(34, 16);
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
            this.AccountsComboBox.Location = new System.Drawing.Point(109, 15);
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
            this.label2.Location = new System.Drawing.Point(34, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 34);
            this.label2.TabIndex = 2;
            this.label2.Text = "Account Contacts:";
            // 
            // ContactsListBox
            // 
            this.ContactsListBox.FormattingEnabled = true;
            this.ContactsListBox.Location = new System.Drawing.Point(109, 51);
            this.ContactsListBox.Name = "ContactsListBox";
            this.ContactsListBox.Size = new System.Drawing.Size(336, 43);
            this.ContactsListBox.TabIndex = 3;
            this.ContactsListBox.SelectedIndexChanged += new System.EventHandler(this.ContactsListBox_SelectedIndexChanged);
            // 
            // AddAccountButton
            // 
            this.AddAccountButton.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.AddAccountButton.Location = new System.Drawing.Point(451, 12);
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
            this.label3.Location = new System.Drawing.Point(64, 126);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 16);
            this.label3.TabIndex = 5;
            this.label3.Text = "First:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(41, 157);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 16);
            this.label5.TabIndex = 7;
            this.label5.Text = "Address:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(67, 188);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(36, 16);
            this.label6.TabIndex = 8;
            this.label6.Text = "City:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(350, 189);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(68, 16);
            this.label8.TabIndex = 10;
            this.label8.Text = "Zip Code:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(50, 219);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 16);
            this.label9.TabIndex = 11;
            this.label9.Text = "Phone:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(292, 219);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(36, 16);
            this.label10.TabIndex = 12;
            this.label10.Text = "Fax:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(55, 250);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(48, 16);
            this.label11.TabIndex = 13;
            this.label11.Text = "Email:";
            // 
            // FirstNameTextBox
            // 
            this.FirstNameTextBox.Location = new System.Drawing.Point(109, 125);
            this.FirstNameTextBox.Name = "FirstNameTextBox";
            this.FirstNameTextBox.Size = new System.Drawing.Size(155, 20);
            this.FirstNameTextBox.TabIndex = 14;
            // 
            // LastNameTextBox
            // 
            this.LastNameTextBox.Location = new System.Drawing.Point(334, 125);
            this.LastNameTextBox.Name = "LastNameTextBox";
            this.LastNameTextBox.Size = new System.Drawing.Size(155, 20);
            this.LastNameTextBox.TabIndex = 15;
            // 
            // AddressTextBox
            // 
            this.AddressTextBox.Location = new System.Drawing.Point(109, 156);
            this.AddressTextBox.Name = "AddressTextBox";
            this.AddressTextBox.Size = new System.Drawing.Size(380, 20);
            this.AddressTextBox.TabIndex = 16;
            // 
            // CityTextBox
            // 
            this.CityTextBox.Location = new System.Drawing.Point(109, 187);
            this.CityTextBox.Name = "CityTextBox";
            this.CityTextBox.Size = new System.Drawing.Size(155, 20);
            this.CityTextBox.TabIndex = 17;
            // 
            // StateTextBox
            // 
            this.StateTextBox.Location = new System.Drawing.Point(320, 188);
            this.StateTextBox.Name = "StateTextBox";
            this.StateTextBox.Size = new System.Drawing.Size(23, 20);
            this.StateTextBox.TabIndex = 18;
            // 
            // ZipTextBox
            // 
            this.ZipTextBox.Location = new System.Drawing.Point(424, 188);
            this.ZipTextBox.Name = "ZipTextBox";
            this.ZipTextBox.Size = new System.Drawing.Size(65, 20);
            this.ZipTextBox.TabIndex = 19;
            // 
            // PhoneTextBox
            // 
            this.PhoneTextBox.Location = new System.Drawing.Point(109, 218);
            this.PhoneTextBox.Name = "PhoneTextBox";
            this.PhoneTextBox.Size = new System.Drawing.Size(155, 20);
            this.PhoneTextBox.TabIndex = 20;
            // 
            // FaxTextBox
            // 
            this.FaxTextBox.Location = new System.Drawing.Point(334, 218);
            this.FaxTextBox.Name = "FaxTextBox";
            this.FaxTextBox.Size = new System.Drawing.Size(155, 20);
            this.FaxTextBox.TabIndex = 21;
            // 
            // EmailTextBox
            // 
            this.EmailTextBox.Location = new System.Drawing.Point(109, 249);
            this.EmailTextBox.Name = "EmailTextBox";
            this.EmailTextBox.Size = new System.Drawing.Size(380, 20);
            this.EmailTextBox.TabIndex = 22;
            // 
            // AddContactButton
            // 
            this.AddContactButton.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.AddContactButton.Location = new System.Drawing.Point(451, 61);
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
            this.label4.Location = new System.Drawing.Point(289, 126);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 16);
            this.label4.TabIndex = 24;
            this.label4.Text = "Last:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(275, 189);
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
            this.AddAccountPanel.Location = new System.Drawing.Point(44, 337);
            this.AddAccountPanel.Name = "AddAccountPanel";
            this.AddAccountPanel.Size = new System.Drawing.Size(452, 116);
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
            this.SaveContactButton.Location = new System.Drawing.Point(216, 285);
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
            this.CancelNewContactButton.Location = new System.Drawing.Point(295, 285);
            this.CancelNewContactButton.Name = "CancelNewContactButton";
            this.CancelNewContactButton.Size = new System.Drawing.Size(48, 24);
            this.CancelNewContactButton.TabIndex = 28;
            this.CancelNewContactButton.Text = "Cancel";
            this.CancelNewContactButton.UseVisualStyleBackColor = false;
            // 
            // ContactsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.BlanchedAlmond;
            this.ClientSize = new System.Drawing.Size(537, 557);
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
            this.Name = "ContactsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Contacts";
            this.Load += new System.EventHandler(this.Contacts_Load);
            this.Shown += new System.EventHandler(this.SetContactDisplay);
            ((System.ComponentModel.ISupportInitialize)(this.AccountsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.customersDataSet)).EndInit();
            this.AddAccountPanel.ResumeLayout(false);
            this.AddAccountPanel.PerformLayout();
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
    }
}