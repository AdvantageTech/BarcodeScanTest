using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;

namespace BarcodeScanTest
{
    class ReadCSVAndFilter
    {
        public DataTable dt;

        //Assume firstRowContainsFieldNames = true by default, as is the case with most CSVs
        public ReadCSVAndFilter(string filePath, bool firstRowContainsFieldNames = true, string searchCriteria = "")
        {
            //Generate the data table for the given file
            dt = GenerateDataTable(filePath, firstRowContainsFieldNames, searchCriteria);
        }

        private static DataTable GenerateDataTable(string filePath, bool firstRowContainsFieldNames = true, string searchCriteria = "")
        {
            DataTable result = new DataTable();

            //If the filePath is not provided or the file does not exist, return no data
            if (filePath == null || filePath == "" || File.Exists(filePath) == false)
            {
                MessageBox.Show("Data file could not be found in data folder!");
                return result;
            }

            //Default delimiter is "," as this is the norm in a CSV
            //Check the file extension so we can verify the delimiter to use (only accounts for CSV or TXT files)
            string delimiters = ",";
            string extension = Path.GetExtension(filePath);

            //A TXT file uses tab delimiter, a CSV uses comma delimiter, any other file type is unsuitable for this class
            if (extension.ToLower() == ".txt")
            {
                delimiters = "\t";
            }
            else if (extension.ToLower() == ".csv")
            {
                delimiters = ",";
            }
            else
            {
                return result;
            }

            using (TextFieldParser parser = new TextFieldParser(filePath))
            {
                //Allow us to treat delimiters enclosed in quotes as data
                //Use the appropriate delimiter for the file type
                parser.HasFieldsEnclosedInQuotes = true;
                parser.SetDelimiters(delimiters);

                //If file has data, get field names & data
                if (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();

                    //Get contents of first row
                    for (int i = 0; i < fields.Count(); i++)
                    {
                        //If the first row contains field names, use as columns
                        if (firstRowContainsFieldNames)
                        {
                            result.Columns.Add(fields[i]);
                        }
                        //Else, create default column names
                        else
                        {
                            result.Columns.Add("Field" + i);
                        }
                    }

                    //If first row did not have field names, then add the first row as data
                    if (!firstRowContainsFieldNames)
                    {
                        result.Rows.Add(fields);
                    }

                    //Get remaining content
                    while (!parser.EndOfData)
                    {
                        result.Rows.Add(parser.ReadFields());
                    }
                }
            }

            //If searchCriteria provided, filter by IDs matching this searchCriteria
            if (searchCriteria != null && searchCriteria != "")
            {
                result.DefaultView.RowFilter = string.Format("ID = '{0}'", searchCriteria);
            }
            return result;
        }
    }
}
