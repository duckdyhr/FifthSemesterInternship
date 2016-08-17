using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using SERVICE;
using System.Windows.Data;

namespace FifthSemester
{
    /// <summary>
    /// Interaction logic for MainProjects.xaml
    /// </summary>
    public partial class MainProjectsWindow : Window
    {
        private Service service;
        private MainProject selectedMainProject;
        private Student selectedParticipant;

        private List<MainProject> MainProjectList;
        private List<Student> StudentList;
        private List<MainProject> listToGroupBy;
        private String groupedBy;

        public MainProjectsWindow()
        {
            service = Service.GetInstance;
            InitializeComponent();
            fillCombobxYear();
            groupedBy = null;
        }

        private void fillCombobxYear()
        {
            comboBxYear.ItemsSource = service.getYearList();
        }

        private void fillGrid()
        {
            MainProjectList = service.getMainProjectsByYear(Int32.Parse(comboBxYear.SelectedItem.ToString()));
            if (comboBxSeason.SelectedItem != null)
            {
                List<MainProject> tempList = new List<MainProject>();
                ComboBoxItem season = comboBxSeason.SelectedItem as ComboBoxItem;
                foreach (MainProject s in MainProjectList)
                {
                    if (s.season.Equals(season.Content.ToString()))
                    {
                        tempList.Add(s);
                    }
                }
                MainProjectList = tempList;
            }
            listToGroupBy = MainProjectList;
            DGMainProjects.ItemsSource = MainProjectList;
        }

        private void selectCompanyButton_Click(object sender, RoutedEventArgs e)
        {
            CompanyMainProjectDialog cd = new CompanyMainProjectDialog(selectedMainProject);
            cd.Owner = this;
            cd.ShowDialog();
            fillGrid();
            DGMainProjects.SelectedItem = selectedMainProject;
        }

        private void selectSupervisorButton_Click(object sender, RoutedEventArgs e)
        {
            SupervisorMainProjectDialog sd = new SupervisorMainProjectDialog(selectedMainProject);
            sd.Owner = this;
            sd.ShowDialog();
            fillGrid();
            DGMainProjects.SelectedItem = selectedMainProject;
        }

        private void selectParticipantButton_Click(object sender, RoutedEventArgs e)
        {
            StudentDialog sd = new StudentDialog(selectedParticipant, selectedMainProject.title);
            sd.Owner = this;
            sd.ShowDialog();
            fillGrid();
            ParticipantsDG.SelectedItem = selectedParticipant;
            StudentList = service.getParticipantList(selectedMainProject.title);
            ParticipantsDG.ItemsSource = StudentList;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            service.isMainProjectsWindowActive = false;
        }

        private void DGMainProjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedMainProject = DGMainProjects.SelectedItem as MainProject;
            if (selectedMainProject != null)
            {
                txbCommentsMainProject.Text = selectedMainProject.comments;
                btnSaveComment.IsEnabled = true;
                StudentList = service.getParticipantList(selectedMainProject.title);
                ParticipantsDG.ItemsSource = StudentList;
            }
            else
            {
                txbCommentsMainProject.Text = null;
                btnSaveComment.IsEnabled = false;
            }
        }

        private void DGMainProjects_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                FrameworkElement element_Name = DGMainProjects.Columns[0].GetCellContent(e.Row);
                if (element_Name.GetType() == typeof(TextBox))
                {
                    var name = ((TextBox)element_Name).Text;
                    selectedMainProject.title = name;
                }
                FrameworkElement element_GroupNo = DGMainProjects.Columns[1].GetCellContent(e.Row);
                if (element_GroupNo.GetType() == typeof(TextBox))
                {
                    var groupNo = ((TextBox)element_GroupNo).Text;
                    selectedMainProject.groupNo = Convert.ToInt16(groupNo);
                }
                FrameworkElement element_Year = DGMainProjects.Columns[2].GetCellContent(e.Row);
                if (element_Year.GetType() == typeof(TextBox))
                {
                    var year = ((TextBox)element_Year).Text;
                    selectedMainProject.year = Convert.ToInt32(year);
                }
                service.updateMainProject(selectedMainProject);
                fillCombobxYear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSaveComment_Click(object sender, RoutedEventArgs e)
        {
            service.updateMainProjectComments(selectedMainProject, txbCommentsMainProject.Text);
            MessageBox.Show("Comments saved succesfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        private void btnDeleteMainProject_Click(object sender, RoutedEventArgs e)
        {
            if (selectedMainProject == null)
                MessageBox.Show("Cannot delete blank entry", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                service.deleteMainProject(selectedMainProject);
                fillGrid();
                if (groupedBy != null)
                {
                    groupBy(groupedBy);
                }
                TxbSearchMainProject.Text = "";
            }
        }

        private void comboBxYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TxbSearchMainProject.Text = "";
            if (comboBxYear.SelectedItem != null)
            {
                MainProjectList = service.getMainProjectsByYear(Int32.Parse(comboBxYear.SelectedItem.ToString()));
                DGMainProjects.ItemsSource = MainProjectList;
                listToGroupBy = MainProjectList;
                comboBxSeason.IsEnabled = true;
                comboBxSeason.Text = "Choose Season";
            }
        }

        private void comboBxSeason_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TxbSearchMainProject.Text = "";
            if (comboBxSeason.SelectedItem != null)
            {
                List<MainProject> tempList = new List<MainProject>();
                ComboBoxItem season = comboBxSeason.SelectedItem as ComboBoxItem;
                foreach (MainProject s in MainProjectList)
                {
                    if (s.season.Equals(season.Content.ToString()))
                    {
                        tempList.Add(s);
                    }
                }
                listToGroupBy = tempList;
                DGMainProjects.ItemsSource = tempList;
            }

        }

