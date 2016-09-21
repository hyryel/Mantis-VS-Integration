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
	/// Interaction logic for Settings.xaml
	/// </summary>
	public partial class SettingsView : UserControl, IStatusUpdater, IView
	{
		// Using a DependencyProperty as the backing store for HasChanged.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty HasChangedProperty = DependencyProperty.RegisterAttached("HasChanged", typeof(bool), typeof(SettingsView), new PropertyMetadata(false));
		public event EventHandler<StatusUpdatedEventArgs> UpdateStatus;

		public bool HasChanged
		{
			get { return (bool)GetValue(HasChangedProperty); }
			set
			{
				SetValue(HasChangedProperty, value);
				if (value)
					OnUpdateStatus("Settings changed. Don't forget to save", 0, false);
				UpdateSaveButtonState();
			}
		}
		public string DisplayName
		{
			get
			{
				return "{Settings}";
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
			OnUpdateStatus("Loading Settings", 0, true);
			try
			{
				this.Dispatcher.InvokeAsync(() =>
				{
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
					OnUpdateStatus("Settings loaded", 0, false);
					_initialized = true;
				});
			}
			catch (Exception ex)
			{
				OnUpdateStatus("Loading settings failed.", 0, false);
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
				Properties.Settings.Default.Language = System.Globalization.CultureInfo.GetCultureInfo("en-US");
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
				OnUpdateStatus("Settings saved", 100, false);
				HasChanged = false;
				MantisClient.Instance.UpdateServiceConfiguration();
			}
			catch (Exception ex)
			{
				Microsoft.VisualStudio.PlatformUI.MessageDialog.Show("Error", ex.Message, Microsoft.VisualStudio.PlatformUI.MessageDialogCommandSet.Ok);
			}
		}
		private void control_ValueChanged(object sender, RoutedEventArgs e)
		{
			HasChanged = true;
		}
		private async void btnTest_Click(object sender, RoutedEventArgs e)
		{
			OnUpdateStatus("Testing connection...", 0, true);
			try
			{
				UserData data = await MantisClient.Instance.Login();
				if (data != null)
				{
					Microsoft.VisualStudio.PlatformUI.MessageDialog.Show("Success", $"User {data.account_data.name} successfully connected", Microsoft.VisualStudio.PlatformUI.MessageDialogCommandSet.Ok);
				}
				OnUpdateStatus("Connection success.", 100, false);
			}
			catch (Exception ex)
			{
				Microsoft.VisualStudio.PlatformUI.MessageDialog.Show("ERROR", $"Cannot connect to Mantis. Reason : {ex.Message}", Microsoft.VisualStudio.PlatformUI.MessageDialogCommandSet.Ok);
				OnUpdateStatus("Connection failed.", 0, false);
			}
		}
		private void tbxPassword_PasswordChanged(object sender, RoutedEventArgs e)
		{
			HasChanged |= !tbxPassword.SecurePassword.Equals(Properties.Settings.Default.Password);
		}
	}
}
