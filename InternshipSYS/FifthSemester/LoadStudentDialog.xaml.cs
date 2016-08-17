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
    public partial class LoadStudentDialog : Window
    {
        private Service service;
        private String strSeason;

        public LoadStudentDialog()
        {
            service = Service.GetInstance;
            InitializeComponent();
            this.Title = "Load file for Students";
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            service.isLoadStudentsWindowActive = false;
        }

        private void opnFileDialog_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "TXT  Files (*.TXT)|*.txt";
            ofd.ShowDialog();

            txtBxLoadStudents.Text = ofd.FileName;
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            service.loadStudents(txtBxLoadStudents.Text, txtBxYear.Text, strSeason);
            this.Close();
            service.isLoadStudentsWindowActive = false;
        }

        private void StudentDialogWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            service.isLoadStudentsWindowActive = false;
        }

        private void comboBxSeason_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem season = comboBxSeason.SelectedItem as ComboBoxItem;
            strSeason = season.Content.ToString();
        }

    }
}
