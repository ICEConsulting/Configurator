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
using System.Globalization;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.xml;

namespace Tecan_Quote_Generator
{
    public partial class MainQuoteForm : Form
    {
        SqlCeConnection TecanDatabase = null;

        Boolean searchPreformed = true;
        Boolean salesTypeChanged = false;
        Boolean instrumentChanged = false;
        Boolean categoryChanged = false;
        Boolean subCategoryChanged = false;
        Boolean formatOnly = false;

        PartsListDetailDisplay DetailsForm;
        Profile profile = new Profile();

        public MainQuoteForm()
        {
            InitializeComponent();
        }

        public void MainQuoteForm_Load(object sender, EventArgs e)
        {
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
                int newGridHeight;
                newGridHeight = Screen.PrimaryScreen.Bounds.Height - (this.menuStrip1.Height + this.partsListBindingNavigator.Height);
                this.partsListDataGridView.Height = newGridHeight - 500;
                setPartDetailTextBox();
                QuoteDataGridView.AllowDrop = true;
                OptionsDataGridView.AllowDrop = true;
                ThirdPartyDataGridView.AllowDrop = true;
                SmartStartDataGridView.AllowDrop = true;
                QuoteTabControl.SelectedTab = QuoteSettingTabPage;
            }
        }

        // Called from Form Shown event, Only processed if there is no current database
        // Reads or Creates this users profile xml file.
        private void getProfileAndDatabase(object sender, EventArgs e)
        {
            String profileFile = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "TecanConfig.cfg");

            // If = 1 then no DB, requires intilization
            if (partsListBindingSource.Count == 1)
            {

                if (MessageBox.Show("The Tecan Quote Generator must be intilized!\r\n\r\nDo you want to perform intialization now?", "Initial Installation", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    this.Close();
                }
                else
                {
                    if (!File.Exists(profileFile))
                    {
                        ProfileForm profileForm = new ProfileForm(true);
                        profileForm.SetForm1Instance(this);
                        profileForm.Show();
                        Application.OpenForms["ProfileForm"].BringToFront();
                    }
                    else
                    {
                        String distributionFolder;
                        getUsersProfile();
                        distributionFolder = profile.DistributionFolder;

                        if (distributionFolder == null)
                        {
                            ProfileForm profileForm = new ProfileForm(true);
                            profileForm.SetForm1Instance(this);
                            profileForm.Show();
                            Application.OpenForms["ProfileForm"].BringToFront();
                            MessageBox.Show("There's a problem with your profile settings.  Please re-enter and save your information!");
                        }

                        Boolean fileFound;
                        fileFound = copyDatabaseToWorkingFolder(distributionFolder);
                        if (!fileFound)
                        {
                            MessageBox.Show("The Distribution Folder you selected in your profile does not contain the Parts List Database!\n\nPlease select a new folder");
                            ProfileForm profileForm = new ProfileForm(true);
                            profileForm.SetForm1Instance(this);
                            profileForm.Show();
                            Application.OpenForms["ProfileForm"].BringToFront();
                        }
                        else
                        {
                            MainQuoteForm_Load(sender, e);
                        }
                    }
                }
            }
            else
            {
                if (!File.Exists(profileFile))
                {
                    ProfileForm profileForm = new ProfileForm(false);
                    profileForm.SetForm1Instance(this);
                    profileForm.Show();
                    Application.OpenForms["ProfileForm"].BringToFront();
                }
                else
                {
                    getUsersProfile();
                }
            }
        }

        // If blank database or new database available copy new database to working folder
        public Boolean copyDatabaseToWorkingFolder(String sourcePath)
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

