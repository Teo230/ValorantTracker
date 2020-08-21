using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ValorantTracker.Client.API;
using ValorantTracker.Client.Models;
using ValorantTracker.Client.Utilities;

namespace ValorantTracker.Client.Model
{
    public class MainWindowModel
    {
        #region Prop
        private HttpRequest _httpRequest;
        
        public UserInfoDTO playerInfo;
        public List<PlayerDTO> player;
        public Match match;
        public UserBalanceDTO userBalance;
        public EventHandler<EventValArgs> elaborationCompleted;
        #endregion

        #region Ctr
        public MainWindowModel()
        {
        }
        #endregion

        #region Methods    
        public void GetPlayerId()
        {
            var task = new Task(async () =>
            {
                _httpRequest = new HttpRequest();
                dynamic data = new JObject();
                playerInfo = _httpRequest.Post<UserInfoDTO>($"{GlobalManager.GetRightEndpoint(GlobalEnum.EndpointsEnum.AuthRiot)}userinfo",data);
                ElaborationCompleted(new EventValArgs { PlayerIdReceived = playerInfo != null});
            });
            task.Start();
        }

        public void GetPlayer()
        {
            var task = new Task(async () =>
            {
                _httpRequest = new HttpRequest();
                dynamic data = new JObject();
                data = "[\"" + playerInfo.sub + "\"]";
                player = _httpRequest.Put<List<PlayerDTO>>($"{GlobalManager.GetRightEndpoint(GlobalManager.Region)}name-service/v2/players", data);
                ElaborationCompleted(new EventValArgs { PlayerReceived = playerInfo != null });
            });
            task.Start();
        }

        public void GetMatchHistory()
        {
            _httpRequest = new HttpRequest();
            match = _httpRequest.Get<Match>($"{GlobalManager.GetRightEndpoint(GlobalManager.Region)}match-history/v1/history/{GlobalManager.Player.Subject}");
            ElaborationCompleted(new EventValArgs { MatchReceived = playerInfo != null });
        }

        public void GetUserBalance()
        {
            _httpRequest = new HttpRequest();
            userBalance = _httpRequest.Get<UserBalanceDTO>($"{GlobalManager.GetRightEndpoint(GlobalManager.Region)}store/v1/wallet/{GlobalManager.Player.Subject}");
            ElaborationCompleted(new EventValArgs { BalanceReceived = userBalance != null });
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
