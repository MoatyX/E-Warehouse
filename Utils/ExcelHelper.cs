using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using System.Windows;
using E_Warehouse.Views.Modals;
using ExcelDataReader;

namespace E_Warehouse.Utils
{
    public static class ExcelHelper
    {
        public static void OpenProcessFile()
        {
            var fileDialog = new OpenFileDialog()
            {
                Filter = "Excel Worksheets|*.xls;*.xlsx",
                Multiselect = false,
            };

            var x = fileDialog.ShowDialog(Application.Current.MainWindow);
            if(!x.Value) return;

            ExcelReaderSettings settingsWindow = new ExcelReaderSettings();
            settingsWindow.Owner = Application.Current.MainWindow;
            var result = settingsWindow.ShowDialog();

            if (!result.HasValue || !result.Value) return;      //the user pressed cancel

            string filename = fileDialog.FileName;
            Extensions.PrintColoredLine(ConsoleColor.Blue, $"file name: {filename}");

            using (var reader = ExcelReaderFactory.CreateReader(fileDialog.OpenFile()))
            {
                var partNoCol = settingsWindow.PartNoTextBox.Text;
                var quantityCol = settingsWindow.QuantityTextBox.Text;
                var descCol = settingsWindow.DescriptionTextBox.Text;
                var priceCol = settingsWindow.PriceTextBox.Text;
                var importantColumns = new string[]
                {
                    partNoCol,
                    quantityCol,
                    descCol,
                    priceCol
                };
                //store which column that we care about has which actual column index in the excel sheet
                Dictionary<string, int> columns = new Dictionary<string, int>(4);

                uint r = 0;
                uint maxRows = Convert.ToUInt32(settingsWindow.MaxRowTextBox.Text);

                while (reader.Read() && r < maxRows)
                {
                    r++;

                    //discover the important columns
                    if (columns.Count < 4)
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (reader.IsDBNull(i)) continue;
                            if (reader.GetFieldType(i) == typeof(string))
                            {
                                var value = reader.GetString(i);
                                if (importantColumns.Contains(value, StringComparer.CurrentCultureIgnoreCase))
                                {
                                    columns.Add(value, i);
                                }
                            }
                        }
                    }
                    else
                    {
                        //read columns data
                        if(reader.IsDBNull(columns[partNoCol])) continue;
                        
                        var itemNo = reader.GetString(columns[partNoCol]);
                        var price = reader.GetDouble(columns[priceCol]);
                        var desc = reader.GetString(columns[descCol]);
                        var quat = reader.GetDouble(columns[quantityCol]);
                        Console.WriteLine($"Item: {itemNo} price: {price}, desc: {desc}, quant: {quat}");

                        //TODO: advance to the next sheet
                    }
                    
                }

            }
        }
    }
}
