using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using SERVICE;

namespace FifthSemester
{
    /// <summary>
    /// Interaction logic for Companies.xaml
    /// </summary>
    public partial class CompaniesWindow : Window
    {

        private Service service;
        private Company selectedCompany;
        private Contact selectedContact;
        private Offering selectedOffering;

        public CompaniesWindow()
        {
            service = Service.GetInstance;
            InitializeComponent();
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
        
        private void CompaniesDG_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                FrameworkElement element_Name = CompaniesDG.Columns[0].GetCellContent(e.Row);
                if (element_Name.GetType() == typeof(TextBox))
                {
                    var name = ((TextBox)element_Name).Text;
                    selectedCompany.name = name;
                }
                FrameworkElement element_Address = CompaniesDG.Columns[1].GetCellContent(e.Row);
                if (element_Address.GetType() == typeof(TextBox))
                {
                    var address = ((TextBox)element_Address).Text;
                    selectedCompany.address = address;
                }
                FrameworkElement element_Zipcode = CompaniesDG.Columns[2].GetCellContent(e.Row);
                if (element_Zipcode.GetType() == typeof(TextBox))
                {
                    var zipcode = ((TextBox)element_Zipcode).Text;
                    selectedCompany.zipcode = Convert.ToInt32(zipcode);
                }
                FrameworkElement element_Phone = CompaniesDG.Columns[3].GetCellContent(e.Row);
                if (element_Phone.GetType() == typeof(TextBox))
                {
                    var phone = ((TextBox)element_Phone).Text;
                    selectedCompany.phone = phone;
                }
                FrameworkElement element_Website = CompaniesDG.Columns[4].GetCellContent(e.Row);
                if (element_Website.GetType() == typeof(TextBox))
                {
                    var website = ((TextBox)element_Website).Text;
                    selectedCompany.website = website;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            try
            {
                service.updateCompany(selectedCompany);
            }
            catch
            {
                MessageBox.Show("An error has occurred", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                fillGrid();
            }
        }

        private void CompaniesDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedCompany = CompaniesDG.SelectedItem as Company;
            ContactsDG.ItemsSource = null;
            OfferingsDG.ItemsSource = null;
            txtBxComments.Text = null;
            btnSaveComment.IsEnabled = false;
            if (selectedCompany != null)
            {
                ContactsDG.ItemsSource = service.getContactList(selectedCompany.id);
                OfferingsDG.ItemsSource = service.getOfferingList(selectedCompany.id);
                txtBxComments.Text = selectedCompany.comments;
                btnSaveComment.IsEnabled = true;
            }
        }


        private void btnVisit_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCompany == null)
            {
                MessageBox.Show("Not possible to visit website", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                try
                {
                    System.Diagnostics.Process.Start(selectedCompany.website);
                }
                catch 
                {
                    MessageBox.Show("An error has occurred", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnDeleteCompany_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCompany == null)
                MessageBox.Show("Cannot delete blank entry", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                try
                {
                    service.deleteCompany(CompaniesDG.SelectedItem as Company);
                    fillGrid();
                }
                catch
                {
                    MessageBox.Show("An error has occurred", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }        

        private void ContactsDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedContact = ContactsDG.SelectedItem as Contact;
        }

        private void ContactsDG_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                FrameworkElement element_Name = ContactsDG.Columns[0].GetCellContent(e.Row);
                if (element_Name.GetType() == typeof(TextBox))
                {
                    var name = ((TextBox)element_Name).Text;
                    selectedContact.name = name;
                }
                FrameworkElement element_Phone = ContactsDG.Columns[1].GetCellContent(e.Row);
                if (element_Phone.GetType() == typeof(TextBox))
                {
                    var phone = ((TextBox)element_Phone).Text;
                    selectedContact.phone = phone;
                }
                FrameworkElement element_Email = ContactsDG.Columns[2].GetCellContent(e.Row);
                if (element_Email.GetType() == typeof(TextBox))
                {
                    var email = ((TextBox)element_Email).Text;
                    selectedContact.email = email;
                }
                selectedContact.Company = selectedCompany;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            try
            {
                service.updateContact(selectedContact);
            }
            catch
            {
                MessageBox.Show("An error has occurred", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                fillGrid();
            }
        }

        private void OfferingsDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedOffering = OfferingsDG.SelectedItem as Offering;
        }

        private void OfferingsDG_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                FrameworkElement element_Year = OfferingsDG.Columns[0].GetCellContent(e.Row);
                if (element_Year.GetType() == typeof(TextBox))
                {
                    var year = ((TextBox)element_Year).Text;
                    selectedOffering.year = Convert.ToInt32(year);
                }
                FrameworkElement element_OfferingNum = OfferingsDG.Columns[2].GetCellContent(e.Row);
                if (element_OfferingNum.GetType() == typeof(TextBox))
                {
                    var offeringNum = ((TextBox)element_OfferingNum).Text;
                    selectedOffering.offeringNumber = Convert.ToInt32(offeringNum);
                }
                selectedOffering.Company = selectedCompany;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            try
            {
                service.updateOffering(selectedOffering);
            }
            catch
            {
                MessageBox.Show("An error has occurred", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                fillGrid();
            }
        }

        private void btnDeleteContact_Click(object sender, RoutedEventArgs e)
        {
            Contact contactToDelete = ContactsDG.SelectedItem as Contact;
            if (contactToDelete == null)
                MessageBox.Show("Cannot delete blank entry", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                try
                {
                    service.deleteContact(contactToDelete);
                    ContactsDG.ItemsSource = service.getContactList(selectedCompany.id);
                }
                catch
                {
                    MessageBox.Show("An error has occurred", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnDeleteOffering_Click(object sender, RoutedEventArgs e)
        {
            Offering offeringToDelete = OfferingsDG.SelectedItem as Offering;
            if (offeringToDelete == null)
                MessageBox.Show("Cannot delete blank entry", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                try
                {
                    service.deleteOffering(offeringToDelete);
                    OfferingsDG.ItemsSource = service.getOfferingList(selectedCompany.id);
                }
                catch
                {
                    MessageBox.Show("An error has occurred", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnSaveComment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                service.updateCompanyComments(selectedCompany, txtBxComments.Text);
                MessageBox.Show("Comments saved succesfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("An error has occurred", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                fillGrid();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            service.isCompaniesWindowActive = false;
        }

        private void ComboBxSeason_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBox comboBxSeason = sender as ComboBox;
                ComboBoxItem selectedseason = comboBxSeason.SelectedItem as ComboBoxItem;
                String season = selectedseason.Content.ToString();
                txtBxComments.Text = season;
                service.updateOfferingSeason(selectedOffering, season);
            }
            catch
            {
                MessageBox.Show("An error has occurred", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


























    }
}
