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
			try
			{
				OnUpdateStatus("Retrieving user's project list...", 0, true);
				_projectList = await MantisClient.Instance.GetProjectsForUser();
				Dictionary<ProjectData, int> dico = new Dictionary<ProjectData, int>();
				foreach (var item in _projectList)
				{
					var issues = await MantisClient.Instance.GetIssuesForUserByProjet(item.id);
					item.name = string.Format("{0} ({1})", item.name, issues.Length);
					dico.Add(item, issues.Count());
				}
				await cbxProjects.Dispatcher.InvokeAsync(() => cbxProjects.DataContext = dico.OrderByDescending( kvp => kvp.Value).Select( kvp => kvp.Key) );
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
					lstIssues.DataContext = _projectIssues.OrderByDescending( p => p.id);
					lstIssueDetail.DataContext = null;
				}
				OnUpdateStatus("Issues loaded", 100, false);
			}
			catch (Exception ex)
			{
				OnUpdateStatus("Error loading issues", 0, false);
				throw ex;
			}
		}

		private void lstIssues_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			OnUpdateStatus("Loading issue detail...", 0, true);
			try
			{
				if (e.AddedItems.Count > 0)
				{
					IssueData item = e.AddedItems[0] as IssueData;
					if (item != null && item.notes != null)
					{
						lstIssueDetail.DataContext = item.notes.Where( n => n.text.Trim().Length > 0).OrderByDescending( n=> n.date_submitted).ToList();
					}
					else
					{
						lstIssueDetail.DataContext = null;
					}
				}
				OnUpdateStatus("Issues detail loaded", 100, false);
			}
			catch (Exception ex)
			{
				OnUpdateStatus("Error loading issue detail", 0, false);
				throw ex;
			}
		}
	}
}
