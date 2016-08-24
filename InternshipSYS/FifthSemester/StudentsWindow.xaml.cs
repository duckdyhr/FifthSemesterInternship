using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using SERVICE;
using System.Windows.Data;

namespace FifthSemester
{
    /// <summary>
    /// Interaction logic for Students.xaml
    /// </summary>
    public partial class StudentsWindow : Window
    {
        private Service service;
        private Student selectedStudent;

        private List<Student> studentList;
        private List<Student> listToGroupBy;
        private String groupedBy;

        public StudentsWindow()
        {
            service = Service.GetInstance;
            InitializeComponent();
            fillCombobxYear();
            groupedBy = null;
        }

        private void fillCombobxYear()
        {
            List<Int32> ys = new List<int> { 2012, 2013, 2014 };
            //comboBxYear.ItemsSource = ys;
            comboBxYear.ItemsSource = service.getYearList();
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
            listToGroupBy = studentList;
            DGStudents.ItemsSource = studentList;
        }

        private void selectCompanyButton_Click(object sender, RoutedEventArgs e)
        {
            CompanyDialog cd = new CompanyDialog(selectedStudent);
            cd.Owner = this;
            cd.ShowDialog();
            fillGrid();
            DGStudents.SelectedItem = selectedStudent;
        }

        private void selectSupervisorButton_Click(object sender, RoutedEventArgs e)
        {
            SupervisorDialog sd = new SupervisorDialog(selectedStudent);
            sd.Owner = this;
            sd.ShowDialog();
            fillGrid();
            DGStudents.SelectedItem = selectedStudent;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            service.isStudentsWindowActive = false;
        }

        private void DGStudents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedStudent = DGStudents.SelectedItem as Student;
            if (selectedStudent != null)
            {
                txbCommentsStudent.Text = selectedStudent.comments;
                btnSaveComment.IsEnabled = true;
            }
            else
            {
                txbCommentsStudent.Text = null;
                btnSaveComment.IsEnabled = false;
            }
        }

        private void DGStudents_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                FrameworkElement element_Name = DGStudents.Columns[0].GetCellContent(e.Row);
                if (element_Name.GetType() == typeof(TextBox))
                {
                    var name = ((TextBox)element_Name).Text;
                    selectedStudent.name = name;
                }
                FrameworkElement element_Email = DGStudents.Columns[1].GetCellContent(e.Row);
                if (element_Email.GetType() == typeof(TextBox))
                {
                    var email = ((TextBox)element_Email).Text;
                    selectedStudent.email = email;
                }
                FrameworkElement element_Phone = DGStudents.Columns[2].GetCellContent(e.Row);
                if (element_Phone.GetType() == typeof(TextBox))
                {
                    var phone = ((TextBox)element_Phone).Text;
                    selectedStudent.phone = phone;
                }
                FrameworkElement element_App = DGStudents.Columns[3].GetCellContent(e.Row);
                if (element_App.GetType() == typeof(CheckBox))
                {
                    var app = ((CheckBox)element_App).IsChecked;
                    selectedStudent.application = app;
                }
                FrameworkElement element_Con = DGStudents.Columns[4].GetCellContent(e.Row);
                if (element_Con.GetType() == typeof(CheckBox))
                {
                    var con = ((CheckBox)element_Con).IsChecked;
                    selectedStudent.contract = con;
                }
                FrameworkElement element_LearnOb = DGStudents.Columns[5].GetCellContent(e.Row);
                if (element_LearnOb.GetType() == typeof(CheckBox))
                {
                    var lOb = ((CheckBox)element_LearnOb).IsChecked;
                    selectedStudent.leaningobjectives = lOb;
                }
                FrameworkElement element_Address = DGStudents.Columns[6].GetCellContent(e.Row);
                if (element_Address.GetType() == typeof(TextBox))
                {
                    var address = ((TextBox)element_Address).Text;
                    selectedStudent.address = address;
                }
                FrameworkElement element_Zipcode = DGStudents.Columns[7].GetCellContent(e.Row);
                if (element_Zipcode.GetType() == typeof(TextBox))
                {
                    var zipcode = ((TextBox)element_Zipcode).Text;
                    selectedStudent.zipcode = Convert.ToInt32(zipcode);
                }
                FrameworkElement element_Class = DGStudents.Columns[8].GetCellContent(e.Row);
                if (element_Class.GetType() == typeof(TextBox))
                {
                    var @class = ((TextBox)element_Class).Text;
                    selectedStudent.@class = @class;
                }
                FrameworkElement element_Year = DGStudents.Columns[9].GetCellContent(e.Row);
                if (element_Year.GetType() == typeof(TextBox))
                {
                    var year = ((TextBox)element_Year).Text;
                    selectedStudent.year = Convert.ToInt32(year);
                }
                service.updateStudent(selectedStudent);
                fillCombobxYear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSaveComment_Click(object sender, RoutedEventArgs e)
        {
            service.updateStudentComments(selectedStudent, txbCommentsStudent.Text);
            MessageBox.Show("Comments saved succesfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        private void btnDeleteStudent_Click(object sender, RoutedEventArgs e)
        {
            if (selectedStudent == null)
                MessageBox.Show("Cannot delete blank entry", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                service.deleteStudent(selectedStudent);
                fillGrid();
                if (groupedBy != null)
                {
                    groupBy(groupedBy);
                }
                TxbSearchStudent.Text = "";
            }
        }

        private void viewDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedStudent == null)
            {
                MessageBox.Show("Not possible to view details", "Information",MessageBoxButton.OK,MessageBoxImage.Information);
            }
            else
            {
                MainProjectDialog md = new MainProjectDialog(selectedStudent);
                md.Owner = this;
                md.ShowDialog();
            }
        }

