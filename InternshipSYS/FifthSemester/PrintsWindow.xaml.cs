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

        private static string[] selection = new string[] { "Student supervisor assignment", "Student company assignment",
            "Students who have a company agreement", "Students who have EAAA or none listed as company",
            "Main project overview Secretary", "Companies with contacts" };

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
                //"Student supervisor assignment"
                if (comboBxSelection.SelectedIndex == 0)
                {
                    selectionState = new StudentCompanyAssignmentState(this);
                    SelectionChanged = true;
                    FillGrid();
                    comboBxYear.IsEnabled = true;
                }
                //"Student company assignment"
                else if (comboBxSelection.SelectedIndex == 1)
                {
                    selectionState = new StudentSupervisorCompanyState(this);
                    SelectionChanged = true;
                    FillGrid();
                    comboBxYear.IsEnabled = true;
                }
                //"Students who has a company agreement"
                else if (comboBxSelection.SelectedIndex == 3)
                {
                    selectionState = new StudentsAssignedCompanyState(this);
                    SelectionChanged = true;
                    FillGrid();
                    comboBxYear.IsEnabled = true;
                }
                //"Students who have EAAA or none listed as company"
                else if (comboBxSelection.SelectedIndex == 4)
                {
                    selectionState = new StudentsAtEAAA(this);
                    SelectionChanged = true;
                    FillGrid();
                    comboBxYear.IsEnabled = true;
                }
                //"Main project overview Secretary"
                else if (comboBxSelection.SelectedIndex == 5)
                {
                    selectionState = new MainProjectOverviewSecretaryState(this);
                    SelectionChanged = true;
                    FillGrid();
                    comboBxYear.IsEnabled = true;
                }
                //"Companies with contacts"
                else if (comboBxSelection.SelectedIndex == 6)
                {

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
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            //Document and PdfWriter are from iTextSharp import
            Document doc = new Document(iTextSharp.text.PageSize.A4.Rotate(), 10, 10, 10, 10);

            Font headingFont = new Font(font, 24, Font.NORMAL, BaseColor.RED);
            Font tableheadFont = new Font(font, 12, Font.BOLD);
            Font tablecellFont = new Font(font, 12);

            PdfWriter writer = null;
            try
            {
                writer = PdfWriter.GetInstance(doc, new FileStream(path + @"\PrintableList.pdf", FileMode.Create));

                doc.Open();


                iTextSharp.text.Paragraph paragHeading = new iTextSharp.text.Paragraph("Heading", headingFont);
                doc.Add(paragHeading);
                List<Student> studentList = new List<Student>();
                if (studentList != null)
                {
                    PdfPTable table = new PdfPTable(13);
                    int[] widths = new int[] { 1, 1, 1, 2, 1, 2, 1, 1, 1, 1, 1, 2, 2 };
                    table.SetWidths(widths);

                    //Move headings to list instead?
                    table.AddCell(new PdfPCell(new iTextSharp.text.Paragraph("Name", tableheadFont)));
                    table.AddCell(new PdfPCell(new iTextSharp.text.Paragraph("Email", tableheadFont)));
                    table.AddCell(new PdfPCell(new iTextSharp.text.Paragraph("Phone", tableheadFont)));
                    table.AddCell(new PdfPCell(new iTextSharp.text.Paragraph("Application", tableheadFont)));
                    table.AddCell(new PdfPCell(new iTextSharp.text.Paragraph("Contract", tableheadFont)));
                    table.AddCell(new PdfPCell(new iTextSharp.text.Paragraph("Learning Objectives", tableheadFont)));
                    table.AddCell(new PdfPCell(new iTextSharp.text.Paragraph("Address", tableheadFont)));
                    table.AddCell(new PdfPCell(new iTextSharp.text.Paragraph("Zipcode", tableheadFont)));
                    table.AddCell(new PdfPCell(new iTextSharp.text.Paragraph("Class", tableheadFont)));
                    table.AddCell(new PdfPCell(new iTextSharp.text.Paragraph("Year", tableheadFont)));
                    table.AddCell(new PdfPCell(new iTextSharp.text.Paragraph("Season", tableheadFont)));
                    table.AddCell(new PdfPCell(new iTextSharp.text.Paragraph("Company", tableheadFont)));
                    table.AddCell(new PdfPCell(new iTextSharp.text.Paragraph("Supervisor", tableheadFont)));

                    foreach (Student s in studentList)
                    {
                        //iTextSharp.text.Paragraph p = new iTextSharp.text.Paragraph(studentStringRepresentation(s) + "\n");
                        //doc.Add(p);
                        table.AddCell(new PdfPCell(new iTextSharp.text.Paragraph(s.name, tablecellFont)));
                        table.AddCell(new PdfPCell(new iTextSharp.text.Paragraph(s.email, tablecellFont)));
                        table.AddCell(new PdfPCell(new iTextSharp.text.Paragraph(s.phone, tablecellFont)));
                        table.AddCell(new PdfPCell(new iTextSharp.text.Paragraph(s.application.ToString(), tablecellFont)));
                        table.AddCell(new PdfPCell(new iTextSharp.text.Paragraph(s.contract.ToString(), tablecellFont)));
                        table.AddCell(new PdfPCell(new iTextSharp.text.Paragraph(s.leaningobjectives.ToString(), tablecellFont)));
                        table.AddCell(new PdfPCell(new iTextSharp.text.Paragraph(s.address, tablecellFont)));
                        table.AddCell(new PdfPCell(new iTextSharp.text.Paragraph(s.zipcode + "", tablecellFont)));
                        table.AddCell(new PdfPCell(new iTextSharp.text.Paragraph(s.@class, tablecellFont)));
                        table.AddCell(new PdfPCell(new iTextSharp.text.Paragraph(s.year + "", tablecellFont)));
                        table.AddCell(new PdfPCell(new iTextSharp.text.Paragraph(s.season, tablecellFont)));
                        table.AddCell(new PdfPCell(new iTextSharp.text.Paragraph(s.Company.name, tablecellFont)));
                        table.AddCell(new PdfPCell(new iTextSharp.text.Paragraph(s.Supervisor.name, tablecellFont)));
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
    }
}
 