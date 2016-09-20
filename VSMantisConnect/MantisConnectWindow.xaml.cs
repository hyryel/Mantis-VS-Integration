using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using VSMantisConnect.Views;
using System.Linq;
using System;
using VSMantisConnect.Interfaces;
using System.Threading.Tasks;

namespace VSMantisConnect
{
	/// <summary>
	/// Interaction logic for MantisConnectWindow.xaml.
	/// </summary>
	[ProvideToolboxControl("VSMantisConnect.MantisConnectWindow", true)]
	public partial class MantisConnectWindow : UserControl
	{

		public MantisConnectWindow()
		{
			InitializeComponent();
		}
		List<UserControl> _viewList;
		private void View_UpdateStatus(object sender, Interfaces.StatusUpdatedEventArgs e)
		{
			try
			{
				stsLabel.Dispatcher.InvokeAsync(() => stsLabel.Text = e.StatusInfo);
				stsProgress.Dispatcher.InvokeAsync(() =>
				{
					stsProgress.Value = (e.StatusPercentage >= 0 && e.StatusPercentage <= 100) ? e.StatusPercentage : 0;
					stsProgress.IsIndeterminate = e.IsIndeterminate;
				});
			}
			catch (Exception ex)
			{
				DisplayError(ex);
			}
		}

		#region Control's views
		SettingsView settingView;

		#endregion
		private void OnUpdateStatus(string info, double percentage, bool isIndeterminate)
		{
			View_UpdateStatus(this, new Interfaces.StatusUpdatedEventArgs(info, percentage, isIndeterminate));
		}
		private void cbxViewSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				OnUpdateStatus("Loading view...", 0, true);
				contentGrid.Children.Clear();
			var v = _viewList.FirstOrDefault(u => (u as IView).DisplayName == cbxViewSelector.SelectedItem.ToString());
			if (!(v as IView).Initialized)
			{
				OnUpdateStatus($"Initializing {(v as IView).DisplayName}", 0, true);
				(v as IView).InitializeData();
			}
			contentGrid.Children.Add(v);
				OnUpdateStatus($"View {(v as IView).DisplayName} loaded", 100, false);
			}
			catch (Exception ex)
			{
				OnUpdateStatus("Error loading view", 0, false);
				DisplayError(ex);
			}
		}
		private void DisplayError(Exception ex)
		{
			Microsoft.VisualStudio.PlatformUI.MessageDialog.Show("ERROR", ex.ToString(), Microsoft.VisualStudio.PlatformUI.MessageDialogCommandSet.Ok);
		}
		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{

			settingView = new Views.SettingsView();
			_viewList = new List<UserControl> {
													settingView,
													new ProjectView(),
													new MantisEnumView(),
													new HomeView()
												   };
			foreach (var view in _viewList)
			{
				if (view is Interfaces.IStatusUpdater)
				{
					(view as Interfaces.IStatusUpdater).UpdateStatus += View_UpdateStatus;
				}
			}
			cbxViewSelector.DataContext = _viewList.Where(v => v is Interfaces.IView).Select(v => (v as Interfaces.IView).DisplayName);

		}
	}
}