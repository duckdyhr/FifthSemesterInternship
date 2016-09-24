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
using FifthSemester.StatePatter;

namespace FifthSemester
{
    /// <summary>
    /// Interaction logic for PrintsWindow.xaml
    /// </summary>
    public partial class PrintsWindow : Window
    {
        private Service service;

        //Base for the document's fonts
        private static readonly BaseFont font = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);

        //private Dictionary<string, string> currentGridColumns;
        public Dictionary<string, string> currentGridColumns { get; set; }

        public bool SelectionChanged { get; set; }

        private SelectionState selectionState;

        private static string[] selection = new string[] { "0 Student supervisor assignment", "1 Student company assignment",
            "2 Students who have a company agreement", "3 Students who have EAAA listed as company",
            "4 Main project overview Secretary", "5 Companies with contacts" };

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
                comboBxSeason.IsEnabled = true;
                comboBxSeason.Text = "Choose Season";
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
            if (comboBxSelection.SelectedItem != null)
            {
                //"Student supervisor Company "
                if (comboBxSelection.SelectedIndex == 0)
                {
                    selectionState = new StudentSupervisorCompanyState(this);
                    SelectionChanged = true;
                    FillGrid();
                    comboBxYear.IsEnabled = true;
                }
                //"Student company assignment"
                else if (comboBxSelection.SelectedIndex == 1)
                {
                    selectionState = new StudentCompanyAssignmentState(this);
                    SelectionChanged = true;
                    FillGrid();
                    comboBxYear.IsEnabled = true;
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
                }
                //"Companies with contacts"
                else if (comboBxSelection.SelectedIndex == 5)
                {
                    selectionState = new CompaniesWithContactState(this);
                    SelectionChanged = true;
                    FillGrid();
                    comboBxYear.IsEnabled = true;
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
                //Set current year selected in combobox as well
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
            service.isStudentsWindowActive = false;
        }

        private void btnSaveAsPdf_Click(object sender, RoutedEventArgs e)
        {


            //Document and PdfWriter are from iTextSharp import
            Document doc = new Document(iTextSharp.text.PageSize.A4.Rotate(), 10, 10, 10, 10);

            Font headingFont = new Font(font, 24, Font.NORMAL, BaseColor.RED);
            Font tableheadFont = new Font(font, 12, Font.BOLD);
            Font tablecellFont = new Font(font, 12);

            var columns = selectionState.GetColumns();
            var data = datagrid.Items;

            PdfWriter writer = null;
            try
            {
                writer = PdfWriter.GetInstance(doc, new FileStream(path + @"\PrintableList.pdf", FileMode.Create));

                doc.Open();


                iTextSharp.text.Paragraph paragHeading = new iTextSharp.text.Paragraph("Heading", headingFont);
                doc.Add(paragHeading);
                //List<Student> studentList = new List<Student>();
                if (data != null)
                {
                    PdfPTable table = new PdfPTable(columns.Count);
                    int[] widths = new int[] { 1, 1, 1, 2, 1, 2, 1, 1, 1, 1, 1, 2, 2 };
                    table.SetWidths(widths);
                    foreach(KeyValuePair<string, string> column in columns)
                    {
                        table.AddCell(new PdfPCell(new iTextSharp.text.Paragraph(column.Key, tableheadFont)));
                    }
                    
                    foreach (var row in data)
                    {
                        //iTextSharp.text.Paragraph p = new iTextSharp.text.Paragraph(studentStringRepresentation(s) + "\n");
                        //doc.Add(p);
                        //table.AddCell(new PdfPCell(new iTextSharp.text.Paragraph(s.name, tablecellFont)));
                    }
                    doc.Add(table);
                }
                else
                {
                    //Give user a meaningful response as no students are selected...
                }

                lblTest.Content = "Saving to .pdf";
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

        private string studentStringRepresentation(Student s)
        {
            string result = s.name + " " + s.email + " " + s.phone + " " + s.application + " " + s.contract + " " + s.leaningobjectives + " " + s.address + " " + s.zipcode + " " + s.@class + " " + s.year + " " + s.season + " " + s.Company + " " + s.Supervisor;
            return result;
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            lblTest.Content = "Printing list ...";
            PrintDialog pDialog = new PrintDialog();
            // Display the dialog. This returns true if the user presses the Print button.
            Nullable<Boolean> print = pDialog.ShowDialog();
            //if (print == true)
            //{
            //    XpsDocument xpsDocument = new XpsDocument("C:\\FixedDocumentSequence.xps", FileAccess.ReadWrite);
            //    FixedDocumentSequence fixedDocSeq = xpsDocument.GetFixedDocumentSequence();
            //    pDialog.PrintDocument(fixedDocSeq.DocumentPaginator, "Test print job");
            //}
        }

        //Brug til comma separerert list!!
        private void SaveToTxt()
        {
            //using (StreamWriter outputFile = new StreamWriter(path + @"\TestToTxt.txt"))
            //{
            //    outputFile.WriteLine("Testing to txt..");
            //    if (studentList != null)
            //    {
            //        foreach (Student s in studentList)
            //        {
            //            outputFile.WriteLine(studentStringRepresentation(s));
            //        }
            //    }
            //}
            //lblTest.Content = "Saving to .txt...";
            //Or:
            //SaveFileDialog sfd = new SaveFileDialog();
        }

        private void SaveToCSV()
        {
            var csv = new StringBuilder();

            var columns = datagrid.Columns;
            csv.AppendLine(string.Join(",", columns.Select(column => "\"" + column.Header + "\"").ToArray()));
            
            //foreach (Type row in datagrid.Items)
            //{
            //    var cells = row.GetProperties();
            //    csv.AppendLine(string.Join(",", cells.Select(cell => "\"" + cell.value() + "\"").ToArray()));
            //}

            lblTest.Content = csv;
        }

        static StringBuilder CreateCSV<T>(IEnumerable<T> data)
        {
            StringBuilder builder = new StringBuilder();
            var properties = typeof(T).GetProperties();

            foreach (var prop in properties)
            {
                builder.Append(prop.Name).Append(", ");
            }

            builder.Remove(builder.Length - 2, 2).AppendLine();

            foreach (var row in data)
            {
                foreach (var prop in properties)
                {
                    builder.Append(prop.GetValue(row, null)).Append(", ");
                }

                builder.Remove(builder.Length - 2, 2).AppendLine();
            }

            return builder;
        }

        private void SaveToCSV2()
        {
            DataGrid dg = datagrid;
            dg.SelectAllCells();
            dg.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, dg);
            dg.UnselectAllCells();
            String Clipboardresult = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);

            StreamWriter outputFile = new StreamWriter(path + @"\ToTxt.txt");
            outputFile.WriteLine(Clipboardresult);
            outputFile.Close();
        }

        public void OnExportGridToCSV(object sender, System.EventArgs e)
        {
            //// Create the CSV file to which grid data will be exported.
            //StreamWriter outputFile = new StreamWriter(path + @"\ToTxt.txt");
            //// First we will write the headers.
            //DataGrid dt = datagrid.Columns

            //int iColCount = dt.Columns.Count;
            //for (int i = 0; i < iColCount; i++)
            //{
            //    sw.Write(dt.Columns[i]);
            //    if (i < iColCount - 1)
            //    {
            //        sw.Write(",");
            //    }
            //}
            //sw.Write(sw.NewLine);
            //// Now write all the rows.
            //foreach (DataRow dr in dt.Rows)
            //{
            //    for (int i = 0; i < iColCount; i++)
            //    {
            //        if (!Convert.IsDBNull(dr[i]))
            //        {
            //            sw.Write(dr[i].ToString());
            //        }
            //        if (i < iColCount - 1)
            //        {
            //            sw.Write(System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator);
            //        }
            //    }
            //    sw.Write(sw.NewLine);
            //}
            //outputFile.Close();
        }

        private StringBuilder SaveToCSV3()
        {
            StringBuilder csv = new StringBuilder();
            //Append headers...
            foreach(var row in datagrid.Items)
            {
                var props = row.GetType().GetProperties();
                foreach(var prop in props)
                {
                    var value = prop.GetValue(row, null);
                    csv.Append(value);
                    csv.Append(" ");
                }
                csv.AppendLine();
            }
            return csv;
        }
        private void btnSaveAsCSV_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder result = SaveToCSV3();
            StreamWriter outputFile = new StreamWriter(path + @"\ToTxt.txt");
            outputFile.WriteLine(result);
            outputFile.Close();

            SaveToCSV3();
        }
    }
}
 