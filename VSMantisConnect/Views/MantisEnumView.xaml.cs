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
	/// Interaction logic for MantisEnumView.xaml
	/// </summary>
	public partial class MantisEnumView : UserControl, IView, IStatusUpdater
	{
		public MantisEnumView()
		{
			InitializeComponent();
			_initialized = false;
		}

		public string DisplayName
		{
			get
			{
				return "Mantis enum";
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

		public event EventHandler<StatusUpdatedEventArgs> UpdateStatus;

		private void OnUpdateStatus(string info, double percentage, bool isIndeterminate)
		{
			if (UpdateStatus != null)
			{
				UpdateStatus(this, new Interfaces.StatusUpdatedEventArgs(info, percentage, isIndeterminate));
			}
		}

		public async void InitializeData()
		{
			try
			{
				OnUpdateStatus("Loading Mantis enum...", 0, true);
				if (Properties.Settings.Default.ExtensionConfigured)
				{
					var enums = await MantisClient.Instance.GetAllMantisEnum();
					await lstEnums.Dispatcher.InvokeAsync(() => lstEnums.DataContext = enums);
				}
				OnUpdateStatus("Mantis enum loaded", 100, false);
				_initialized = true;
			}
			catch (Exception ex)
			{
				_initialized = false;
				OnUpdateStatus("Error loading Mantis enum", 0, false);
				throw ex;
			}
		}
	}
}