            FileInfo fi = new FileInfo(quoteSourceFile);
            profile.DatabaseCreationDate = fi.CreationTime;
            saveUsersProfile();
            System.IO.File.Copy(quoteSourceFile, quoteTargetFile, true);
            System.IO.File.Copy(supplementSourceFile, supplementTargetFile, true);
            return true;
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
            }
            SumItems(QuoteDataGridView);
        }

        private void SumItems(DataGridView myDataGridView)
        {
            Int32 rowCount = myDataGridView.Rows.GetRowCount(DataGridViewElementStates.Displayed);
            Int32 rowIndex;
            Decimal itemPrice = 0;

            for (int s = 0; s < rowCount; s++)
            {
                rowIndex = myDataGridView.Rows[s].Index;
                DataGridViewRow srow = myDataGridView.Rows[rowIndex];
                itemPrice = itemPrice + (Decimal)srow.Cells[5].Value;
            }

            switch (myDataGridView.Name)
            {
                case "QuoteDataGridView":
                    QuoteItemsPriceTextBox.Text = String.Format("{0:C2}", itemPrice); // getFormatedDollarValue(itemPrice.ToString());
                    break;

                case "OptionsDataGridView":
                    OptionsItemsPriceTextBox.Text = String.Format("{0:C2}", itemPrice); //getFormatedDollarValue(itemPrice.ToString());
                    break;
            }
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
            Decimal itemPrice;
            Int32 itemQty;
            Decimal itemDiscount;
            Decimal discountPercentage;
            Decimal extendedPrice;
            String itemPriceCheck;
            String discountCheck;

            itemPriceCheck = myDataGridView.Rows[e.RowIndex].Cells[2].Value.ToString();
            discountCheck = myDataGridView.Rows[e.RowIndex].Cells[4].Value.ToString();

            if ((itemPriceCheck.IndexOf("$") == -1) || (discountCheck.IndexOf("%") == -1))
            {
                formatOnly = false;
            }

            if (formatOnly == false && (e.ColumnIndex == 2 || e.ColumnIndex == 3 || e.ColumnIndex == 4))
            {
                if (itemPriceCheck.IndexOf("$") != -1) itemPriceCheck = itemPriceCheck.Substring(1, itemPriceCheck.Length - 1);
                itemPrice = Convert.ToDecimal(itemPriceCheck);

                if (discountCheck.IndexOf("%") != -1) discountCheck = discountCheck.Substring(0, discountCheck.Length - 2);
                discountPercentage = Convert.ToDecimal(discountCheck);

                MessageBox.Show(myDataGridView.Rows[e.RowIndex].Cells[3].Value.ToString());
                itemQty = Convert.ToInt32(myDataGridView.Rows[e.RowIndex].Cells[3].Value);
                extendedPrice = itemPrice * itemQty;
                if (discountPercentage == 0)
                {
                    myDataGridView.Rows[e.RowIndex].Cells[5].Value = extendedPrice;
                }
                else
                {
                    itemDiscount = extendedPrice * (discountPercentage / 100);
                    myDataGridView.Rows[e.RowIndex].Cells[5].Value = extendedPrice - itemDiscount;
                }
                SumItems(myDataGridView);
            }

            switch (e.ColumnIndex)
            {
                // Price
                case 2:
                    if (itemPriceCheck.IndexOf("$") == -1)
                    {
                        formatOnly = true;
                        itemPrice = Convert.ToDecimal(itemPriceCheck);
                        QuoteDataGridView.Rows[e.RowIndex].Cells[2].Value = String.Format("{0:C2}", itemPrice);
                    }
                    break;

                // Discount
                case 4:
                    if (discountCheck.IndexOf("%") == -1)
                    {
                        formatOnly = true;
                        discountPercentage = Convert.ToDecimal(discountCheck);
                        QuoteDataGridView.Rows[e.RowIndex].Cells[4].Value = String.Format("{0:P2}", discountPercentage / 100);
                    }
                    break;
            }

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
            String itemSAPID;
            String itemDescription;
            Decimal itemPrice;

            DataGridViewSelectedRowCollection rows = (DataGridViewSelectedRowCollection)e.Data.GetData(typeof(DataGridViewSelectedRowCollection));
            foreach (DataGridViewRow row in rows)
            {
                itemSAPID = row.Cells[0].Value.ToString();
                itemDescription = row.Cells[1].Value.ToString();
                itemPrice = getPartPrice(itemSAPID);
                OptionsDataGridView.Rows.Add(itemSAPID, itemDescription, itemPrice, 1, String.Format("{0:P2}", 0.00), itemPrice);
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

        private void ThirdPartyDataGridView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DataGridViewSelectedRowCollection)))
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        // The drop into the desired object
        private void ThirdPartyDataGridView_DragDrop(object sender, DragEventArgs e)
        {
            String itemSAPID;
            String itemDescription;
            Decimal itemPrice;

            DataGridViewSelectedRowCollection rows = (DataGridViewSelectedRowCollection)e.Data.GetData(typeof(DataGridViewSelectedRowCollection));
            foreach (DataGridViewRow row in rows)
            {
                itemSAPID = row.Cells[0].Value.ToString();
                itemDescription = row.Cells[1].Value.ToString();
                itemPrice = getPartPrice(itemSAPID);
                ThirdPartyDataGridView.Rows.Add(itemSAPID, itemDescription, itemPrice, 1, String.Format("{0:P2}", 0.00), itemPrice);
            }
            SumItems(OptionsDataGridView);
        }

        // Update Extended Price and Totals when QTY and/or Discount Change
        private void ThirdPartyDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                processCellValueChange(ThirdPartyDataGridView, e);
            }

        }

        private void SmartStartDataGridView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DataGridViewSelectedRowCollection)))
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        // The drop into the desired object
        private void SmartStartDataGridView_DragDrop(object sender, DragEventArgs e)
        {
            String itemSAPID;
            String itemDescription;
            Decimal itemPrice;

            DataGridViewSelectedRowCollection rows = (DataGridViewSelectedRowCollection)e.Data.GetData(typeof(DataGridViewSelectedRowCollection));
            foreach (DataGridViewRow row in rows)
            {
                itemSAPID = row.Cells[0].Value.ToString();
                itemDescription = row.Cells[1].Value.ToString();
                itemPrice = getPartPrice(itemSAPID);
                SmartStartDataGridView.Rows.Add(itemSAPID, itemDescription, itemPrice, 1, String.Format("{0:P2}", 0.00), itemPrice);
            }
        }

        // Update Extended Price and Totals when QTY and/or Discount Change
        private void SmartStartDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                processCellValueChange(SmartStartDataGridView, e);
            }

        }

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
                    partImagePictureBox.Image = null;
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
            subCategoryChanged = false;
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
            subCategoryChanged = false;
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
            subCategoryChanged = true;
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

        private void ThirdPartyRemoveSelectedButton_Click(object sender, EventArgs e)
        {
            RemoveItems(ThirdPartyDataGridView);
            // SumItems(ThirdPartyDataGridView);
        }

        private void SmartStartRemoveSelectButton_Click(object sender, EventArgs e)
        {
            RemoveItems(SmartStartDataGridView);
            // SumItems(SmartStartDataGridView);
        }

        private void RemoveItems(DataGridView myDataGridView)
        {
            Int32 selectedRowCount = myDataGridView.Rows.GetRowCount(DataGridViewElementStates.Selected);
            Int32 removedRowCount = 0;
            Int32 totalRowCount = myDataGridView.RowCount;
            if (selectedRowCount > 0)
            {
                if (selectedRowCount == totalRowCount)
                {
                    MessageBox.Show("All cells are selected", "Selected Cells");
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
            String QuoteFileName = QuoteTitleTextBox.Text + ".tbq";
            Quote quote = new Quote();
            quote.QuoteTitle = QuoteTitleTextBox.Text;
            quote.Items = AddQuoteItems(QuoteDataGridView);
            quote.Options = AddQuoteItems(OptionsDataGridView);
            quote.ThirdParty = AddQuoteItems(ThirdPartyDataGridView);
            quote.SmartStart = AddQuoteItems(SmartStartDataGridView);

            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(Quote));

            System.IO.StreamWriter file = new System.IO.StreamWriter(@"c:\temp\" + QuoteFileName);
            writer.Serialize(file, quote);
            file.Close();
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
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(Quote));
            System.IO.StreamReader file = new System.IO.StreamReader(@"c:\temp\MyFirstQuote.tbq");
            Quote quote = new Quote();
            quote = (Quote)reader.Deserialize(file);

            String itemSAPID;
            String itemDescription;
            Decimal itemPrice;
            Int32 itemQuantity;
            Decimal itemDiscount;

            foreach (QuoteItems row in quote.Items)
            {
                itemSAPID = row.SAPID;
                itemDescription = row.Description;
                itemPrice = row.Price;
                itemQuantity = row.Quantity;
                itemDiscount = row.Discount;

                QuoteDataGridView.Rows.Add(itemSAPID, itemDescription, itemPrice, itemQuantity, String.Format("{0:P2}", itemDiscount), itemPrice);
            }
            SumItems(QuoteDataGridView);


        }

        private void myProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProfileForm profileForm = new ProfileForm(false);
            profileForm.SetForm1Instance(this);
            profileForm.Show();
            Application.OpenForms["ProfileForm"].BringToFront();
        }

        public void getUsersProfile()
        {
            String profileFile = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "TecanConfig.cfg");
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(Profile));
            System.IO.StreamReader file = new System.IO.StreamReader(profileFile);
            // Profile profile = new Profile();
            profile = (Profile)reader.Deserialize(file);
            file.Close();
            SalemansNameLabel.Text = "Welcome " + profile.Name;
        }

        private void saveUsersProfile()
        {
            // Save to Profile config file
            String profileFile = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "TecanConfig.cfg");
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
            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(
                newFile, FileMode.Create));
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

    }
}
