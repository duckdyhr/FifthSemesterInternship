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
    /// Interaction logic for MainProjectDialog.xaml
    /// </summary>
    public partial class MainProjectDialog : Window
    {
        private Student student;
        private Service service;

        public MainProjectDialog(Student s)
        {
            student = s;
            service = Service.GetInstance;
            InitializeComponent();
            this.Title = "Mainproject for " + student.name;
            fillTextboxes();

        }

        private void fillTextboxes()
        {
            if (student.mainProjectTitle == null)
            {
                txbMainProjectTitle.Text = "Write title for mainproject";
            }
            else
            {
                txbMainProjectTitle.Text = student.mainProjectTitle;
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            service.updateMainProject(student, txbMainProjectTitle.Text, TxbMainProjectDiscription.Text);
            MessageBox.Show("Mainproject details have been succesfully updated", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
