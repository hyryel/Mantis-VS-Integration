using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Security;
using System.Runtime.InteropServices;
using MantisConnectAPI;
namespace VSMantisConnect.Interfaces
{
    class MantisClient
    {
        #region Singleton
        private static MantisClient _instance;
        public static MantisClient Instance
        {
            get
            {
                return _instance;
            }
        }
        static MantisClient()
        {
            _instance = new MantisClient();
        }

        #endregion
        private MantisClient()
        {
        }
        private MantisConnectPortTypeClient Client
        {
            get
            {
                if (svcClient == null)
                {
                    UpdateServiceConfiguration();
                }
                return svcClient;
            }
        }
        public void UpdateServiceConfiguration()
        {
            if (svcClient != null && svcClient.State == CommunicationState.Opened)
            {
                svcClient.Close();
                svcClient = null;
            }

            System.ServiceModel.Channels.Binding b = new BasicHttpBinding();
            if (Properties.Settings.Default.ExtensionConfigured)
            {
                Uri baseUrl = new Uri(Properties.Settings.Default.BaseUrl);
                if (baseUrl.Scheme.Equals("http", StringComparison.InvariantCultureIgnoreCase))
                {
                    b = new BasicHttpBinding();
                }
                if (baseUrl.Scheme.Equals("https", StringComparison.InvariantCultureIgnoreCase))
                {
                    b = new BasicHttpsBinding();
                }
                svcClient = new MantisConnectPortTypeClient(b, _endpointAddr);
            }
        }
        public async Task<UserData> Login(string username, string password)
        {
            return await Client.mc_loginAsync(username, GetPassword(password));
        }
        public async Task<ProjectData[]> GetProjectsForUser()
        {
            return await Client.mc_projects_get_user_accessibleAsync(Properties.Settings.Default.UserName, GetPassword(Properties.Settings.Default.Password));
        }

        public async Task<IssueData[]> GetIssuesForUserByProjet(string projectId)
        {
            string pwd = GetPassword(Properties.Settings.Default.Password);
            UserData usr = await Login(Properties.Settings.Default.UserName, pwd);
            return await Client.mc_project_get_issues_for_userAsync(Properties.Settings.Default.UserName, pwd, projectId.ToString(), "assigned", usr.account_data , "0", "-1");
        }

        public async Task<Dictionary<string, ObjectRef[]>> GetAllMantisEnum()
        {
            string usr = Properties.Settings.Default.UserName;
            string pwd = GetPassword(Properties.Settings.Default.Password);

            Dictionary<string, ObjectRef[]> enums = new Dictionary<string, ObjectRef[]>();
            ObjectRef[] val = await Client.mc_enum_access_levelsAsync(usr, pwd);
            enums.Add("Access Levels", val);

            val = await Client.mc_enum_custom_field_typesAsync(usr, pwd);
            enums.Add("Custom field types", val);
            val = await Client.mc_enum_etasAsync(usr, pwd);
            enums.Add("Etas", val);
            val = await Client.mc_enum_prioritiesAsync(usr, pwd);
            enums.Add("Priorities", val);
            val = await Client.mc_enum_projectionsAsync(usr, pwd);
            enums.Add("Projections", val);
            val = await Client.mc_enum_project_statusAsync(usr, pwd);
            enums.Add("Project Status", val);
            val = await Client.mc_enum_project_view_statesAsync(usr, pwd);
            enums.Add("Project view states", val);
            val = await Client.mc_enum_reproducibilitiesAsync(usr, pwd);
            enums.Add("Reproductibilities", val);
            val = await Client.mc_enum_resolutionsAsync(usr, pwd);
            enums.Add("Resolutions", val);
            val = await Client.mc_enum_severitiesAsync(usr, pwd);
            enums.Add("Severities", val);
            val = await Client.mc_enum_statusAsync(usr, pwd);
            enums.Add("Status", val);
            val = await Client.mc_enum_view_statesAsync(usr, pwd);
            enums.Add("View states", val);

            return enums;
        }


        private string GetPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                PasswordInputDialog.Show("Password needed", "Input your password", out password);
            }
            return password;
        }
        private MantisConnectPortTypeClient svcClient;
        private System.ServiceModel.EndpointAddress _endpointAddr
        {
            get
            {
                string completeUri = "";
                if (!Properties.Settings.Default.BaseUrl.EndsWith("/") && !Properties.Settings.Default.EndPointAddress.StartsWith("/"))
                {
                    completeUri = Properties.Settings.Default.BaseUrl + "/" + Properties.Settings.Default.EndPointAddress;
                }
                else
                {
                    if (Properties.Settings.Default.BaseUrl.EndsWith("/") && Properties.Settings.Default.EndPointAddress.StartsWith("/"))
                    {
                        completeUri = Properties.Settings.Default.BaseUrl.Substring(0, Properties.Settings.Default.BaseUrl.Length - 1) + Properties.Settings.Default.EndPointAddress;
                    }
                    else
                    {
                        completeUri = Properties.Settings.Default.BaseUrl + Properties.Settings.Default.EndPointAddress;
                    }
                }
                return new EndpointAddress(completeUri);
            }
        }

    }
}
