using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VSMantisConnect.Interfaces;

namespace VSMantisConnect
{
	public static class LocalizationHelper
	{
		public static string GetString(string keyName, params object[] oparams)
		{
			try
			{
				return string.Format(Resources.VSMantisConnect.ResourceManager.GetString(keyName, Properties.Settings.Default.Language), oparams);
			}
			catch (Exception)
			{
				try
				{
					return string.Format(Resources.VSMantisConnect.ResourceManager.GetString(keyName), oparams);
				}
				catch (Exception)
				{
					return $"Missing {keyName} localization";
				}
				throw;
			}
		}

		public static void LocalizeUIElement<T>(this T element, FrameworkElement baseParent) where T : FrameworkElement
		{
			try
			{
				if (element is ILocalizable)
				{
					(element as ILocalizable).LocalizeUI();
					return;
				}
				if (element is ContentControl)
				{
					(element as ContentControl).Content = GetString(baseParent.GetType().Name + element.Name);
					return;
				}
				if (element is TextBlock)
				{
					(element as TextBlock).Text = GetString(baseParent.GetType().Name + element.Name);
					return;
				}
			}
			catch (Exception)
			{
				//do nothing, add loggin info later
			}
		}
	}
}
