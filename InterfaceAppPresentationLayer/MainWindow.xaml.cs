using DataLayer;
using DomainLibrary.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataTable dataTable;

        List<RunningSession> RunningSessions;
        List<CyclingSession> CyclingSessions;


        public MainWindow()
        {
            InitializeComponent();

            // Setup table
            dataTable = new DataTable();
            dataTable.Clear();
            dataTable.Columns.Add(new DataColumn("Type", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Length", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Duration", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Av. Speed", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Av. Watt", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Bicicle Type", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Date", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Comment", typeof(string)));
            DataTable.ItemsSource = dataTable.DefaultView;


            TrainingManager m = new TrainingManager(new UnitOfWork(new TrainingContext("Production")));
            RunningSessions = m.GetAllRunningSessions();
            CyclingSessions = m.GetAllCyclingSessions();

            CountTotal.Text =  "" + (RunningSessions.Count + CyclingSessions.Count);
            CountRunning.Text = "" + RunningSessions.Count;
            CountCycling.Text = "" + CyclingSessions.Count;

            foreach(RunningSession session in RunningSessions)
            {
                addDataLine(session.TrainingType.ToString(), session.Distance.ToString(), session.Time.ToString(), session.AverageSpeed.ToString(), "", "", session.When.ToString(), session.Comments);
            }

            foreach (CyclingSession session in CyclingSessions)
            {
                addDataLine(session.TrainingType.ToString(), session.Distance.ToString(), session.Time.ToString(), session.AverageSpeed.ToString(), "", session.BikeType.ToString(), session.When.ToString(), session.Comments);
            }
        }

        private void addDataLine(String type, String length, String time, String speed, String watt, String cicleType, String date, String comment)
        {
            DataRow row = dataTable.NewRow();
            row[0] = type;
            row[1] = length;
            row[2] = time;
            row[3] = speed;
            row[4] = watt;
            row[5] = cicleType;
            row[6] = date;
            row[7] = comment;
            dataTable.Rows.Add(row);
        }

        private void DataDeleteClickEvent(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Are you sure you want to remove those items?", "Configuration", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                List<int> list = DataTable.SelectedItems.Cast<DataRowView>().Select(x => dataTable.Rows.IndexOf(x.Row)).ToList();
                foreach (int i in list)
                {
                    if (dataTable.Rows.Count > i)
                        dataTable.Rows.RemoveAt(i);
                }
            }
        }

        private void MenuAddTrainingClickEvent(object sender, RoutedEventArgs e)
        {
            AddTrainingWindow window = new AddTrainingWindow();
            window.Show();
            this.Close();
        }
    }
}

