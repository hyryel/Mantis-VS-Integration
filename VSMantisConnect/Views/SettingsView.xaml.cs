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
using System.Globalization;

namespace VSMantisConnect.Views
{
	/// <summary>
	/// Interaction logic for Settings.xaml
	/// </summary>
	public partial class SettingsView : UserControl, IStatusUpdater, IView
	{
		// Using a DependencyProperty as the backing store for HasChanged.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty HasChangedProperty = DependencyProperty.RegisterAttached("HasChanged", typeof(bool), typeof(SettingsView), new PropertyMetadata(false));
		public event EventHandler<StatusUpdatedEventArgs> UpdateStatus;
		public event EventHandler SettingsUpdated;
		public bool HasChanged
		{
			get { return (bool)GetValue(HasChangedProperty); }
			set
			{
				SetValue(HasChangedProperty, value);
				if (value)
					OnUpdateStatus(LocalizationHelper.GetString("SettingsViewSettingsChanged"), 0, false);
				UpdateSaveButtonState();
			}
		}
		public string DisplayName
		{
			get
			{
				return LocalizationHelper.GetString("SettingsViewDisplayName");
			}
		}
		bool _initialized;
		bool IView.Initialized
		{
			get
			{
				return _initialized;
			}
		}

		private void OnUpdateStatus(string info, double percentage, bool isIndeterminate)
		{
			if (UpdateStatus != null)
			{
				UpdateStatus(this, new StatusUpdatedEventArgs(info, percentage, isIndeterminate));
			}
		}
		public SettingsView()
		{

			InitializeComponent();
			_initialized = false;
		}

		public void InitializeData()
		{
			OnUpdateStatus(LocalizationHelper.GetString("SettingsViewLoading"), 0, true);
			try
			{
				this.Dispatcher.InvokeAsync(() =>
				{
					cbxLanguage.DataContext = new CultureInfo[] { new CultureInfo("fr"), new CultureInfo("en") };
					cbxLanguage.SelectedItem = Properties.Settings.Default.Language;
					tbxBaseUrl.Text = Properties.Settings.Default.BaseUrl;
					tbxEndPointAddress.Text = "/api/soap/mantisconnect.php";
					if (Properties.Settings.Default.CustomizeEndPointAddress)
					{
						tbxEndPointAddress.Text = Properties.Settings.Default.EndPointAddress;
					}
					tbxPassword.Password = "";
					if (Properties.Settings.Default.SavePassword)
					{
						tbxPassword.Password = Properties.Settings.Default.Password;
					}
					tbxUsername.Text = Properties.Settings.Default.UserName;
					ckbCustomizeEndPoint.IsChecked = Properties.Settings.Default.CustomizeEndPointAddress;
					ckbSavePassword.IsChecked = Properties.Settings.Default.SavePassword;
					HasChanged = false;
					OnUpdateStatus(LocalizationHelper.GetString("SettingsViewLoaded"), 0, false);
					_initialized = true;
				});
			}
			catch (Exception ex)
			{
				OnUpdateStatus(LocalizationHelper.GetString("SettingsViewErrorLoading"), 0, false);
				_initialized = false;
				throw ex;
			}
		}
		private void tbx_TextChanged(object sender, TextChangedEventArgs e)
		{
			HasChanged = true;
		}
		private void UpdateSaveButtonState()
		{
			btnSave.IsEnabled = HasChanged;
		}
		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Properties.Settings.Default.BaseUrl = tbxBaseUrl.Text;
				Properties.Settings.Default.CustomizeEndPointAddress = ckbCustomizeEndPoint.IsChecked.Value;
				if (ckbCustomizeEndPoint.IsChecked.Value)
				{
					Properties.Settings.Default.EndPointAddress = tbxEndPointAddress.Text;
				}
				else
				{
					Properties.Settings.Default.EndPointAddress = "/api/soap/mantisconnect.php";
				}
				Properties.Settings.Default.ExtensionConfigured = true;
				Properties.Settings.Default.Language = cbxLanguage.SelectedItem as CultureInfo;
				if (ckbSavePassword.IsChecked.Value)
				{
					//TODO : encrypt password and create GetPassword and SetPassword
					Properties.Settings.Default.Password = tbxPassword.Password;
				}
				else
				{
					Properties.Settings.Default.Password = "";
				}
				Properties.Settings.Default.SavePassword = ckbSavePassword.IsChecked.Value;
				Properties.Settings.Default.UserName = tbxUsername.Text;
				Properties.Settings.Default.Save();
				OnUpdateStatus(LocalizationHelper.GetString("SettingsViewSettingsSaved"), 100, false);
				HasChanged = false;
				if (SettingsUpdated != null)
				{
					SettingsUpdated(this, EventArgs.Empty);
				}
				MantisClient.Instance.UpdateServiceConfiguration();
			}
			catch (Exception ex)
			{
				Microsoft.VisualStudio.PlatformUI.MessageDialog.Show(LocalizationHelper.GetString("ERROR"), ex.Message, Microsoft.VisualStudio.PlatformUI.MessageDialogCommandSet.Ok);
			}
		}
		private void control_ValueChanged(object sender, RoutedEventArgs e)
		{
			HasChanged = true;
		}
		private async void btnTest_Click(object sender, RoutedEventArgs e)
		{
			OnUpdateStatus(LocalizationHelper.GetString("SettingsViewTestingConnection"), 0, true);
			try
			{
				UserData data = await MantisClient.Instance.Login();
				if (data != null)
				{
					Microsoft.VisualStudio.PlatformUI.MessageDialog.Show(LocalizationHelper.GetString("SUCCESS"), LocalizationHelper.GetString("SettingsViewTestingSuccessMessage", data.account_data.name), Microsoft.VisualStudio.PlatformUI.MessageDialogCommandSet.Ok);
				}
				OnUpdateStatus(LocalizationHelper.GetString("SettingsViewTestingSuccess"), 100, false);
			}
			catch (Exception ex)
			{
				Microsoft.VisualStudio.PlatformUI.MessageDialog.Show(LocalizationHelper.GetString("ERROR"), LocalizationHelper.GetString("SettingsViewTestingFailedMessage", ex.Message), Microsoft.VisualStudio.PlatformUI.MessageDialogCommandSet.Ok);
				OnUpdateStatus(LocalizationHelper.GetString("SettingsViewTestingFailed"), 0, false);
			}
		}
		private void tbxPassword_PasswordChanged(object sender, RoutedEventArgs e)
		{
			HasChanged |= !tbxPassword.SecurePassword.Equals(Properties.Settings.Default.Password);
		}

		public void LocalizeUI()
		{
			lblBaseUrl.LocalizeUIElement(this);
			lblEndpoint.LocalizeUIElement(this);
			lblPassword.LocalizeUIElement(this);
			lblUsername.LocalizeUIElement(this);
			lblLanguage.LocalizeUIElement(this);
			ckbCustomizeEndPoint.LocalizeUIElement(this);
			ckbSavePassword.LocalizeUIElement(this);
			btnSave.LocalizeUIElement(this);
			btnTest.LocalizeUIElement(this);
		}

		private void cbxLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			HasChanged = true;
		}
	}
}
