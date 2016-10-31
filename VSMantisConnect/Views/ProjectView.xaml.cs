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
	public partial class ProjectView : UserControl, Interfaces.IStatusUpdater, Interfaces.IView, ILocalizable
	{
		public ProjectView()
		{
			InitializeComponent();
			_initialized = false;
		}

		public async Task<Exception> InitializeData()
		{
			try
			{
				OnUpdateStatus(LocalizationHelper.GetString("ProjectViewLoading"), 0, true);
				Exception ex = await lstIssues.UpdateList();
				if (ex != null)
				{
					_initialized = false;
					throw ex;
				}
				_initialized = true;
				OnUpdateStatus(LocalizationHelper.GetString("ProjectViewLoaded"), 100, false);
				return null;
			}
			catch (Exception ex)
			{
				OnUpdateStatus(LocalizationHelper.GetString("ProjectViewErrorLoading"), 0, false);
				_initialized = false;
				return ex;
			}

		}

		#region Interface implementation
		public string DisplayName
		{
			get
			{
				return LocalizationHelper.GetString("ProjectViewDisplayName");
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
		public void LocalizeUI()
		{
			lstIssues.LocalizeUIElement(this);
			detIssue.LocalizeUIElement(this);
		}

	}
}
