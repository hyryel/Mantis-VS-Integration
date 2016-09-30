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
	/// Interaction logic for IssueDetail.xaml
	/// </summary>
	public partial class IssueDetail : UserControl, ILocalizable
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
			OnUpdateStatus(LocalizationHelper.GetString("IssueDetailLoading"), 0, true);
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
				OnUpdateStatus(LocalizationHelper.GetString("IssueDetailLoaded"), 100, false);
			}
			catch
			{
				OnUpdateStatus(LocalizationHelper.GetString("IssueDetailErrorLoading"), 0, false);
				// TODO : log error
			}
		}
		private void OnUpdateStatus(string msg, double percentage, bool isIndeterminate)
		{
			UpdateInfoRoutedEventArgs e = new Views.UpdateInfoRoutedEventArgs(StatusUpdateEvent, msg, percentage, isIndeterminate);
			RaiseEvent(e);
		}

		private async void btnAddNote_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(tbxNewNote.Text))
			{
				OnUpdateStatus(LocalizationHelper.GetString("IssueDetailErrorEmptyNote"), 0, false);
			}
			else
			{
				try
				{
					OnUpdateStatus(LocalizationHelper.GetString("IssueDetailAddingNote"), 0, true);
					IssueNoteData note = new IssueNoteData();
					note.text = tbxNewNote.Text;
					await Interfaces.MantisClient.Instance.AddNoteToIssue(IssueId, note);
					OnUpdateStatus(LocalizationHelper.GetString("IssueDetailNoteAdded"), 100, false);
					RefreshIssueDetail();
				}
				catch
				{
					OnUpdateStatus(LocalizationHelper.GetString("IssueDetailErrorAddingNote"), 0, false);
					// TODO : log error
				}

			}
		}

		public void LocalizeUI()
		{
			btnAddNote.LocalizeUIElement(this);
		}
	}
}
