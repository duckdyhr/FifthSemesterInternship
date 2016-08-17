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
    /// Interaction logic for SupervisorDialog.xaml
    /// </summary>
    public partial class SupervisorDialog : Window
    {
        private Service service;
        private Student student;

        public SupervisorDialog(Student s)
        {
            student = s;
            service = Service.GetInstance;
            InitializeComponent();
            SupervisorDialogWindow.Title = "Assign Supervisor to " + student.name;
            fillGrid();
        }

        private void fillGrid()
        {
            SupervisorDG.ItemsSource = service.getSupervisorList();
        }

        private void SearchTextChanged(object sender, TextChangedEventArgs e)
        {
            SupervisorDG.ItemsSource = service.getSupervisorSearchList(txtBxSearchSupervisors.Text);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            Supervisor supervisor = SupervisorDG.SelectedItem as Supervisor;
            MessageBoxResult reply = MessageBox.Show("Do you want to assign " + supervisor.name + " to " + student.name, "Assign " + supervisor.name + " to " + student.name, MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (reply == MessageBoxResult.Yes)
            {
                service.assignSupervisorToStudent(supervisor, student);
                MessageBox.Show("You have successfully assigned " + supervisor.name + " to " + student.name, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
            }
        }
    }
}
