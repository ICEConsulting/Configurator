using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Tecan_Quote_Generator
{
    public partial class ProfileForm : Form
    {

        MainQuoteForm mainForm;
        Boolean doInitialization;
        public void SetForm1Instance(MainQuoteForm inst)
        {
            mainForm = inst;
        }

        public ProfileForm(Boolean NeedsDB)
        {
            InitializeComponent();
            doInitialization = NeedsDB;
            String profileFile = @"c:\TecanFiles\" + "TecanQuoteConfig.cfg";
            if (File.Exists(profileFile))
            {
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(Profile));
                System.IO.StreamReader file = new System.IO.StreamReader(profileFile);
                Profile profile = new Profile();
                profile = (Profile)reader.Deserialize(file);
                file.Close();
                ProfileNameTextBox.Text = profile.Name;
                ProfileInitialsTextBox.Text = profile.Initials;
                ProfilePhoneTextBox.Text = profile.Phone;
                ProfileEmailTextBox.Text = profile.Email;
                ProfileDistributionFolderTextBox.Text = profile.DistributionFolder;
            }
        }

        private void BrowseDistributionFolderButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            folderBrowserDialog1.Description = "Please select your Quote Database Distribution Folder";
            folderBrowserDialog1.ShowNewFolderButton = false;

            String sourcePath = "";
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                sourcePath = folderBrowserDialog1.SelectedPath;
            }
            ProfileDistributionFolderTextBox.Text = sourcePath;
        }

        private void ProfileSaveButton_Click(object sender, EventArgs e)
        {
            String errorMessage = "All fields must be entered!\n\nPlease enter \\ correct the following information.\n\n";
            Boolean profileError = false;
            if (ProfileNameTextBox.Text == "")
            {
                profileError = true;
                errorMessage = errorMessage + "Your Full Name.\n\n";
            }
            if (ProfileInitialsTextBox.Text == "")
            {
                profileError = true;
                errorMessage = errorMessage + "Your Initials.\n\n";
            }
            if (ProfilePhoneTextBox.Text == "" || ProfilePhoneTextBox.Text.Length < 12)
            {
                profileError = true;
                errorMessage = errorMessage + "Your Phone Number.\n\n";
            }
            RegexUtilities util = new RegexUtilities();
            if (ProfileEmailTextBox.Text == "" || !util.IsValidEmail(ProfileEmailTextBox.Text))
            {
                profileError = true;
                errorMessage = errorMessage + "Your Email Address.\n\n";
            }
            if (ProfileDistributionFolderTextBox.Text == "")
            {
                profileError = true;
                errorMessage = errorMessage + "Tecan Database Distribution Folder Location.\n\n";
            }
            if (profileError)
            {
                MessageBox.Show(errorMessage);
                // return;
            }
            else
            {
                Profile profile = new Profile();

                profile.Name = ProfileNameTextBox.Text;
                profile.Initials = ProfileInitialsTextBox.Text;
                profile.Phone = ProfilePhoneTextBox.Text;
                profile.Email = ProfileEmailTextBox.Text;
                profile.DistributionFolder = ProfileDistributionFolderTextBox.Text;

                // Save to Profile config file
                String tecanFilesFilePath = @"c:\TecanFiles";
                System.IO.Directory.CreateDirectory(tecanFilesFilePath);

                String profileFile = @"c:\TecanFiles\" + "TecanQuoteConfig.cfg";
                System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(Profile));
                System.IO.StreamWriter file = new System.IO.StreamWriter(profileFile);
                writer.Serialize(file, profile);
                file.Close();
                this.Close();

                if (doInitialization)
                {
                    // Copy database from distribution folder 
                    Boolean fileFound;
                    fileFound = mainForm.copyDatabaseToWorkingFolder(profile.DistributionFolder);
                    if (!fileFound)
                    {
                        MessageBox.Show("The Distribution Folder you selected does not contain the Quote Database!\n\nPlease select a new folder");
                        mainForm.showUserProfileForm(true);
                    }
                    else
                    {
                        mainForm.getUsersProfile();
                        mainForm.MainQuoteForm_Load(sender, e);
                    }
                }
                else
                {
                    mainForm.getUsersProfile();
                }
            }

        }

        private void ProfilePhoneTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ProfilePhoneTextBox.Text.Length == 3)
                ProfilePhoneTextBox.Text = ProfilePhoneTextBox.Text + "-";
            if (ProfilePhoneTextBox.Text.Length == 7)
                ProfilePhoneTextBox.Text = ProfilePhoneTextBox.Text + "-";
            ProfilePhoneTextBox.SelectionStart = ProfilePhoneTextBox.Text.Length;

        }
    }
}
