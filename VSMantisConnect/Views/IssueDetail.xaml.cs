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

namespace VSMantisConnect.Views
{
	/// <summary>
	/// Interaction logic for IssueDetail.xaml
	/// </summary>
	public partial class IssueDetail : UserControl
	{


		public string IssueId
		{
			get { return (string)GetValue(IssueIdProperty); }
			set { SetValue(IssueIdProperty, value); }
		}
		// Using a DependencyProperty as the backing store for IssueIt.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IssueIdProperty = DependencyProperty.Register("IssueId", typeof(string), typeof(IssueDetail));
		public IssueData CurrentIssue
		{
			get { return (IssueData)GetValue(CurrentIssueProperty); }
			set { SetValue(CurrentIssueProperty, value); }
		}
		// Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CurrentIssueProperty = DependencyProperty.Register("CurrentIssue", typeof(IssueData), typeof(IssueDetail));
		public static readonly RoutedEvent StatusUpdateEvent = EventManager.RegisterRoutedEvent("StatusUpdated", RoutingStrategy.Bubble, typeof(UpdateInfoRoutedEventHandler), typeof(IssueDetail));

		public event UpdateInfoRoutedEventHandler StatusUpdated
		{
			add { AddHandler(StatusUpdateEvent, value); }
			remove { RemoveHandler(StatusUpdateEvent, value); }
		}
		protected async override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.Property == CurrentIssueProperty)
			{
				if (e.NewValue != null && IssueId != (e.NewValue as IssueData).id)
				{
					IssueId = CurrentIssue.id;
				}
				RefreshIssueDetail();
			}
			if (e.Property == IssueIdProperty)
			{
				if (e.NewValue != null && e.NewValue as string != CurrentIssue.id)
				{
					CurrentIssue = await Interfaces.MantisClient.Instance.GetIssueById(e.NewValue.ToString());
				}
				RefreshIssueDetail();
			}
		}
		public IssueDetail()
		{
			InitializeComponent();
		}
		private void RefreshIssueDetail()
		{
			OnUpdateStatus("Loading issue detail...", 0, true);
			try
			{
				btnAddNote.IsEnabled = CurrentIssue != null;
				if (CurrentIssue != null && CurrentIssue.notes != null)
				{

					lstIssueDetail.DataContext = CurrentIssue.notes.Where(n => n.text.Trim().Length > 0).OrderByDescending(n => n.date_submitted).ToList();
				}
				else
				{
					lstIssueDetail.DataContext = null;
				}
				OnUpdateStatus("Issues detail loaded", 100, false);
			}
			catch
			{
				OnUpdateStatus("Error loading issue detail", 0, false);
				throw;
			}
		}
		private void OnUpdateStatus(string msg, double percentage, bool isIndeterminate)
		{
			UpdateInfoRoutedEventArgs e = new Views.UpdateInfoRoutedEventArgs(StatusUpdateEvent, msg, percentage, isIndeterminate);
			RaiseEvent(e);
		}
	}
}
