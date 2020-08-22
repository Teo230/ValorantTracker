using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ValorantTracker.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ValorantTracker.Client.Properties;
using static ValorantTracker.Client.Utilities.GlobalEnum;
using Valorant.Rest.API;

namespace ValorantTracker.Client.API
{
    public class HttpRequest
    {
        #region Prop
        private HttpClient session { get; set; }
        private ValorantClient valorantClient { get; set; }
        private string clientVersion { get; set; }
        #endregion

        #region Ctr
        public HttpRequest()
        {
            session = new HttpClient();

            if(!session.DefaultRequestHeaders.Contains("Authorization") && !string.IsNullOrEmpty(GlobalManager.BearerToken))
                session.DefaultRequestHeaders.Add("Authorization", "Bearer " + GlobalManager.BearerToken);

            if (!session.DefaultRequestHeaders.Contains("X-Riot-Entitlements-JWT") && !string.IsNullOrEmpty(GlobalManager.X_Riot_Entitlements_JWT))
                session.DefaultRequestHeaders.Add("X-Riot-Entitlements-JWT", GlobalManager.X_Riot_Entitlements_JWT);

            clientVersion = GlobalManager.clientVersion;
        }
        #endregion

        #region Methods

        #region HttpRequestMethods
        public T Post<T>(string url, dynamic data)
        {
            var result = session.PostAsync(url, new StringContent(data.ToString(), Encoding.UTF8, "application/json")).Result;

            if (result.IsSuccessStatusCode)
            {
                var resultString = result.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(resultString);
            }
            else
            {
                if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    throw new Exception();
                throw new Exception("POST Call failure");
            }
        }

        public T Get<T>(string url)
        {
            var result = session.GetAsync(url).Result;
            if (result.IsSuccessStatusCode)
            {
                var resultString = result.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(resultString);
            }
            else
            {
                if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    throw new Exception();
                throw new Exception("GET Call failure");
            }
        }

        public T Put<T>(string url, dynamic data)
        {
            var result = session.PutAsync(url, new StringContent(data.ToString(), Encoding.UTF8, "application/json")).Result;

            if (result.IsSuccessStatusCode)
            {
                var resultString = result.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(resultString);
            }
            else
            {
                if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    throw new Exception();
                throw new Exception("PUT Call failure");
            }
        }

        public bool PerfomLogin(string username, string password, bool rememberMe)
        {
            valorantClient = new ValorantClient(session, clientVersion);
            string userConfig = valorantClient.GetUserParameters(username,password).response.parameters.uri;

            string[] config = userConfig.ToString().Remove(0, userConfig.IndexOf('#') + 1).Split('&');
            GlobalManager.BearerToken = config[0].Remove(0, config[0].IndexOf('=') + 1);
            double seconds = Convert.ToDouble(config[4].Remove(0, config[4].IndexOf('=') + 1));
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            GlobalManager.ExpiresDateTime = DateTime.Now.Add(time);
            session.DefaultRequestHeaders.Add("Authorization", "Bearer " + GlobalManager.BearerToken);
            valorantClient = new ValorantClient(session, clientVersion);

            GlobalManager.X_Riot_Entitlements_JWT = valorantClient.GetEntitlementsToken();
            session.DefaultRequestHeaders.Add("X-Riot-Entitlements-JWT", GlobalManager.X_Riot_Entitlements_JWT);

            GlobalManager.settings = new UserSettings();
            GlobalManager.settings.Password = password;
            GlobalManager.settings.Username = username;
            GlobalManager.settings.RememberMe = rememberMe;
            GlobalManager.settings.Region = GlobalManager.Region;
            GlobalManager.SaveUserInfo();

            StartTimerExpires(username, password, rememberMe);
            return true;
        }

        private void StartTimerExpires(string username, string password, bool rememberMe)
        {
            var task = new Task(async () =>
            {
                while (GlobalManager.ExpiresDateTime > DateTime.Now)
                {

                }

                if (rememberMe && GlobalManager.ExpiresDateTime != null)
                {
                    PerfomLogin(username, password, rememberMe);
                }
            });

            task.Start();
        }
        #endregion

        #endregion
    }
}
