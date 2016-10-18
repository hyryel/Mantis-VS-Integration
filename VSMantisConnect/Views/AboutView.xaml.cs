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
	/// Interaction logic for AboutView.xaml
	/// </summary>
	public partial class AboutView : UserControl, Interfaces.IView, Interfaces.ILocalizable
	{
		public AboutView()
		{
			InitializeComponent();
		}

		public string DisplayName
		{
			get
			{
				return VSMantisConnect.LocalizationHelper.GetString("AboutViewName");
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

		public Task<Exception> InitializeData()
		{
			//throw new NotImplementedException();
			return null;
		}

		public void LocalizeUI()
		{
			//throw new NotImplementedException();
		}
	}
}
