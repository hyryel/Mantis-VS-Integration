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
		public static readonly RoutedEvent IssueSelectionChangedEvent = EventManager.RegisterRoutedEvent("IssueSelectionChanged", RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), typeof(IssuesList));

		public event SelectionChangedEventHandler IssueSelectionChanged
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
		private IssueData[] _projectIssues;
		private void OnUpdateStatus(string msg, double percentage, bool isIndeterminate)
		{
			UpdateInfoRoutedEventArgs e = new Views.UpdateInfoRoutedEventArgs(StatusUpdatedEvent, msg, percentage, isIndeterminate);
			RaiseEvent(e);
		}
		private async void UpdateList()
		{
			OnUpdateStatus("#Loading issues...#", 0, true);
			try
			{
				if (!string.IsNullOrEmpty(ProjectId))
				{
					_projectIssues = await MantisClient.Instance.GetIssuesForUserByProjet(ProjectId);
					lstIssues.DataContext = _projectIssues.OrderByDescending(p => p.id);
				}
				else
				{
					lstIssues.DataContext = null;
				}
				SelectedIssue = null;
				SelectedIssueId = string.Empty;
				OnUpdateStatus($"#{_projectIssues.Length} Issues loaded#", 100, false);
			}
			catch
			{
				OnUpdateStatus("#Error loading issues#", 0, false);
				// TODO : log error
			}
		}
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.Property == ProjectIdProperty)
			{
				UpdateList();
			}
		}
		private void lstIssues_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count > 0)
			{
				SelectedIssueId = (e.AddedItems[0] as IssueData).id;
				SelectedIssue = (e.AddedItems[0] as IssueData);
			}
			SelectionChangedEventArgs ee = new SelectionChangedEventArgs(IssueSelectionChangedEvent, e.RemovedItems, e.AddedItems);
			ee.Source = this;
			RaiseEvent(ee);
		}

		public void LocalizeUI()
		{
		}
	}
}
