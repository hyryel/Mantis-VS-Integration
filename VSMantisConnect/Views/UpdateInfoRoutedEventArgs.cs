using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VSMantisConnect.Views
{
	public delegate void UpdateInfoRoutedEventHandler(object sender, UpdateInfoRoutedEventArgs e);
	public class UpdateInfoRoutedEventArgs : RoutedEventArgs
	{
		public string Message { get; set; }
		public double Percentage { get; set; }
		public bool IsIndeterminate { get; set; }

		public UpdateInfoRoutedEventArgs(RoutedEvent evt, string msg, double percent, bool isIndeterminate)
		{
			RoutedEvent = evt;
			Message = msg;
			Percentage = percent;
			IsIndeterminate = IsIndeterminate;
		}
	}
}
