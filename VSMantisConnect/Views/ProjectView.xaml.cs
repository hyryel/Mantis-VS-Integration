using System;
using System.Collections.Generic;
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
using VSMantisConnect.Interfaces;
using MantisConnectAPI;

namespace VSMantisConnect.Views
{
    /// <summary>
    /// Interaction logic for ProjectView.xaml
    /// </summary>
    public partial class ProjectView : UserControl, Interfaces.IStatusUpdater, Interfaces.IView
    {
        public ProjectView()
        {
            InitializeComponent();
            _initialized = false;
        }

        public async void InitializeData()
        {
            if (Properties.Settings.Default.ExtensionConfigured)
            {
                try
                {
                    OnUpdateStatus("Retrieving user's project list...", 0, true);
                    _projectList = await MantisClient.Instance.GetProjectsForUser();
                    cbxProjects.DataContext = _projectList;
                    _initialized = true;
                    OnUpdateStatus("Project list loaded", 100, false);
                }
                catch (Exception ex)
                {
                    OnUpdateStatus("Error retrieving user's projects", 0, false);
                    _initialized = false;
                    throw ex;
                }
            }
            else
            {
                cbxProjects.DataContext = new List<string> { "Please, go to settings before trying to access Mantis" };
                OnUpdateStatus("Please, configure extension", 0, false);
            }
        }

        ProjectData[] _projectList;
        IssueData[] _projectIssues;
        #region Interface implementation
        public string DisplayName
        {
            get
            {
                return "{Projects}";
            }
        }
        private bool _initialized;
        bool IView.Initialized
        {
            get
            {
                return _initialized;
            }
        }

        public event EventHandler<StatusUpdatedEventArgs> UpdateStatus;

        private void OnUpdateStatus(string info, double percentage, bool isIndeterminate)
        {
            if (UpdateStatus != null)
            {
                UpdateStatus(this, new Interfaces.StatusUpdatedEventArgs(info, percentage, isIndeterminate));
            }
        }
        #endregion

        private async void cbxProjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnUpdateStatus("Loading issues...", 0, true);
            try
            {
                ProjectData item = e.AddedItems[0] as ProjectData;
                if (item != null)
                {
                    _projectIssues = await MantisClient.Instance.GetIssuesForUserByProjet(item.id);
                    lstIssues.DataContext = _projectIssues;
                }
                OnUpdateStatus("Issues loaded", 100, false);
            }
            catch (Exception ex)
            {
                OnUpdateStatus("Error loading issues", 0, false);
                throw ex;
            }
        }
    }
}
