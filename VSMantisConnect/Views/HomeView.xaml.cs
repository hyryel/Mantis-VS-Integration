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
	/// Interaction logic for HomeView.xaml
	/// </summary>
	public partial class HomeView : UserControl, IView, IStatusUpdater
	{
		public HomeView()
		{
			InitializeComponent();
		}

		public string DisplayName
		{
			get
			{
				return "#Home view#";
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
				UpdateStatus(this, new StatusUpdatedEventArgs(info, percentage, isIndeterminate));
			}
		}

		public Task<Exception> InitializeData()
		{
			//TODO : initialize tickets
			return null;
		}

		public void LocalizeUI()
		{

		}
	}
}
