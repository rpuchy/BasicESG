using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;
using OfficeOpenXml;

namespace BasicESG
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ReturnGenerator returnGenerator;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Xml Files (.xml)|*.xml|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ReturnGenerator));

                StreamReader reader = new StreamReader(openFileDialog.FileName);
                returnGenerator = (ReturnGenerator)serializer.Deserialize(reader);
                reader.Close();


                var scenarios = 10000;
                var timeSteps = 2;
                var results = returnGenerator.Generate(timeSteps, scenarios);
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel files (.xlsx)|*.xlsx|All Files (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == true)
                {

                    File.Delete(saveFileDialog.FileName);
                    using (ExcelPackage excel = new ExcelPackage(new FileInfo(saveFileDialog.FileName)))
                    {
                        var ws = excel.Workbook.Worksheets.Add("Returns");
                        ws.Cells[1, 1].Value = "Scenario";
                        ws.Cells[1, 2].Value = "TimeStep";
                        for (int i = 0; i < returnGenerator.Mean.Count(); i++)
                        {
                            ws.Cells[1, 3 + i].Value = "Asset " + i;
                        }

                        for (int i = 1; i <= scenarios; i++)
                        {
                            for (int j = 1; j <= timeSteps; j++)
                            {
                                ws.Cells[(i - 1) * timeSteps + j + 1, 1].Value = i;
                                ws.Cells[(i - 1) * timeSteps + j + 1, 2].Value = j;
                                for (int k = 0; k < returnGenerator.Mean.Count(); k++)
                                {
                                    ws.Cells[(i - 1) * timeSteps + j + 1, 3 + k].Value = results[i][j][k];
                                }
                            }
                        }
                        excel.Save();
                    }

                }
            }



        }
    }
}
