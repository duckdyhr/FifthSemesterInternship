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
using SERVICE;

namespace FifthSemester
{
    /// <summary>
    /// Interaction logic for StudentDialog.xaml
    /// </summary>
    public partial class StudentDialog : Window
    {
        private Service service;
        private Student student;
        private String selectedMainProjectTitle;

        public StudentDialog(Student s, String title)
        {
            student = s;
            selectedMainProjectTitle = title;
            service = Service.GetInstance;
            InitializeComponent();
            StudentDialogWindow.Title = "Assign Student to " + selectedMainProjectTitle;
            fillGrid();
        }

        private void fillGrid()
        {
            StudentDG.ItemsSource = service.getStudentList();
        }

        private void SearchTextChanged(object sender, TextChangedEventArgs e)
        {
            StudentDG.ItemsSource = service.getStudentSearchList(txtBxSearchStudents.Text);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            Student Student = StudentDG.SelectedItem as Student;
            MessageBoxResult reply = MessageBox.Show("Do you want to assign " + Student.name + " to " + selectedMainProjectTitle, "Assign " + Student.name + " to " + selectedMainProjectTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (reply == MessageBoxResult.Yes)
            {
                service.assignStudentToMainProject(Student, selectedMainProjectTitle);
                MessageBox.Show("You have successfully assigned " + Student.name + " to " + selectedMainProjectTitle, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
            }
        }
    }
}
