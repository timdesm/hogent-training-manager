using DataLayer;
using DomainLibrary.Domain;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace InterfaceAppPresentationLayer
{
    /// <summary>
    /// Interaction logic for RunningRapport.xaml
    /// </summary>
    public partial class RunningRapport : Page
    {
        private DataTable dataTable;

        private object referedRoot;
        private Boolean changeDate = false;

        public RunningRapport(object root)
        {
            this.referedRoot = root;

            InitializeComponent();

            Dialog.IsOpen = true;

            // Setup table
            dataTable = new DataTable();
            dataTable.Clear();
            dataTable.Columns.Add(new DataColumn("Max Type", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Type", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Length", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Duration", typeof(TimeSpan)));
            dataTable.Columns.Add(new DataColumn("Av Speed", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Date", typeof(DateTime)));
            dataTable.Columns.Add(new DataColumn("Comment", typeof(string)));
            DataTable.ItemsSource = dataTable.DefaultView;
        }

        private void addDataLine(String maxType, String type, String length, TimeSpan time, String speed, DateTime date, String comment)
        {
            if (length == "m" || length == "km")
                length = "N/A";

            DataRow row = dataTable.NewRow();
            row[0] = maxType;
            row[1] = type;
            row[2] = length;
            row[3] = time;
            row[4] = speed;
            row[5] = date;
            row[6] = comment;
            dataTable.Rows.Add(row);
        }

        private void DialogGenerateClickEevent(object sender, RoutedEventArgs e)
        {
            DateTime inputDate = new DateTime();

            try
            {
                inputDate = InputDate.SelectedDate.Value;
            }
            catch (Exception)
            {
                MessageBox.Show("You must fill in the date");
                return;
            }

            if(inputDate > DateTime.Now)
            {
                MessageBox.Show("You can't select a date in the future");
                return;
            }

            TrainingManager manager = new TrainingManager(new UnitOfWork(new TrainingContext("Production")));
            Report rapport = manager.GenerateMonthlyRunningReport(inputDate.Year, inputDate.Month);

            if(rapport.TotalRunningTrainingTime.ToString(@"h\h") != "0h")
                CountTime.Text = rapport.TotalRunningTrainingTime.ToString(@"h\h") + " " + rapport.TotalRunningTrainingTime.ToString(@"m\m");
            else
                CountTime.Text = rapport.TotalRunningTrainingTime.ToString(@"m\m");

            CountRunning.Text = rapport.RunningSessions.ToString();

            int distance = rapport.TotalRunningDistance;
            if (distance < 10000)
                CountDistance.Text = distance.ToString() + "m";
            else
                CountDistance.Text = (distance / 1000) + "km";

            dataTable.Clear();

            RunningSession maxDistance = rapport.MaxDistanceSessionRunning;
            addDataLine("Distance", maxDistance.TrainingType.ToString(), maxDistance.Distance + " m", maxDistance.Time, Math.Round(maxDistance.AverageSpeed, 2) + " km/h", maxDistance.When, maxDistance.Comments);

            RunningSession maxSpeed = rapport.MaxSpeedSessionRunning;
            addDataLine("Speed", maxSpeed.TrainingType.ToString(), maxSpeed.Distance + " m", maxSpeed.Time, Math.Round(maxSpeed.AverageSpeed, 2) + " km/h", maxSpeed.When, maxSpeed.Comments);

            Dialog.IsOpen = false;
        }

        private void DialogBackClickEevent(object sender, RoutedEventArgs e)
        {
            if(!changeDate)
                this.NavigationService.Navigate(this.referedRoot);
            else
                Dialog.IsOpen = false;
        }

        private void NewDateClickEvent(object sender, RoutedEventArgs e)
        {
            changeDate = true;
            DialogBackButton.Content = "Close";
            Dialog.IsOpen = true;
        }

        private void MenuDashboardClickEvent(object sender, RoutedEventArgs e)
        {
            DashboardPage page = new DashboardPage();
            this.NavigationService.Navigate(page);
        }

        private void MenuAddTrainingClickEvent(object sender, RoutedEventArgs e)
        {
            AddTrainingPage page = new AddTrainingPage();
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
    }
}