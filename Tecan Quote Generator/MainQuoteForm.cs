using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlServerCe;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Globalization;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.xml;
using System.Threading;
using System.Diagnostics;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Tecan_Quote_Generator
{
    public partial class MainQuoteForm : Form
    {
        public static Boolean isManager = false;
        SqlCeConnection TecanDatabase = null;
        SqlCeConnection TecanAppDocDatabase = null;
        SqlCeConnection ContactDatabase = null;

        Boolean searchPreformed = true;
        Boolean salesTypeChanged = false;
        Boolean instrumentChanged = false;
        Boolean categoryChanged = false;
        // Boolean subCategoryChanged = false;
        // Boolean formatOnly = false;
        Boolean quoteSaved = true;

        PartsListDetailDisplay DetailsForm;

        public Profile profile = new Profile();
        Quote passRequiredItems = new Quote();

        public MainQuoteForm()
        {
            InitializeComponent();
        }

        public void MainQuoteForm_Load(object sender, EventArgs e)
        {
            this.accountsTableAdapter.FillBy(this.customersDataSet.Accounts);
            short currentAccountID = Convert.ToInt16(AccountComboBox.SelectedValue);
            this.contactsTableAdapter.FillByAccountID(this.customersDataSet.Contacts, currentAccountID);

            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.PartNumberClearButton, "Clear Part Number Search");
            ToolTip1.SetToolTip(this.DescriptionClearButton, "Clear Description Search");
            ToolTip1.SetToolTip(this.ClearFiltersButton, "Reset All Category Filters");

            // Center all panels
            RequiredPartsPanel.Location = new Point(
            this.ClientSize.Width / 2 - RequiredPartsPanel.Size.Width / 2,
            this.ClientSize.Height / 2 - RequiredPartsPanel.Size.Height / 2);
            RequiredPartsPanel.Anchor = AnchorStyles.None;

            PleaseWaitPanel.Location = new Point(
            this.ClientSize.Width / 2 - PleaseWaitPanel.Size.Width / 2,
            this.ClientSize.Height / 2 - PleaseWaitPanel.Size.Height / 2);
            PleaseWaitPanel.Anchor = AnchorStyles.None;

            BugReportPanel.Location = new Point(
            this.ClientSize.Width / 2 - BugReportPanel.Size.Width / 2,
            this.ClientSize.Height / 2 - BugReportPanel.Size.Height / 2);
            BugReportPanel.Anchor = AnchorStyles.None;

            this.subCategoryTableAdapter.Fill(this.tecanQuoteGeneratorPartsListDataSet.SubCategory);
            this.categoryTableAdapter.Fill(this.tecanQuoteGeneratorPartsListDataSet.Category);
            this.instrumentTableAdapter.Fill(this.tecanQuoteGeneratorPartsListDataSet.Instrument);
            this.salesTypeTableAdapter.Fill(this.tecanQuoteGeneratorPartsListDataSet.SalesType);
            this.partsListTableAdapter.Fill(this.tecanQuoteGeneratorPartsListDataSet.PartsList);

            QuoteDataGridView.AllowDrop = true;
            OptionsDataGridView.AllowDrop = true;

            // Check if Quote Database is empty
            if (partsListBindingSource.Count != 0)
            {
                loadFilterComboBoxes();
                setPartDetailTextBox();
                loadTemplateCategories();
                //QuoteDataGridView.AllowDrop = true;
                //OptionsDataGridView.AllowDrop = true;
                // ThirdPartyDataGridView.AllowDrop = true;
                // SmartStartDataGridView.AllowDrop = true;
                QuoteTabControl.SelectedTab = QuoteSettingTabPage;
            }
        }

        private void loadTemplateCategories()
        {
            // Load appication categories
            openAppDocDatabase();
            SqlCeCommand cmd = TecanAppDocDatabase.CreateCommand();
            SqlCeDataReader reader;

            // Load Load AppCat combobox
            cmd.CommandText = "SELECT AppCategoryID, AppCategoryName FROM ApplicationCategories WHERE AppCategoryName != 'General'";
            reader = cmd.ExecuteReader();
            DataTable ct = new DataTable();
            ct.Columns.Add("AppCategoryID");
            ct.Columns.Add("AppCategoryName");
            ct.Load(reader);
            QuoteTypeComboBox.ValueMember = "AppCategoryID";
            QuoteTypeComboBox.DisplayMember = "AppCategoryName";
            QuoteTypeComboBox.DataSource = ct;
            QuoteTypeComboBox.SelectedIndex = 0;
            TecanAppDocDatabase.Close();
            reader.Dispose();
        }

        private void loadTemplateList()
        {
            // Load appication categories
            openAppDocDatabase();
            SqlCeCommand cmd = TecanAppDocDatabase.CreateCommand();
            SqlCeDataReader reader;

            // Load Template combobox
            cmd.CommandText = "SELECT DocID, DocumentDescription FROM Documents WHERE ApplicationCategory = " + QuoteTypeComboBox.SelectedValue + " AND DocumentPosition = 1";
            reader = cmd.ExecuteReader();
            DataTable st = new DataTable();
            st.Clear();
            st.Columns.Add("DocID");
            st.Columns.Add("DocumentDescription");
            st.Load(reader);
            // MessageBox.Show(st.Rows.Count.ToString());
            SmartStartHeaderComboBox.ValueMember = "DocID";
            SmartStartHeaderComboBox.DisplayMember = "DocumentDescription";
            SmartStartHeaderComboBox.DataSource = st;
            // SmartStartHeaderComboBox.Refresh();

            cmd.CommandText = "SELECT DocID, DocumentDescription FROM Documents WHERE ApplicationCategory = " + QuoteTypeComboBox.SelectedValue + " AND DocumentPosition = 2";
            reader = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Clear();
            dt.Columns.Add("DocID");
            dt.Columns.Add("DocumentDescription");
            dt.Load(reader);
            QuoteTemplateComboBox.ValueMember = "DocID";
            QuoteTemplateComboBox.DisplayMember = "DocumentDescription";
            QuoteTemplateComboBox.DataSource = dt;
            // QuoteTemplateComboBox.Refresh();
            
            TecanAppDocDatabase.Close();
            reader.Dispose();
        }

        private void FormIsClosing(object sender, FormClosingEventArgs e)
        {
            if (!quoteSaved)
            {
                if (MessageBox.Show("This quote has not been saved or you have made changes!\r\n\r\nDo you want to save before closing?", "Save Quote", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    e.Cancel = true;
                    saveQuoteToolStripMenuItem_Click(sender, e);
                }
            }
        }

        public void doFormInitialization()
        {
            this.subCategoryTableAdapter.Fill(this.tecanQuoteGeneratorPartsListDataSet.SubCategory);
            this.categoryTableAdapter.Fill(this.tecanQuoteGeneratorPartsListDataSet.Category);
            this.instrumentTableAdapter.Fill(this.tecanQuoteGeneratorPartsListDataSet.Instrument);
            this.salesTypeTableAdapter.Fill(this.tecanQuoteGeneratorPartsListDataSet.SalesType);
            this.partsListTableAdapter.Fill(this.tecanQuoteGeneratorPartsListDataSet.PartsList);

            // Check for salesman profile, if no file get salesman information
            // Check if Quote Database is empty
            if (partsListBindingSource.Count != 0)
            {
                loadFilterComboBoxes();
                setPartDetailTextBox();
                loadTemplateCategories();
                QuoteTabControl.SelectedTab = QuoteSettingTabPage;
            }
        }

        // Called from Form Shown event, Only processed if there is no current database
        // Reads or Creates this users profile xml file.
        private void getProfileAndDatabase(object sender, EventArgs e)
        {
            String profileFile = @"c:\TecanFiles\" + "TecanQuoteConfig.cfg";
            // Normal Condition, just load profile and run
            if (File.Exists(profileFile) && partsListBindingSource.Count > 1)
            {
                getUsersProfile();
                checkForNewDatabase();
            }
            // Checks DB and copies / loads if required
            else if (partsListBindingSource.Count <= 1)
            {
                showUserProfileForm(true);
            }
            // Just get new users profile
            else if (!File.Exists(profileFile))
            {
                showUserProfileForm(false);
            }
        }

        public void checkForNewDatabase()
        {
            // todo May want to check dates on all files (Smart Start, Supp, and App databases)
            String distributionPath = profile.DistributionFolder;
            String quoteDistributionFile;
            DateTime distributionFileDate;

            String quoteCurrentFile;
            DateTime currentFileDate;

            quoteDistributionFile = System.IO.Path.Combine(distributionPath, "TecanQuoteGeneratorPartsList.sdf");
            quoteCurrentFile = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "TecanQuoteGeneratorPartsList.sdf");

            if (File.Exists(quoteDistributionFile))
            {
                FileInfo fi = new FileInfo(quoteDistributionFile);
                distributionFileDate = fi.LastWriteTime;
                currentFileDate = profile.DatabaseCreationDate;
                if (distributionFileDate > currentFileDate)
                {
                    if (MessageBox.Show("There is a new parts list database file available. Do you want me to update the database now?", "Update Database", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        copyDatabaseToWorkingFolder();
                    }
                }
            } 
        }

        public void showUserProfileForm(Boolean NeedsDB)
        {
            ProfileForm profileForm = new ProfileForm(NeedsDB);
            profileForm.SetForm1Instance(this);
            profileForm.Show();
            Application.OpenForms["ProfileForm"].BringToFront();
        }

        // If blank database or new database available copy new database to working folder
        public void copyDatabaseToWorkingFolder()
        {
            PleaseWaitMessageTextBox.Text = "Copying the database may take a couple minutes!";
            PleaseWaitPanel.Visible = true;

            Thread copyThread;
            copyThread = new Thread(new ThreadStart(doTheCopy));
            copyThread.Start();
            while(copyThread.IsAlive)
            {
                Application.DoEvents();
            }
            if (!PleaseWaitMessageTextBox.Text.Contains("failed"))
            {
                doFormInitialization();
                PleaseWaitPanel.Visible = false;
            }
            else
            {
                PleaseWaitHeadingLabel.ForeColor = System.Drawing.Color.Red;
                PleaseWaitHeadingLabel.Text = "Copy Failed!";
                PLeaseWaitPanelOKButton.Visible = true;
            }
        }

        private void doTheCopy()
        {
            String quoteSourceFile = "";
            String smartStartSourceFile = "";
            String supplementSourceFile = "";
            String appDocsSourceFile = "";
            String sourcePath = profile.DistributionFolder;

            // Db source file locations
            quoteSourceFile = System.IO.Path.Combine(sourcePath, "TecanQuoteGeneratorPartsList.sdf");
            smartStartSourceFile = System.IO.Path.Combine(sourcePath, "TecanSmartStartQuoteGeneratorPartsList.sdf");
            supplementSourceFile = System.IO.Path.Combine(sourcePath, "TecanSuppDocs.sdf");
            appDocsSourceFile = System.IO.Path.Combine(sourcePath, "TecanAppDocs.sdf");

            String errorMessage = "";
            Boolean foundError = false;

            // Test file exists
            if (!File.Exists(quoteSourceFile))
            {
                errorMessage = "Your distribution folder does not contain a Partslist Database file (TecanQuoteGeneratorPartsList.sdf) \r\n\r\n";
                foundError = true;
            }
            else if (!File.Exists(smartStartSourceFile))
            {
                errorMessage += "Your distribution folder does not contain a Smart Start Partslist Database file (TecanSmartStartQuoteGeneratorPartsList.sdf) \r\n";
                foundError = true;
            }
            else if (!File.Exists(supplementSourceFile))
            {
                errorMessage += "Your distribution folder does not contain a Suppumental Documents file (TecanSuppDocs.sdf) \r\n";
                foundError = true;
            }
            else if (!File.Exists(appDocsSourceFile))
            {
                errorMessage += "Your distribution folder does not contain a Template Documents file (TecanAppDocs.sdf) \r\n";
                foundError = true;
            }

            if (!foundError)
            {
                // Where new files will go
                String currentFolder = Directory.GetCurrentDirectory();
                String quoteTargetFile = System.IO.Path.Combine(currentFolder, "TecanQuoteGeneratorPartsList.sdf");
                String smartStartTargetFile = System.IO.Path.Combine(currentFolder, "TecanSmartStartQuoteGeneratorPartsList.sdf");
                String supplementTargetFile = System.IO.Path.Combine(currentFolder, "TecanSuppDocs.sdf");
                String appDocsTargetFile = System.IO.Path.Combine(currentFolder, "TecanAppDocs.sdf");

                // Copy the files
                System.IO.File.Copy(quoteSourceFile, quoteTargetFile, true);
                System.IO.File.Copy(smartStartSourceFile, smartStartTargetFile, true);
                System.IO.File.Copy(supplementSourceFile, supplementTargetFile, true);
                System.IO.File.Copy(appDocsSourceFile, appDocsTargetFile, true);

                // Save the new Db Timestamp
                // getUsersProfile();
                FileInfo fi = new FileInfo(quoteSourceFile);
                profile.DatabaseCreationDate = fi.LastWriteTime;
                saveUsersProfile();

                if (PleaseWaitMessageTextBox.InvokeRequired)
                {
                    // It's on a different thread, so use Invoke.
                    SetTextCallback d = new SetTextCallback(SetText);
                    Invoke(d, new object[] { "Copy Complete!" });
                }
                else
                {
                    // It's on the same thread, no need for Invoke 
                    PleaseWaitMessageTextBox.Text = "Copy Complete!";
                }
            }
            else
            {
                // Tell user to get the files
                if (PleaseWaitMessageTextBox.InvokeRequired)
                {
                    // It's on a different thread, so use Invoke.
                    SetTextCallback d = new SetTextCallback(SetText);
                    Invoke(d, new object[] { "Copying the database failed!\r\n\r\n" + errorMessage + 
                        "\r\nPlease copy the required file(s) to your distribution folder, or use the Edit - " +
                        "My Profile menu item to select a different distribution folder!"});
                }
                else
                {
                    // It's on the same thread, no need for Invoke 
                    PleaseWaitMessageTextBox.Text = "Copying the database failed!\r\n\r\n" + errorMessage + 
                        "\r\nPlease copy the required file(s) to your distribution folder, or use the Edit - " +
                        "My Profile menu item to select a different distribution folder!";
                }
            }
        }

        // This delegate enables asynchronous calls for setting
        // the text property on a TextBox control.
        delegate void SetTextCallback(string text);

        // This method is passed in to the SetTextCallBack delegate 
        // to set the Text property of textBox1. 
        private void SetText(string text)
        {
            this.PleaseWaitMessageTextBox.Text = text;
        }

        // Initial Lookup Table lists, used for filtering displayed Parts
        // Lists are all loaded from lookup table items with all items
        // Look at updateCategoryComboBox() and updateSubCategoryComboBox() if only avaible items are required
        private void loadFilterComboBoxes()
        {
            // Sales Type
            SalesTypeComboBox.DataSource = this.SalesTypeBindingSource;
            SalesTypeComboBox.DisplayMember = "SalesTypeName";
            SalesTypeComboBox.ValueMember = "SalesTypeID";
            if (this.SalesTypeBindingSource.Count > 0) SalesTypeComboBox.SelectedIndex = 0;

            // Instrument
            InstrumentComboBox.DataSource = this.InstrumentBindingSource;
            InstrumentComboBox.DisplayMember = "InstrumentName";
            InstrumentComboBox.ValueMember = "InstrumentID";
            if (this.InstrumentBindingSource.Count > 0) InstrumentComboBox.SelectedIndex = 0;

            // Category
            CategoryComboBox.DataSource = this.CategoryBindingSource;
            CategoryComboBox.DisplayMember = "CategoryName";
            CategoryComboBox.ValueMember = "CategoryID";
            if (this.CategoryBindingSource.Count > 0) CategoryComboBox.SelectedIndex = 0;

            // Sub Categories
            SubCategoryComboBox.DataSource = this.SubCategoryBindingSource;
            SubCategoryComboBox.DisplayMember = "SubCategoryName";
            SubCategoryComboBox.ValueMember = "SubCategoryID";
            if (this.SubCategoryBindingSource.Count > 0) SubCategoryComboBox.SelectedIndex = 0;
        }

        private void setPartDetailTextBox()
        {
            System.Data.DataRowView SelectedRowView;
            TecanQuoteGeneratorPartsListDataSet.PartsListRow SelectedRow;
            SelectedRowView = (System.Data.DataRowView)partsListBindingSource.Current;
            SelectedRow = (TecanQuoteGeneratorPartsListDataSet.PartsListRow)SelectedRowView.Row;
            PartDetailTextBox.Text = SelectedRow.DetailDescription;
            itemPriceTextBox.Text = String.Format("{0:C2}", SelectedRow.ILP);
            NotesTextBox.Text = SelectedRow.NotesFromFile;
            loadImage(SelectedRow.SAPId);
        }

        // Ensures the selected item is displayed in the detail and image info
        private void partsListDataGridView_MouseMove(object sender, MouseEventArgs e)
        {
            Int32 selectedRowCount = partsListDataGridView.Rows.GetRowCount(DataGridViewElementStates.Selected);
            DataGridView.HitTestInfo info = partsListDataGridView.HitTest(e.X, e.Y);
            
            // set the currentcell property manually.   
            if (info.RowIndex >= 0 && selectedRowCount == 1)
            {
                try
                {
                    this.partsListDataGridView.CurrentCell = this.partsListDataGridView[info.ColumnIndex, info.RowIndex];
                }
                catch { }
                setPartDetailTextBox();
            }
        }

        // Event Trigers Drag Drop (originator)
        private void partsListDataGridView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                partsListDataGridView.DoDragDrop(partsListDataGridView.SelectedRows, DragDropEffects.Move);
            }
            else
            {
                System.Data.DataRowView SelectedRowView;
                TecanQuoteGeneratorPartsListDataSet.PartsListRow SelectedRow;

                SelectedRowView = (System.Data.DataRowView)partsListBindingSource.Current;
                SelectedRow = (TecanQuoteGeneratorPartsListDataSet.PartsListRow)SelectedRowView.Row;

                if (DetailsForm == null || DetailsForm.IsDisposed) DetailsForm = new PartsListDetailDisplay();
                try
                {
                    DetailsForm.SetForm1Instance(this);
                    DetailsForm.LoadParts(SelectedRow.SAPId);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                DetailsForm.Show();
                Application.OpenForms["PartsListDetailDisplay"].BringToFront();
            }
        }

        // Start of the Drag/Drop Operation
        private void QuoteDataGridView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DataGridViewSelectedRowCollection)))
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        // The drop into the desired object
        private void QuoteDataGridView_DragDrop(object sender, DragEventArgs e)
        {
            quoteSaved = false;
            String itemSAPID;
            String itemDescription;
            Decimal itemPrice;

            DataGridViewSelectedRowCollection rows = (DataGridViewSelectedRowCollection)e.Data.GetData(typeof(DataGridViewSelectedRowCollection));
            foreach (DataGridViewRow row in rows)
            {
                itemSAPID = row.Cells[0].Value.ToString();
                itemDescription = row.Cells[1].Value.ToString();
                itemPrice = getPartPrice(itemSAPID);
                QuoteDataGridView.Rows.Add(itemSAPID, itemDescription, itemPrice, 1, String.Format("{0:P2}", 0.00), itemPrice);

                // Check requiredParts
                String[] hasRequiredParts = checkForRequiredParts(itemSAPID);
                if (hasRequiredParts != null)
                {
                    doAddRequiredParts(hasRequiredParts, itemSAPID, itemDescription, "QuoteItems");
                }
            }
            SumItems(QuoteDataGridView);
        }

        public void SumItems(DataGridView myDataGridView)
        {
            Int32 rowCount = myDataGridView.Rows.Count;
            Int32 rowIndex;
            Decimal itemPrice = 0;
            Int32 itemQty = 0;
            Decimal itemDiscount = 0;
            Decimal totalItemPrice = 0;
            Decimal totalDiscount = 0;
            Decimal discountPercentage = 0;
            Decimal extendedPrice = 0;

            for (int s = 0; s < rowCount; s++)
            {
                rowIndex = myDataGridView.Rows[s].Index;
                DataGridViewRow srow = myDataGridView.Rows[rowIndex];
                if (srow.Cells[0].FormattedValue.ToString() != "Heading")
                {
                    itemPrice = (Decimal)srow.Cells[2].Value;
                    itemQty = Convert.ToInt32(srow.Cells[3].Value);
                    totalItemPrice = totalItemPrice + (itemPrice * itemQty);
                    discountPercentage = Convert.ToDecimal(srow.Cells[4].Value.ToString().Replace(" %", ""));
                    itemDiscount = (itemPrice * (discountPercentage / 100)) * itemQty;
                    totalDiscount = totalDiscount + itemDiscount;
                    extendedPrice = extendedPrice + (Decimal)srow.Cells[5].Value;
                }
            }

            switch (myDataGridView.Name)
            {
                case "QuoteDataGridView":
                    QuoteItemsPriceTextBox.Text = String.Format("{0:C2}", totalItemPrice); // getFormatedDollarValue(itemPrice.ToString());
                    QuoteItemsDiscountPercentageTextBox.Text = String.Format("{0:C2}", totalDiscount);
                    QuoteItemsPriceAfterDiscountTextBox.Text = String.Format("{0:C2}", extendedPrice);
                    break;

                case "OptionsDataGridView":
                    OptionsItemsPriceTextBox.Text = String.Format("{0:C2}", totalItemPrice); // getFormatedDollarValue(itemPrice.ToString());
                    OptionsItemsDiscountPercentageTextBox.Text = String.Format("{0:C2}", totalDiscount);
                    OptionsItemsPriceAfterDiscountTextBox.Text = String.Format("{0:C2}", extendedPrice);
                    break;
            }
            applyTotals();
        }

        private void ShippingTextBox_Lost_Focus(object sender, EventArgs e)
        {
            ShippingTextBox.Text = String.Format("{0:C2}", Convert.ToDecimal(ShippingTextBox.Text));
            applyTotals();
        }

        public void applyTotals()
        {
            Decimal itemsTotal = 0;
            Decimal optionsTotal = 0;
            Decimal shipping = 0;
            Decimal quoteTotal = 0;

            if (QuoteItemsPriceAfterDiscountTextBox.Text != "")
                itemsTotal = Convert.ToDecimal(QuoteItemsPriceAfterDiscountTextBox.Text.Replace("$", ""));
            if (OptionsItemsPriceAfterDiscountTextBox.Text != "")
                optionsTotal = Convert.ToDecimal(OptionsItemsPriceAfterDiscountTextBox.Text.Replace("$", ""));
            if (ShippingTextBox.Text != "")
                shipping = Convert.ToDecimal(ShippingTextBox.Text.Replace("$", ""));
            // quoteTotal = Convert.ToDecimal(QuoteItemsPriceAfterDiscountTextBox.Text.Replace("$", "")) + Convert.ToDecimal(OptionsItemsPriceAfterDiscountTextBox.Text.Replace("$", "")) + Convert.ToDecimal(ShippingTextBox.Text.Replace("$", ""));
            quoteTotal = itemsTotal + optionsTotal + shipping;
            TotalQuotePriceQuoteAndOptionsTextBox.Text = String.Format("{0:C2}", quoteTotal);
        }

        // Update Extended Price and Totals when QTY and/or Discount Change
        private void QuoteDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                String firstCellValue;
                firstCellValue = QuoteDataGridView.Rows[e.RowIndex].Cells[0].FormattedValue.ToString();
                if (firstCellValue != "Heading")
                {
                    processCellValueChange(QuoteDataGridView, e);
                }
                else
                {
                    QuoteDataGridView.Columns[1].ReadOnly = true;
                    QuoteDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    QuoteDataGridView.BeginEdit(false);
                }
            }
        }

        private void processCellValueChange(DataGridView myDataGridView, DataGridViewCellEventArgs e)
        {
            quoteSaved = false;
            Int32 rowIndex;
            Decimal itemPrice = 0;
            Int32 itemQty = 0;
            Decimal itemDiscount = 0;
            Decimal discountPercentage;
            Decimal extendedPrice = 0;

            rowIndex = e.RowIndex;
            DataGridViewRow srow = myDataGridView.Rows[rowIndex];
            itemPrice = (Decimal)srow.Cells[2].Value;
            itemQty = Convert.ToInt32(srow.Cells[3].Value);
            // totalItemPrice = totalItemPrice + (itemPrice * itemQty);
            discountPercentage = Convert.ToDecimal(srow.Cells[4].Value.ToString().Replace(" %", ""));
            itemDiscount = (itemPrice * (discountPercentage / 100)) * itemQty;
            // totalDiscount = totalDiscount + itemDiscount;
            extendedPrice = (itemPrice * itemQty) - itemDiscount;

            myDataGridView.Rows[e.RowIndex].Cells[4].Value = String.Format("{0:P2}", discountPercentage / 100);
            myDataGridView.Rows[e.RowIndex].Cells[5].Value = extendedPrice;
            SumItems(myDataGridView);
        }

        private void OptionsDataGridView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DataGridViewSelectedRowCollection)))
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        // The drop into the desired object
        private void OptionsDataGridView_DragDrop(object sender, DragEventArgs e)
        {
            quoteSaved = false;
            String itemSAPID;
            String itemDescription;
            Decimal itemPrice;

            DataGridViewSelectedRowCollection rows = (DataGridViewSelectedRowCollection)e.Data.GetData(typeof(DataGridViewSelectedRowCollection));
            foreach (DataGridViewRow row in rows)
            {
                itemSAPID = row.Cells[0].Value.ToString();
                itemDescription = row.Cells[1].Value.ToString();
                itemPrice = getPartPrice(itemSAPID);
                // todo need format?
                OptionsDataGridView.Rows.Add(itemSAPID, itemDescription, itemPrice, 1, String.Format("{0:P2}", 0.00), itemPrice);

                // Check requiredParts
                String[] hasRequiredParts = checkForRequiredParts(itemSAPID);
                if (hasRequiredParts != null)
                {
                    doAddRequiredParts(hasRequiredParts, itemSAPID, itemDescription, "QuoteOptions");
                }
            }
            SumItems(OptionsDataGridView);
        }

        // Update Extended Price and Totals when QTY and/or Discount Change
        private void OptionsDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                processCellValueChange(OptionsDataGridView, e);
            }

        }

        public String[] checkForRequiredParts(String itemSAPID)
        {
            var foundRequiredPartsList = new List<string>();
            openDB();
            SqlCeCommand cmd = TecanDatabase.CreateCommand();

            cmd.CommandText = "SELECT R.RequiredSAPId, P.Description, P.ILP FROM RequiredParts R" +
            " INNER JOIN PartsList P " +
            " ON R.RequiredSAPId = P.SAPId" +
            " WHERE R.SAPId = '" + itemSAPID + "'" +
            " ORDER BY RequiredSAPId";
            try
            {
                SqlCeDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    foundRequiredPartsList.Add(reader[0].ToString() + "^" + reader[1].ToString() + "^" + reader[2].ToString());
                }
                reader.Dispose();
                // return foundRequiredParts;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            TecanDatabase.Close();
            if (foundRequiredPartsList.Count != 0)
            {
                String[] foundRequiredParts = foundRequiredPartsList.ToArray();
                return foundRequiredParts;
            }
            else
            {
                return null;
            }
        }

        public void doAddRequiredParts(String[] hasRequiredParts, String itemSAPID, String itemDescription, String whichGrid)
        {
            // Check if they are already added
            DataGridViewRowCollection itemsRows = QuoteDataGridView.Rows;
            DataGridViewRowCollection optionRows = OptionsDataGridView.Rows;
            String existsSAPID;
            var PartsToAddList = new List<string>();
            String[] requiredPart;
            Boolean foundSAPID = false;
            // Loop throught required parts
            foreach (String part in hasRequiredParts)
            {
                requiredPart = part.Split('^');
                foundSAPID = false;
                // Already selected items in quote
                foreach (DataGridViewRow rowItem in itemsRows)
                {
                    existsSAPID = rowItem.Cells[0].Value.ToString();
                    if (existsSAPID == requiredPart[0])
                    {
                        foundSAPID = true;
                        break;
                    }
                }
                // Already selected items in options
                foreach (DataGridViewRow rowOption in optionRows)
                {
                    existsSAPID = rowOption.Cells[0].Value.ToString();
                    if (existsSAPID == requiredPart[0])
                    {
                        foundSAPID = true;
                        break;
                    }
                }
                if (!foundSAPID)
                {
                    PartsToAddList.Add(part);
                }
            }
            String[] toAddPart = PartsToAddList.ToArray();
            switch (toAddPart.Length)
            {
                // Required part already added, do nothing
                case 0:
                    break;

                // 1 required part, simple message
                case 1:
                    String[] partToAdd = null;
                    partToAdd = toAddPart[0].Split('^');
                    Decimal partItemPrice = Convert.ToDecimal(partToAdd[2]);
                    // Ask to add part
                    if (MessageBox.Show("The part\r\n\r\n" + itemSAPID + "  " + itemDescription + "\r\n\r\nhas a required part \r\n\r\n" + partToAdd[0] + "  " + partToAdd[1] + ".\r\n\r\nDo you want to add it?", "Required Part", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        if (whichGrid == "QuoteItems")
                        {
                            QuoteDataGridView.Rows.Add(partToAdd[0], partToAdd[1], partItemPrice, 1, String.Format("{0:P2}", 0.00), partItemPrice);
                        }
                        else
                        {
                            OptionsDataGridView.Rows.Add(partToAdd[0], partToAdd[1], partItemPrice, 1, String.Format("{0:P2}", 0.00), partItemPrice);
                        }
                    }
                    break;

                // Multiple requires parts, do required panel
                default:
                    // Ask to add parts
                    RequiredPartCheckedListBox.Items.Clear();
                    RequiredPartsPanelSelectAllCheckBox.Checked = false;
                    RequiredPartsPanelHeadingLabel.Text = "The part " + itemSAPID + "  " + itemDescription + " has multiple parts that are required.\r\nPlease select (Double-Click) the parts you would like to add.";
                    RequiredPartsPanel.Visible = true;

                    ArrayList quoteItems = new ArrayList();
                    foreach (String part in toAddPart)
                    {
                        requiredPart = part.Split('^');
                        RequiredPartCheckedListBox.Items.Add(requiredPart[0] + "  " + requiredPart[1]);

                        QuoteItems newItem = new QuoteItems();
                        newItem.SAPID = requiredPart[0];
                        newItem.Description = requiredPart[1];
                        newItem.Price = Convert.ToDecimal(requiredPart[2]);
                        quoteItems.Add(newItem);
                    }
                    if (whichGrid == "QuoteItems")
                    {
                        passRequiredItems.QuoteTitle = "QuoteItems";
                    }
                    else
                    {
                        passRequiredItems.QuoteTitle = "OptionItems";
                    }
                    passRequiredItems.Items = quoteItems;
                    break;
            }

        }

        //private void ThirdPartyDataGridView_DragEnter(object sender, DragEventArgs e)
        //{
        //    if (e.Data.GetDataPresent(typeof(DataGridViewSelectedRowCollection)))
        //    {
        //        e.Effect = DragDropEffects.Move;
        //    }
        //}

        // The drop into the desired object
        //private void ThirdPartyDataGridView_DragDrop(object sender, DragEventArgs e)
        //{
        //    String itemSAPID;
        //    String itemDescription;
        //    Decimal itemPrice;

        //    DataGridViewSelectedRowCollection rows = (DataGridViewSelectedRowCollection)e.Data.GetData(typeof(DataGridViewSelectedRowCollection));
        //    foreach (DataGridViewRow row in rows)
        //    {
        //        itemSAPID = row.Cells[0].Value.ToString();
        //        itemDescription = row.Cells[1].Value.ToString();
        //        itemPrice = getPartPrice(itemSAPID);
        //        ThirdPartyDataGridView.Rows.Add(itemSAPID, itemDescription, itemPrice, 1, String.Format("{0:P2}", 0.00), itemPrice);
        //    }
        //    SumItems(OptionsDataGridView);
        //}

        // Update Extended Price and Totals when QTY and/or Discount Change
        //private void ThirdPartyDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (e.RowIndex >= 0)
        //    {
        //        processCellValueChange(ThirdPartyDataGridView, e);
        //    }

        //}

        //private void SmartStartDataGridView_DragEnter(object sender, DragEventArgs e)
        //{
        //    if (e.Data.GetDataPresent(typeof(DataGridViewSelectedRowCollection)))
        //    {
        //        e.Effect = DragDropEffects.Move;
        //    }
        //}

        // The drop into the desired object
        //private void SmartStartDataGridView_DragDrop(object sender, DragEventArgs e)
        //{
        //    String itemSAPID;
        //    String itemDescription;
        //    Decimal itemPrice;

        //    DataGridViewSelectedRowCollection rows = (DataGridViewSelectedRowCollection)e.Data.GetData(typeof(DataGridViewSelectedRowCollection));
        //    foreach (DataGridViewRow row in rows)
        //    {
        //        itemSAPID = row.Cells[0].Value.ToString();
        //        itemDescription = row.Cells[1].Value.ToString();
        //        itemPrice = getPartPrice(itemSAPID);
        //        SmartStartDataGridView.Rows.Add(itemSAPID, itemDescription, itemPrice, 1, String.Format("{0:P2}", 0.00), itemPrice);
        //    }
        //}

        // Update Extended Price and Totals when QTY and/or Discount Change
        //private void SmartStartDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (e.RowIndex >= 0)
        //    {
        //        processCellValueChange(SmartStartDataGridView, e);
        //    }

        //}

        public Decimal getPartPrice(String SAPID)
        {
            Decimal itemPrice = 0;
            openDB();
            SqlCeCommand cmd = TecanDatabase.CreateCommand();
            cmd.CommandText = "SELECT ILP FROM PartsList WHERE SAPId = '" + SAPID + "'";
            try
            {
                itemPrice = (Decimal)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            return itemPrice;
        }

        private void loadImage(string SAPID)
        {
            Byte[] imageData;
            try
            {
                openDB();
                SqlCeCommand cmd = TecanDatabase.CreateCommand();
                cmd.CommandText = "SELECT Document FROM PartImages WHERE SAPId = '" + SAPID + "'";
                imageData = (byte[])cmd.ExecuteScalar();
                if (imageData != null)
                {
                    System.Drawing.Image newImage = byteArrayToImage(imageData);
                    newImage = ResizeImage(newImage, new Size(123, 119));
                    partImagePictureBox.Image = newImage;
                }
                else
                {
                    // If no image available
                    System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
                    Stream myStream = myAssembly.GetManifestResourceStream("Tecan_Quote_Generator.noimage.bmp");
                    Bitmap image = new Bitmap(myStream);
                    System.Drawing.Image newImage = image;
                    newImage = ResizeImage(newImage, new Size(123, 135));
                    partImagePictureBox.Image = newImage;
                }

            }
            finally
            {
                TecanDatabase.Close();
            }
        }

        public System.Drawing.Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            System.Drawing.Image returnImage = System.Drawing.Image.FromStream(ms);
            return returnImage;
        }

        private void openDB()
        {
            TecanDatabase = new SqlCeConnection();
            if (IsSSPCheckBox.Checked == false)
            {
                TecanDatabase.ConnectionString = "Data Source=|DataDirectory|\\TecanQuoteGeneratorPartsList.sdf;Max Database Size=4000;Max Buffer Size=1024;Persist Security Info=False";
            }
            else
            {
                TecanDatabase.ConnectionString = "Data Source=|DataDirectory|\\TecanSmartStartQuoteGeneratorPartsList.sdf;Max Database Size=4000;Max Buffer Size=1024;Persist Security Info=False";
            }
            TecanDatabase.Open();
        }

        private void openAppDocDatabase()
        {
            TecanAppDocDatabase = new SqlCeConnection();
            TecanAppDocDatabase.ConnectionString = "Data Source=|DataDirectory|\\TecanAppDocs.sdf;Max Database Size=4000;Max Buffer Size=1024;Persist Security Info=False";
            TecanAppDocDatabase.Open();
        }

        private void openContactsDatabase()
        {
            ContactDatabase = new SqlCeConnection();
            ContactDatabase.ConnectionString = "Data Source=|DataDirectory|\\Customers.sdf;Max Database Size=4000;Max Buffer Size=1024;Persist Security Info=False";
            ContactDatabase.Open();
        }

        public static System.Drawing.Image ResizeImage(System.Drawing.Image image, Size size, bool preserveAspectRatio = true)
        {
            int newWidth;
            int newHeight;
            if (preserveAspectRatio)
            {
                int originalWidth = image.Width;
                int originalHeight = image.Height;
                float percentWidth = (float)size.Width / (float)originalWidth;
                float percentHeight = (float)size.Height / (float)originalHeight;
                float percent = percentHeight < percentWidth ? percentHeight : percentWidth;
                newWidth = (int)(originalWidth * percent);
                newHeight = (int)(originalHeight * percent);
            }
            else
            {
                newWidth = size.Width;
                newHeight = size.Height;
            }
            System.Drawing.Image newImage = new Bitmap(newWidth, newHeight);
            using (Graphics graphicsHandle = Graphics.FromImage(newImage))
            {
                graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsHandle.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            return newImage;
        }

        private void doSearch()
        {
            if (!searchPreformed)
            {
                searchPreformed = true;
                String PartSearchValue;
                String DescriptionSearchValue;

                short InstrumentSearchValue = (short)Convert.ToInt16(InstrumentComboBox.SelectedValue);
                short CategorySearchValue = (short)Convert.ToInt16(CategoryComboBox.SelectedValue);
                short SubCategorySearchValue = (short)Convert.ToInt16(SubCategoryComboBox.SelectedValue);
                byte SalesTypeSearchValue = (byte)Convert.ToInt16(SalesTypeComboBox.SelectedValue);

                // Set Text Search values
                if (PartNumberSearchTextBox.Text != "")
                {
                    PartSearchValue = PartNumberSearchTextBox.Text + "%";
                }
                else
                {
                    PartSearchValue = "%%";
                }

                if (DescriptionSearchTextBox.Text != "")
                {
                    DescriptionSearchValue = "%" + DescriptionSearchTextBox.Text + "%";
                }
                else
                {
                    DescriptionSearchValue = "%%";
                }

                // Test Filter Values values
                // No Filters Set
                if ((InstrumentSearchValue == 0) && (CategorySearchValue == 0) && (SubCategorySearchValue == 0) && (SalesTypeSearchValue == 0))
                {
                    partsListTableAdapter.FillByLike(this.tecanQuoteGeneratorPartsListDataSet.PartsList, PartSearchValue, DescriptionSearchValue);
                }
                // Instrument Only
                else if ((InstrumentSearchValue != 0) && (CategorySearchValue == 0) && (SubCategorySearchValue == 0) && (SalesTypeSearchValue == 0))
                {
                    partsListTableAdapter.FillByLIKEANDInstrument(this.tecanQuoteGeneratorPartsListDataSet.PartsList, PartSearchValue, DescriptionSearchValue, InstrumentSearchValue);
                }
                // Category Only
                else if ((InstrumentSearchValue == 0) && (CategorySearchValue != 0) && (SubCategorySearchValue == 0) && (SalesTypeSearchValue == 0))
                {
                    partsListTableAdapter.FillByLIKEANDCategory(this.tecanQuoteGeneratorPartsListDataSet.PartsList, PartSearchValue, DescriptionSearchValue, CategorySearchValue);
                }
                // Sub Category Only
                else if ((InstrumentSearchValue == 0) && (CategorySearchValue == 0) && (SubCategorySearchValue != 0) && (SalesTypeSearchValue == 0))
                {
                    partsListTableAdapter.FillByLIKEANDSubCategory(this.tecanQuoteGeneratorPartsListDataSet.PartsList, PartSearchValue, DescriptionSearchValue, SubCategorySearchValue);
                }
                // Sales Type Only
                else if ((InstrumentSearchValue == 0) && (CategorySearchValue == 0) && (SubCategorySearchValue == 0) && (SalesTypeSearchValue != 0))
                {
                    partsListTableAdapter.FillByLIKEANDSalesType(this.tecanQuoteGeneratorPartsListDataSet.PartsList, PartSearchValue, DescriptionSearchValue, SalesTypeSearchValue);
                }
                // Instrunment Combinations
                // Instrument AND Category
                else if ((InstrumentSearchValue != 0) && (CategorySearchValue != 0) && (SubCategorySearchValue == 0) && (SalesTypeSearchValue == 0))
                {
                    partsListTableAdapter.FillByLIKEANDInstrumentANDCategory(this.tecanQuoteGeneratorPartsListDataSet.PartsList, PartSearchValue, DescriptionSearchValue, InstrumentSearchValue, CategorySearchValue);
                }
                // Instrument AND SubCategory
                else if ((InstrumentSearchValue != 0) && (CategorySearchValue == 0) && (SubCategorySearchValue != 0) && (SalesTypeSearchValue == 0))
                {
                    partsListTableAdapter.FillByLIKEANDInstrumentANDSubCategory(this.tecanQuoteGeneratorPartsListDataSet.PartsList, PartSearchValue, DescriptionSearchValue, InstrumentSearchValue, SubCategorySearchValue);
                }
                // Instrument AND Sales Type
                else if ((InstrumentSearchValue != 0) && (CategorySearchValue == 0) && (SubCategorySearchValue == 0) && (SalesTypeSearchValue != 0))
                {
                    partsListTableAdapter.FillByLIKEANDInstrumentANDSalesType(this.tecanQuoteGeneratorPartsListDataSet.PartsList, PartSearchValue, DescriptionSearchValue, InstrumentSearchValue, SalesTypeSearchValue);
                }
                // Instrument AND Category AND Sub Category
                else if ((InstrumentSearchValue != 0) && (CategorySearchValue != 0) && (SubCategorySearchValue != 0) && (SalesTypeSearchValue == 0))
                {
                    partsListTableAdapter.FillByLIKEANDInstrumentANDCategoryANDSubCategory(this.tecanQuoteGeneratorPartsListDataSet.PartsList, PartSearchValue, DescriptionSearchValue, InstrumentSearchValue, CategorySearchValue, SubCategorySearchValue);
                }
                // Instrument AND Category AND Sales Type
                else if ((InstrumentSearchValue != 0) && (CategorySearchValue != 0) && (SubCategorySearchValue == 0) && (SalesTypeSearchValue != 0))
                {
                    partsListTableAdapter.FillByLIKEANDInstrumentANDCategoryANDSalesType(this.tecanQuoteGeneratorPartsListDataSet.PartsList, PartSearchValue, DescriptionSearchValue, InstrumentSearchValue, CategorySearchValue, SalesTypeSearchValue);
                }
                // Instrument AND SubCategory AND Sales Type
                else if ((InstrumentSearchValue != 0) && (CategorySearchValue == 0) && (SubCategorySearchValue != 0) && (SalesTypeSearchValue != 0))
                {
                    partsListTableAdapter.FillByLIKEANDInstrumentANDSubCategoryANDSalesType(this.tecanQuoteGeneratorPartsListDataSet.PartsList, PartSearchValue, DescriptionSearchValue, InstrumentSearchValue, SubCategorySearchValue, SalesTypeSearchValue);
                }
                // Instrument AND Category AND SubCategory AND Sales Type
                else if ((InstrumentSearchValue != 0) && (CategorySearchValue != 0) && (SubCategorySearchValue != 0) && (SalesTypeSearchValue != 0))
                {
                    partsListTableAdapter.FillByLIKEANDInstrumentANDCategoryANDSubCategoryANDSalesType(this.tecanQuoteGeneratorPartsListDataSet.PartsList, PartSearchValue, DescriptionSearchValue, InstrumentSearchValue, CategorySearchValue, SubCategorySearchValue, SalesTypeSearchValue);
                }
                // Category Combinations
                // Category AND SubCategory
                else if ((InstrumentSearchValue == 0) && (CategorySearchValue != 0) && (SubCategorySearchValue != 0) && (SalesTypeSearchValue == 0))
                {
                    partsListTableAdapter.FillByLIKEANDCategoryANDSubCategory(this.tecanQuoteGeneratorPartsListDataSet.PartsList, PartSearchValue, DescriptionSearchValue, CategorySearchValue, SubCategorySearchValue);
                }
                //// Category AND Sales Type
                else if ((InstrumentSearchValue == 0) && (CategorySearchValue != 0) && (SubCategorySearchValue == 0) && (SalesTypeSearchValue != 0))
                {
                    partsListTableAdapter.FillByLIKEANDCategoryANDSalesType(this.tecanQuoteGeneratorPartsListDataSet.PartsList, PartSearchValue, DescriptionSearchValue, CategorySearchValue, SalesTypeSearchValue);
                }
                // Category AND SubCategory AND Sales Type
                else if ((InstrumentSearchValue == 0) && (CategorySearchValue != 0) && (SubCategorySearchValue != 0) && (SalesTypeSearchValue != 0))
                {
                    partsListTableAdapter.FillByLIKEANDCategoryANDSubCategoryANDSalesType(this.tecanQuoteGeneratorPartsListDataSet.PartsList, PartSearchValue, DescriptionSearchValue, CategorySearchValue, SubCategorySearchValue, SalesTypeSearchValue);
                }
                // SubCategory Combinations
                // SubCategory AND Sales Type
                else if ((InstrumentSearchValue == 0) && (CategorySearchValue == 0) && (SubCategorySearchValue != 0) && (SalesTypeSearchValue != 0))
                {
                    partsListTableAdapter.FillByLIKEANDSubCategoryANDSalesType(this.tecanQuoteGeneratorPartsListDataSet.PartsList, PartSearchValue, DescriptionSearchValue, SubCategorySearchValue, SalesTypeSearchValue);
                }
                if (salesTypeChanged)
                {
                    updateInstrumentComboBox();
                    updateCategoryComboBox();
                    updateSubCategoryComboBox();
                }
                if(instrumentChanged)
                {
                    // updateSalesTypeComboBox();
                    updateCategoryComboBox();
                    updateSubCategoryComboBox();
                }
                if(categoryChanged)
                {
                    // updateSalesTypeComboBox();
                    // updateInstrumentComboBox();
                    updateSubCategoryComboBox();
                }
                //if(subCategoryChanged)
                //{
                //    updateSalesTypeComboBox();
                //    updateInstrumentComboBox();
                //    updateCategoryComboBox();
                //}

            }
        }

        // Called after doSearch() if Instrument, Category, or SubCategory has changed
        // Only include Sales Types that are avaiable for selected Dataset
        private void updateSalesTypeComboBox()
        {
            // SalesTypes
            instrumentChanged = false;
            categoryChanged = false;
            // subCategoryChanged = false;
            String CurrentSalesTypeSearchValue = SalesTypeComboBox.SelectedValue.ToString();
            short InstrumentSearchValue = (short)Convert.ToInt16(InstrumentComboBox.SelectedValue);
            short CategorySearchValue = (short)Convert.ToInt16(CategoryComboBox.SelectedValue);
            short SubCategorySearchValue = (short)Convert.ToInt16(SubCategoryComboBox.SelectedValue);

            if (InstrumentSearchValue != 0 || CategorySearchValue != 0 || SubCategorySearchValue != 0)
            {
                // Set up new Available Category list
                ArrayList theAvailableSalesTypes = new ArrayList();
                theAvailableSalesTypes.Add(new LookupTableDefinitions.AvailableSalesTypes("Any", "0"));

                foreach (DataRow dr in this.tecanQuoteGeneratorPartsListDataSet.Tables["SalesType"].Rows)
                {
                    foreach (DataRow pr in this.tecanQuoteGeneratorPartsListDataSet.Tables["PartsList"].Rows)
                    {
                        if (pr["SalesType"].ToString() == dr["SalesTypeID"].ToString())
                        {
                            theAvailableSalesTypes.Add(new LookupTableDefinitions.AvailableSalesTypes(dr["SalesTypeName"].ToString(), dr["SalesTypeID"].ToString()));
                            break;
                        }
                    }
                }
                // SalesTypeComboBox
                SalesTypeComboBox.DataSource = theAvailableSalesTypes;
                SalesTypeComboBox.DisplayMember = "Name";
                SalesTypeComboBox.ValueMember = "ID";
            }
            else
            {
                // InstrumentListComboBox
                SalesTypeComboBox.DataSource = this.SalesTypeBindingSource;
                SalesTypeComboBox.DisplayMember = "SalesTypeName";
                SalesTypeComboBox.ValueMember = "SalesTypeID";
            }
        }

        // Called after doSearch() if Category or SubCategory has changed
        // Only include Instruments that are avaiable for selected Dataset
        private void updateInstrumentComboBox()
        {
            // Instruments
            salesTypeChanged = false;
            categoryChanged = false;
            // subCategoryChanged = false;
            String CurrentInstrumentSearchValue = InstrumentComboBox.SelectedValue.ToString();
            short SalesTypeSearchValue = (short)Convert.ToInt16(SalesTypeComboBox.SelectedValue);
            short CategorySearchValue = (short)Convert.ToInt16(CategoryComboBox.SelectedValue);
            short SubCategorySearchValue = (short)Convert.ToInt16(SubCategoryComboBox.SelectedValue);

            if (SalesTypeSearchValue != 0 || CategorySearchValue != 0 || SubCategorySearchValue != 0)
            {
                // Set up new Available Category list
                ArrayList theAvailableInstruments = new ArrayList();
                theAvailableInstruments.Add(new LookupTableDefinitions.AvailableInstruments("Any", "0"));

                foreach (DataRow dr in this.tecanQuoteGeneratorPartsListDataSet.Tables["Instrument"].Rows)
                {
                    foreach (DataRow pr in this.tecanQuoteGeneratorPartsListDataSet.Tables["PartsList"].Rows)
                    {
                        if (pr["Instrument"].ToString() == dr["InstrumentID"].ToString())
                        {
                            theAvailableInstruments.Add(new LookupTableDefinitions.AvailableInstruments(dr["InstrumentName"].ToString(), dr["InstrumentID"].ToString()));
                            break;
                        }
                    }
                }
                // InstrumentListComboBox
                InstrumentComboBox.DataSource = theAvailableInstruments;
                InstrumentComboBox.DisplayMember = "Name";
                InstrumentComboBox.ValueMember = "ID";
            }
            else
            {
                // InstrumentListComboBox
                InstrumentComboBox.DataSource = this.InstrumentBindingSource;
                InstrumentComboBox.DisplayMember = "InstrumentName";
                InstrumentComboBox.ValueMember = "InstrumentID";
            }

            // Set the perviously selected item
            if (CurrentInstrumentSearchValue != "0")
            {
                searchPreformed = true;
                int currentItem = 0;

                try
                {
                    foreach (LookupTableDefinitions.AvailableInstruments instruments in InstrumentComboBox.Items)
                    {
                        if (instruments.ID == CurrentInstrumentSearchValue)
                        {
                            InstrumentComboBox.SelectedIndex = currentItem;
                        }
                        currentItem++;
                    }
                }
                catch (Exception)
                { }

            }

        }

        // Called after doSearch() if Instrument has changed
        // Only include Categories that are avaiable for selected Instrument
        private void updateCategoryComboBox()
        {
            // Categories
            String CurrentCategorySearchValue = CategoryComboBox.SelectedValue.ToString();
            salesTypeChanged = false;
            instrumentChanged = false;
            short SalesTypeSearchValue = (short)Convert.ToInt16(SalesTypeComboBox.SelectedValue);
            short InstrumentSearchValue = (short)Convert.ToInt16(InstrumentComboBox.SelectedValue);
            short SubCategorySearchValue = (short)Convert.ToInt16(SubCategoryComboBox.SelectedValue);

            if (SalesTypeSearchValue != 0 || InstrumentSearchValue != 0 || SubCategorySearchValue != 0)
            {
                // Set up new Available Category list
                ArrayList theAvailableCategories = new ArrayList();
                theAvailableCategories.Add(new LookupTableDefinitions.AvailableCategories("Any", "0"));

                foreach (DataRow dr in this.tecanQuoteGeneratorPartsListDataSet.Tables["Category"].Rows)
                {
                    foreach (DataRow pr in this.tecanQuoteGeneratorPartsListDataSet.Tables["PartsList"].Rows)
                    {
                        if (pr["Category"].ToString() == dr["CategoryID"].ToString())
                        {
                            theAvailableCategories.Add(new LookupTableDefinitions.AvailableCategories(dr["CategoryName"].ToString(), dr["CategoryID"].ToString()));
                            break;
                        }
                    }
                }
                // CategoryListComboBox
                CategoryComboBox.DataSource = theAvailableCategories;
                CategoryComboBox.DisplayMember = "Name";
                CategoryComboBox.ValueMember = "ID";
            }
            else
            {
                // CategoryListComboBox
                CategoryComboBox.DataSource = this.CategoryBindingSource;
                CategoryComboBox.DisplayMember = "CategoryName";
                CategoryComboBox.ValueMember = "CategoryID";
            }

            // Set the perviously selected item
            if (CurrentCategorySearchValue != "0")
            {
                searchPreformed = true;
                int currentItem = 0;

                try
                {
                    foreach (LookupTableDefinitions.AvailableCategories category in CategoryComboBox.Items)
                    {
                        if (category.ID == CurrentCategorySearchValue)
                        {
                            CategoryComboBox.SelectedIndex = currentItem;
                        }
                        currentItem++;
                    }
                }
                catch (Exception)
                { }
            }

        }

        // Called after doSearch() if Category has changed
        // Only include Sub-Categories that are avaiable for selected Category
        private void updateSubCategoryComboBox()
        {
            // SubCategories
            String CurrentSubCategorySearchValue = SubCategoryComboBox.SelectedValue.ToString();
            categoryChanged = false;
            salesTypeChanged = false;
            instrumentChanged = false;
            short SalesTypeSearchValue = (short)Convert.ToInt16(SalesTypeComboBox.SelectedValue);
            short InstrumentSearchValue = (short)Convert.ToInt16(InstrumentComboBox.SelectedValue);
            short CategorySearchValue = (short)Convert.ToInt16(CategoryComboBox.SelectedValue);

            if (SalesTypeSearchValue != 0 || InstrumentSearchValue != 0 || CategorySearchValue != 0)
            {
                // Set up new Available SubCategory list
                ArrayList theAvailableSubCategories = new ArrayList();
                theAvailableSubCategories.Add(new LookupTableDefinitions.AvailableSubCategories("Any", "0"));

                foreach (DataRow dr in this.tecanQuoteGeneratorPartsListDataSet.Tables["SubCategory"].Rows)
                {
                    foreach (DataRow pr in this.tecanQuoteGeneratorPartsListDataSet.Tables["PartsList"].Rows)
                    {
                        if (pr["SubCategory"].ToString() == dr["SubCategoryID"].ToString())
                        {
                            theAvailableSubCategories.Add(new LookupTableDefinitions.AvailableSubCategories(dr["SubCategoryName"].ToString(), dr["SubCategoryID"].ToString()));
                            break;
                        }
                    }
                }
                // SubCategoryListComboBox
                SubCategoryComboBox.DataSource = theAvailableSubCategories;
                SubCategoryComboBox.DisplayMember = "Name";
                SubCategoryComboBox.ValueMember = "ID";
            }
            else
            {
                // SubCategoryListComboBox
                SubCategoryComboBox.DataSource = this.SubCategoryBindingSource;
                SubCategoryComboBox.DisplayMember = "SubCategoryName";
                SubCategoryComboBox.ValueMember = "SubCategoryID";
            }

            // Set the perviously selected item
            if (CurrentSubCategorySearchValue != "0")
            {
                searchPreformed = true;
                int currentItem = 0;

                try
                {
                    foreach (LookupTableDefinitions.AvailableSubCategories category in SubCategoryComboBox.Items)
                    {
                        if (category.ID == CurrentSubCategorySearchValue)
                        {
                            SubCategoryComboBox.SelectedIndex = currentItem;
                        }
                        currentItem++;
                    }
                }
                catch (Exception)
                { }
            }
        }

        private void PartNumberSearchTextBox_TextChanged(object sender, EventArgs e)
        {
            searchPreformed = false;
            doSearch();
        }

        private void PartNumberClearButton_Click(object sender, EventArgs e)
        {
            PartNumberSearchTextBox.Text = "";
            searchPreformed = false;
            doSearch();
        }

        private void DescriptionClearButton_Click(object sender, EventArgs e)
        {
            DescriptionSearchTextBox.Text = "";
            searchPreformed = false;
            doSearch();
        }

        private void DescriptionSearchTextBox_TextChanged(object sender, EventArgs e)
        {
            searchPreformed = false;
            doSearch();
        }

        private void SalesTypeComboBox_Click(object sender, EventArgs e)
        {
            searchPreformed = false;
            salesTypeChanged = true;
        }

        private void SalesTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            doSearch();
        }

        private void InstrumentComboBox_Click(object sender, EventArgs e)
        {
            searchPreformed = false;
            instrumentChanged = true;
        }

        private void InstrumentComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CategoryComboBox.SelectedIndex != -1)
            {
                CategoryComboBox.SelectedIndex = 0;
                SubCategoryComboBox.SelectedIndex = 0;
                doSearch();
            }
        }

        private void CategoryComboBox_Click(object sender, EventArgs e)
        {
            searchPreformed = false;
            categoryChanged = true;
        }

        private void CategoryComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SubCategoryComboBox.SelectedIndex != 0 && SubCategoryComboBox.SelectedIndex != -1) SubCategoryComboBox.SelectedIndex = 0; 
            doSearch();
        }

        private void SubCategoryComboBox_Click(object sender, EventArgs e)
        {
            searchPreformed = false;
            // subCategoryChanged = true;
        }

        private void SubCategoryComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            doSearch();
        }

        private void ClearFiltersButton_Click(object sender, EventArgs e)
        {
            loadFilterComboBoxes();
            searchPreformed = false;
            doSearch();
        }

        private void accountListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ContactsForm contacts = new ContactsForm();
            contacts.Show();
            contacts.FormClosing += new FormClosingEventHandler(upDateContacts);
        }

        private void upDateContacts(object sender, EventArgs e)
        {
            this.accountsTableAdapter.FillBy(this.customersDataSet.Accounts);
            short currentAccountID = Convert.ToInt16(AccountComboBox.SelectedValue);
            this.contactsTableAdapter.FillByAccountID(this.customersDataSet.Contacts, currentAccountID);
        }

        private void reloadMe(object sender, EventArgs e)
        {
            doFormInitialization();
        }

        private void QuoteRemoveSelectedButton_Click(object sender, EventArgs e)
        {
            RemoveItems(QuoteDataGridView);
            // SumItems(QuoteDataGridView);
        }

        private void MoveToOptionsButton_Click(object sender, EventArgs e)
        {
            MoveItems(QuoteDataGridView, OptionsDataGridView);
        }

        private void QuoteAddHeadingButton_Click(object sender, EventArgs e)
        {
            userInsertHeading(QuoteDataGridView);
        }

        private void OptionsRemoveSelectedButton_Click(object sender, EventArgs e)
        {
            RemoveItems(OptionsDataGridView);
            // SumItems(OptionsDataGridView);
        }

        private void MoveToItemsButton_Click(object sender, EventArgs e)
        {
            MoveItems(OptionsDataGridView, QuoteDataGridView);
        }

        private void OptionsAddHeadingButton_Click(object sender, EventArgs e)
        {
            userInsertHeading(OptionsDataGridView);
        }

        private void userInsertHeading(DataGridView myDataGridView)
        {
            Int32 selectedRow = myDataGridView.SelectedRows[0].Index;

            myDataGridView.Columns[1].ReadOnly = false;
            myDataGridView.Rows.Insert(selectedRow+1, "Heading");
            DataGridViewCell cell = myDataGridView.Rows[selectedRow+1].Cells[1];
            myDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            myDataGridView.CurrentCell = cell;
            myDataGridView.BeginEdit(true);

        }

        //private void ThirdPartyRemoveSelectedButton_Click(object sender, EventArgs e)
        //{
        //    RemoveItems(ThirdPartyDataGridView);
        //    // SumItems(ThirdPartyDataGridView);
        //}

        //private void SmartStartRemoveSelectButton_Click(object sender, EventArgs e)
        //{
        //    RemoveItems(SmartStartDataGridView);
        //    // SumItems(SmartStartDataGridView);
        //}

        private void RemoveItems(DataGridView myDataGridView)
        {
            quoteSaved = false;
            Int32 selectedRowCount = myDataGridView.Rows.GetRowCount(DataGridViewElementStates.Selected);
            Int32 removedRowCount = 0;
            Int32 totalRowCount = myDataGridView.RowCount;
            if (selectedRowCount > 0)
            {
                if (selectedRowCount == totalRowCount)
                {
                    // MessageBox.Show("All cells are selected", "Selected Cells");
                    myDataGridView.Rows.Clear();
                }
                else
                {
                    while (selectedRowCount > removedRowCount)
                    {
                        myDataGridView.Rows.RemoveAt(myDataGridView.SelectedCells[0].RowIndex);
                        removedRowCount++;
                    }
                }
                SumItems(myDataGridView);
            }
        }

        private void MoveItems(DataGridView fromDataGridView, DataGridView toDataGridView)
        {
            quoteSaved = false;
            String itemSAPID;
            String itemDescription;
            Decimal itemPrice;
            Int32 itemQty;
            String itemDiscount;
            
            Int32 selectedRowCount = fromDataGridView.Rows.GetRowCount(DataGridViewElementStates.Selected);
            if (selectedRowCount > 0)
            {
                // Copy the items in the Rows collection into an array.
                DataGridViewRow[] rowArray = new DataGridViewRow[selectedRowCount]; 
                fromDataGridView.SelectedRows.CopyTo(rowArray, 0);
                foreach (DataGridViewRow row in rowArray)
                {
                    itemSAPID = row.Cells[0].Value.ToString();
                    itemDescription = row.Cells[1].Value.ToString();
                    itemPrice = (decimal)row.Cells[2].Value;
                    itemQty = Convert.ToInt32(row.Cells[3].Value);
                    itemDiscount = row.Cells[4].Value.ToString();
                    toDataGridView.Rows.Add(itemSAPID, itemDescription, itemPrice, itemQty, String.Format("{0:P2}", itemDiscount), itemPrice);
                }
            }
            RemoveItems(fromDataGridView);
            SumItems(toDataGridView);
        }

        private void saveQuoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (AccountComboBox.Items.Count == 0)
            {
                if (MessageBox.Show("You currently have no accounts and contacts added.\n\n Do you want to import or add accounts now?", "Please Add Account Information", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    accountListToolStripMenuItem_Click(sender, e);
                    return;
                }
                else
                {
                    return;
                }
            }

            if (QuoteTitleTextBox.Text == "" || AccountComboBox.SelectedIndex == 0)
            {
                String messageString = "";
                if (QuoteTitleTextBox.Text == "") messageString = "Please enter a Quote Title before saving.\n\n";
                if (AccountComboBox.SelectedIndex == 0) messageString = "Please select an Account and Contact before saving.";
                QuoteTabControl.SelectedTab = QuoteSettingTabPage;
                //PleaseWaitHeadingLabel.Text = "Incomplete Quote.";
                //PleaseWaitMessageTextBox.Text = "Please enter a Quote Title before saving.";
                //PLeaseWaitPanelOKButton.Visible = true;
                //PleaseWaitPanel.Visible = true;


                QuoteTitleTextBox.Focus();
                MessageBox.Show(messageString);
                return;
            }
            
            Quote quote = new Quote();
            quote.QuoteAccount = (short)Convert.ToInt16(AccountComboBox.SelectedValue);
            quote.QuoteContact = (short)Convert.ToInt16(ContactComboBox.SelectedValue);
            quote.QuoteTitle = QuoteTitleTextBox.Text;
            quote.QuoteDate = QuoteDateTimePicker.Text;
            quote.QuoteDescription = QuoteDescriptionTextBox.Text;
            quote.QuoteType = (short)Convert.ToInt16(QuoteTypeComboBox.SelectedValue);
            if (IsSSPCheckBox.Checked)
            {
                quote.QuoteisSSP = true;
            }
            else
            {
                quote.QuoteisSSP = false;            
            }
            quote.QuoteTemplate = (short)Convert.ToInt16(QuoteTemplateComboBox.SelectedValue);
            quote.Items = AddQuoteItems(QuoteDataGridView);
            quote.Options = AddQuoteItems(OptionsDataGridView);
            // quote.ThirdParty = AddQuoteItems(ThirdPartyDataGridView);
            // quote.SmartStart = AddQuoteItems(SmartStartDataGridView);
            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(Quote));

            // Save theQuote file
            String tecanFilesFilePath = @"c:\TecanFiles";
            String quoteDate = QuoteDateTimePicker.Text.Replace("/","_");
            System.IO.Directory.CreateDirectory(tecanFilesFilePath);
            String QuoteFileName = AccountComboBox.Text + "_" + QuoteTitleTextBox.Text + "_" + quoteDate + ".tbq";

            if (File.Exists(tecanFilesFilePath + "\\" + QuoteFileName))
            {
                if (MessageBox.Show("The Quote file c:\\TecanFiles\\" + QuoteFileName + " already exists!\r\n\r\nDo you want to overwrite quote?", "Overwrite Quote", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    MessageBox.Show("Please select a new title for this quote.");
                    QuoteTabControl.SelectedTab = QuoteSettingTabPage;
                    QuoteTitleTextBox.Focus();
                    return;
                }
            }
            quoteSaved = true;
            System.IO.StreamWriter file = new System.IO.StreamWriter(@"c:\TecanFiles\" + QuoteFileName);
            writer.Serialize(file, quote);
            file.Close();

            // Save the pdf version
            createQuotePDFHeader();
            int pageCount = quotePDFaddItemsToQuote();
            quotePDFaddApplicationDocument(pageCount);
            String tempFilePath = AddHeaderFooter();
            String fullTempPDF = tempFilePath + "\\" + QuoteTitleTextBox.Text + ".pdf";

            String QuotePDFFileName = AccountComboBox.Text + "_" + QuoteTitleTextBox.Text + "_" + quoteDate + ".pdf";
            String fullPDFfilename = tecanFilesFilePath + "\\" + QuotePDFFileName;
            System.IO.File.Copy(fullTempPDF, fullPDFfilename, true);

            MessageBox.Show("Quote c:\\TecanFiles\\" + QuoteFileName + " and c:\\TecanFiles\\" + fullPDFfilename + " saved.");
        }

        private ArrayList AddQuoteItems(DataGridView myDataGridView)
        {
            ArrayList quoteItems = new ArrayList();

            String SAPID;
            String Description;
            Decimal itemPrice;
            Int32 itemQty;
            Boolean Note;
            Boolean Image;
            Decimal discountPercentage;
            String itemPriceCheck;
            String discountCheck;
            Int16 rowCount = 0;

            // todo Remove myDataGridView.Rows[rowCount] with row and do for each
            foreach (DataGridViewRow row in myDataGridView.Rows)
            {
                SAPID = myDataGridView.Rows[rowCount].Cells[0].Value.ToString();
                Description = myDataGridView.Rows[rowCount].Cells[1].Value.ToString();

                if (myDataGridView.Rows[rowCount].Cells[0].FormattedValue.ToString() != "Heading")
                {
                    itemPriceCheck = myDataGridView.Rows[rowCount].Cells[2].Value.ToString();
                    discountCheck = myDataGridView.Rows[rowCount].Cells[4].Value.ToString();

                    if (itemPriceCheck.IndexOf("$") != -1) itemPriceCheck = itemPriceCheck.Substring(1, itemPriceCheck.Length - 1);
                    itemPrice = Convert.ToDecimal(itemPriceCheck);

                    if (discountCheck.IndexOf("%") != -1) discountCheck = discountCheck.Substring(0, discountCheck.Length - 2);
                    discountPercentage = Convert.ToDecimal(discountCheck);

                    itemQty = Convert.ToInt32(myDataGridView.Rows[rowCount].Cells[3].Value);

                    Note = Convert.ToBoolean(myDataGridView.Rows[rowCount].Cells[6].Value);
                    Image = Convert.ToBoolean(myDataGridView.Rows[rowCount].Cells[7].Value);
                }
                else
                {
                    itemQty = 0;
                    itemPrice = 0;
                    discountPercentage = 0;
                    Note = false;
                    Image = false;

                }
                QuoteItems newItem = new QuoteItems();
                newItem.SAPID = SAPID;
                newItem.Description = Description;
                newItem.Quantity = itemQty;
                newItem.Price = itemPrice;
                newItem.Discount = discountPercentage;
                newItem.IncludeNote = Note;
                newItem.IncludeImage = Image;
                quoteItems.Add(newItem);
                rowCount++;
            }

            return quoteItems;
        }

        private void loadQuoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Boolean doClear = clearQuote();
            if (!doClear) return;

            // Get Quote Filename and Path
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\TecanFiles";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.Filter = "tbq files (*.tbq)|*.tbq";
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(Quote));
                System.IO.StreamReader file = new System.IO.StreamReader(openFileDialog1.FileName);
                Quote quote = new Quote();
                quote = (Quote)reader.Deserialize(file);
                file.Close();

                AccountComboBox.SelectedValue = quote.QuoteAccount;
                ContactComboBox.SelectedValue = quote.QuoteContact;
                QuoteTitleTextBox.Text = quote.QuoteTitle;
                QuoteDateTimePicker.Text = quote.QuoteDate;
                QuoteDescriptionTextBox.Text = quote.QuoteDescription;
                QuoteTypeComboBox.SelectedValue = quote.QuoteType;
                if (quote.QuoteisSSP == true)
                {
                    IsSSPCheckBox.Checked = true;
                }
                else
                {
                    IsSSPCheckBox.Checked = false;
                }
                QuoteTemplateComboBox.SelectedValue = quote.QuoteTemplate;

                String itemSAPID;
                String itemDescription;
                Decimal itemPrice;
                Int32 itemQuantity;
                Decimal itemDiscount;
                Boolean itemNote;
                Boolean itemImage;
                QuoteItems replacementItem = new QuoteItems();
                String replacementString;
                Boolean itemFound;
                foreach (QuoteItems row in quote.Items)
                {
                    itemSAPID = row.SAPID;
                    itemDescription = row.Description;
                    itemPrice = row.Price;
                    itemQuantity = row.Quantity;
                    itemDiscount = row.Discount;
                    itemNote = row.IncludeNote;
                    itemImage = row.IncludeImage;
                    itemFound = false;
                    if (itemSAPID != "Heading")
                    {
                        replacementItem = validateItem(itemSAPID);
                        if (replacementItem.SAPID == "found")
                        {
                            itemFound = true;
                        }
                    }
                    else
                    {
                        itemFound = true;
                    }

                    if (itemFound)
                    {
                        QuoteDataGridView.Rows.Add(itemSAPID, itemDescription, itemPrice, itemQuantity, String.Format("{0:P2}", itemDiscount), itemPrice, itemNote, itemImage);
                    }
                    else
                    {
                        replacementString = "This item " +  itemSAPID+ " " + itemDescription + " is no longer available!\r\n\r\n" +
                            "Do you want to add it's replacement instead?\r\n\r\n Please verify the image and note selection for the new item.";
                            
                        if (MessageBox.Show(replacementString, "Part no longer available", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            QuoteDataGridView.Rows.Add(replacementItem.SAPID, replacementItem.Description, replacementItem.Price, itemQuantity, String.Format("{0:P2}", itemDiscount), replacementItem.Price, itemNote, itemImage);
                        }
                    }
                }

                foreach (QuoteItems row in quote.Options)
                {
                    itemSAPID = row.SAPID;
                    itemDescription = row.Description;
                    itemPrice = row.Price;
                    itemQuantity = row.Quantity;
                    itemDiscount = row.Discount;
                    itemNote = row.IncludeNote;
                    itemImage = row.IncludeImage;

                    replacementItem = validateItem(itemSAPID);
                    // "found" indicates that the item was found in the Db and no replacement is required
                    if (replacementItem.SAPID == "found")
                    {
                        OptionsDataGridView.Rows.Add(itemSAPID, itemDescription, itemPrice, itemQuantity, String.Format("{0:P2}", itemDiscount), itemPrice, itemNote, itemImage);
                    }
                    else
                    {
                        replacementString = "This item " + itemSAPID + " " + itemDescription + " is no longer available!\r\n\r\n" +
                            "Do you want to add it's replacement instead?\r\n\r\n Please verify the image and note selection for the new item.";

                        if (MessageBox.Show(replacementString, "Part no longer available", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            OptionsDataGridView.Rows.Add(replacementItem.SAPID, replacementItem.Description, replacementItem.Price, itemQuantity, String.Format("{0:P2}", itemDiscount), replacementItem.Price, itemNote, itemImage);
                        }
                    }
                }

                QuoteTabControl.SelectedTab = QuoteTabPage;
                SumItems(QuoteDataGridView);
                SumItems(OptionsDataGridView);
            }
        }

        private QuoteItems validateItem(String itemSAPID)
        {
            QuoteItems foundItem = new QuoteItems();
            Boolean itemFound = false;

            openDB();
            SqlCeCommand cmd = TecanDatabase.CreateCommand();
            cmd.CommandText = "SELECT SAPId FROM PartsList WHERE SAPId = '" + itemSAPID + "'";
            SqlCeDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                itemFound = true;
            }
            if (itemFound)
            {
                foundItem.SAPID = "found";
            }
            else
            {
                cmd.CommandText = "SELECT SAPId, Description, ILP FROM PartsList WHERE OldPartNum = '" + itemSAPID + "'";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader[0].ToString() != "")
                    {
                        foundItem.SAPID = reader[0].ToString();
                        foundItem.Description = reader[1].ToString();
                        foundItem.Price = (Decimal)reader[2];
                    }
                }
            }
            reader.Dispose();
            TecanDatabase.Close();
            return foundItem;
        }

        private Boolean clearQuote()
        {
            if (!quoteSaved)
            {
                if (MessageBox.Show("This quote has not been saved or you have made changes!\r\n\r\nDo you want to clear these items?", "Clear List", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return false;
                }
            }

            // Clear Grids
            QuoteDataGridView.Rows.Clear();
            OptionsDataGridView.Rows.Clear();
            //ThirdPartyDataGridView.Rows.Clear();
            //SmartStartDataGridView.Rows.Clear();

            // Clear all pricing textboxes
            QuoteItemsPriceTextBox.Text = "";
            QuoteItemsDiscountPercentageTextBox.Text = "";
            ShippingTextBox.Text = "";
            OptionsItemsPriceTextBox.Text = "";
            OptionsItemsDiscountPercentageTextBox.Text = "";
            OptionsItemsPriceAfterDiscountTextBox.Text = "";
            TotalQuotePriceQuoteAndOptionsTextBox.Text = "";

            // Reset all quote info page items
            QuoteTitleTextBox.Text = "";
            QuoteDescriptionTextBox.Text = "";
            if (AccountComboBox.Items.Count != 0) AccountComboBox.SelectedIndex = 0;
            if (ContactComboBox.Items.Count != 0) ContactComboBox.SelectedIndex = 0;
            if (QuoteTypeComboBox.Items.Count != 0) QuoteTypeComboBox.SelectedIndex = 0;
            IsSSPCheckBox.Checked = false;
            // if (QuoteTemplateComboBox.Items.Count != 0) QuoteTemplateComboBox.SelectedIndex = 0;
            return true;
        }

        private void myProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showUserProfileForm(false);
        }

        public void getUsersProfile()
        {
            String profileFile = @"c:\TecanFiles\" + "TecanQuoteConfig.cfg";
            if (File.Exists(profileFile))
            {
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(Profile));
                System.IO.StreamReader file = new System.IO.StreamReader(profileFile);
                // Profile profile = new Profile();
                profile = (Profile)reader.Deserialize(file);
                file.Close();
                SalemansNameLabel.Text = "Welcome " + profile.Name;
            }
            else
            {
                MessageBox.Show("There was an error getting your profile information!\n\nPlease re-enter your profile");
                showUserProfileForm(false);
            }
        }

        private void saveUsersProfile()
        {
            // Save to Profile config file
            String profileFile = @"c:\TecanFiles\" + "TecanQuoteConfig.cfg";
            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(Profile));
            System.IO.StreamWriter file = new System.IO.StreamWriter(profileFile);
            writer.Serialize(file, profile);
            file.Close();
        }

        private void viewQuoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!quoteSaved)
            {
                saveQuoteToolStripMenuItem_Click(sender, e);
                return;
            }

            //createQuotePDFHeader();
            //int pageCount = quotePDFaddItemsToQuote();
            //quotePDFaddApplicationDocument(pageCount);
            //String tempFilePath = AddHeaderFooter();
            String tecanFilesFilePath = @"c:\TecanFiles";
            String quoteDate = QuoteDateTimePicker.Text.Replace("/", "_");
            String QuoteFileName = AccountComboBox.Text + "_" + QuoteTitleTextBox.Text + "_" + quoteDate + ".pdf";

            Process.Start(tecanFilesFilePath + "\\" + QuoteFileName);
            //String fullTempPDF = tempFilePath + "\\" + QuoteTitleTextBox.Text + ".pdf";

            // Save theQuote file
            //String tecanFilesFilePath = @"c:\TecanFiles";
            //System.IO.Directory.CreateDirectory(tecanFilesFilePath);
            //String QuoteFileName = AccountComboBox.Text + "_" + QuoteTitleTextBox.Text + "_" + QuoteDateTimePicker.Text + ".tbq";
            //String fullPDFfilename = tecanFilesFilePath + "\\" + QuoteFileName;
            //System.IO.File.Copy(fullTempPDF, fullPDFfilename, true);
        }

        private void createQuotePDFHeader()
        {
            // Get Account & Contact Address Information
            String AccountName = AccountComboBox.Text;
            String ContactName = ContactComboBox.Text;
            String Address = "";
            String Address2 = "";
            String Email = "";
            String Phone = "";
            String Fax = "";

            openContactsDatabase();
            SqlCeCommand cmd = ContactDatabase.CreateCommand();
            cmd.CommandText = "SELECT Address, City, State, PostalCode,  WorkPhone, Fax, Email FROM Contacts WHERE AccountID = " + (short)Convert.ToInt16(AccountComboBox.SelectedValue) + " AND ContactID = " + (short)Convert.ToInt16(ContactComboBox.SelectedValue);
            SqlCeDataReader dBreader = cmd.ExecuteReader();
            while (dBreader.Read())
            {
                Address = dBreader[0].ToString();
                Address2 = dBreader[1].ToString() + ", " + dBreader[2].ToString() + " " + dBreader[3].ToString();
                Phone = dBreader[4].ToString();
                Fax = dBreader[5].ToString();
                Email = dBreader[6].ToString();
            }
            dBreader.Dispose();
            ContactDatabase.Close();

            if (IsSSPCheckBox.Checked == false)
            {
                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                iTextSharp.text.Font font = new iTextSharp.text.Font(bf, 7);
                Document document = new Document(iTextSharp.text.PageSize.LETTER);

                // Create the temp directory if it does not exist
                String tempFilePath = @AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\pdftemp";
                System.IO.Directory.CreateDirectory(tempFilePath);

                // If temp directory current contains any files, delete them
                System.IO.DirectoryInfo tempFiles = new DirectoryInfo(tempFilePath);

                foreach (FileInfo delfile in tempFiles.GetFiles())
                {
                    delfile.Delete();
                }

                System.IO.FileStream file = new System.IO.FileStream(tempFilePath + "\\heading.pdf", System.IO.FileMode.OpenOrCreate);
                PdfWriter writer = PdfWriter.GetInstance(document, file);

                document.Open();

                // For non Smart Start - Smart Start will use fields.
                // Quote Heading Table
                PdfPTable table = new PdfPTable(4);
                PdfPCell cell = new PdfPCell(new Phrase("Customer Name:", font));
                cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                cell.HorizontalAlignment = 2;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(ContactName, font));
                cell.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Quote ID:", font));
                cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                cell.HorizontalAlignment = 2;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("The Quote ID", font));
                cell.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                table.AddCell(cell);

                //
                cell = new PdfPCell(new Phrase("Company:", font));
                cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                cell.HorizontalAlignment = 2;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(AccountName, font));
                cell.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Quote Description:", font));
                cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                cell.HorizontalAlignment = 2;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(QuoteTitleTextBox.Text, font));
                cell.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                table.AddCell(cell);

                //
                cell = new PdfPCell(new Phrase("Address:", font));
                cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                cell.HorizontalAlignment = 2;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(Address, font));
                cell.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Quote Date:", font));
                cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                cell.HorizontalAlignment = 2;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(QuoteDateTimePicker.Text, font));
                cell.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                table.AddCell(cell);

                //
                cell = new PdfPCell(new Phrase("City, State, Zip:", font));
                cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                cell.HorizontalAlignment = 2;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(Address2, font));
                cell.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("", font));
                cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                cell.HorizontalAlignment = 2;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("", font));
                cell.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                table.AddCell(cell);

                //
                cell = new PdfPCell(new Phrase("Email:", font));
                cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                cell.HorizontalAlignment = 2;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(Email, font));
                cell.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Sales Rep:", font));
                cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                cell.HorizontalAlignment = 2;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(profile.Name, font));
                cell.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                table.AddCell(cell);

                //
                cell = new PdfPCell(new Phrase("Phone:", font));
                cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                cell.HorizontalAlignment = 2;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(Phone, font));
                cell.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Sales Rep Email:", font));
                cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                cell.HorizontalAlignment = 2;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(profile.Email, font));
                cell.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                table.AddCell(cell);

                //
                cell = new PdfPCell(new Phrase("Fax:", font));
                cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                cell.HorizontalAlignment = 2;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(Fax, font));
                cell.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Sales Rep Phone:", font));
                cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                cell.HorizontalAlignment = 2;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(profile.Phone, font));
                cell.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                table.AddCell(cell);
                document.Add(table);
                // End non-Smart Start Heading

                document.Close();
                file.Close();
            }
            else
            {
                // get the smart start header document
                Int16 whichDoc = (short)Convert.ToInt16(QuoteTemplateComboBox.SelectedValue);
                string pdfTemplate = getApplicationDocument(whichDoc);

                // this.Text += " - " + pdfTemplate;
                PdfReader pdfReader = new PdfReader(pdfTemplate);
                AcroFields pdfFormFields = pdfReader.AcroFields;

                string newFile = @"c:\temp\heading.pdf";
                PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFile, FileMode.Create));
                pdfFormFields = pdfStamper.AcroFields;

                // set form pdfFormFields
                pdfFormFields.SetField("Customer Name", ContactName);
                pdfFormFields.SetField("Company", AccountName);
                pdfFormFields.SetField("Sales Rep", profile.Name);
                pdfFormFields.SetField("Phone", Phone);
                pdfFormFields.SetField("Email", Email);
                pdfFormFields.SetField("Quote ID", "");
                pdfFormFields.SetField("Budgetary Quote Date", QuoteDateTimePicker.Text);
                pdfFormFields.SetField("Quote Description", QuoteTitleTextBox.Text);
                pdfFormFields.SetField("Instrument Description", QuoteDescriptionTextBox.Text);

                pdfFormFields.SetField("ReaderTotal", "100");
                pdfFormFields.SetField("WasherTotal", "200");
                // pdfFormFields.SetField("QCKitTotal", "300");
                // pdfFormFields.SetField("HPTotal", "400");
                // pdfFormFields.SetField("TipsTotal", "1000");
                pdfFormFields.SetField("AppSuppTotal", "2000");
                pdfFormFields.SetField("ContractTotal", "3000");
                pdfFormFields.SetField("Total Price", "10000");
                pdfFormFields.SetField("Instrument Price", "10000");

                // flatten the form to remove editting options, set it to false
                // to leave the form open to subsequent manual edits
                pdfStamper.FormFlattening = false;

                // close the pdf
                pdfStamper.Close();
            }
        }

        private int quotePDFaddItemsToQuote()
        {
            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            iTextSharp.text.Font font = new iTextSharp.text.Font(bf, 7);

            // Create the temp directory if it does not exist
            String tempFilePath = @AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\pdftemp";
            // System.IO.Directory.CreateDirectory(tempFilePath);

            PdfReader reader = new PdfReader(tempFilePath + "\\heading.pdf");
            int pageCount = 1;
            PdfStamper stamper = new PdfStamper(reader, new FileStream(tempFilePath + "\\heading_and_items" + pageCount + ".pdf", FileMode.Create));

            // calling PDFFooter class to Include in document
            // writer.PageEvent = new PDFFooter();
            //document.Open();

            Quote quote = new Quote();
            quote.QuoteTitle = QuoteTitleTextBox.Text;
            quote.Items = AddQuoteItems(QuoteDataGridView);
            quote.Options = AddQuoteItems(OptionsDataGridView);

            PdfPCell cell;
            // Quote Items Heading
            PdfPTable QuoteHeading = new PdfPTable(1);
            QuoteHeading.SpacingBefore = 4;
            QuoteHeading.SpacingAfter = 1;
            cell = new PdfPCell(new Phrase(quote.QuoteTitle.ToString(), font));
            cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            cell.HorizontalAlignment = 1;
            QuoteHeading.AddCell(cell);
            ColumnText ct = new ColumnText(stamper.GetOverContent(1));
            ct.AddElement(QuoteHeading);
            iTextSharp.text.Rectangle rect = new iTextSharp.text.Rectangle(36, 682, 577, 50);
            ct.SetSimpleColumn(rect);
            ct.Go();

            // // Item Column Headings
            PdfPTable ItemHeading = new PdfPTable(8);
            ItemHeading.SpacingBefore = 1;
            float[] theWidths = new float[] { 10f, 15f, 35f, 55f, 60f, 120f, 30f, 30f };
            ItemHeading.SetWidths(theWidths);

            cell = new PdfPCell(new Phrase("#", font));
            cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            cell.HorizontalAlignment = 1;
            ItemHeading.AddCell(cell);

            cell = new PdfPCell(new Phrase("Qty", font));
            cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            cell.HorizontalAlignment = 1;
            ItemHeading.AddCell(cell);

            cell = new PdfPCell(new Phrase("Part #", font));
            cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            cell.HorizontalAlignment = 1;
            ItemHeading.AddCell(cell);

            cell = new PdfPCell(new Phrase("Description", font));
            cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            cell.HorizontalAlignment = 1;
            ItemHeading.AddCell(cell);

            cell = new PdfPCell(new Phrase("Detail Description", font));
            cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            cell.HorizontalAlignment = 1;
            ItemHeading.AddCell(cell);

            cell = new PdfPCell(new Phrase("Image", font));
            cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            cell.HorizontalAlignment = 1;
            ItemHeading.AddCell(cell);

            cell = new PdfPCell(new Phrase("Unit Price", font));
            cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            cell.HorizontalAlignment = 1;
            ItemHeading.AddCell(cell);

            cell = new PdfPCell(new Phrase("Price", font));
            cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            cell.HorizontalAlignment = 1;
            ItemHeading.AddCell(cell);
            ct.AddElement(ItemHeading);
            rect = new iTextSharp.text.Rectangle(36, 667, 577, 50);
            ct.SetSimpleColumn(rect);
            ct.Go();

            // The Items Table
            PdfPTable table3 = new PdfPTable(9);
            table3.SpacingBefore = 1;
            float[] theWidths2 = new float[] { 10f, 15f, 35f, 55f, 60f, 90f, 30f, 30f, 30f };
            table3.SetWidths(theWidths2);

            // Add items
            String DetailDescription = "";
            float lly = 654;
            int loopCount = 0;
            int endOfLoop = 5;
            foreach (QuoteItems row in quote.Items)
            {
                if (loopCount == endOfLoop)
                {
                    stamper.Close();
                    reader = new PdfReader(tempFilePath + "\\heading_and_items" + pageCount + ".pdf");
                    pageCount++;
                    stamper = new PdfStamper(reader, new FileStream(tempFilePath + "\\heading_and_items" + pageCount + ".pdf", FileMode.Create));
                    stamper.InsertPage(pageCount, iTextSharp.text.PageSize.LETTER);
                    ct = new ColumnText(stamper.GetOverContent(pageCount));
                    ct.AddElement(ItemHeading);
                    rect = new iTextSharp.text.Rectangle(36, 760, 577, 50);
                    ct.SetSimpleColumn(rect);
                    ct.Go();
                    lly = 747;
                    loopCount = 0;
                    endOfLoop = 6;
                    // break;
                }

                if (row.SAPID == "Heading")
                {
                    cell = new PdfPCell(new Phrase(row.Description, font));
                    cell.Colspan = 9;
                    // cell.BackgroundColor = iTextSharp.text.Color.LIGHT_GRAY;
                    cell.HorizontalAlignment = 1;
                    table3.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Phrase("1", font));
                    // cell.BackgroundColor = iTextSharp.text.Color.LIGHT_GRAY;
                    cell.HorizontalAlignment = 1;
                    table3.AddCell(cell);

                    cell = new PdfPCell(new Phrase(row.Quantity.ToString(), font));
                    // cell.BackgroundColor = iTextSharp.text.Color.LIGHT_GRAY;
                    cell.HorizontalAlignment = 1;
                    table3.AddCell(cell);

                    cell = new PdfPCell(new Phrase(row.SAPID, font));
                    // cell.BackgroundColor = iTextSharp.text.Color.LIGHT_GRAY;
                    cell.HorizontalAlignment = 1;
                    table3.AddCell(cell);

                    cell = new PdfPCell(new Phrase(row.Description, font));
                    // cell.BackgroundColor = iTextSharp.text.Color.LIGHT_GRAY;
                    cell.Colspan = 4;
                    cell.FixedHeight = 13f;
                    cell.HorizontalAlignment = 0;
                    table3.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format("{0:C2}", row.Price), font));
                    // cell.BackgroundColor = iTextSharp.text.Color.LIGHT_GRAY;
                    cell.HorizontalAlignment = 1;
                    table3.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format("{0:C2}", row.Price * row.Quantity), font));
                    // cell.BackgroundColor = iTextSharp.text.Color.LIGHT_GRAY;
                    cell.HorizontalAlignment = 1;
                    table3.AddCell(cell);

                    DetailDescription = getDetailDescription(row.SAPID);
                    cell = new PdfPCell(new Phrase(DetailDescription, font));
                    // cell.BackgroundColor = iTextSharp.text.Color.LIGHT_GRAY;
                    cell.Colspan = 6;
                    cell.FixedHeight = 90f;
                    cell.HorizontalAlignment = 0;
                    table3.AddCell(cell);

                    cell = new PdfPCell(getImage(row.SAPID));
                    // cell.BackgroundColor = iTextSharp.text.Color.LIGHT_GRAY;
                    cell.Colspan = 3;
                    cell.HorizontalAlignment = 1;
                    cell.Padding = 5f;
                    table3.AddCell(cell);
                    ct.AddElement(table3);
                    rect = new iTextSharp.text.Rectangle(36, lly, 577, 103);
                    ct.SetSimpleColumn(rect);
                    ct.Go();
                    table3.DeleteBodyRows();
                    lly = lly - 103;
                    loopCount++;
                }
            }
            stamper.Close();

            // Add Options
            if (quote.Options.Count > 0)
            {
                reader = new PdfReader(tempFilePath + "\\heading_and_items" + pageCount + ".pdf");
                pageCount++;
                stamper = new PdfStamper(reader, new FileStream(tempFilePath + "\\heading_and_items" + pageCount + ".pdf", FileMode.Create));
                stamper.InsertPage(pageCount, iTextSharp.text.PageSize.LETTER);
                ct = new ColumnText(stamper.GetOverContent(pageCount));

                // Quote Options Heading
                PdfPTable OptionsHeading = new PdfPTable(1);
                cell = new PdfPCell(new Phrase("Quote Options", font));
                cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                cell.HorizontalAlignment = 1;
                OptionsHeading.AddCell(cell);
                ct.AddElement(OptionsHeading);
                rect = new iTextSharp.text.Rectangle(36, 760, 577, 50);
                ct.SetSimpleColumn(rect);
                ct.Go();
                ct.AddElement(ItemHeading);
                rect = new iTextSharp.text.Rectangle(36, 748, 577, 50);
                ct.SetSimpleColumn(rect);
                ct.Go();

                // The Option Items Table
                PdfPTable table4 = new PdfPTable(9);
                table4.SpacingBefore = 1;
                table4.SetWidths(theWidths2);

                // Add options
                lly = 735;
                loopCount = 0;
                endOfLoop = 6;
                foreach (QuoteItems row in quote.Options)
                {
                    if (loopCount == endOfLoop)
                    {
                        stamper.Close();
                        reader = new PdfReader(tempFilePath +  "\\heading_and_items" + pageCount + ".pdf");
                        pageCount++;
                        stamper = new PdfStamper(reader, new FileStream(tempFilePath + "\\heading_and_items" + pageCount + ".pdf", FileMode.Create));
                        stamper.InsertPage(pageCount, iTextSharp.text.PageSize.LETTER);
                        ct = new ColumnText(stamper.GetOverContent(pageCount));
                        ct.AddElement(ItemHeading);
                        rect = new iTextSharp.text.Rectangle(36, 760, 577, 50);
                        ct.SetSimpleColumn(rect);
                        ct.Go();
                        lly = 748;
                        loopCount = 0;
                    }

                    if (row.SAPID == "Heading")
                    {
                        cell = new PdfPCell(new Phrase(row.Description, font));
                        cell.Colspan = 9;
                        // cell.BackgroundColor = iTextSharp.text.Color.LIGHT_GRAY;
                        cell.HorizontalAlignment = 1;
                        table4.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Phrase("1", font));
                        // cell.BackgroundColor = iTextSharp.text.Color.LIGHT_GRAY;
                        cell.HorizontalAlignment = 1;
                        table4.AddCell(cell);

                        cell = new PdfPCell(new Phrase(row.Quantity.ToString(), font));
                        // cell.BackgroundColor = iTextSharp.text.Color.LIGHT_GRAY;
                        cell.HorizontalAlignment = 1;
                        table4.AddCell(cell);

                        cell = new PdfPCell(new Phrase(row.SAPID, font));
                        // cell.BackgroundColor = iTextSharp.text.Color.LIGHT_GRAY;
                        cell.HorizontalAlignment = 1;
                        table4.AddCell(cell);

                        cell = new PdfPCell(new Phrase(row.Description, font));
                        // cell.BackgroundColor = iTextSharp.text.Color.LIGHT_GRAY;
                        cell.Colspan = 4;
                        cell.FixedHeight = 13f;
                        cell.HorizontalAlignment = 0;
                        table4.AddCell(cell);

                        cell = new PdfPCell(new Phrase(String.Format("{0:C2}", row.Price), font));
                        // cell.BackgroundColor = iTextSharp.text.Color.LIGHT_GRAY;
                        cell.HorizontalAlignment = 1;
                        table4.AddCell(cell);

                        cell = new PdfPCell(new Phrase(String.Format("{0:C2}", row.Price * row.Quantity), font));
                        // cell.BackgroundColor = iTextSharp.text.Color.LIGHT_GRAY;
                        cell.HorizontalAlignment = 1;
                        table4.AddCell(cell);

                        DetailDescription = getDetailDescription(row.SAPID);
                        cell = new PdfPCell(new Phrase(DetailDescription, font));
                        // cell.BackgroundColor = iTextSharp.text.Color.LIGHT_GRAY;
                        cell.Colspan = 6;
                        cell.FixedHeight = 90f;
                        cell.HorizontalAlignment = 0;
                        table4.AddCell(cell);

                        cell = new PdfPCell(getImage(row.SAPID));
                        // cell.BackgroundColor = iTextSharp.text.Color.LIGHT_GRAY;
                        cell.Colspan = 3;
                        cell.HorizontalAlignment = 1;
                        cell.Padding = 5f;
                        table4.AddCell(cell);
                        ct.AddElement(table4);
                        rect = new iTextSharp.text.Rectangle(36, lly, 577, 103);
                        ct.SetSimpleColumn(rect);
                        ct.Go();
                        table4.DeleteBodyRows();
                        lly = lly - 103;
                        loopCount++;
                    }
                }
            }
            stamper.Close();
            return pageCount;
        }

        private void stamperTest()
        {
            // Get the file contents from the database
            openAppDocDatabase();
            SqlCeCommand cmd = TecanAppDocDatabase.CreateCommand();
            SqlCeDataReader reader;

            cmd.CommandText = "SELECT DocID, FileName FROM Documents WHERE DocumentPosition = 1";
            reader = cmd.ExecuteReader();

            PdfReader pdfReader;
            AcroFields pdfFormFields;
            StringBuilder sb = new StringBuilder();
            while (reader.Read())
            {
                // get the smart start header document
                Int16 whichDoc = (short)Convert.ToInt16(reader[0]);
                string pdfTemplate = getApplicationDocument(whichDoc);

                pdfReader = new PdfReader(pdfTemplate);
                pdfFormFields = pdfReader.AcroFields;

                sb.Append(reader[1].ToString() + Environment.NewLine);
                foreach (KeyValuePair<string, AcroFields.Item> kvp in pdfFormFields.Fields)
                {
                    sb.Append(kvp.Key.ToString() + Environment.NewLine);
                }
                sb.Append(Environment.NewLine + Environment.NewLine);
            }
            string fieldsFile = @"c:\temp\fields.txt";
            System.IO.File.WriteAllText(fieldsFile, sb.ToString());

            reader.Dispose();
            TecanAppDocDatabase.Close();

            //string pdfTemplate = @"c:\temp\SS D300e.pdf";
            //// this.Text += " - " + pdfTemplate;
            //PdfReader pdfReader = new PdfReader(pdfTemplate);
            //AcroFields pdfFormFields = pdfReader.AcroFields;
            //StringBuilder sb = new StringBuilder();
            //foreach (KeyValuePair<string, AcroFields.Item> kvp in pdfFormFields.Fields)
            //{
            //    sb.Append(kvp.Key.ToString() + Environment.NewLine);
            //}

            //string fieldsFile = @"c:\temp\fields.txt";
            //System.IO.File.WriteAllText(fieldsFile, sb.ToString());

            //string newFile = @"c:\temp\output.pdf";
            //PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFile, FileMode.Create));
            //pdfFormFields = pdfStamper.AcroFields;

            //// set form pdfFormFields
            //pdfFormFields.SetField("Customer Name", ContactName);
            //pdfFormFields.SetField("Company", AccountName);
            //pdfFormFields.SetField("Sales Rep", profile.Name);
            //pdfFormFields.SetField("Phone", Phone);
            //pdfFormFields.SetField("Email", Email);
            //pdfFormFields.SetField("Quote ID", quote.QuoteDate);
            //pdfFormFields.SetField("Budgetary Quote Date", quote.QuoteDate);
            //pdfFormFields.SetField("Quote Description", QuoteTitleTextBox.Text);
            //pdfFormFields.SetField("Instrument Description", QuoteDescriptionTextBox.Text);

            //pdfFormFields.SetField("ReaderTotal", "100");
            //pdfFormFields.SetField("WasherTotal", "200");
            //// pdfFormFields.SetField("QCKitTotal", "300");
            //// pdfFormFields.SetField("HPTotal", "400");
            //// pdfFormFields.SetField("TipsTotal", "1000");
            //pdfFormFields.SetField("AppSuppTotal", "2000");
            //pdfFormFields.SetField("ContractTotal", "3000");
            //pdfFormFields.SetField("Total Price", "10000");
            //pdfFormFields.SetField("Instrument Price", "10000");

            //// flatten the form to remove editting options, set it to false
            //// to leave the form open to subsequent manual edits
            //pdfStamper.FormFlattening = false;

            //// close the pdf
            //pdfStamper.Close();
        }

        private void quotePDFaddApplicationDocument(int pageCount)
        {
            // Create the temp directory if it does not exist
            String tempFilePath = @AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\pdftemp";
            System.IO.Directory.CreateDirectory(tempFilePath);

            Document document = new Document();
            PdfCopy writer = new PdfCopy(document, new FileStream(tempFilePath + "\\withApp.pdf", FileMode.Create));
            if (writer == null)
            {
                return;
            }
            document.Open();

            // get the heading document we just created
            PdfReader reader = new PdfReader(tempFilePath + "\\heading_and_items" + pageCount + ".pdf");
            reader.ConsolidateNamedDestinations();

            PdfContentByte cb = writer.DirectContent;
            for (int i = 1; i <= reader.NumberOfPages; i++)
            {
                PdfImportedPage page = writer.GetImportedPage(reader, i);
                writer.AddPage(page);
            }
            reader.Close();

            // get the application document
            Int16 whichDoc = (short)Convert.ToInt16(QuoteTemplateComboBox.SelectedValue);
            reader = new PdfReader(getApplicationDocument(whichDoc));
            reader.ConsolidateNamedDestinations();

            // step 4: we add content
            for (int i = 1; i <= reader.NumberOfPages; i++)
            {
                PdfImportedPage page = writer.GetImportedPage(reader, i);
                writer.AddPage(page);
            }
            reader.Close();

            // get the terms document
            reader = new PdfReader(getApplicationDocument(99999));
            reader.ConsolidateNamedDestinations();

            // step 4: we add content
            for (int i = 1; i <= reader.NumberOfPages; i++)
            {
                PdfImportedPage page = writer.GetImportedPage(reader, i);
                writer.AddPage(page);
            }
            reader.Close();

            writer.Close();
            document.Close();
        }

        private String getApplicationDocument(int whichDoc)
        {
            // Get the file contents from the database
            openAppDocDatabase();
            SqlCeCommand cmd = TecanAppDocDatabase.CreateCommand();
            SqlCeDataReader reader;

            if (whichDoc != 99999)
            {
                cmd.CommandText = "SELECT Document FROM Documents WHERE DocID = '" + whichDoc + "'";
            }
            else
            {
                cmd.CommandText = "SELECT Document FROM Documents WHERE FileName = 'terms.pdf'";
            }
            reader = cmd.ExecuteReader();

            Byte[] documentData = new Byte[0];
            while (reader.Read())
            {
                documentData = (byte[])reader[0];
            }
            reader.Dispose();
            TecanAppDocDatabase.Close();

            // Create the temp directory if it does not exist
            String tempFilePath = @AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\pdftemp";
            System.IO.Directory.CreateDirectory(tempFilePath);

            String fullFilePathName = @tempFilePath + "\\" + whichDoc;
            System.IO.FileStream fs = System.IO.File.Create(fullFilePathName);
            fs.Close();

            // Write file contents into file
            BinaryWriter Writer = null;

            try
            {
                // Create a new stream to write to the file
                Writer = new BinaryWriter(File.OpenWrite(fullFilePathName));

                // Writer raw data                
                Writer.Write(documentData);
                Writer.Flush();
                Writer.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return fullFilePathName;
        }

        protected String AddHeaderFooter()
        {
            // Create the temp directory if it does not exist
            String tempFilePath = @AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\pdftemp";
            System.IO.Directory.CreateDirectory(tempFilePath);

            byte[] bytes = File.ReadAllBytes(tempFilePath + "\\withApp.pdf");
            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            iTextSharp.text.Font boxfont = new iTextSharp.text.Font(bf, 5);
            iTextSharp.text.Font font = new iTextSharp.text.Font(bf, 8);

            PdfPTable tabHead = new PdfPTable(new float[] { 1F });
            tabHead.SpacingAfter = 10F;
            PdfPCell cell;
            tabHead.TotalWidth = 615;
            cell = new PdfPCell(new Phrase("\n", boxfont));

            cell.BackgroundColor = iTextSharp.text.BaseColor.RED;
            cell.Border = iTextSharp.text.Rectangle.BOX;
            cell.BorderColor = iTextSharp.text.BaseColor.RED;
            cell.HorizontalAlignment = 1;
            tabHead.AddCell(cell);

            String HeaderSting = "TECAN U.S.  9401 Globe Center Drive Suite 140 Morrisville, NC 27560 \nTelephone (919) 361-5200 \u2022" +
                " (800) 338-3226 \u2022 Fax (919) 361-3601";
            cell = new PdfPCell(new Phrase(HeaderSting, font));
            cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            cell.HorizontalAlignment = 1;
            tabHead.AddCell(cell);


            PdfPTable tabFot = new PdfPTable(new float[] { 1F });
            tabFot.TotalWidth = 615;
            System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Tecan_Quote_Generator.footer.png");
            iTextSharp.text.Image newImage = iTextSharp.text.Image.GetInstance(myStream);
            newImage.ScaleAbsolute(580f, 20f);

            cell = new PdfPCell(newImage);
            cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            cell.HorizontalAlignment = 1;
            tabFot.AddCell(cell);

            String pageNumberPhrase = "";
            using (MemoryStream stream = new MemoryStream())
            {
                PdfReader reader = new PdfReader(bytes);
                int pages = reader.NumberOfPages;
                using (PdfStamper stamper = new PdfStamper(reader, stream))
                {
                    // int pages = reader.NumberOfPages;
                    for (int i = 1; i <= pages; i++)
                    {
                        pageNumberPhrase = "Page " + i.ToString() + " of " + pages.ToString();
                        ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_LEFT, new Phrase(pageNumberPhrase, font), 10, 25, 0);
                        tabHead.WriteSelectedRows(0, -1, 0, 792, stamper.GetOverContent(i));
                        tabFot.WriteSelectedRows(0, -1, 0, 20, stamper.GetOverContent(i));
                    }
                }
                bytes = stream.ToArray();
            }
            File.WriteAllBytes(tempFilePath +  "\\" + QuoteTitleTextBox.Text + ".pdf", bytes);
            return tempFilePath;
        }

        private iTextSharp.text.Image getImage(string SAPID)
        {
            Byte[] imageData;
            try
            {
                openDB();
                SqlCeCommand cmd = TecanDatabase.CreateCommand();
                cmd.CommandText = "SELECT Document FROM PartImages WHERE SAPId = '" + SAPID + "'";
                imageData = (byte[])cmd.ExecuteScalar();
                if (imageData != null)
                {
                    iTextSharp.text.Image newImage = byteArrayToImage2(imageData);
                    newImage.ScaleAbsolute(80f, 80f);
                    return newImage;
                }
                else
                {
                    // If no image available
                    System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
                    Stream myStream = myAssembly.GetManifestResourceStream("Tecan_Quote_Generator.noimage.bmp");
                    // Bitmap image = new Bitmap(myStream);
                    iTextSharp.text.Image newImage = iTextSharp.text.Image.GetInstance(myStream);
                    newImage.ScaleAbsolute(80f, 80f);
                    return newImage;
                }

            }
            finally
            {
                TecanDatabase.Close();
            }
        }

        public iTextSharp.text.Image byteArrayToImage2(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            iTextSharp.text.Image returnImage = iTextSharp.text.Image.GetInstance(ms);
            return returnImage;
        }

        private String getDetailDescription(String SAPID)
        {
            String detailDescription = "";
            openDB();
            SqlCeCommand cmd = TecanDatabase.CreateCommand();
            cmd.CommandText = "SELECT DetailDescription FROM PartsList WHERE SAPId = '" + SAPID + "'";
            detailDescription = (string)cmd.ExecuteScalar();
            TecanDatabase.Close();
            return detailDescription;
        }

        private void clearQuoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Boolean doClear = clearQuote();
        }

        private void AccountComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            short currentAccountID = Convert.ToInt16(AccountComboBox.SelectedValue);
            this.contactsTableAdapter.FillByAccountID(this.customersDataSet.Contacts, currentAccountID);
        }

        private void getNewDatabseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (profile.DistributionFolder != "")
            {
                copyDatabaseToWorkingFolder();
            }
            else
            {
                // Show please wait panel with error, open profile form
                MessageBox.Show("error");
            }
        }

        private void RequiredPanelCancelButton_Click(object sender, EventArgs e)
        {
            RequiredPartsPanel.Visible = false;
        }

        private void RequiredPanelAddButton_Click(object sender, EventArgs e)
        {
            int i = 0;
            foreach (QuoteItems row in passRequiredItems.Items)
            {
                if (RequiredPartCheckedListBox.GetItemChecked(i))
                {
                    quoteSaved = false;
                    if (passRequiredItems.QuoteTitle == "QuoteItems")
                    {
                        QuoteDataGridView.Rows.Add(row.SAPID, row.Description, Convert.ToDecimal(row.Price), 1, String.Format("{0:P2}", 0.00), Convert.ToDecimal(row.Price));
                    }
                    else
                    {
                        OptionsDataGridView.Rows.Add(row.SAPID, row.Description, Convert.ToDecimal(row.Price), 1, String.Format("{0:P2}", 0.00), Convert.ToDecimal(row.Price));
                    }
                }
                i++;
            }
            RequiredPartsPanel.Visible = false;
            SumItems(QuoteDataGridView);
            SumItems(OptionsDataGridView);
        }

        private void RequiredPartsPanelSelectAllCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (RequiredPartsPanelSelectAllCheckBox.Checked == true)
            {
                for (int i = 0; i < RequiredPartCheckedListBox.Items.Count; i++)
                {
                    RequiredPartCheckedListBox.SetItemChecked(i, true);
                }
            }
            else
            {
                for (int i = 0; i < RequiredPartCheckedListBox.Items.Count; i++)
                {
                    RequiredPartCheckedListBox.SetItemChecked(i, false);
                }
            }
        }

        private void PLeaseWaitPanelOKButton_Click(object sender, EventArgs e)
        {
            PLeaseWaitPanelOKButton.Visible = false;
            PleaseWaitHeadingLabel.ForeColor = System.Drawing.Color.Red;
            PleaseWaitHeadingLabel.Text = "Please Wait";
            PleaseWaitMessageTextBox.Text = "";
            PleaseWaitPanel.Visible = false;
            var control = FindFocusedControl(this);
            MessageBox.Show(control.Name);
        }

        public static Control FindFocusedControl(Control control)
        {
            var container = control as IContainerControl;
            while (container != null)
            {
                control = container.ActiveControl;
                container = control as IContainerControl;
            }
            return control;
        }

        private void sendBugReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BugReportPanel.Visible = true;
            BugReportTextBox.Focus();
        }

        private void BugReportSendButton_Click(object sender, EventArgs e)
        {
            String bugReportMessageString = "Operation System: " + Environment.OSVersion + "\n\n";
            bugReportMessageString += BugReportTextBox.Text;

            // Setup mail message
            MailAddress to = new MailAddress(profile.TecanEmail);
            MailAddress from = new MailAddress(profile.Email);
            var mailMessage = new MailMessage(from, to);
            mailMessage.Subject = "Configurator Bug Report";
            mailMessage.Body = bugReportMessageString;
            // mailMessage.Attachments.Add(new Attachment(fullAttachmentPathName));

            // var filename = attachmentPath + "\\mymessage.eml";
            String tempFilePath = createTempFolder();
            var filename = tempFilePath + "\\mymessage.eml";

            //save the MailMessage to the filesystem
            mailMessage.Save(filename);

            //Open the file with the default associated application registered on the local machine
            Process.Start(filename);
            BugReportTextBox.Text = "";
            BugReportPanel.Visible = false;
        }

        private string createTempFolder()
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
            return tempFilePath;
        }

        private void BugReportCancelButton_Click(object sender, EventArgs e)
        {
            BugReportTextBox.Text = "";
            BugReportPanel.Visible = false;
        }

        private void sendQuoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!quoteSaved)
            {
                saveQuoteToolStripMenuItem_Click(sender, e);
                return;
            }

            // Get Contact email Address
            String Email = "";
            openContactsDatabase();
            SqlCeCommand cmd = ContactDatabase.CreateCommand();
            cmd.CommandText = "SELECT Email FROM Contacts WHERE AccountID = " + (short)Convert.ToInt16(AccountComboBox.SelectedValue) + " AND ContactID = " + (short)Convert.ToInt16(ContactComboBox.SelectedValue);
            SqlCeDataReader dBreader = cmd.ExecuteReader();
            while (dBreader.Read())
            {
                Email = dBreader[0].ToString();
            }
            dBreader.Dispose();
            ContactDatabase.Close();

            // Setup mail message
            MailAddress to = new MailAddress(Email);
            MailAddress from = new MailAddress(profile.Email);
            var mailMessage = new MailMessage(from, to);
            mailMessage.Subject = "Tecan Budgetary Quote - " + QuoteTitleTextBox.Text;
            // mailMessage.Body = "Tecan Budgetary Quote - " + QuoteTitleTextBox.Text;

            String tecanFilesFilePath = @"c:\TecanFiles";
            String quoteDate = QuoteDateTimePicker.Text.Replace("/", "_");
            String QuoteFileName = AccountComboBox.Text + "_" + QuoteTitleTextBox.Text + "_" + quoteDate + ".pdf";
            String fullPDFfilename = tecanFilesFilePath + "\\" + QuoteFileName;
            mailMessage.Attachments.Add(new Attachment(fullPDFfilename));


            // var filename = attachmentPath + "\\mymessage.eml";
            String tempFilePath = createTempFolder();
            var filename = tempFilePath + "\\mymessage.eml";
            //save the MailMessage to the filesystem
            mailMessage.Save(filename);

            //Open the file with the default associated application registered on the local machine
            Process.Start(filename);
        }

        private void convertQuoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Boolean doClear = clearQuote();
            if (!doClear) return;

            // Get Old Quote Filename and Path to Convert
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                QuoteTitleTextBox.Text = System.IO.Path.GetFileNameWithoutExtension(openFileDialog1.FileName);
                openDB();
                SqlCeCommand cmd = TecanDatabase.CreateCommand();
                SqlCeDataReader reader;

                // Read the file and display it line by line.
                System.IO.StreamReader file = new System.IO.StreamReader(openFileDialog1.FileName);
                
                String line;
                String SAPId;
                while ((line = file.ReadLine()) != null)
                {
                    if (Regex.IsMatch(line, @"^\d{8}"))
                    {
                        // Find Item in Db, add item to quote
                        SAPId = line.Substring(0, 8);
                        cmd.CommandText = "SELECT SAPId, Description, ILP FROM PartsList WHERE SAPId = '" + SAPId + "'";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            QuoteDataGridView.Rows.Add(reader[0].ToString(), reader[1].ToString(), (Decimal)reader[2], 1, String.Format("{0:P2}", 0.00), (Decimal)reader[2]);
                        }
                    }
                }
                SumItems(QuoteDataGridView);
            }
        }

        private void IsSSPCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            String myConnStr = partsListTableAdapter.Connection.ConnectionString;
            if (IsSSPCheckBox.Checked == true)
            {
                myConnStr = myConnStr.Replace("TecanQuoteGeneratorPartsList", "TecanSmartStartQuoteGeneratorPartsList");
                SmartStartHeaderComboBox.Visible = true;
            }
            else
            {
                myConnStr = myConnStr.Replace("TecanSmartStartQuoteGeneratorPartsList", "TecanQuoteGeneratorPartsList");
                SmartStartHeaderComboBox.Visible = false;
            }
            partsListTableAdapter.Connection.ConnectionString = myConnStr;
            salesTypeTableAdapter.Connection.ConnectionString = myConnStr;
            subCategoryTableAdapter.Connection.ConnectionString = myConnStr;
            categoryTableAdapter.Connection.ConnectionString = myConnStr;
            instrumentTableAdapter.Connection.ConnectionString = myConnStr;
            doFormInitialization();
        }

        private void QuoteTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadTemplateList();
        }

    }
}
