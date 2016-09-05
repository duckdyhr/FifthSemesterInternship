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

        //Base for the document's fonts
        private static readonly BaseFont font = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);


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
                studentList = service.getStudentsByYear(Int32.Parse(comboBxYear.SelectedItem.ToString()));
                DGStudents.ItemsSource = studentList;
                //listToGroupBy = studentList;
                comboBxSeason.IsEnabled = true;
                comboBxSeason.Text = "Choose Season";
            }
        }
        private void fillGrid()
        {
            studentList = service.getStudentsByYear(Int32.Parse(comboBxYear.SelectedItem.ToString()));
            if (comboBxSeason.SelectedItem != null)
            {
                List<Student> tempList = new List<Student>();
                ComboBoxItem season = comboBxSeason.SelectedItem as ComboBoxItem;
                foreach (Student s in studentList)
                {
                    if (s.season.Equals(season.Content.ToString()))
                    {
                        tempList.Add(s);
                    }
                }
                studentList = tempList;
            }
            //listToGroupBy = studentList;
            DGStudents.ItemsSource = studentList;
        }

        private void comboBxSeason_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBxSeason.SelectedItem != null)
            {
                List<Student> tempList = new List<Student>();
                ComboBoxItem season = comboBxSeason.SelectedItem as ComboBoxItem;
                foreach (Student s in studentList)
                {
                    if (s.season.Equals(season.Content.ToString()))
                    {
                        tempList.Add(s);
                    }
                }
                //listToGroupBy = tempList;
                DGStudents.ItemsSource = tempList;
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
            Document doc = new Document(iTextSharp.text.PageSize.A4, 10, 10, 10, 10);
            PdfWriter writer = null;
            try
            {
                writer = PdfWriter.GetInstance(doc, new FileStream(path + @"\PrintableList.pdf", FileMode.Create));

                doc.Open();

                Font headingFont = new Font(font, 24, Font.NORMAL, BaseColor.RED);
                iTextSharp.text.Paragraph pHeading = new iTextSharp.text.Paragraph("Heading", headingFont);
                doc.Add(pHeading);

                if (studentList != null)
                {
                    PdfPTable table = new PdfPTable(3);

                    foreach (Student s in studentList)
                    {
                        //iTextSharp.text.Paragraph p = new iTextSharp.text.Paragraph(studentStringRepresentation(s) + "\n");
                        //doc.Add(p);
                        table.AddCell(s.id + "");
                        table.AddCell(s.name);
                        table.AddCell(s.Company.name);
                    }
                    doc.Add(table);
                }

                lblTest.Content = "Saving to .pdf";
            }
            catch (DocumentException de)
            {
                lblTest.Content = "Failed to save";
                throw de;
            }
            catch(IOException ie)
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
    }
}