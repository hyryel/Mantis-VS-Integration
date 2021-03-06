﻿using System;
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
	public class MantisClient
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
		private string _usr { get { return MantisSettings.UserName; } }
		private string _pwd { get { return ValidatePassword(MantisSettings.Password); } }
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
			
			if (MantisSettings.ExtensionConfigured)
			{
				Uri baseUrl = new Uri(MantisSettings.BaseURL);
				if (baseUrl.Scheme.Equals("http", StringComparison.InvariantCultureIgnoreCase))
				{
					b = new BasicHttpBinding();
					((BasicHttpBinding)b).MaxReceivedMessageSize = 5 * 1024 * 1024;
				}
				if (baseUrl.Scheme.Equals("https", StringComparison.InvariantCultureIgnoreCase))
				{
					b = new BasicHttpsBinding();
					((BasicHttpsBinding)b).MaxReceivedMessageSize = 5 * 1024 * 1024;

				}
				svcClient = new MantisConnectPortTypeClient(b, _endpointAddr);
			}
		}
		public async Task<UserData> Login()
		{
			try
			{
				return await Client.mc_loginAsync(_usr, _pwd);
			}
			catch (System.ServiceModel.FaultException ex)
			{
				if (ex.Message.Equals("Access denied", StringComparison.InvariantCultureIgnoreCase))
				{
					_invalidPassword = true;
				}
				throw;
			}
		}
		public async Task<ProjectData[]> GetProjectsForUser()
		{
			try
			{
				return await Client.mc_projects_get_user_accessibleAsync(_usr, _pwd);
			}
			catch (System.ServiceModel.FaultException ex)
			{
				if (ex.Message.Equals("Access denied", StringComparison.InvariantCultureIgnoreCase))
				{
					_invalidPassword = true;
				}
				throw;
			}
		}

		public async Task<IssueData[]> GetIssuesForUserByProjet(string projectId)
		{
			try
			{
				UserData usr = await Login();
				return await Client.mc_project_get_issues_for_userAsync(_usr, _pwd, projectId.ToString(), "assigned", usr.account_data, "0", "-1");
			}
			catch (System.ServiceModel.FaultException ex)
			{
				if (ex.Message.Equals("Access denied", StringComparison.InvariantCultureIgnoreCase))
				{
					_invalidPassword = true;
				}
				throw;
			}
		}

		public async Task<IssueData> GetIssueById(string issueId)
		{
			try
			{
				return await Client.mc_issue_getAsync(_usr, _pwd, issueId);
			}
			catch (System.ServiceModel.FaultException ex)
			{
				if (ex.Message.Equals("Access denied", StringComparison.InvariantCultureIgnoreCase))
				{
					_invalidPassword = true;
				}
				throw;
			}
		}

		public async Task<Dictionary<string, ObjectRef[]>> GetAllMantisEnum()
		{
			Dictionary<string, ObjectRef[]> enums = new Dictionary<string, ObjectRef[]>();
			ObjectRef[] val = await Client.mc_enum_access_levelsAsync(_usr, _pwd);
			enums.Add("Access Levels", val);

			val = await Client.mc_enum_custom_field_typesAsync(_usr, _pwd);
			enums.Add("Custom field types", val);
			val = await Client.mc_enum_etasAsync(_usr, _pwd);
			enums.Add("Etas", val);
			val = await Client.mc_enum_prioritiesAsync(_usr, _pwd);
			enums.Add("Priorities", val);
			val = await Client.mc_enum_projectionsAsync(_usr, _pwd);
			enums.Add("Projections", val);
			val = await Client.mc_enum_project_statusAsync(_usr, _pwd);
			enums.Add("Project Status", val);
			val = await Client.mc_enum_project_view_statesAsync(_usr, _pwd);
			enums.Add("Project view states", val);
			val = await Client.mc_enum_reproducibilitiesAsync(_usr, _pwd);
			enums.Add("Reproductibilities", val);
			val = await Client.mc_enum_resolutionsAsync(_usr, _pwd);
			enums.Add("Resolutions", val);
			val = await Client.mc_enum_severitiesAsync(_usr, _pwd);
			enums.Add("Severities", val);
			val = await Client.mc_enum_statusAsync(_usr, _pwd);
			enums.Add("Status", val);
			val = await Client.mc_enum_view_statesAsync(_usr, _pwd);
			enums.Add("View states", val);

			return enums;
		}
		public async Task<FilterData[]> GetFilterForProject(string projectId)
		{
			try
			{
				return await Client.mc_filter_getAsync(_usr, _pwd, projectId);
			}
			catch (System.ServiceModel.FaultException ex)
			{
				if (ex.Message.Equals("Access denied", StringComparison.InvariantCultureIgnoreCase))
				{
					_invalidPassword = true;
				}
				throw;
			}
		}
		public async Task<string> AddNoteToIssue(string issueId, IssueNoteData newNote)
		{
			try
			{
				var usr = await Login();
				newNote.reporter = usr.account_data;
				newNote.date_submitted = DateTime.Now;
				return await Client.mc_issue_note_addAsync(_usr, _pwd, issueId, newNote);
			}
			catch (System.ServiceModel.FaultException ex)
			{
				if (ex.Message.Equals("Access denied", StringComparison.InvariantCultureIgnoreCase))
				{
					_invalidPassword = true;
				}
				throw;
			}
		}

		private string __pwd;
		private TimeSpan _pwdExpiration;
		private bool _invalidPassword;
		private string ValidatePassword(string password)
		{
			if (!string.IsNullOrWhiteSpace(password))
			{
				return password;
			}
			if (_pwdExpiration.Ticks < DateTime.Now.Ticks)
			{
				__pwd = "";
			}
			if (string.IsNullOrWhiteSpace(__pwd) || _invalidPassword)
			{
				_pwdExpiration = new TimeSpan(DateTime.Now.AddMinutes(15).Ticks);
				PasswordInputDialog.Show(LocalizationHelper.GetString("PasswordBoxTitle"), LocalizationHelper.GetString("PasswordBoxMessage"), out password);
				__pwd = password;
				_invalidPassword = false;
			}
			return __pwd;
		}
		private MantisConnectPortTypeClient svcClient;
		private System.ServiceModel.EndpointAddress _endpointAddr
		{
			get
			{
				string completeUri = "";
				if (!MantisSettings.BaseURL.EndsWith("/") && !MantisSettings.EndPointAddress.StartsWith("/"))
				{
					completeUri = MantisSettings.BaseURL + "/" + MantisSettings.EndPointAddress;
				}
				else
				{
					if (MantisSettings.BaseURL.EndsWith("/") && MantisSettings.EndPointAddress.StartsWith("/"))
					{
						completeUri = MantisSettings.BaseURL.Substring(0, MantisSettings.BaseURL.Length - 1) + MantisSettings.EndPointAddress;
					}
					else
					{
						completeUri = MantisSettings.BaseURL + MantisSettings.EndPointAddress;
					}
				}
				return new EndpointAddress(completeUri);
			}
		}
	}
}
