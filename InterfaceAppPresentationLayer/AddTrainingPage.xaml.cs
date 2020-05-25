using DataLayer;
using DomainLibrary.Domain;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for AddTrainingPage.xaml
    /// </summary>
    public partial class AddTrainingPage : Page
    {
        TrainingManager manager;

        public AddTrainingPage()
        {
            InitializeComponent();

            manager = new TrainingManager(new UnitOfWork(new TrainingContext("Production")));

            CountTotal.Text = "" + (manager.GetAllRunningSessions().Count + manager.GetAllCyclingSessions().Count);
            CountRunning.Text = "" + manager.GetAllRunningSessions().Count;
            CountCycling.Text = "" + manager.GetAllCyclingSessions().Count;

            InputBikeType.Visibility = Visibility.Collapsed;
            InputBikeTypeLabel.Visibility = Visibility.Collapsed;
        }

        private void SportTypeChangeEvent(object sender, SelectionChangedEventArgs e)
        {
            int type = InputSportType.SelectedIndex;
            if(type == 1)
            {
                InputBikeTypeLabel.Visibility = Visibility.Visible;
                InputBikeType.Visibility = Visibility.Visible;
            }
            else
            {
                InputBikeTypeLabel.Visibility = Visibility.Collapsed;
                InputBikeType.Visibility = Visibility.Collapsed;
            }
        }

        private void FormSaveClickEevent(object sender, RoutedEventArgs e)
        {
            SaveButton.Content = "Saving...";

            DateTime inputDate = new DateTime();
            DateTime inputTime = new DateTime();

            try
            {
                inputDate = InputDate.SelectedDate.Value;
                inputTime = InputTime.SelectedTime.Value;
            }
            catch(Exception)
            {
                MessageBox.Show("You must fill in the date and time");
                return;
            }

            DateTime date = new DateTime(inputDate.Year, inputDate.Month, inputDate.Day, inputTime.Hour, inputTime.Minute, inputTime.Second);

            int trainingType = InputTrainingType.SelectedIndex;
            if(trainingType < 0)
            {
                MessageBox.Show("You must select a training type");
                return;
            }

            int sportType = InputSportType.SelectedIndex;
            if(sportType < 0)
            {
                MessageBox.Show("You must select a sport type");
                return;
            }

            int bikeType = InputBikeType.SelectedIndex;
            if (sportType == 1)
            {
                if(bikeType < 0)
                {
                    MessageBox.Show("You must select a bike type");
                    return;
                }
            }

            if(InputLength.Text == "")
            {
                MessageBox.Show("You must fill in the distance");
                return;
            }

            if(!Int32.TryParse(InputLength.Text, out int inputLength))
            {
                MessageBox.Show("You must fill in a valid number as distance");
                return;
            }

            if(inputLength <= 0)
            {
                MessageBox.Show("Distance must be greater than 0");
                return;
            }

            if(InputDuration.Text == "")
            {
                MessageBox.Show("You must fill in a duration");
                return;
            }

            if(!TimeSpan.TryParse(InputDuration.Text, out TimeSpan duration))
            {
                MessageBox.Show("Duration must be a valid time format");
                return;
            }

            String inputComment = InputComment.Text;

            

            TrainingType TrainingType = TrainingType.Interval;
            switch(trainingType)
            {
                case 0:
                    TrainingType = TrainingType.Recuperation;
                    break;
                case 1:
                    TrainingType = TrainingType.Endurance;
                    break;
                case 2:
                    TrainingType = TrainingType.Interval;
                    break;
            }

            try
            {
                switch (sportType)
                {
                    case 0:
                        int runSpeed = Convert.ToInt32((inputLength * 1000) / duration.TotalHours);
                        manager.AddRunningTraining(date, inputLength, duration, runSpeed, TrainingType, inputComment);
                        break;
                    case 1:
                        BikeType BikeType = BikeType.CityBike;
                        switch (bikeType)
                        {
                            case 0:
                                BikeType = BikeType.MountainBike;
                                break;
                            case 1:
                                BikeType = BikeType.IndoorBike;
                                break;
                            case 2:
                                BikeType = BikeType.RacingBike;
                                break;
                            case 3:
                                BikeType = BikeType.CityBike;
                                break;
                        }

                        int bikeSpeed = Convert.ToInt32(inputLength / duration.TotalHours);
                        manager.AddCyclingTraining(date, (float)inputLength, duration, bikeSpeed, 100, TrainingType, inputComment, BikeType);
                        break;
                    default:
                        break;
                }

                MessageBox.Show("New training has been added");
            }
            catch(Exception)
            {
                MessageBox.Show("Something went wrong while adding new a training");
            }

            SaveButton.Content = "Save Training";
        }

        private void MenuDashboardClickEvent(object sender, RoutedEventArgs e)
        {
            DashboardPage page = new DashboardPage();
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
    }
}