        private void TxbSearchMainProject_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (DGMainProjects.HasItems)
            {
                if (comboBxSeason.SelectedItem != null)
                {
                    List<MainProject> temp = new List<MainProject>();
                    ComboBoxItem season = comboBxSeason.SelectedItem as ComboBoxItem;
                    foreach (MainProject s in MainProjectList)
                    {
                        if (s.title.ToLower().Contains(TxbSearchMainProject.Text.ToLower()))
                            if (s.season.Equals(season.Content.ToString()))
                                temp.Add(s);
                    }
                    DGMainProjects.ItemsSource = temp;
                }
                else
                {
                    List<MainProject> temp = new List<MainProject>();
                    foreach (MainProject s in MainProjectList)
                    {
                        if (s.title.ToLower().Contains(TxbSearchMainProject.Text.ToLower()))
                            temp.Add(s);
                    }
                    DGMainProjects.ItemsSource = temp;
                }
            }
        }

        private void comboBxMainProjectDGSeason_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBxSeason = sender as ComboBox;
            ComboBoxItem selectedseason = comboBxSeason.SelectedItem as ComboBoxItem;
            String season = selectedseason.Content.ToString();
            service.updateMainProjectSeason(selectedMainProject, season);
        }

        private void CheckBxGroupByClass_Unchecked(object sender, RoutedEventArgs e)
        {
            DGMainProjects.CanUserAddRows = true;
            DGMainProjects.ItemsSource = listToGroupBy;
            comboBxYear.IsEnabled = true;
        }

        private void groupBy(String attribut)
        {
            ListCollectionView collection = new ListCollectionView(listToGroupBy);
            collection.GroupDescriptions.Add(new PropertyGroupDescription(attribut));
            DGMainProjects.ItemsSource = collection;
            comboBxYear.IsEnabled = false;
            DGMainProjects.CanUserAddRows = false;
        }

        private void ParticipantsDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedParticipant = ParticipantsDG.SelectedItem as Student;

        }

        private void ParticipantsDG_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                FrameworkElement element_Name = ParticipantsDG.Columns[0].GetCellContent(e.Row);
                if (element_Name.GetType() == typeof(TextBox))
                {
                    var name = ((TextBox)element_Name).Text;
                    selectedParticipant.name = name;
                }
                FrameworkElement element_Phone = ParticipantsDG.Columns[1].GetCellContent(e.Row);
                if (element_Phone.GetType() == typeof(TextBox))
                {
                    var phone = ((TextBox)element_Phone).Text;
                    selectedParticipant.phone = phone;
                }
                FrameworkElement element_Email = ParticipantsDG.Columns[2].GetCellContent(e.Row);
                if (element_Email.GetType() == typeof(TextBox))
                {
                    var email = ((TextBox)element_Email).Text;
                    selectedParticipant.email = email;
                }
                selectedParticipant.mainProjectTitle = selectedMainProject.title;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            try
            {
                selectedParticipant.mainProjectTitle = selectedMainProject.title;
            }
            catch
            {
                MessageBox.Show("An error has occurred", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                fillGrid();
            }
        }
        private void btnDeleteParticipant_Click(object sender, RoutedEventArgs e)
        {
            Student participantToDelete = ParticipantsDG.SelectedItem as Student;
            if (participantToDelete == null)
                MessageBox.Show("Cannot delete blank entry", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                try
                {
                    selectedParticipant.mainProjectTitle = null;
                    service.updateStudent(participantToDelete);
                    ParticipantsDG.ItemsSource = service.getParticipantList(selectedMainProject.title);
                }
                catch
                {
                    MessageBox.Show("An error has occurred", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
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
            DGMainProjects.ItemsSource = listToGroupBy;
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
            DGMainProjects.ItemsSource = listToGroupBy;
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
            DGMainProjects.ItemsSource = listToGroupBy;
            groupedBy = null;
        }

        private void enableComboBoxes(Boolean result)
        {
            comboBxSeason.IsEnabled = result;
            comboBxYear.IsEnabled = result;
            TxbSearchMainProject.IsEnabled = result;
            DGMainProjects.CanUserAddRows = true;
        }
        #endregion
    }
}
