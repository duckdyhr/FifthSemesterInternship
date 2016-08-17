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
    /// Interaction logic for CompanyDialog.xaml
    /// </summary>
    public partial class CompanyDialog : Window
    {
        private Service service;
        private Student student;

        public CompanyDialog(Student s) 
        {
            student = s;
            service = Service.GetInstance;
            InitializeComponent();
            CompanyDialogWindow.Title = "Select company for " + student.name;
            fillGrid();
        }

        private void fillGrid()
        {
            CompaniesDG.ItemsSource = service.getCompaniesList();
            CompaniesDG.SelectedIndex = 0;
        }

        private void SearchTextChanged(object sender, TextChangedEventArgs e)
        {
            CompaniesDG.ItemsSource = service.getCompaniesSearchList(txtBxSearchCompanies.Text);
        }

        private void btnVisit_Click(object sender, RoutedEventArgs e)
        {
            Company c = CompaniesDG.SelectedItem as Company;
            string website = c.website;

            System.Diagnostics.Process.Start(website);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            Company company = CompaniesDG.SelectedItem as Company;
            MessageBoxResult reply = MessageBox.Show("Do you want to assign " + student.name + "'s internship to be at " + company.name, "Assign " + student.name + " to " + company.name, MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (reply == MessageBoxResult.Yes)
            {
                try
                {
                    service.assignStudentToCompany(company, student);
                    MessageBox.Show("You have successfully assigned " + student.name + " to " + company.name, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                }
                catch
                {
                    MessageBox.Show("An error has occurred", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    fillGrid();
                }
            }
        }

    }
}
