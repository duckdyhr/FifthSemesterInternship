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
    /// Interaction logic for Supervisors.xaml
    /// </summary>
    public partial class SupervisorsWindow : Window
    {
        private Service service;
        private Supervisor selectedSupervisor;

        public SupervisorsWindow()
        {
            service = Service.GetInstance;
            InitializeComponent();
            fillGrid();
        }

        private void fillGrid()
        {
            SupervisorDG.ItemsSource = service.getSupervisorList();
        }

        private void SearchTextChanged(object sender, TextChangedEventArgs e)
        {
            SupervisorDG.ItemsSource = service.getSupervisorSearchList(txtBxSearchSupervisor.Text);
        }

        private void SupervisorDG_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                FrameworkElement element_Name = SupervisorDG.Columns[0].GetCellContent(e.Row);
                if (element_Name.GetType() == typeof(TextBox))
                {
                    var name = ((TextBox)element_Name).Text;
                    selectedSupervisor.name = name;
                }
                FrameworkElement element_Email = SupervisorDG.Columns[1].GetCellContent(e.Row);
                if (element_Email.GetType() == typeof(TextBox))
                {
                    var email = ((TextBox)element_Email).Text;
                    selectedSupervisor.email = email;
                }
                FrameworkElement element_Phone = SupervisorDG.Columns[2].GetCellContent(e.Row);
                if (element_Phone.GetType() == typeof(TextBox))
                {
                    var phone = ((TextBox)element_Phone).Text;
                    selectedSupervisor.phone = phone;
                }
                FrameworkElement element_Office = SupervisorDG.Columns[3].GetCellContent(e.Row);
                if (element_Office.GetType() == typeof(TextBox))
                {
                    var office = ((TextBox)element_Office).Text;
                    selectedSupervisor.office = office;
                }
                service.updateSupervisor(selectedSupervisor);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedSupervisor == null)
                MessageBox.Show("Cannot delete blank entry","Information", MessageBoxButton.OK,MessageBoxImage.Information);
            else
            {
                service.deleteSupervisor(selectedSupervisor);
                fillGrid();
            }
        }

        private void SupervisorDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedSupervisor = SupervisorDG.SelectedItem as Supervisor;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            service.isSupervisorsWindowActive = false;
        }    
    }
}
