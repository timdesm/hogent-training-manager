using DataLayer;
using DomainLibrary.Domain;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace InterfaceAppPresentationLayer
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : Page
    {
        private DataTable dataTable;

        public DashboardPage()
        {
            InitializeComponent();
            ShowsNavigationUI = false;

            // Setup table
            dataTable = new DataTable();
            dataTable.Clear();
            dataTable.Columns.Add(new DataColumn("Id", typeof(int)));
            dataTable.Columns.Add(new DataColumn("Sport", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Type", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Distance", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Duration", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Av Speed", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Av Watt", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Bike Type", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Date", typeof(DateTime)));
            dataTable.Columns.Add(new DataColumn("Comment", typeof(string)));
            DataTable.ItemsSource = dataTable.DefaultView;

            TrainingManager manager = new TrainingManager(new UnitOfWork(new TrainingContext("Production")));

            CountTotal.Text = "" + (manager.GetAllCyclingSessions().Count + manager.GetAllRunningSessions().Count);
            CountRunning.Text = "" + manager.GetAllRunningSessions().Count;
            CountCycling.Text = "" + manager.GetAllCyclingSessions().Count;

            foreach (RunningSession session in manager.GetAllRunningSessions())
            {
                addDataLine(session.Id, "Running", session.TrainingType.ToString(), session.Distance.ToString() + " m", session.Time.ToString(), Math.Round(session.AverageSpeed, 2) + " km/h", "", "", session.When, session.Comments);
            }

            foreach (CyclingSession session in manager.GetAllCyclingSessions())
            {
                addDataLine(session.Id, "Cycling", session.TrainingType.ToString(), session.Distance.ToString() + " km", session.Time.ToString(), Math.Round(Convert.ToDecimal(session.AverageSpeed), 2) + " km/h", session.AverageWatt + "", session.BikeType.ToString(), session.When, session.Comments);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddTrainingPage page = new AddTrainingPage();
            this.NavigationService.Navigate(page);
        }

        private void addDataLine(int id, String sport, String type, String length, String time, String speed, String watt, String bikeType, DateTime date, String comment)
        {
            if (length == "m" || length == "km")
                length = "N/A";

            DataRow row = dataTable.NewRow();
            row[0] = id;
            row[1] = sport;
            row[2] = type;
            row[3] = length;
            row[4] = time;
            row[5] = speed;
            row[6] = watt;
            row[7] = bikeType;
            row[8] = date;
            row[9] = comment;
            dataTable.Rows.Add(row);
        }

        private void DataDeleteClickEvent(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to remove those items?", "Configuration", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                List<int> runningIDs = new List<int>();
                List<int> cyclingIDs = new List<int>();

                List<int> list = DataTable.SelectedItems.Cast<DataRowView>().Select(x => dataTable.Rows.IndexOf(x.Row)).ToList();
                foreach (int i in list)
                {
                    if (dataTable.Rows.Count > i)
                    {
                        DataRow row = dataTable.Rows[i];
                        String sport = (string)row[1];
                        if (sport.ToLower().Equals("running"))
                            runningIDs.Add((int)row[0]);
                        else if(sport.ToLower().Equals("cycling"))
                            cyclingIDs.Add((int)row[0]);
                        dataTable.Rows.RemoveAt(i);
                    }
                }

                TrainingManager manager = new TrainingManager(new UnitOfWork(new TrainingContext("Production")));
                manager.RemoveTrainings(cyclingIDs, runningIDs);
            }
        }

        private void DataDeleteAllClickEvent(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to remove all items?", "Configuration", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                TrainingManager manager = new TrainingManager(new UnitOfWork(new TrainingContext("Production")));
                dataTable.Clear();
                List<int> runningIDs = manager.GetAllRunningSessions().Select(x => x.Id).ToList();
                List<int> cyclingIDs = manager.GetAllCyclingSessions().Select(x => x.Id).ToList();
                manager.RemoveTrainings(cyclingIDs, runningIDs);
            }
        }

        private void MenuAddTrainingClickEvent(object sender, RoutedEventArgs e)
        {
            AddTrainingPage page = new AddTrainingPage();
            this.NavigationService.Navigate(page);
        }

        private void MenuRunningRapportClickEvent(object sender, RoutedEventArgs e)
        {
            RunningRapport page = new RunningRapport(this);
            this.NavigationService.Navigate(page);
        }

        private void MenuCyclingRapportClickEevent(object sender, RoutedEventArgs e)
        {
            CyclingRapport page = new CyclingRapport(this);
            this.NavigationService.Navigate(page);
        }

        private void MenuTrainingRapportClickEevent(object sender, RoutedEventArgs e)
        {
            TrainingRapport page = new TrainingRapport(this);
            this.NavigationService.Navigate(page);
        }

        private void DataTableAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString().ToLower().Equals("id"))
                e.Column.Visibility = Visibility.Collapsed;
        }
    }
}