        private void comboBxYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TxbSearchStudent.Text = "";
            if (comboBxYear.SelectedItem != null)
            {
                studentList = service.getStudentsByYear(Int32.Parse(comboBxYear.SelectedItem.ToString()));
                DGStudents.ItemsSource = studentList;
                listToGroupBy = studentList;
                comboBxSeason.IsEnabled = true;
                comboBxSeason.Text = "Choose Season";
            }
        }

        private void comboBxSeason_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TxbSearchStudent.Text = "";
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
                listToGroupBy = tempList;
                DGStudents.ItemsSource = tempList;
            }

        }

        private void TxbSearchStudent_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (DGStudents.HasItems)
            {
                if (comboBxSeason.SelectedItem != null)
                {
                    List<Student> temp = new List<Student>();
                    ComboBoxItem season = comboBxSeason.SelectedItem as ComboBoxItem;
                    foreach (Student s in studentList)
                    {
                        if (s.name.ToLower().Contains(TxbSearchStudent.Text.ToLower()))
                            if (s.season.Equals(season.Content.ToString()))
                                temp.Add(s);
                    }
                    DGStudents.ItemsSource = temp;
                }
                else
                {
                    List<Student> temp = new List<Student>();
                    foreach (Student s in studentList)
                    {
                        if (s.name.ToLower().Contains(TxbSearchStudent.Text.ToLower()))
                            temp.Add(s);
                    }
                    DGStudents.ItemsSource = temp;
                }
            }
        }

        private void comboBxStudentDGSeason_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBxSeason = sender as ComboBox;
            ComboBoxItem selectedseason = comboBxSeason.SelectedItem as ComboBoxItem;
            String season = selectedseason.Content.ToString();
            service.updateStudentSeason(selectedStudent, season);
        }

        private void CheckBxGroupByClass_Unchecked(object sender, RoutedEventArgs e)
        {
            DGStudents.CanUserAddRows = true;
            DGStudents.ItemsSource = listToGroupBy;
            comboBxYear.IsEnabled = true;
        }

        private void groupBy(String attribut)
        {
            ListCollectionView collection = new ListCollectionView(listToGroupBy);
            collection.GroupDescriptions.Add(new PropertyGroupDescription(attribut));
            DGStudents.ItemsSource = collection;
            comboBxYear.IsEnabled = false;
            DGStudents.CanUserAddRows = false;
        }

        #region CheckBoxes
        private void checkBxClass_Checked(object sender, RoutedEventArgs e)
        {
            checkBxCompany.IsChecked = false;
            checkBxSupervisor.IsChecked = false;
            enableComboBoxes(false);
            groupedBy = "class";
            groupBy("class");
        }

        private void checkBxClass_Unchecked(object sender, RoutedEventArgs e)
        {
            enableComboBoxes(true);
            DGStudents.ItemsSource = listToGroupBy;
            groupedBy = null;
        }

        private void checkBxSupervisor_Checked(object sender, RoutedEventArgs e)
        {
            checkBxCompany.IsChecked = false;
            checkBxClass.IsChecked = false;
            enableComboBoxes(false);
            groupedBy = "Supervisor.name";
            groupBy("Supervisor.name");
        }

        private void checkBxSupervisor_Unchecked(object sender, RoutedEventArgs e)
        {
            enableComboBoxes(true);
            DGStudents.ItemsSource = listToGroupBy;
            groupedBy = null;
        }

        private void checkBxCompany_Checked(object sender, RoutedEventArgs e)
        {
            checkBxClass.IsChecked = false;
            checkBxSupervisor.IsChecked = false;
            enableComboBoxes(false);
            groupedBy = "Company.name";
            groupBy("Company.name");
        }

        private void checkBxCompany_Unchecked(object sender, RoutedEventArgs e)
        {
            enableComboBoxes(true);
            DGStudents.ItemsSource = listToGroupBy;
            groupedBy = null;
        }

        private void enableComboBoxes(Boolean result)
        {
            comboBxSeason.IsEnabled = result;
            comboBxYear.IsEnabled = result;
            TxbSearchStudent.IsEnabled = result;
            DGStudents.CanUserAddRows = true;
        }
        #endregion
    }
}
