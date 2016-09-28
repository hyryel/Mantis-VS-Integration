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
				OnUpdateStatus("#Retrieving user's project list...#", 0, true);
				_projectList = await MantisClient.Instance.GetProjectsForUser();
				Dictionary<ProjectData, int> dico = new Dictionary<ProjectData, int>();
				foreach (var item in _projectList)
				{
					var issues = await MantisClient.Instance.GetIssuesForUserByProjet(item.id);
					item.name = string.Format("{0} ({1})", item.name, issues.Length);
					dico.Add(item, issues.Count());
				}
				await cbxProjects.Dispatcher.InvokeAsync(() => cbxProjects.DataContext = dico.OrderByDescending(kvp => kvp.Value).Select(kvp => kvp.Key));
				_initialized = true;
				OnUpdateStatus("#Project list loaded#", 100, false);
			}
			catch (Exception ex)
			{
				OnUpdateStatus("#Error retrieving user's projects#", 0, false);
				_initialized = false;
				throw ex;
			}

		}

		ProjectData[] _projectList;
		#region Interface implementation
		public string DisplayName
		{
			get
			{
				return "#Projects#";
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
		private void lstIssues_StatusUpdated(object sender, UpdateInfoRoutedEventArgs e)
		{
			OnUpdateStatus(e.Message, e.Percentage, e.IsIndeterminate);
		}
	}
}
