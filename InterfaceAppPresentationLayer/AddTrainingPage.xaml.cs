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
        List<RunningSession> RunningSessions;
        List<CyclingSession> CyclingSessions;

        public AddTrainingPage()
        {
            InitializeComponent();

            TrainingManager m = new TrainingManager(new UnitOfWork(new TrainingContext("Production")));
            RunningSessions = m.GetAllRunningSessions();
            CyclingSessions = m.GetAllCyclingSessions();

            CountTotal.Text = "" + (RunningSessions.Count + CyclingSessions.Count);
            CountRunning.Text = "" + RunningSessions.Count;
            CountCycling.Text = "" + CyclingSessions.Count;
        }

        private void FormSaveClickEevent(object sender, RoutedEventArgs e)
        {
            SaveButton.Content = "Saving...";

            String inputDate = InputDate.Text;
            String inputTime = InputTime.Text;
            String inputTrainingType = InputTrainingType.Text;
            String inputSportType = InputSportType.Text;
            String inputBikeType = InputBikeType.Text;
            String inputLength = InputLength.Text;
            String inputDuration = InputDuration.Text;
            String inputComment = InputComment.Text;



            SaveButton.Content = "Save Training";
        }

        private void MenuDashboardClickEvent(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
