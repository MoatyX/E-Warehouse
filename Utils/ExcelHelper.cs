using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using System.Windows;
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

            string filename = fileDialog.FileName;
            Extensions.PrintColoredLine(ConsoleColor.Blue, $"file name: {filename}");

            using (var reader = ExcelReaderFactory.CreateReader(fileDialog.OpenFile()))
            {

                Console.WriteLine(reader.ResultsCount);
                Console.WriteLine(reader.FieldCount);
                Console.WriteLine(reader.RowCount);

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    while (reader.Read())
                    {
                        if (reader.IsDBNull(i)) continue;
                        Console.WriteLine(reader.GetFieldType(i));
                    }

                    if (i != reader.FieldCount - 1)
                        reader.Reset();
                }

            }
        }
    }
}
