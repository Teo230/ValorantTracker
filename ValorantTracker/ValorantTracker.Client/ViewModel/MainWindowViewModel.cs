using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
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

        private List<MatchDTO.HistoryDTO> _matchHistory;
        public List<MatchDTO.HistoryDTO> MatchHistory
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
                    _windowModel.GetPlayer();
                }
            }

            if (e.PlayerReceived.HasValue)
            {
                if (e.PlayerReceived.Value)
                {
                    GlobalManager.Player = new PlayerDTO();
                    GlobalManager.Player = _windowModel.player.FirstOrDefault();
                    LoginText = GlobalManager.Player.GameName + "#" + GlobalManager.Player.TagLine;
                    LogoutVisibility = Visibility.Visible;
                    _windowModel.GetMatchHistory();
                    _windowModel.GetUserBalance();
                }
            }

            if (e.MatchReceived.HasValue)
            {
                if (e.MatchReceived.Value)
                {
                    //MessageBox.Show("Matches!");
                    var matchHistoryList = new List<MatchDTO.HistoryDTO>();
                    _windowModel.match.history.ForEach(x =>
                    {
                        var matchHistory = new MatchDTO.HistoryDTO()
                        {
                            MatchID = x.MatchID,
                            TeamID = x.TeamID,
                            GameStartTime = new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(x.GameStartTime).ToLocalTime()
                        };
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
        }

        private void TabItemElaboration(object sender, EventValArgs e)
        {
            if (GlobalManager.Player != null)
            {
                if (GlobalManager.Player.Subject != null)
                {
                    if (e.TabItemReceived.HasValue)
                    {
                        if (e.TabItemReceived.Value)
                        {
                            if (_mainWindow.tabItem.Header.ToString() == "Matches")
                            {
                                _windowModel.GetMatchHistory();
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
                            MatchHistory = new List<MatchDTO.HistoryDTO>();
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

