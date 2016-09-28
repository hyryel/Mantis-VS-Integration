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
				stsLabel.Dispatcher.InvokeAsync(() => stsLabel.Content = e.StatusInfo);
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
		private async void cbxViewSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

			try
			{
				if (e.AddedItems.Count <= 0)
				{
					return;
				}
				OnUpdateStatus(LocalizationHelper.GetString("MantisConnectWindowLoadingView"), 0, true);
				contentGrid.Children.Clear();
				var v = _viewList.FirstOrDefault(vue => (vue as IView).DisplayName == cbxViewSelector.SelectedValue.ToString());
				if (!(v as IView).Initialized)
				{
					OnUpdateStatus(LocalizationHelper.GetString("MantisConnectWindowViewInitializing", (v as IView).DisplayName), 0, true);
					if (Properties.Settings.Default.ExtensionConfigured)
					{
						await Task.Run(() => (v as IView).InitializeData());
					}
					else
					{
						throw new System.Configuration.ConfigurationException(LocalizationHelper.GetString("ErrExtensionNotConfigured"));
					}
				}
				contentGrid.Children.Add(v);
				OnUpdateStatus(LocalizationHelper.GetString("MantisConnectWindowViewLoaded", (v as IView).DisplayName), 100, false);
			}
			catch (System.Configuration.ConfigurationException cer)
			{
				if (!(settingView as IView).Initialized)
				{
					settingView.InitializeData();
				}
				contentGrid.Children.Add(settingView);

				cbxViewSelector.SelectedItem = (settingView as IView).DisplayName;
				OnUpdateStatus(LocalizationHelper.GetString("ErrExtensionNotConfigured"), 100, false);
				DisplayError(cer);
			}
			catch (Exception ex)
			{
				OnUpdateStatus(LocalizationHelper.GetString("MantisConnectWindowErrorLoadingView"), 0, false);
				DisplayError(ex);
			}
		}
		private void DisplayError(Exception ex)
		{
			Microsoft.VisualStudio.PlatformUI.MessageDialog.Show(LocalizationHelper.GetString("ERROR"), ex.ToString(), Microsoft.VisualStudio.PlatformUI.MessageDialogCommandSet.Ok);
		}
		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{

			settingView = new Views.SettingsView();
			settingView.SettingsUpdated += SettingView_SettingsUpdated;
			_viewList = new List<UserControl> {
												settingView
												, new ProjectView()
												, new MantisEnumView()
												, new HomeView()};

			LocalizeUI();
			foreach (IStatusUpdater view in _viewList.OfType<IStatusUpdater>())
			{
				view.UpdateStatus += View_UpdateStatus;
			}
			OnUpdateStatus(LocalizationHelper.GetString("MantisConnectWindowLoaded"), 0, false);
		}

		private void SettingView_SettingsUpdated(object sender, EventArgs e)
		{
			LocalizeUI();
		}

		private void LocalizeUI()
		{
			int idx = cbxViewSelector.SelectedIndex;
			cbxViewSelector.DataContext = _viewList.Where(v => v is Interfaces.IView).Select( v => (v as IView).DisplayName);
			cbxViewSelector.SelectedIndex = idx;
			foreach (IView item in _viewList.OfType<IView>())
			{
				item.LocalizeUI();
			}
		}
	}
}