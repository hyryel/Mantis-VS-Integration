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

namespace VSMantisConnect
{
    /// <summary>
    /// Interaction logic for PasswordInputDialog.xaml
    /// </summary>
    public partial class PasswordInputDialog : Microsoft.VisualStudio.PlatformUI.DialogWindow
    {
        public static void Show(string title, string prompt, out string password)
        {
            PasswordInputDialog dlg = new VSMantisConnect.PasswordInputDialog();
            dlg.Title = title;
            dlg.lblPrompt.Text = prompt;
            if (dlg.ShowModal().Value)
                password = dlg.Password;
            else
                password = null;
        }
        private PasswordInputDialog()
        {
            InitializeComponent();
            tbxPassword.Focus();
        }
        public string Password => tbxPassword.Password;
        private void DialogButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
