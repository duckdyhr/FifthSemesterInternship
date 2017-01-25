using Microsoft.Win32;
using SERVICE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using FifthSemester.StatePattern;

namespace FifthSemester
{
    /// <summary>
    /// Interaction logic for PrintsWindow.xaml
    /// </summary>
    public partial class PrintsWindow : Window
    {
        private Service service;

        //Base for the pdf document's fonts
        private static readonly BaseFont font = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);

        //private Dictionary<string, string> currentGridColumns;
        public Dictionary<string, string> currentGridColumns { get; set; }

        public bool SelectionChanged { get; set; }

        private SelectionState selectionState;

        private static string[] selection = new string[] { "Student supervisor assignment", "Student company assignment",
            "Students who have a company agreement", "Students who have EAAA listed as company",
            "Main project overview Secretary", "Companies" };

        //All files created are saved to Desktop
        private static readonly string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        public PrintsWindow()
        {
            service = Service.GetInstance;
            InitializeComponent();
            selectionState = new StudentSupervisorCompanyState(this);
            fillCombobxYear();
            comboBxSelection.ItemsSource = selection;
        }

        public Service GetService()
        {
            return service;
        }

        private void fillCombobxYear()
        {
            comboBxYear.ItemsSource = service.getYearList();
        }

        public void EnableComboBxSeason()
        {
            comboBxSeason.IsEnabled = true;
            comboBxSeason.Text = "Choose Season";
        }
        private void comboBxYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBxYear.SelectedItem != null)
            {
                int year = Int32.Parse(comboBxYear.SelectedItem.ToString());
                ComboBoxItem cmbSeason = comboBxSeason.SelectedItem as ComboBoxItem;
                string season = "";
                if (cmbSeason != null)
                {
                    season = season = (comboBxSeason.SelectedItem as ComboBoxItem).Content.ToString();
                }
                selectionState.YearChanged(year, season);
                //EnableComboBxSeason();
            }
        }

        private void comboBxSeason_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBxSeason.SelectedItem != null)
            {
                int year = Int32.Parse(comboBxYear.SelectedItem.ToString());
                string season = (comboBxSeason.SelectedItem as ComboBoxItem).Content.ToString();
                selectionState.SeasonChanged(year, season);
            }
        }
        
        private void comboBxSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            comboBxSeason.IsEnabled = false;
            if (comboBxSelection.SelectedItem != null)
            {
                //"Student supervisor Company "
                if (comboBxSelection.SelectedIndex == 0)
                {
                    selectionState = new StudentSupervisorCompanyState(this);
                    SelectionChanged = true;
                    FillGrid();
                    comboBxYear.IsEnabled = true;
                    if(comboBxYear.SelectedItem != null)
                    {
                        EnableComboBxSeason();
                    }
                }
                //"Student company assignment"
                else if (comboBxSelection.SelectedIndex == 1)
                {
                    selectionState = new StudentCompanyAssignmentState(this);
                    SelectionChanged = true;
                    FillGrid();
                    comboBxYear.IsEnabled = true;
                    if (comboBxYear.SelectedItem != null)
                    {
                        EnableComboBxSeason();
                    }
                }
                //"Students who has a company agreement"
                else if (comboBxSelection.SelectedIndex == 2)
                {
                    selectionState = new StudentsAssignedCompanyState(this);
                    SelectionChanged = true;
                    FillGrid();
                    comboBxYear.IsEnabled = true;
                }
                //"Students who have EAAA listed as company"
                else if (comboBxSelection.SelectedIndex == 3)
                {
                    selectionState = new StudentsAtEAAA(this);
                    SelectionChanged = true;
                    FillGrid();
                    comboBxYear.IsEnabled = true;
                }
                //"Main project overview Secretary"
                else if (comboBxSelection.SelectedIndex == 4)
                {
                    selectionState = new MainProjectOverviewSecretaryState(this);
                    SelectionChanged = true;
                    FillGrid();
                    comboBxYear.IsEnabled = true;
                    if (comboBxYear.SelectedItem != null)
                    {
                        EnableComboBxSeason();
                    }
                }
                //"Companies with contacts"
                else if (comboBxSelection.SelectedIndex == 5)
                {
                    selectionState = new CompaniesWithContactState(this);
                    SelectionChanged = true;
                    FillGrid();
                    comboBxYear.IsEnabled = false;
                }
            }
        }

        public void GenerateDataGrid()
        {
            datagrid.Columns.Clear();
            DataGridTextColumn textColumn;
            foreach (KeyValuePair<string, string> column in selectionState.GetColumns())
            {
                textColumn = new DataGridTextColumn();
                textColumn.Header = column.Key;
                textColumn.Binding = new Binding(column.Value);
                datagrid.Columns.Add(textColumn);
            }
            SelectionChanged = false;
        }

        private void FillGrid()
        {
            if (SelectionChanged)
            {
                GenerateDataGrid();
            }
            //if year is not selected, current year is default.
            int year;
            if (comboBxYear.SelectedIndex < 0)
            {
                year = DateTime.Now.Year;
                //Set current year as selected in combobox as well
            }
            else
            {
                year = Int32.Parse(comboBxYear.SelectedItem.ToString());
            }
            ComboBoxItem cmbSeason = comboBxSeason.SelectedItem as ComboBoxItem;
            string season = "";
            if (cmbSeason != null)
            {
                season = season = (comboBxSeason.SelectedItem as ComboBoxItem).Content.ToString();
            }
            selectionState.SetItemsSource(year, season);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            service.isPrintsWindowActive = false;
        }

        private void btnSaveAsPdf_Click(object sender, RoutedEventArgs e)
        {
            //Document and PdfWriter are from iTextSharp import
            Document doc = new Document();

            Font headingFont = new Font(font, 26, Font.NORMAL);
            Font tableheadFont = new Font(font, 12, Font.BOLD);
            Font tablecellFont = new Font(font, 12, Font.NORMAL);

            PdfWriter writer = null;
            try
            {
                writer = PdfWriter.GetInstance(doc, new FileStream(path + @"\PrintableList.pdf", FileMode.Create));

                doc.Open();

                iTextSharp.text.Paragraph heading = new iTextSharp.text.Paragraph("Printable List", headingFont);
                
                heading.Alignment = Element.ALIGN_CENTER;
                heading.SpacingAfter = 10;
                doc.Add(heading);
                
                PdfPTable table = new PdfPTable(selectionState.GetColumns().Count);
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                table.WidthPercentage = 100;

                var headers = selectionState.GetColumns().Keys;
                var properties = selectionState.GetColumns().Values;

                foreach (var header in headers){
                    Phrase phrase = new Phrase(header, tableheadFont);
                    table.AddCell(new PdfPCell(phrase));
                }

                foreach (var row in datagrid.Items)
                {
                    foreach (var prop in properties)
                    {
                        Chunk chunk = new Chunk(row.GetType()?.GetProperty(prop)?.GetValue(row, null)?.ToString(), tablecellFont);
                        Phrase phrase = new Phrase(chunk);
                        PdfPCell cell = new PdfPCell(phrase);
                        cell.Padding = 3;
                        cell.Border = PdfPCell.LISTITEM;
                        table.AddCell(cell);
                    }
                }
                doc.Add(table);
                lblTest.Content = "pdf file saved to desktop";
            }
            catch (DocumentException de)
            {
                lblTest.Content = "Failed to save";
                throw de;
            }
            catch (IOException ie)
            {
                lblTest.Content = "Failed to save";
                throw ie;
            }
            finally
            {
                doc.Close();
                writer?.Close();
            }
        }
        
        private StringBuilder GenerateCSV()
        {
            var columns = selectionState.GetColumns();
            var headers = columns.Keys;
            var properties = columns.Values;

            StringBuilder data = new StringBuilder();
            data.AppendLine(string.Join(";", headers));
            foreach (var row in datagrid.Items)
            {
                foreach(var prop in properties)
                {
                    var value = row.GetType()?.GetProperty(prop)?.GetValue(row, null);
                    data.Append(value);
                    data.Append(";");
                }
                data.AppendLine();
            }
            return data;
        }

        private void btnSaveAsCSV_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder result = GenerateCSV();
            StreamWriter outputFile = new StreamWriter(path + @"\Output.csv", false, Encoding.Default);
            outputFile.WriteLine(result);
            outputFile.Close();
            lblTest.Content = "csv file saved to desktop";
        }
    }
}