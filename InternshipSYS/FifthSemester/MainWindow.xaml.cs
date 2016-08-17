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
using System.Windows.Navigation;
using System.Windows.Shapes;
using SERVICE;

namespace FifthSemester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Service service;

        private CompaniesWindow c = null;
        private SupervisorsWindow s = null;
        private StudentsWindow st = null;
        private MainProjectsWindow mp = null;
        private LoadStudentDialog ls = null;

        public MainWindow()
        {
            service = Service.GetInstance;
            InitializeComponent();
        }

        private void imgSupervisors_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!service.isSupervisorsWindowActive)
            {
                s = new SupervisorsWindow();
                s.Show();
                service.isSupervisorsWindowActive = true;
            }
            else
            {
                s.Activate();
                s.WindowState = WindowState.Normal;
            }
        }

        private void imgCompanies_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!service.isCompaniesWindowActive)
            {
                c = new CompaniesWindow();
                c.Show();
                service.isCompaniesWindowActive = true;
            }
            else
            {
                c.Activate();
                c.WindowState = WindowState.Normal;
            }
        }

        private void imgStudents_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!service.isStudentsWindowActive)
            {
                st = new StudentsWindow();
                st.Show();
                service.isStudentsWindowActive = true;
            }
            else
            {
                st.Activate();
                st.WindowState = WindowState.Normal;
            }
        }
        private void imgMainProjects_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!service.isMainProjectsWindowActive)
            {
                mp = new MainProjectsWindow();
                mp.Show();
                service.isMainProjectsWindowActive = true;
            }
            else
            {
                mp.Activate();
                mp.WindowState = WindowState.Normal;
            }
        }
        private void imgLoadStudents_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!service.isLoadStudentsWindowActive)
            {
                ls = new LoadStudentDialog();
                ls.Show();
                service.isLoadStudentsWindowActive = true;
            }
            else
            {
                ls.Activate();
                ls.WindowState = WindowState.Normal;
            }
        }
    }
}
