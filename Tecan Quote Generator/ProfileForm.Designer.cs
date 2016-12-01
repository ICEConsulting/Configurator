namespace Tecan_Quote_Generator
{
    partial class ProfileForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.ProfileNameTextBox = new System.Windows.Forms.TextBox();
            this.ProfileInitialsTextBox = new System.Windows.Forms.TextBox();
            this.ProfilePhoneTextBox = new System.Windows.Forms.TextBox();
            this.ProfileEmailTextBox = new System.Windows.Forms.TextBox();
            this.ProfileDistributionFolderTextBox = new System.Windows.Forms.TextBox();
            this.BrowseDistributionFolderButton = new System.Windows.Forms.Button();
            this.ProfileSaveButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(113, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(63, 86);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Phone Number:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(67, 117);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "Email Address:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(111, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 15);
            this.label4.TabIndex = 3;
            this.label4.Text = "Initials:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(63, 167);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(204, 15);
            this.label5.TabIndex = 4;
            this.label5.Text = "Tecan Database Distribution Folder:";
            // 
            // ProfileNameTextBox
            // 
            this.ProfileNameTextBox.Location = new System.Drawing.Point(163, 22);
            this.ProfileNameTextBox.Name = "ProfileNameTextBox";
            this.ProfileNameTextBox.Size = new System.Drawing.Size(276, 20);
            this.ProfileNameTextBox.TabIndex = 5;
            // 
            // ProfileInitialsTextBox
            // 
            this.ProfileInitialsTextBox.Location = new System.Drawing.Point(163, 53);
            this.ProfileInitialsTextBox.Name = "ProfileInitialsTextBox";
            this.ProfileInitialsTextBox.Size = new System.Drawing.Size(35, 20);
            this.ProfileInitialsTextBox.TabIndex = 6;
            // 
            // ProfilePhoneTextBox
            // 
            this.ProfilePhoneTextBox.Location = new System.Drawing.Point(163, 84);
            this.ProfilePhoneTextBox.Name = "ProfilePhoneTextBox";
            this.ProfilePhoneTextBox.Size = new System.Drawing.Size(119, 20);
            this.ProfilePhoneTextBox.TabIndex = 7;
            this.ProfilePhoneTextBox.TextChanged += new System.EventHandler(this.ProfilePhoneTextBox_TextChanged);
            // 
            // ProfileEmailTextBox
            // 
            this.ProfileEmailTextBox.Location = new System.Drawing.Point(163, 115);
            this.ProfileEmailTextBox.Name = "ProfileEmailTextBox";
            this.ProfileEmailTextBox.Size = new System.Drawing.Size(276, 20);
            this.ProfileEmailTextBox.TabIndex = 8;
            // 
            // ProfileDistributionFolderTextBox
            // 
            this.ProfileDistributionFolderTextBox.Location = new System.Drawing.Point(66, 192);
            this.ProfileDistributionFolderTextBox.Name = "ProfileDistributionFolderTextBox";
            this.ProfileDistributionFolderTextBox.ReadOnly = true;
            this.ProfileDistributionFolderTextBox.Size = new System.Drawing.Size(380, 20);
            this.ProfileDistributionFolderTextBox.TabIndex = 9;
            this.ProfileDistributionFolderTextBox.TabStop = false;
            // 
            // BrowseDistributionFolderButton
            // 
            this.BrowseDistributionFolderButton.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.BrowseDistributionFolderButton.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BrowseDistributionFolderButton.Location = new System.Drawing.Point(273, 165);
            this.BrowseDistributionFolderButton.Name = "BrowseDistributionFolderButton";
            this.BrowseDistributionFolderButton.Size = new System.Drawing.Size(65, 21);
            this.BrowseDistributionFolderButton.TabIndex = 10;
            this.BrowseDistributionFolderButton.Text = "Browse";
            this.BrowseDistributionFolderButton.UseVisualStyleBackColor = false;
            this.BrowseDistributionFolderButton.Click += new System.EventHandler(this.BrowseDistributionFolderButton_Click);
            // 
            // ProfileSaveButton
            // 
            this.ProfileSaveButton.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ProfileSaveButton.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProfileSaveButton.Location = new System.Drawing.Point(219, 228);
            this.ProfileSaveButton.Name = "ProfileSaveButton";
            this.ProfileSaveButton.Size = new System.Drawing.Size(65, 21);
            this.ProfileSaveButton.TabIndex = 11;
            this.ProfileSaveButton.Text = "Save";
            this.ProfileSaveButton.UseVisualStyleBackColor = false;
            this.ProfileSaveButton.Click += new System.EventHandler(this.ProfileSaveButton_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(288, 86);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(102, 14);
            this.label6.TabIndex = 12;
            this.label6.Text = "Numbers Only (0-9)";
            // 
            // ProfileForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.BlanchedAlmond;
            this.ClientSize = new System.Drawing.Size(502, 261);
            this.ControlBox = false;
            this.Controls.Add(this.label6);
            this.Controls.Add(this.ProfileSaveButton);
            this.Controls.Add(this.BrowseDistributionFolderButton);
            this.Controls.Add(this.ProfileDistributionFolderTextBox);
            this.Controls.Add(this.ProfileEmailTextBox);
            this.Controls.Add(this.ProfilePhoneTextBox);
            this.Controls.Add(this.ProfileInitialsTextBox);
            this.Controls.Add(this.ProfileNameTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProfileForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Please enter your personal information ";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox ProfileNameTextBox;
        private System.Windows.Forms.TextBox ProfileInitialsTextBox;
        private System.Windows.Forms.TextBox ProfilePhoneTextBox;
        private System.Windows.Forms.TextBox ProfileEmailTextBox;
        private System.Windows.Forms.TextBox ProfileDistributionFolderTextBox;
        private System.Windows.Forms.Button BrowseDistributionFolderButton;
        private System.Windows.Forms.Button ProfileSaveButton;
        private System.Windows.Forms.Label label6;
    }
}