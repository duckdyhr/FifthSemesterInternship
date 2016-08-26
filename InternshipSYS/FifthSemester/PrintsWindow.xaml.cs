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

namespace FifthSemester
{
    /// <summary>
    /// Interaction logic for PrintsWindow.xaml
    /// </summary>
    public partial class PrintsWindow : Window
    {
        private Service service;
        private List<Student> studentList;

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

    }
}