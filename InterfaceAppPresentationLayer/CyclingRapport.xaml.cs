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
    public partial class CyclingRapport : Page
    {
        private DataTable dataTable;

        private object referedRoot;
        private Boolean changeDate = false;

        public CyclingRapport(object root)
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
            dataTable.Columns.Add(new DataColumn("Av Watt", typeof(int)));
            dataTable.Columns.Add(new DataColumn("Bike Type", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Date", typeof(DateTime)));
            dataTable.Columns.Add(new DataColumn("Comment", typeof(string)));
            DataTable.ItemsSource = dataTable.DefaultView;
        }

        private void addDataLine(String maxType, String type, String length, TimeSpan time, String speed, int watt, String bikeType, DateTime date, String comment)
        {
            if (length == "m" || length == "km")
                length = "N/A";

            DataRow row = dataTable.NewRow();
            row[0] = maxType;
            row[1] = type;
            row[2] = length;
            row[3] = time;
            row[4] = speed;
            row[5] = watt;
            row[6] = bikeType;
            row[7] = date;
            row[8] = comment;
            dataTable.Rows.Add(row);
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
            Report rapport = manager.GenerateMonthlyCyclingReport(inputDate.Year, inputDate.Month);

            if (rapport.TotalCyclingTrainingTime.ToString(@"h\h") != "0h")
                CountTime.Text = rapport.TotalCyclingTrainingTime.ToString(@"h\h") + " " + rapport.TotalCyclingTrainingTime.ToString(@"m\m");
            else
                CountTime.Text = rapport.TotalCyclingTrainingTime.ToString(@"m\m");

            CountRunning.Text = rapport.CyclingSessions.ToString();
            CountDistance.Text = rapport.TotalCyclingDistance + "km";

            dataTable.Clear();
            CyclingSession maxDistance = rapport.MaxDistanceSessionCycling;
            addDataLine("Distance", maxDistance.TrainingType.ToString(), maxDistance.Distance + " km", maxDistance.Time, Math.Round(Convert.ToDecimal(maxDistance.AverageSpeed), 2) + " km/h", (int) maxDistance.AverageWatt, maxDistance.BikeType.ToString(), maxDistance.When, maxDistance.Comments);

            CyclingSession maxSpeed = rapport.MaxSpeedSessionCycling;
            addDataLine("Speed", maxSpeed.TrainingType.ToString(), maxSpeed.Distance + " km", maxSpeed.Time, Math.Round(Convert.ToDecimal(maxSpeed.AverageSpeed), 2) + " km/h", (int) maxSpeed.AverageWatt, maxSpeed.BikeType.ToString(), maxSpeed.When, maxSpeed.Comments);

            CyclingSession maxWatt = rapport.MaxWattSessionCycling;
            addDataLine("Watt", maxWatt.TrainingType.ToString(), maxWatt.Distance + " km", maxWatt.Time, Math.Round(Convert.ToDecimal(maxWatt.AverageSpeed), 2) + " km/h", (int) maxWatt.AverageWatt, maxWatt.BikeType.ToString(), maxWatt.When, maxWatt.Comments);

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

        private void MenuRunningRapportClickEvent(object sender, RoutedEventArgs e)
        {
            RunningRapport page = new RunningRapport(this);
            this.NavigationService.Navigate(page);
        }

        private void MenuTrainingRapportClickEevent(object sender, RoutedEventArgs e)
        {
            TrainingRapport page = new TrainingRapport(this);
            this.NavigationService.Navigate(page);
        }
    }
}