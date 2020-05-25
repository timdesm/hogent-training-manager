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
    public partial class TrainingRapport : Page
    {
        private DataTable dataTable;

        private object referedRoot;
        private Boolean changeDate = false;

        public TrainingRapport(object root)
        {
            this.referedRoot = root;

            InitializeComponent();
            Dialog.IsOpen = true;
            
            // Setup table
            dataTable = new DataTable();
            dataTable.Clear();
            dataTable.Columns.Add(new DataColumn("Max Type", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Sport", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Type", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Length", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Duration", typeof(TimeSpan)));
            dataTable.Columns.Add(new DataColumn("Av Speed", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Av Watt", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Bike Type", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Date", typeof(DateTime)));
            dataTable.Columns.Add(new DataColumn("Comment", typeof(string)));
            DataTable.ItemsSource = dataTable.DefaultView;
        }

        private void addDataLine(String maxType, String sport, String type, String length, TimeSpan time, String speed, string watt, String bikeType, DateTime date, String comment)
        {
            if (length == "m" || length == "km")
                length = "N/A";

            DataRow row = dataTable.NewRow();
            row[0] = maxType;
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

            if (inputDate > DateTime.Now)
            {
                MessageBox.Show("You can't select a date in the future");
                return;
            }

            TrainingManager manager = new TrainingManager(new UnitOfWork(new TrainingContext("Production")));
            Report rapport = manager.GenerateMonthlyTrainingsReport(inputDate.Year, inputDate.Month);


            if (rapport.TotalTrainingTime.ToString(@"h\h") != "0h")
                CountTime.Text = rapport.TotalTrainingTime.ToString(@"h\h") + " " + rapport.TotalTrainingTime.ToString(@"m\m");
            else
                CountTime.Text = rapport.TotalCyclingTrainingTime.ToString(@"m\m");

            CountRunning.Text = (rapport.CyclingSessions + rapport.RunningSessions).ToString();
            CountDistance.Text = (rapport.TotalCyclingDistance + (rapport.TotalRunningDistance / 1000)) + "km";

            dataTable.Clear();

            RunningSession maxRunDistance = rapport.MaxDistanceSessionRunning;
            addDataLine("Distance", "Running", maxRunDistance.TrainingType.ToString(), maxRunDistance.Distance + " m", maxRunDistance.Time, Math.Round(maxRunDistance.AverageSpeed, 2) + " km/h", "", "", maxRunDistance.When, maxRunDistance.Comments);

            RunningSession maxRunSpeed = rapport.MaxSpeedSessionRunning;
            addDataLine("Speed", "Running", maxRunSpeed.TrainingType.ToString(), maxRunSpeed.Distance + " m", maxRunSpeed.Time, Math.Round(maxRunSpeed.AverageSpeed, 2) + " km/h", "", "", maxRunSpeed.When, maxRunSpeed.Comments);

            CyclingSession maxDistance = rapport.MaxDistanceSessionCycling;
            addDataLine("Distance", "Cycling", maxDistance.TrainingType.ToString(), maxDistance.Distance + " km", maxDistance.Time, Math.Round(Convert.ToDecimal(maxDistance.AverageSpeed), 2) + " km/h", maxDistance.AverageWatt.ToString(), maxDistance.BikeType.ToString(), maxDistance.When, maxDistance.Comments);

            CyclingSession maxSpeed = rapport.MaxSpeedSessionCycling;
            addDataLine("Speed", "Cycling", maxSpeed.TrainingType.ToString(), maxSpeed.Distance + " km", maxSpeed.Time, Math.Round(Convert.ToDecimal(maxSpeed.AverageSpeed), 2) + " km/h", maxSpeed.AverageWatt.ToString(), maxSpeed.BikeType.ToString(), maxSpeed.When, maxSpeed.Comments);

            CyclingSession maxWatt = rapport.MaxWattSessionCycling;
            addDataLine("Watt", "Cycling", maxWatt.TrainingType.ToString(), maxWatt.Distance + " km", maxWatt.Time, Math.Round(Convert.ToDecimal(maxWatt.AverageSpeed), 2) + " km/h", maxWatt.AverageWatt.ToString(), maxWatt.BikeType.ToString(), maxWatt.When, maxWatt.Comments);

            Dialog.IsOpen = false;
        }

        private void DialogBackClickEevent(object sender, RoutedEventArgs e)
        {
            if (!changeDate)
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
    }
}