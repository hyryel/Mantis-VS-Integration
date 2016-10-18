using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSMantisConnect.Views
{
	[System.Windows.Data.ValueConversion(typeof(string), typeof(string), ParameterType = typeof(int))]
	public class StringShrinker : System.Windows.Data.IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is string))
			{
				throw new ArgumentException("Parameter 'value' must be of type System.String", "value");
			}
			int param;
			if (!Int32.TryParse(parameter.ToString(), out param))
			{
				throw new ArgumentException("Parameter 'parameter' must be of type System.Int32", "parameter");
			}
			string svalue = value.ToString();
			if (svalue.Length < param)
			{
				return svalue;
			}
			else
			{
				return value.ToString().Substring(0, param > 5 ? param - 5 : param) + "(...)";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException("Cannot convert back from shrinked string to full length string");
		}
	}
}
