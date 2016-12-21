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

namespace Tecan_Quote_Generator
{
    public partial class MainQuoteForm : Form
    {
        public static Boolean isManager = false;
        SqlCeConnection TecanDatabase = null;

        Boolean searchPreformed = true;
        Boolean salesTypeChanged = false;
        Boolean instrumentChanged = false;
        Boolean categoryChanged = false;
        // Boolean subCategoryChanged = false;
        // Boolean formatOnly = false;
        Boolean quoteSaved = true;

        PartsListDetailDisplay DetailsForm;
        Profile profile = new Profile();
        Quote passRequiredItems = new Quote();

        public MainQuoteForm()
        {
            InitializeComponent();
        }

        public void MainQuoteForm_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'customersDataSet.Contacts' table. You can move, or remove it, as needed.
            // this.contactsTableAdapter.Fill(this.customersDataSet.Contacts);
            // TODO: This line of code loads data into the 'customersDataSet.Accounts' table. You can move, or remove it, as needed.
            this.accountsTableAdapter.Fill(this.customersDataSet.Accounts);
            short currentAccountID = Convert.ToInt16(AccountComboBox.SelectedValue);
            this.contactsTableAdapter.FillByAccountID(this.customersDataSet.Contacts, currentAccountID);

            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.PartNumberClearButton, "Clear Part Number Search");
            ToolTip1.SetToolTip(this.DescriptionClearButton, "Clear Description Search");
            ToolTip1.SetToolTip(this.ClearFiltersButton, "Reset All Category Filters");

            // TODO: This line of code loads data into the 'tecanQuoteGeneratorPartsListDataSet.SubCategory' table. You can move, or remove it, as needed.
            this.subCategoryTableAdapter.Fill(this.tecanQuoteGeneratorPartsListDataSet.SubCategory);
            // TODO: This line of code loads data into the 'tecanQuoteGeneratorPartsListDataSet.Category' table. You can move, or remove it, as needed.
            this.categoryTableAdapter.Fill(this.tecanQuoteGeneratorPartsListDataSet.Category);
            // TODO: This line of code loads data into the 'tecanQuoteGeneratorPartsListDataSet.Instrument' table. You can move, or remove it, as needed.
            this.instrumentTableAdapter.Fill(this.tecanQuoteGeneratorPartsListDataSet.Instrument);
            // TODO: This line of code loads data into the 'tecanQuoteGeneratorPartsListDataSet.SalesType' table. You can move, or remove it, as needed.
            this.salesTypeTableAdapter.Fill(this.tecanQuoteGeneratorPartsListDataSet.SalesType);
            // TODO: This line of code loads data into the 'tecanQuoteGeneratorPartsListDataSet.PartsList' table. You can move, or remove it, as needed.
            this.partsListTableAdapter.Fill(this.tecanQuoteGeneratorPartsListDataSet.PartsList);
            // Check for salesman profile, if no file get salesman information
            // Check if Quote Database is empty
            if (partsListBindingSource.Count != 0)
            {
                loadFilterComboBoxes();
                // int newGridHeight;
                // newGridHeight = Screen.PrimaryScreen.Bounds.Height - (this.menuStrip1.Height + this.partsListBindingNavigator.Height);
                // this.partsListDataGridView.Height = newGridHeight - 500;
                // this.partsListDataGridView.Height = 525;
                setPartDetailTextBox();
                QuoteDataGridView.AllowDrop = true;
                OptionsDataGridView.AllowDrop = true;
                // ThirdPartyDataGridView.AllowDrop = true;
                // SmartStartDataGridView.AllowDrop = true;
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
            }
            // Checks DB and copies / loads if required
            else if (partsListBindingSource.Count == 1)
            {
                showUserProfileForm(true);
            }
            else if (!File.Exists(profileFile))
            {
                showUserProfileForm(false);
            }

            // If = 1 then no DB, requires intilization
            //if (partsListBindingSource.Count == 1)
            //{

