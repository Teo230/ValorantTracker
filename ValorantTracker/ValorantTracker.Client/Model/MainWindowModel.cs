using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Valorant.Rest.API;
using Valorant.Rest.API.ModelDTO;
using ValorantTracker.Client.API;
using ValorantTracker.Client.Models;
using ValorantTracker.Client.Utilities;

namespace ValorantTracker.Client.Model
{
    public class MainWindowModel
    {
        #region Prop
        public HttpClient _httpClient;
        private ValorantClient _valorantClient;
        
        public UserInfoDTO playerInfo;
        public PlayerDTO player;
        public Valorant.Rest.API.ModelDTO.MatchDTO match;
        public CompetitiveMatchDTO competitiveMatch;
        public PlayerStoreDTO store;
        public BalanceDTO userBalance;
        public IdListDTO contentList;
        public EventHandler<EventValArgs> elaborationCompleted;
        #endregion

        #region Ctr
        public MainWindowModel()
        {
            _httpClient = new HttpClient();

            if (!_httpClient.DefaultRequestHeaders.Contains("Authorization") && !string.IsNullOrEmpty(GlobalManager.BearerToken))
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + GlobalManager.BearerToken);

            if (!_httpClient.DefaultRequestHeaders.Contains("X-Riot-Entitlements-JWT") && !string.IsNullOrEmpty(GlobalManager.X_Riot_Entitlements_JWT))
                _httpClient.DefaultRequestHeaders.Add("X-Riot-Entitlements-JWT", GlobalManager.X_Riot_Entitlements_JWT);

            _valorantClient = new ValorantClient(_httpClient, GlobalManager.clientVersion);
        }
        #endregion

        #region Methods    
        public void GetPlayerId()
        {
            var task = new Task(async () =>
            {
                var httprequest = new HttpRequest();
                dynamic data = new JObject();
                playerInfo = httprequest.Post<UserInfoDTO>($"https://auth.riotgames.com/userinfo",data);
                ElaborationCompleted(new EventValArgs { PlayerIdReceived = playerInfo != null});
            });
            task.Start();
        }

        public void GetPlayer()
        {
            var task = new Task(async () =>
            {
                player = _valorantClient.GetPlayer(GlobalManager.GetRightEndpoint(GlobalManager.Region));
                ElaborationCompleted(new EventValArgs { PlayerReceived = player != null });
            });
            task.Start();
        }

        public void GetMatchHistory()
        {
            var task = new Task(async () =>
            {
                match = _valorantClient.GetMatches(GlobalManager.GetRightEndpoint(GlobalManager.Region), GlobalManager.Player.PlayerId);
                ElaborationCompleted(new EventValArgs { MatchReceived = match != null });
            });
            task.Start();
        }

        public void GetUserBalance()
        {
            var task = new Task(async () =>
            {
                userBalance = _valorantClient.GetBalance(GlobalManager.GetRightEndpoint(GlobalManager.Region), GlobalManager.Player.PlayerId);
                ElaborationCompleted(new EventValArgs { BalanceReceived = userBalance != null });;
            });
            task.Start();

        }

        public void GetCompetitiveMatchHistory()
        {
            var task = new Task(async () =>
            {
                competitiveMatch = _valorantClient.GetCompetitiveMatch(GlobalManager.GetRightEndpoint(GlobalManager.Region), GlobalManager.Player.PlayerId);
                ElaborationCompleted(new EventValArgs { CompMatchReceived = competitiveMatch != null });
            });
            task.Start();
        }

        public void GetStore()
        {
            var task = new Task(async () =>
            {
                store = _valorantClient.GetPlayerStore(GlobalManager.GetRightEndpoint(GlobalManager.Region), GlobalManager.Player.PlayerId);
                ElaborationCompleted(new EventValArgs { StoreReceived = store != null });
            });
            task.Start();
        }

        public void GetContent()
        {
            var task = new Task(async () =>
            {
                contentList = _valorantClient.GetIDList(GlobalManager.GetRightEndpoint(GlobalManager.Region));
                ElaborationCompleted(new EventValArgs { ContentReceived = contentList != null });
            });
            task.Start();
        }
        #endregion

        #region Events
        private void ElaborationCompleted(EventValArgs args)
        {
            this.elaborationCompleted?.Invoke(this, args);
        }
        #endregion
    }
}
