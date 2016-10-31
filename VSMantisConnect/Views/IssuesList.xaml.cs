using MantisConnectAPI;
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

namespace VSMantisConnect.Views
{
	/// <summary>
	/// Interaction logic for IssuesList.xaml
	/// </summary>
	public partial class IssuesList : UserControl, ILocalizable
	{


		public string ProjectId
		{
			get { return (string)GetValue(ProjectIdProperty); }
			set { SetValue(ProjectIdProperty, value); }
		}
		public string SelectedIssueId
		{
			get { return (string)GetValue(SelectedIssueIdProperty); }
			set { SetValue(SelectedIssueIdProperty, value); }
		}
		// Using a DependencyProperty as the backing store for ProjectId.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ProjectIdProperty = DependencyProperty.Register("ProjectId", typeof(string), typeof(IssuesList));
		// Using a DependencyProperty as the backing store for SelectedIssueId.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty SelectedIssueIdProperty = DependencyProperty.Register("SelectedIssueId", typeof(string), typeof(IssuesList));


		public IssueData SelectedIssue
		{
			get { return (IssueData)GetValue(SelectedIssueProperty); }
			set { SetValue(SelectedIssueProperty, value); }
		}

		// Using a DependencyProperty as the backing store for SelectedIssue.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty SelectedIssueProperty = DependencyProperty.Register("SelectedIssue", typeof(IssueData), typeof(IssuesList));
		public static readonly RoutedEvent StatusUpdatedEvent = EventManager.RegisterRoutedEvent("StatusUpdated", RoutingStrategy.Bubble, typeof(UpdateInfoRoutedEventHandler), typeof(IssuesList));
		public static readonly RoutedEvent IssueSelectionChangedEvent = EventManager.RegisterRoutedEvent("IssueSelectionChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<IssueData>), typeof(IssuesList));

		public event RoutedPropertyChangedEventHandler<IssueData> IssueSelectionChanged
		{
			add { AddHandler(IssueSelectionChangedEvent, value); }
			remove { RemoveHandler(IssueSelectionChangedEvent, value); }
		}
		public event UpdateInfoRoutedEventHandler StatusUpdated
		{
			add { AddHandler(StatusUpdatedEvent, value); }
			remove { RemoveHandler(StatusUpdatedEvent, value); }
		}
		public IssuesList()
		{
			InitializeComponent();
		}
		private void OnUpdateStatus(string msg, double percentage, bool isIndeterminate)
		{
			UpdateInfoRoutedEventArgs e = new Views.UpdateInfoRoutedEventArgs(StatusUpdatedEvent, msg, percentage, isIndeterminate);
			RaiseEvent(e);
		}
		public async Task<Exception> UpdateList()
		{
			OnUpdateStatus(LocalizationHelper.GetString("IssuesListLoading"), 0, true);
			try
			{
				var projects = await MantisClient.Instance.GetProjectsForUser();
				List<ProjectDetail> data = new List<ProjectDetail>();

				foreach (var p in projects)
				{
					var issues = await MantisClient.Instance.GetIssuesForUserByProjet(p.id);
					data.Add(new ProjectDetail{ Project = p, Issues = issues.ToList()  });
				}
				lstIssues.DataContext = data.OrderByDescending( p=> p.Issues.Count );

				SelectedIssue = null;
				SelectedIssueId = string.Empty;
				OnUpdateStatus(LocalizationHelper.GetString("IssuesListLoaded", data.Count), 100, false);
			}
			catch (Exception ex)
			{
				OnUpdateStatus(LocalizationHelper.GetString("IssuesListErrorLoading"), 0, false);
				// TODO : log error
				return ex;
			}
			return null;
		}
		public void LocalizeUI()
		{
		}

		private void lstIssues_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (e.NewValue is IssueData)
			{
				SelectedIssueId = (e.NewValue as IssueData).id;
				SelectedIssue = (e.NewValue as IssueData);
				RoutedPropertyChangedEventArgs<IssueData> ee = new RoutedPropertyChangedEventArgs<IssueData>(e.OldValue as IssueData, e.NewValue as IssueData, IssueSelectionChangedEvent);
				ee.Source = this;
				RaiseEvent(ee);
			}
		}
	}
	public class ProjectDetail
	{
		public ProjectData Project { get; set; }
		public List<IssueData> Issues { get; set; }
	}
}
