using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Valorant.Rest.API.ModelDTO;
using ValorantTracker.Client.API;
using ValorantTracker.Client.Model;
using ValorantTracker.Client.Models;
using ValorantTracker.Client.Popups;
using ValorantTracker.Client.Utilities;
using ValorantTracker.Client.View;

namespace ValorantTracker.Client.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Prop
        public MainWindowModel _windowModel;
        public MainWindow _mainWindow;

        private string _loginText = "Login";
        public string LoginText
        {
            get => _loginText;

            set
            {
                if (_loginText != value)
                {
                    _loginText = value;
                    NotifyPropertyChanged(nameof(LoginText));
                    NotifyPropertyChanged(nameof(LoginEnable));
                }
            }
        }

        private List<Match.HistoryDTO> _matchHistory;
        public List<Match.HistoryDTO> MatchHistory
        {
            get => _matchHistory;

            set
            {
                if (_matchHistory != value)
                {
                    _matchHistory = value;
                    NotifyPropertyChanged(nameof(MatchHistory));
                }
            }
        }

        private List<Match.HistoryDTO> _competitiveMatchHistory;
        public List<Match.HistoryDTO> CompetitiveMatchHistory
        {
            get => _competitiveMatchHistory;

            set
            {
                if (_competitiveMatchHistory != value)
                {
                    _competitiveMatchHistory = value;
                    NotifyPropertyChanged(nameof(CompetitiveMatchHistory));
                }
            }
        }

        private List<Offers> _offers;
        public List<Offers> Offers
        {
            get => _offers;

            set
            {
                if (_offers != value)
                {
                    _offers = value;
                    NotifyPropertyChanged(nameof(Offers));
                }
            }
        }

        private List<Offers> _featured;
        public List<Offers> Featured
        {
            get => _featured;

            set
            {
                if (_featured != value)
                {
                    _featured = value;
                    NotifyPropertyChanged(nameof(Featured));
                }
            }
        }

        private Visibility _logoutVisibility = Visibility.Collapsed;
        public Visibility LogoutVisibility
        {
            get => _logoutVisibility;

            set
            {
                if (_logoutVisibility != value)
                {
                    _logoutVisibility = value;
                    NotifyPropertyChanged(nameof(LogoutVisibility));
                }
            }
        }

        public bool LoginEnable => LoginText == "Login";

        private int _valorantPoints;
        public int ValorantPoints
        {
            get => _valorantPoints;

            set
            {
                if (_valorantPoints != value)
                {
                    _valorantPoints = value;
                    NotifyPropertyChanged(nameof(ValorantPoints));
                }
            }
        }

        private int _radiantPoints;
        public int RadiantPoints
        {
            get => _radiantPoints;

            set
            {
                if (_radiantPoints != value)
                {
                    _radiantPoints = value;
                    NotifyPropertyChanged(nameof(RadiantPoints));
                }
            }
        }
        #endregion

        #region Ctr
        public MainWindowViewModel(MainWindow mainWindow)
        {
            _windowModel = new MainWindowModel();
            _mainWindow = mainWindow;

            _mainWindow.elaborationCompleted += TabItemElaboration;
            _windowModel.elaborationCompleted += ModelElaboration;
            Initialize();
        }
        #endregion

        #region Methods
        private void ModelElaboration(object sender, EventValArgs e)
        {
            if (e.PlayerIdReceived.HasValue)
            {
                if (e.PlayerIdReceived.Value)
                {
                    _windowModel = new MainWindowModel();
                    _windowModel.elaborationCompleted += ModelElaboration;

                    _windowModel.GetPlayer();
                }
            }

            if (e.PlayerReceived.HasValue)
            {
                if (e.PlayerReceived.Value)
                {
                    GlobalManager.Player = new PlayerDTO();
                    GlobalManager.Player = _windowModel.player;
                    LoginText = GlobalManager.Player.GameName + "#" + GlobalManager.Player.TagLine;
                    LogoutVisibility = Visibility.Visible;

                    _windowModel.GetContent();
                }
            }

            if (e.MatchReceived.HasValue)
            {
                if (e.MatchReceived.Value)
                {
                    //MessageBox.Show("Matches!");
                    var matchHistoryList = new List<Match.HistoryDTO>();
                    _windowModel.match.history.ForEach(x =>
                    {
                        var matchHistory = new Match.HistoryDTO()
                        {
                            MatchID = x.MatchID,
                            TeamID = x.TeamID,
                            GameStartTime = new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(x.GameStartTime).ToLocalTime()
                        };

                        if (matchHistory.TeamID == GlobalManager.Player.PlayerId)
                        {
                            matchHistory.TeamID = "Deathmatch";
                        }

                        matchHistoryList.Add(matchHistory);
                    });
                    MatchHistory = matchHistoryList;
                }
            }

            if (e.BalanceReceived.HasValue)
            {
                if (e.BalanceReceived.Value)
                {
                    ValorantPoints = _windowModel.userBalance.balances.ValorantPoints;
                    RadiantPoints = _windowModel.userBalance.balances.RadianitePoints;
                }
            }

            if (e.CompMatchReceived.HasValue)
            {
                if (e.CompMatchReceived.Value)
                {
                    var matchCompHistoryList = new List<Match.HistoryDTO>();
                    _windowModel.competitiveMatch.Matches.ForEach(x =>
                    {
                        var matchHistory = new Match.HistoryDTO()
                        {
                            MatchID = x.MatchID,
                            Movement = x.CompetitiveMovement,
                            GameStartTime = new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(x.MatchStartTime).ToLocalTime(),
                            TierUpdate = x.TierAfterUpdate
                        };
                        matchCompHistoryList.Add(matchHistory);
                    });
                    CompetitiveMatchHistory = matchCompHistoryList;
                }
            }

            if (e.StoreReceived.HasValue)
            {
                if (e.StoreReceived.Value)
                {
                    var offersList = new List<Offers>();
                    _windowModel.store.SkinsPanelLayout.SingleItemOffers.ForEach(x =>
                    {
                        var offer = new Offers()
                        {
                            ItemId = x,
                            ItemName = GetContent(x).SkinLevels.FirstOrDefault().Name,
                            RemainSeconds = new TimeSpan(0,0,_windowModel.store.SkinsPanelLayout.SingleItemOffersRemainingDurationInSeconds),
                            ValorantPrice = 0
                        };
                        offersList.Add(offer);
                    });
                    Offers = offersList;

                    var featureList = new List<Offers>();
                    _windowModel.store.FeaturedBundle.Bundle.Items.ForEach(x =>
                    {
                        var offer = new Offers()
                        {
                            ItemId = x.Item.ItemID,
                            RemainSeconds = new TimeSpan(0, 0, _windowModel.store.FeaturedBundle.BundleRemainingDurationInSeconds),
                            ValorantPrice = x.BasePrice
                        };
                        featureList.Add(offer);
                    });
                    Featured = featureList;
                }
            }

            if (e.ContentReceived.HasValue)
            {
                if (e.ContentReceived.Value)
                {
                    _windowModel.GetMatchHistory();
                    _windowModel.GetUserBalance();
                }
            }
        }

        private void TabItemElaboration(object sender, EventValArgs e)
        {
            if (GlobalManager.Player != null)
            {
                if (GlobalManager.Player.PlayerId != null)
                {
                    if (e.TabItemReceived.HasValue)
                    {
                        if (e.TabItemReceived.Value)
                        {
                            if (_mainWindow.tabItem.Header.ToString() == "Matches")
                            {
                                _windowModel.GetMatchHistory();
                            }

                            if (_mainWindow.tabItem.Header.ToString() == "Competitive Matches")
                            {
                                _windowModel.GetCompetitiveMatchHistory();
                            }

                            if (_mainWindow.tabItem.Header.ToString() == "Store")
                            {
                                _windowModel.GetStore();
                            }
                        }
                    }
                }
            }
        }

        private void Initialize()
        {
            var settings = GlobalManager.GetSettingObject();

            if (settings != null)
            {
                if (settings.RememberMe)
                {
                    var loginViewModel = new LoginPopupViewModel(this, new LoginPopup(this));

                    loginViewModel.Username = settings.Username;
                    loginViewModel.Password = settings.Password;
                    loginViewModel.StaySignedChecked = settings.RememberMe;
                    GlobalManager.Region = settings.Region ?? GlobalEnum.EndpointsEnum.Sys;
                    loginViewModel.LoginButton.Execute(null);
                }
            }
        }

        private IdListDTO GetContent(string id)
        {
            id = id.ToUpper();
            IdListDTO idListDTO = new IdListDTO();

            var attachment = _windowModel.contentList.Attachments.Where(x => x.ID == id).FirstOrDefault() ?? null;
            if (attachment != null)
            {
                idListDTO.Attachments = new List<IdListDTO.Content>();
                idListDTO.Attachments.Add(attachment);
                return idListDTO;
            }

            var characters = _windowModel.contentList.Characters.Where(x => x.ID == id).FirstOrDefault() ?? null;
            if (characters != null)
            {
                idListDTO.Characters = new List<IdListDTO.Content>();
                idListDTO.Characters.Add(characters);
                return idListDTO;
            }

            var charmLevels = _windowModel.contentList.CharmLevels.Where(x => x.ID == id).FirstOrDefault() ?? null;
            if (charmLevels != null)
            {
                idListDTO.CharmLevels = new List<IdListDTO.Content>();
                idListDTO.CharmLevels.Add(charmLevels);
                return idListDTO;
            }

            var charms = _windowModel.contentList.Charms.Where(x => x.ID == id).FirstOrDefault() ?? null;
            if (charms != null)
            {
                idListDTO.Charms = new List<IdListDTO.Content>();
                idListDTO.Charms.Add(charms);
                return idListDTO;
            }

            var chromas = _windowModel.contentList.Chromas.Where(x => x.ID == id).FirstOrDefault() ?? null;
            if (chromas != null)
            {
                idListDTO.Chromas = new List<IdListDTO.Content>();
                idListDTO.Chromas.Add(chromas);
                return idListDTO;
            }

            var equips = _windowModel.contentList.Equips.Where(x => x.ID == id).FirstOrDefault() ?? null;
            if (equips != null)
            {
                idListDTO.Equips = new List<IdListDTO.Content>();
                idListDTO.Equips.Add(equips);
                return idListDTO;
            }

            var gameModes = _windowModel.contentList.GameModes.Where(x => x.ID == id).FirstOrDefault() ?? null;
            if (gameModes != null)
            {
                idListDTO.GameModes = new List<IdListDTO.Content>();
                idListDTO.GameModes.Add(gameModes);
                return idListDTO;
            }

            var maps = _windowModel.contentList.Maps.Where(x => x.ID == id).FirstOrDefault() ?? null;
            if (maps != null)
            {
                idListDTO.Maps = new List<IdListDTO.Content>();
                idListDTO.Maps.Add(maps);
                return idListDTO;
            }

            var playerCards = _windowModel.contentList.PlayerCards.Where(x => x.ID == id).FirstOrDefault() ?? null;
            if (playerCards != null)
            {
                idListDTO.PlayerCards = new List<IdListDTO.Content>();
                idListDTO.PlayerCards.Add(playerCards);
                return idListDTO;
            }

            var playerTitles = _windowModel.contentList.PlayerTitles.Where(x => x.ID == id).FirstOrDefault() ?? null;
            if (playerTitles != null)
            {
                idListDTO.PlayerTitles = new List<IdListDTO.Content>();
                idListDTO.PlayerTitles.Add(playerTitles);
                return idListDTO;
            }

            var seasons = _windowModel.contentList.Seasons.Where(x => x.ID == id).FirstOrDefault() ?? null;
            if (seasons != null)
            {
                idListDTO.Seasons = new List<IdListDTO.Season>();
                idListDTO.Seasons.Add(seasons);
                return idListDTO;
            }

            var skinLevels = _windowModel.contentList.SkinLevels.Where(x => x.ID == id).FirstOrDefault() ?? null;
            if (skinLevels != null)
            {
                idListDTO.SkinLevels = new List<IdListDTO.Content>();
                idListDTO.SkinLevels.Add(skinLevels);
                return idListDTO;
            }

            var skins = _windowModel.contentList.Skins.Where(x => x.ID == id).FirstOrDefault() ?? null;
            if (skins != null)
            {
                idListDTO.Skins = new List<IdListDTO.Content>();
                idListDTO.Skins.Add(skins);
                return idListDTO;
            }

            var sprayLevels = _windowModel.contentList.SprayLevels.Where(x => x.ID == id).FirstOrDefault() ?? null;
            if (sprayLevels != null)
            {
                idListDTO.SprayLevels = new List<IdListDTO.Content>();
                idListDTO.SprayLevels.Add(sprayLevels);
                return idListDTO;
            }

            var sprays = _windowModel.contentList.Sprays.Where(x => x.ID == id).FirstOrDefault() ?? null;
            if (sprays != null)
            {
                idListDTO.Sprays = new List<IdListDTO.Content>();
                idListDTO.Sprays.Add(sprays);
                return idListDTO;
            }

            var storefrontItems = _windowModel.contentList.StorefrontItems.Where(x => x.ID == id).FirstOrDefault() ?? null;
            if (storefrontItems != null)
            {
                idListDTO.StorefrontItems = new List<IdListDTO.Content>();
                idListDTO.StorefrontItems.Add(storefrontItems);
                return idListDTO;
            }

            var themes = _windowModel.contentList.Themes.Where(x => x.ID == id).FirstOrDefault() ?? null;
            if (themes != null)
            {
                idListDTO.Themes = new List<IdListDTO.Content>();
                idListDTO.Themes.Add(themes);
                return idListDTO;
            }

            return null;
        }
        #endregion

        #region Commands
        private ICommand _login;
        public ICommand Login
        {
            get
            {
                if (this._login == null)
                {
                    this._login = new RelayCommands(obj =>
                    {
                        try
                        {
                            var loginWindow = new LoginPopup(this);
                            loginWindow.ShowDialog();
                        }
                        catch (Exception exc)
                        {

                        }
                    }, obj => true);
                }
                return this._login;
            }
        }

        private ICommand _logout;
        public ICommand Logout
        {
            get
            {
                if (this._logout == null)
                {
                    this._logout = new RelayCommands(obj =>
                    {
                        try
                        {
                            LogoutVisibility = Visibility.Collapsed;
                            LoginText = "Login";
                            MatchHistory = new List<Match.HistoryDTO>();
                            RadiantPoints = 0;
                            ValorantPoints = 0;
                            GlobalManager.BearerToken = string.Empty;
                            GlobalManager.X_Riot_Entitlements_JWT = string.Empty;
                            GlobalManager.ExpiresDateTime = null;
                            GlobalManager.Player = new PlayerDTO();
                            GlobalManager.settings = new UserSettings() { RememberMe = false, Username = "", Password = "", Region = null };
                            GlobalManager.SaveUserInfo();
                        }
                        catch (Exception exc) { }
                    }, obj => true);
                }
                return this._logout;
            }
        }

        private ICommand _infoHelp;
        public ICommand InfoHelp
        {
            get
            {
                if (this._infoHelp == null)
                {
                    this._infoHelp = new RelayCommands(obj =>
                    {
                        try
                        {
                            System.Diagnostics.Process.Start("https://github.com/Teo230/ValorantTracker#readme");
                        }
                        catch (Exception exc)
                        {

                        }
                    }, obj => true);
                }
                return this._infoHelp;
            }
        }

        private ICommand _settings;
        public ICommand Settings
        {
            get
            {
                if (this._settings == null)
                {
                    this._settings = new RelayCommands(obj =>
                    {
                        try
                        {
                            //MessageBox.Show("Settings!");
                            var setting = new Settings();
                            setting.ShowDialog();
                        }
                        catch (Exception exc)
                        {

                        }
                    }, obj => true);
                }
                return this._settings;
            }
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        #endregion
    }
}

