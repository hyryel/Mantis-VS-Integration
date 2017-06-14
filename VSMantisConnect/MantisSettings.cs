using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSMantisConnect
{
    public static class MantisSettings
    {
        private static Microsoft.VisualStudio.Shell.Interop.IVsSettingsManager _manager;
        private static Microsoft.VisualStudio.Shell.Interop.IVsWritableSettingsStore _store;
        private const string _collectionPath = "VSMantisConnect";
        static MantisSettings()
        {
            _manager = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(Microsoft.VisualStudio.Shell.Interop.SVsSettingsManager)) as Microsoft.VisualStudio.Shell.Interop.IVsSettingsManager;
            if (_manager != null)
            {
                if (_manager.GetWritableSettingsStore((uint)Microsoft.VisualStudio.Shell.Interop.__VsSettingsScope.SettingsScope_UserSettings, out _store) == Microsoft.VisualStudio.VSConstants.S_OK)
                {
                    _store.CollectionExists(_collectionPath, out int exists);
                    if (exists == 0) //store doesn't exists
                    {
                        _store.CreateCollection(_collectionPath);
                    }
                    LoadFromConfigurationFile();
                }
                else
                {
                    //beurk, ça plante.... :(
                }
            }
        }

        public static void LoadFromConfigurationFile()
        {
            //initialization des vairables
            int value;
            _store.GetBoolOrDefault(_collectionPath, "ExtensionConfigured", 0, out value);
            _extensionConfigured = value > 0;
            _store.GetBoolOrDefault(_collectionPath, "CustomizeEndPoint", 0, out value);
            _customizeEndPoint = value > 0;
            _store.GetBoolOrDefault(_collectionPath, "SavePassword", 0, out value);
            _savePassword = value > 0;
            _store.GetStringOrDefault(_collectionPath, "UserName", "Your Mantis username", out _userName);
            _store.GetStringOrDefault(_collectionPath, "Password", "", out _password);
            _store.GetStringOrDefault(_collectionPath, "BaseURL", "Your Mantis base url", out _baseURL);
            _store.GetStringOrDefault(_collectionPath, "EndPointAddress", "/api/soap/mantisconnect.php", out _enPointAddress);
            string sValue;
            _store.GetStringOrDefault(_collectionPath, "Language", "en-US", out sValue);
            _language = new System.Globalization.CultureInfo(sValue);
        }

        private static bool _extensionConfigured;
        public static bool ExtensionConfigured
        {
            get
            {
                return _extensionConfigured;
            }
            set
            {
                _extensionConfigured = value;
            }
        }
        private static bool _customizeEndPoint;
        public static bool CustomizeEndPoint
        {
            get
            {
                return _customizeEndPoint;
            }
            set
            {
                _customizeEndPoint = value;
            }
        }
        private static bool _savePassword;
        public static bool SavePassword
        {
            get
            {
                return _savePassword;
            }
            set
            {
                _savePassword = value;
            }
        }
        private static string _userName;
        public static string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
            }
        }
        private static string _password;
        public static string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
            }
        }
        private static string _baseURL;
        public static string BaseURL
        {
            get
            {
                return _baseURL;
            }
            set
            {
                _baseURL = value;
            }
        }
        private static string _enPointAddress;
        public static string EndPointAddress
        {
            get
            {
                return _enPointAddress;
            }
            set
            {
                _enPointAddress = value;
            }
        }
        private static System.Globalization.CultureInfo _language;
        public static System.Globalization.CultureInfo Language
        {
            get
            {
                return _language;
            }
            set
            {
                _language = value;
            }
        }

        public static void Save()
        {
            if (_store != null)
            {
                _store.SetBool(_collectionPath, "ExtensionConfigured", _extensionConfigured ? 1 : 0);
                _store.SetBool(_collectionPath, "CustomizeEndPoint", _customizeEndPoint ? 1 : 0);
                _store.SetBool(_collectionPath, "SavePassword", _savePassword ? 1 : 0);
                _store.SetString(_collectionPath, "UserName", _userName);
                if (_savePassword)
                {
                    _store.SetString(_collectionPath, "Password", _password);
                }
                else
                {
                    _store.SetString(_collectionPath, "Password", string.Empty);
                }
                _store.SetString(_collectionPath, "BaseURL", _baseURL);
                if (_customizeEndPoint)
                {
                    _store.SetString(_collectionPath, "EndPointAddress", _enPointAddress);
                }
                else
                {
                    _store.SetString(_collectionPath, "EndPointAddress", "/api/soap/mantisconnect.php");
                }
                _store.SetString(_collectionPath, "Language", _language.Name);
            }

        }
    }
}
