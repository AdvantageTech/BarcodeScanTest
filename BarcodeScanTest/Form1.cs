using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BarcodeScanTest
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            //Get barcode value from textbox
            string barcode = txtBarcode.Text;
            string folder = txtDataFolder.Text;
            if (folder == null || folder == "")
            {
                MessageBox.Show("Please select the data folder!");
            }
            else
            {
                //Find the filepath for the barcode datafile matching the barcode format
                string file = FindBarcodeTypeFile(barcode, folder);

                //If the filepath is found, begin search
                if(file != null && file != "")
                {
                    //Load data and search for match
                    LoadCSVOnDataGridViewAndFilter(file, dgvResults, barcode);
                }
                else
                {
                    MessageBox.Show("Invalid barcode format!");
                }

                //Re-select the barcode so it is faster to scan multiples
                txtBarcode.SelectionStart = 0;
                txtBarcode.SelectionLength = txtBarcode.Text.Length;
            }
        }

        private void LoadCSVOnDataGridViewAndFilter(string filePath, DataGridView dataGridView, string searchCriteria = "")
        {
            //Load the data from CSV in filePath and set as datasource for datagridview
            try
            {
                ReadCSVAndFilter csv = new ReadCSVAndFilter(filePath, true, searchCriteria);

                try
                {
                    dataGridView.DataSource = csv.dt;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string FindBarcodeTypeFile(string barcode, string dataFolderPath)
        {
            //Get the format of the scanned barcode
            string barcodeTypeFileLocation = "";
            string barcodeFormat = "";
            string[] barcodeElements = barcode.Split(' ');

            //Separate the elements of the barcode by whitespace, get the counts of each element, record these (hyphen between counts) 
            foreach (var barcodeElement in barcodeElements)
            {
                barcodeFormat += ("-" + barcodeElement.Length.ToString());
            }

            //Trim leading "-"
            barcodeFormat = barcodeFormat.TrimStart('-');

            switch (barcodeFormat)
            {
                case "8-12":
                    barcodeTypeFileLocation = dataFolderPath + "\\Barcode_Type_8-12.csv";
                    break;
                case "7-11":
                    barcodeTypeFileLocation = dataFolderPath + "\\Barcode_Type_7-11.csv";
                    break;
                case "9":
                    barcodeTypeFileLocation = dataFolderPath + "\\Barcode_Type_9.csv";
                    break;
                case "7":
                    barcodeTypeFileLocation = dataFolderPath + "\\Barcode_Type_7.csv";
                    break;
                case "7-7":
                    barcodeTypeFileLocation = dataFolderPath + "\\Barcode_Type_7-7.csv";
                    break;
                case "11":
                    barcodeTypeFileLocation = dataFolderPath + "\\Barcode_Type_11.csv";
                    break;
                default:
                    break;
            }

            return barcodeTypeFileLocation;
        }

        private void btnSelectDataFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog openFileDialog = new FolderBrowserDialog();

            //openFileDialog.Title = "Select Data Folder";
            //openFileDialog.Filter = "CSV files"
            //openFileDialog.InitialDirectory = @"C:\";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtDataFolder.Text = openFileDialog.SelectedPath.ToString();
                txtBarcode.Focus();
            }
        }
    }
}