            //    if (MessageBox.Show("The Tecan Quote Generator must be intilized!\r\n\r\nDo you want to perform intialization now?", "Initial Installation", MessageBoxButtons.YesNo) == DialogResult.No)
            //    {
            //        this.Close();
            //    }
            //    else
            //    {
            //        if (!File.Exists(profileFile))
            //        {
            //            ProfileForm profileForm = new ProfileForm(true);
            //            profileForm.SetForm1Instance(this);
            //            profileForm.Show();
            //            Application.OpenForms["ProfileForm"].BringToFront();
            //        }
            //        else
            //        {
            //            String distributionFolder;
            //            getUsersProfile();
            //            distributionFolder = profile.DistributionFolder;

            //            if (distributionFolder == null)
            //            {
            //                ProfileForm profileForm = new ProfileForm(true);
            //                profileForm.SetForm1Instance(this);
            //                profileForm.Show();
            //                Application.OpenForms["ProfileForm"].BringToFront();
            //                MessageBox.Show("There's a problem with your profile settings.  Please re-enter and save your information!");
            //            }

            //            Boolean fileFound;
            //            fileFound = copyDatabaseToWorkingFolder(distributionFolder);
            //            if (!fileFound)
            //            {
            //                MessageBox.Show("The Distribution Folder you selected in your profile does not contain the Parts List Database!\n\nPlease select a new folder");
            //                ProfileForm profileForm = new ProfileForm(true);
            //                profileForm.SetForm1Instance(this);
            //                profileForm.Show();
            //                Application.OpenForms["ProfileForm"].BringToFront();
            //            }
            //            else
            //            {
            //                MainQuoteForm_Load(sender, e);
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    if (!File.Exists(profileFile))
            //    {
            //        ProfileForm profileForm = new ProfileForm(false);
            //        profileForm.SetForm1Instance(this);
            //        profileForm.Show();
            //        Application.OpenForms["ProfileForm"].BringToFront();
            //    }
            //    else
            //    {
            //        getUsersProfile();
            //    }
            //}
        }

        public void showUserProfileForm(Boolean NeedsDB)
        {
            ProfileForm profileForm = new ProfileForm(NeedsDB);
            profileForm.SetForm1Instance(this);
            profileForm.Show();
            Application.OpenForms["ProfileForm"].BringToFront();
        }

        // If blank database or new database available copy new database to working folder
        public Boolean copyDatabaseToWorkingFolder(String sourcePath)
        {
            String profileFile = @"c:\TecanFiles\" + "TecanQuoteConfig.cfg";
            if (File.Exists(profileFile))
            {
                String quoteSourceFile = "";
                String supplementSourceFile;

                // Where new files will go
                String quoteTargetFile = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "TecanQuoteGeneratorPartsList.sdf");
                String supplementTargetFile = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "TecanSuppDocs.sdf");

                // Where the new files will come from
                try
                {
                    quoteSourceFile = System.IO.Path.Combine(sourcePath, "TecanQuoteGeneratorPartsList.sdf");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
                supplementSourceFile = System.IO.Path.Combine(sourcePath, "TecanSuppDocs.sdf");

                // Verify the files exisit before copy
                if (!File.Exists(quoteSourceFile))
                {
                    return false;
                }

                getUsersProfile();
                FileInfo fi = new FileInfo(quoteSourceFile);
                profile.DatabaseCreationDate = fi.CreationTime;
                saveUsersProfile();
                System.IO.File.Copy(quoteSourceFile, quoteTargetFile, true);
                System.IO.File.Copy(supplementSourceFile, supplementTargetFile, true);
                return true;
            }
            else
            {
                getUsersProfile();
                return false;
            }
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
                    DetailsForm.LoadParts(SelectedRow.SAPId);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                DetailsForm.SetForm1Instance(this);
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
                    //// Check if they are already added
                    //DataGridViewRowCollection itemsRows = QuoteDataGridView.Rows;
                    //DataGridViewRowCollection optionRows = OptionsDataGridView.Rows;
                    //String existsSAPID;
                    //var PartsToAddList = new List<string>();
                    //String[] requiredPart;
                    //Boolean foundSAPID = false;
                    //// Loop throught required parts
                    //foreach (String part in hasRequiredParts)
                    //{
                    //    requiredPart = part.Split('^');
                    //    foundSAPID = false;
                    //    // Already selected items in quote
                    //    foreach (DataGridViewRow rowItem in itemsRows)
                    //    {
                    //        existsSAPID = rowItem.Cells[0].Value.ToString();
                    //        if (existsSAPID == requiredPart[0])
                    //        {
                    //            foundSAPID = true;
                    //            break;
                    //        }
                    //    }
                    //    // Already selected items in options
                    //    foreach (DataGridViewRow rowOption in optionRows)
                    //    {
                    //        existsSAPID = rowOption.Cells[0].Value.ToString();
                    //        if (existsSAPID == requiredPart[0])
                    //        {
                    //            foundSAPID = true;
                    //            break;
                    //        }
                    //    }
                    //    if (!foundSAPID)
                    //    {
                    //        PartsToAddList.Add(part);
                    //    }
                    //}
                    //String[] toAddPart = PartsToAddList.ToArray();
                    //switch (toAddPart.Length)
                    //{
                    //    // Required part already added, do nothing
                    //    case 0:
                    //        break;

                    //    // 1 required part, simple message
                    //    case 1:
                    //        String[] partToAdd = null;
                    //        partToAdd = toAddPart[0].Split('^');
                    //        Decimal partItemPrice = Convert.ToDecimal(partToAdd[2]);
                    //        // Ask to add part
                    //        if (MessageBox.Show("The part\r\n\r\n" + itemSAPID + "  " + itemDescription + "\r\n\r\nhas a required part \r\n\r\n" + partToAdd[0] + "  " + partToAdd[1] + ".\r\n\r\nDo you want to add it?", "Required Part", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    //        {
                    //            QuoteDataGridView.Rows.Add(partToAdd[0], partToAdd[1], partItemPrice, 1, String.Format("{0:P2}", 0.00), partItemPrice);
                    //        }
                    //        break;

                    //    // Multiple requires parts, do required panel
                    //    default:
                    //    // Ask to add parts
                    //        RequiredPartCheckedListBox.Items.Clear();
                    //        RequiredPartsPanelSelectAllCheckBox.Checked = false;
                    //        RequiredPartsPanelHeadingLabel.Text = "The part " + itemSAPID + "  " + itemDescription + " has multiple parts that are required.\r\nPlease select (Double-Click) the parts you would like to add.";
                    //        RequiredPartsPanel.Location = new Point(
                    //        this.ClientSize.Width / 2 - RequiredPartsPanel.Size.Width / 2,
                    //        this.ClientSize.Height / 2 - RequiredPartsPanel.Size.Height / 2);
                    //        RequiredPartsPanel.Anchor = AnchorStyles.None;
                    //        RequiredPartsPanel.Visible = true;

                    //        ArrayList quoteItems = new ArrayList();
                    //        foreach (String part in toAddPart)
                    //        {
                    //            requiredPart = part.Split('^');
                    //            RequiredPartCheckedListBox.Items.Add(requiredPart[0] + "  " + requiredPart[1]);

                    //            QuoteItems newItem = new QuoteItems();
                    //            newItem.SAPID = requiredPart[0];
                    //            newItem.Description = requiredPart[1];
                    //            newItem.Price = Convert.ToDecimal(requiredPart[2]);
                    //            quoteItems.Add(newItem);
                    //        }
                    //        passRequiredItems.QuoteTitle = "QuoteItems";
                    //        passRequiredItems.Items = quoteItems;
                    //        break;
                    //}

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
                itemPrice = (Decimal)srow.Cells[2].Value;
                itemQty = Convert.ToInt32(srow.Cells[3].Value);
                totalItemPrice = totalItemPrice + (itemPrice * itemQty);
                discountPercentage = Convert.ToDecimal(srow.Cells[4].Value.ToString().Replace(" %", ""));
                itemDiscount = (itemPrice * (discountPercentage/100)) * itemQty;
                totalDiscount = totalDiscount + itemDiscount;
                extendedPrice = extendedPrice + (Decimal)srow.Cells[5].Value;
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
                processCellValueChange(QuoteDataGridView, e);
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
            //String itemPriceCheck;
            //String discountCheck;

            // itemPriceCheck = myDataGridView.Rows[e.RowIndex].Cells[2].Value.ToString();
            // discountCheck = myDataGridView.Rows[e.RowIndex].Cells[4].Value.ToString();

            //if ((itemPriceCheck.IndexOf("$") == -1) || (discountCheck.IndexOf("%") == -1))
            //{
            //    formatOnly = false;
            //}

            rowIndex = e.RowIndex;
            DataGridViewRow srow = myDataGridView.Rows[rowIndex];
            itemPrice = (Decimal)srow.Cells[2].Value;
            itemQty = Convert.ToInt32(srow.Cells[3].Value);
            // totalItemPrice = totalItemPrice + (itemPrice * itemQty);
            discountPercentage = Convert.ToDecimal(srow.Cells[4].Value.ToString().Replace(" %", ""));
            itemDiscount = (itemPrice * (discountPercentage / 100)) * itemQty;
            // totalDiscount = totalDiscount + itemDiscount;
            extendedPrice = (itemPrice * itemQty) - itemDiscount;




            //if ((e.ColumnIndex == 3 || e.ColumnIndex == 4))
            //{
            //    itemPrice = (Decimal)myDataGridView.Rows[e.RowIndex].Cells[2].Value;
            //    itemQty = Convert.ToInt32(myDataGridView.Rows[e.RowIndex].Cells[3].Value);
            //    itemDiscount = Convert.ToDecimal(myDataGridView.Rows[e.RowIndex].Cells[4].Value);
            //    extendedPrice = itemPrice * itemQty;
            //    if (itemDiscount != 0)
            //    {
            //        extendedPrice = extendedPrice * (itemDiscount / 100);
            //    }
            //}

            //switch (e.ColumnIndex)
            //{
            //    // Price
            //    case 2:
            //        if (itemPriceCheck.IndexOf("$") == -1)
            //        {
            //            formatOnly = true;
            //            itemPrice = Convert.ToDecimal(itemPriceCheck);
            //            QuoteDataGridView.Rows[e.RowIndex].Cells[2].Value = String.Format("{0:C2}", itemPrice);
            //        }
            //        break;

            //    // Discount
            //    case 4:
            //        if (discountCheck.IndexOf("%") == -1)
            //        {
            //            formatOnly = true;
            //            discountPercentage = Convert.ToDecimal(discountCheck);
            //            QuoteDataGridView.Rows[e.RowIndex].Cells[4].Value = String.Format("{0:P2}", discountPercentage / 100);
            //        }
            //        break;
            //}
            //QuoteDataGridView.Rows[e.RowIndex].Cells[2].Value = String.Format("{0:C2}", itemPrice);
            myDataGridView.Rows[e.RowIndex].Cells[4].Value = String.Format("{0:P2}", discountPercentage / 100);
            // myDataGridView.Rows[e.RowIndex].Cells[5].Value = String.Format("{0:C2}", extendedPrice);
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
                    //// Check if they are already added
                    //DataGridViewRowCollection itemsRows = QuoteDataGridView.Rows;
                    //DataGridViewRowCollection optionRows = OptionsDataGridView.Rows;
                    //String existsSAPID;
                    //var PartsToAddList = new List<string>();
                    //String[] requiredPart;
                    //Boolean foundSAPID = false;
                    //// Loop throught required parts
                    //foreach (String part in hasRequiredParts)
                    //{
                    //    requiredPart = part.Split('^');
                    //    foundSAPID = false;
                    //    // Already selected items in quote
                    //    foreach (DataGridViewRow rowItem in itemsRows)
                    //    {
                    //        existsSAPID = rowItem.Cells[0].Value.ToString();
                    //        if (existsSAPID == requiredPart[0])
                    //        {
                    //            foundSAPID = true;
                    //            break;
                    //        }
                    //    }
                    //    // Already selected items in options
                    //    foreach (DataGridViewRow rowOption in optionRows)
                    //    {
                    //        existsSAPID = rowOption.Cells[0].Value.ToString();
                    //        if (existsSAPID == requiredPart[0])
                    //        {
                    //            foundSAPID = true;
                    //            break;
                    //        }
                    //    }
                    //    if (!foundSAPID)
                    //    {
                    //        PartsToAddList.Add(part);
                    //    }
                    //}
                    //String[] toAddPart = PartsToAddList.ToArray();
                    //switch (toAddPart.Length)
                    //{
                    //    // Required part already added, do nothing
                    //    case 0:
                    //        break;

                    //    // 1 required part, simple message
                    //    case 1:
                    //        String[] partToAdd = null;
                    //        partToAdd = toAddPart[0].Split('^');
                    //        Decimal partItemPrice = Convert.ToDecimal(partToAdd[2]);
                    //        // Ask to add part
                    //        if (MessageBox.Show("The part\r\n\r\n" + itemSAPID + "  " + itemDescription + "\r\n\r\nhas a required part \r\n\r\n" + partToAdd[0] + "  " + partToAdd[1] + ".\r\n\r\nDo you want to add it?", "Required Part", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    //        {
                    //            OptionsDataGridView.Rows.Add(partToAdd[0], partToAdd[1], partItemPrice, 1, String.Format("{0:P2}", 0.00), partItemPrice);
                    //        }
                    //        break;

                    //    // Multiple requires parts, do required panel
                    //    default:
                    //        // Ask to add parts
                    //        RequiredPartCheckedListBox.Items.Clear();
                    //        RequiredPartsPanelHeadingLabel.Text = "The part " + itemSAPID + "  " + itemDescription + " has multiple parts that are required.\r\nPlease select (Double-Click) the parts you would like to add.";
                    //        RequiredPartsPanel.Location = new Point(
                    //        this.ClientSize.Width / 2 - RequiredPartsPanel.Size.Width / 2,
                    //        this.ClientSize.Height / 2 - RequiredPartsPanel.Size.Height / 2);
                    //        RequiredPartsPanel.Anchor = AnchorStyles.None;
                    //        RequiredPartsPanel.Visible = true;

                    //        ArrayList quoteItems = new ArrayList();
                    //        foreach (String part in toAddPart)
                    //        {
                    //            requiredPart = part.Split('^');
                    //            RequiredPartCheckedListBox.Items.Add(requiredPart[0] + "  " + requiredPart[1]);

                    //            QuoteItems newItem = new QuoteItems();
                    //            newItem.SAPID = requiredPart[0];
                    //            newItem.Description = requiredPart[1];
                    //            newItem.Price = Convert.ToDecimal(requiredPart[2]);
                    //            quoteItems.Add(newItem);
                    //        }
                    //        passRequiredItems.QuoteTitle = "QuoteOptions";
                    //        passRequiredItems.Items = quoteItems;
                    //        break;
                    //}

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
                    RequiredPartsPanel.Location = new Point(
                    this.ClientSize.Width / 2 - RequiredPartsPanel.Size.Width / 2,
                    this.ClientSize.Height / 2 - RequiredPartsPanel.Size.Height / 2);
                    RequiredPartsPanel.Anchor = AnchorStyles.None;
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
            String dataPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            TecanDatabase.ConnectionString = "Data Source=|DataDirectory|\\TecanQuoteGeneratorPartsList.sdf;Max Database Size=4000;Max Buffer Size=1024;Persist Security Info=False";
            TecanDatabase.Open();
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
            contacts.FormClosing += new FormClosingEventHandler(reloadMe);
        }

        private void reloadMe(object sender, EventArgs e)
        {
            MainQuoteForm_Load(sender, e);
        }

        private void QuoteRemoveSelectedButton_Click(object sender, EventArgs e)
        {
            RemoveItems(QuoteDataGridView);
            SumItems(QuoteDataGridView);
        }

        private void OptionsRemoveSelectedButton_Click(object sender, EventArgs e)
        {
            RemoveItems(OptionsDataGridView);
            SumItems(OptionsDataGridView);
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
            }
        }

        private void saveQuoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (QuoteTitleTextBox.Text == "")
            {
                QuoteTabControl.SelectedTab = QuoteSettingTabPage;
                QuoteTitleTextBox.Focus();
                MessageBox.Show("Please enter a Quote Title before saving.");
                return;
            }
            
            Quote quote = new Quote();
            quote.QuoteAccount = (short)Convert.ToInt16(AccountComboBox.SelectedValue);
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
            System.IO.Directory.CreateDirectory(tecanFilesFilePath);
            String QuoteFileName = QuoteTitleTextBox.Text + ".tbq";

            if(File.Exists(@"c:\TecanFiles\" + QuoteFileName))
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
            MessageBox.Show("Quote c:\\TecanFiles\\" + QuoteFileName + " saved.");
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

            // todo Remove myDataGridView.Rows[rowCount] with row
            foreach (DataGridViewRow row in myDataGridView.Rows)
            {
                SAPID = myDataGridView.Rows[rowCount].Cells[0].Value.ToString();
                Description = myDataGridView.Rows[rowCount].Cells[1].Value.ToString();

                itemPriceCheck = myDataGridView.Rows[rowCount].Cells[2].Value.ToString();
                discountCheck = myDataGridView.Rows[rowCount].Cells[4].Value.ToString();

                if (itemPriceCheck.IndexOf("$") != -1) itemPriceCheck = itemPriceCheck.Substring(1, itemPriceCheck.Length - 1);
                itemPrice = Convert.ToDecimal(itemPriceCheck);

                if (discountCheck.IndexOf("%") != -1) discountCheck = discountCheck.Substring(0, discountCheck.Length - 2);
                discountPercentage = Convert.ToDecimal(discountCheck);

                itemQty = Convert.ToInt32(myDataGridView.Rows[rowCount].Cells[3].Value);

                Note = Convert.ToBoolean(myDataGridView.Rows[rowCount].Cells[6].Value);
                Image = Convert.ToBoolean(myDataGridView.Rows[rowCount].Cells[7].Value);

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
            Int32 quoteRowCount = QuoteDataGridView.Rows.Count;
            Int32 optionsRowCount = OptionsDataGridView.Rows.Count;
            if (quoteRowCount > 0 || optionsRowCount > 0)
            {
                if (MessageBox.Show("You already have items selected!\r\n\r\nDo you want to clear these items?", "Clear List", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }

            }
            QuoteDataGridView.Rows.Clear();
            QuoteItemsPriceTextBox.Text = "";
            OptionsDataGridView.Rows.Clear();
            OptionsItemsPriceTextBox.Text = "";
            //ThirdPartyDataGridView.Rows.Clear();
            //SmartStartDataGridView.Rows.Clear();

            // Get Quote Filename and Path
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\TecanFiles";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(Quote));
                System.IO.StreamReader file = new System.IO.StreamReader(openFileDialog1.FileName);
                Quote quote = new Quote();
                quote = (Quote)reader.Deserialize(file);

                AccountComboBox.SelectedValue = quote.QuoteAccount;
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

                foreach (QuoteItems row in quote.Items)
                {
                    itemSAPID = row.SAPID;
                    itemDescription = row.Description;
                    itemPrice = row.Price;
                    itemQuantity = row.Quantity;
                    itemDiscount = row.Discount;
                    itemNote = row.IncludeNote;
                    itemImage = row.IncludeImage;

                    QuoteDataGridView.Rows.Add(itemSAPID, itemDescription, itemPrice, itemQuantity, String.Format("{0:P2}", itemDiscount), itemPrice, itemNote, itemImage);
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

                    OptionsDataGridView.Rows.Add(itemSAPID, itemDescription, itemPrice, itemQuantity, String.Format("{0:P2}", itemDiscount), itemPrice, itemNote, itemImage);
                }

                QuoteTabControl.SelectedTab = QuoteTabPage;
                SumItems(QuoteDataGridView);
                SumItems(OptionsDataGridView);
            }
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
            string pdfTemplate = @"c:\temp\Tecan DiTi Systems Form - Ian2.pdf";
            this.Text += " - " + pdfTemplate;
            PdfReader pdfReader = new PdfReader(pdfTemplate);
            StringBuilder sb = new StringBuilder();
            foreach (DictionaryEntry de in pdfReader.AcroFields.Fields)
            {
                sb.Append(de.Key.ToString() + Environment.NewLine);
            }

            string fieldsFile = @"c:\temp\fields.txt";
            System.IO.File.WriteAllText(fieldsFile, sb.ToString());

            string newFile = @"c:\temp\output.pdf";
            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFile, FileMode.Create));
            AcroFields pdfFormFields = pdfStamper.AcroFields;

            // set form pdfFormFields
            pdfFormFields.SetField("Company", "The Company Name");
            pdfFormFields.SetField("Customer Name", "The Customer");
            pdfFormFields.SetField("Address", "123 Main Street");
            pdfFormFields.SetField("Phone", "919-555-1212");
            pdfFormFields.SetField("Email", "email@email.com");

            pdfFormFields.SetField("ReadersTotal", "100");
            pdfFormFields.SetField("WasherTotal", "200");
            pdfFormFields.SetField("QCKitTotal", "300");
            pdfFormFields.SetField("HPTotal", "400");
            pdfFormFields.SetField("TipsTotal", "1000");
            pdfFormFields.SetField("AppSuppTotal", "2000");
            pdfFormFields.SetField("ContractTotal", "3000");
            pdfFormFields.SetField("Price", "10000");
            pdfFormFields.SetField("Instrument Price", "10000");

            // flatten the form to remove editting options, set it to false
            // to leave the form open to subsequent manual edits
            pdfStamper.FormFlattening = false;
            
            // close the pdf
            pdfStamper.Close();
        }

        private void clearQuoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!quoteSaved)
            {
                if (MessageBox.Show("This quote has not been saved or you have made changes!\r\n\r\nDo you want to clear these items?", "Clear List", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
            }
            QuoteDataGridView.Rows.Clear();
            QuoteItemsPriceTextBox.Text = "";
            OptionsDataGridView.Rows.Clear();
            OptionsItemsPriceTextBox.Text = "";
        }

        private void AccountComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            short currentAccountID = Convert.ToInt16(AccountComboBox.SelectedValue);
            this.contactsTableAdapter.FillByAccountID(this.customersDataSet.Contacts, currentAccountID);
        }

        private void getNewDatabseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            folderBrowserDialog1.Description = "Please select your New Database folder location.";
            folderBrowserDialog1.SelectedPath = profile.DistributionFolder;
            folderBrowserDialog1.ShowNewFolderButton = false;

            Boolean dBUpdated = false;

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                dBUpdated = copyDatabaseToWorkingFolder(folderBrowserDialog1.SelectedPath);
                if (dBUpdated)
                {
                    MessageBox.Show("Database Updated!");
                }
                else
                {
                    MessageBox.Show("Database Updated Failed!");
                }
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
                        SumItems(QuoteDataGridView);
                    }
                    else
                    {
                        OptionsDataGridView.Rows.Add(row.SAPID, row.Description, Convert.ToDecimal(row.Price), 1, String.Format("{0:P2}", 0.00), Convert.ToDecimal(row.Price));
                        SumItems(OptionsDataGridView);
                    }
                }
                i++;
            }
            RequiredPartsPanel.Visible = false;
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
    }
}
