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

namespace FifthSemester
{
    /// <summary>
    /// Interaction logic for PrintsWindow.xaml
    /// </summary>
    public partial class PrintsWindow : Window
    {
        private Service service;
        private List<Student> studentList;
        private List<Season> seasonList;

        //Base for the document's fonts
        private static readonly BaseFont font = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);

        private Dictionary<string, string> currentGridColumns;
        private bool selectionChanged = false;

        private static readonly Dictionary<string, string> studentSupervisorColums = new Dictionary<string, string>() {
            {"Name", "name" },
            {"Class", "class" },
            {"Supervisor", "Supervisor.name" },
            {"Email", "email" },
            {"Company", "Company.name" },
            {"Comments", "comments" }
        };
        private static readonly Dictionary<string, string> studentCompanyColumns = new Dictionary<string, string>()
        {
            {"Name", "name" },
            {"Class", "class"},
            {"Company", "Company.name" },
            {"Comments", "comments" }
        };

        private static readonly Dictionary<string, string> studentsPrSeasonColumns = new Dictionary<string, string>()
        {
            {"Year", "" },
            {"Season", "" },
            {"Number", "" }
        };

        public PrintsWindow()
        {
            service = Service.GetInstance;
            InitializeComponent();
            fillCombobxYear();
        }

        private void fillCombobxYear()
        {
            comboBxYear.ItemsSource = service.getYearList();
        }

        private void comboBxYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (comboBxYear.SelectedItem != null)
            {
                FillGrid();
                comboBxSeason.IsEnabled = true;
                comboBxSeason.Text = "Choose Season";
            }
        }

        private void comboBxSeason_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBxSeason.SelectedItem != null)
            {
                FillGrid();
            }
        }

        //Use index instead of string comparison to get which selection is selected?
        private void comboBxSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBxSelection.SelectedItem != null)
            {
                ComboBoxItem selection = comboBxSelection.SelectedItem as ComboBoxItem;
                if (selection.Content.ToString().Equals("Student company assignment"))
                {
                    currentGridColumns = studentCompanyColumns;
                    selectionChanged = true;
                    FillGrid();
                    comboBxYear.IsEnabled = true;
                }
                else if (selection.Content.ToString().Equals("Student supervisor assignment"))
                {
                    currentGridColumns = studentSupervisorColums;
                    selectionChanged = true;
                    FillGrid();
                    comboBxYear.IsEnabled = true;
                }
                else if (selection.Content.ToString().Equals("Students who has a company agreement"))
                {
                    currentGridColumns = studentsPrSeasonColumns;
                    selectionChanged = true;
                    FillGrid();
                    comboBxYear.IsEnabled = true;
                }
                else if (selection.Content.ToString().Equals(""))
                {

                }
                else if (selection.Content.ToString().Equals(""))
                {

                }
                else if (selection.Content.ToString().Equals(""))
                {

                }

            }
        }

        private void GenerateDataGrid()
        {
            datagrid.Columns.Clear();
            DataGridTextColumn textColumn;
            foreach (KeyValuePair<string, string> column in currentGridColumns)
            {
                textColumn = new DataGridTextColumn();
                textColumn.Header = column.Key;
                textColumn.Binding = new Binding(column.Value);
                datagrid.Columns.Add(textColumn);
            }
            selectionChanged = false;
        }

        private void FillSeasonGrid()
        {
            if (selectionChanged)
            {
                GenerateDataGrid();
            }
            List<Student> temp = new List<Student>();
            int year = 2016;
            temp = service.getStudentsByYear(year);
            temp.Select(s => !s.Company.Equals("Erhvervsakademi Aarhus"));

            
            var s1 = new { year = 2016, season = "Autumn", number = 3};

            datagrid.ItemsSource = seasonList;
        }
        private void FillGrid()
        {
            if (selectionChanged)
            {
                GenerateDataGrid();
            }
            //if year is not selected, current year is default.
            int year;
            if (comboBxYear.SelectedIndex < 0)
            {
                year = DateTime.Now.Year;
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

            studentList = service.getStudentsByYear(year);

            List<Student> tempList = new List<Student>();
            if (season.Length > 0)
            {
                foreach (Student s in studentList)
                {
                    if (s.season.Equals(season))
                    {
                        tempList.Add(s);
                    }
                }
                datagrid.ItemsSource = tempList;
            }
            else
            {
                datagrid.ItemsSource = studentList;

            }
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
    class Season
    {
        public int Year { get; set; }
        public string SeasonName { get; set; }
        public int Number { get; set; }
    }
}