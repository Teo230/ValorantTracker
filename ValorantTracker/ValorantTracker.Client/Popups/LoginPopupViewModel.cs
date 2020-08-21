using ModernWPF.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ValorantTracker.Client.API;
using ValorantTracker.Client.Model;
using ValorantTracker.Client.Models;
using ValorantTracker.Client.Utilities;
using ValorantTracker.Client.ViewModel;
using static ValorantTracker.Client.Utilities.GlobalEnum;

namespace ValorantTracker.Client.Popups
{
    public class LoginPopupViewModel : INotifyPropertyChanged
    {

        #region Prop
        private MainWindowViewModel _mainWindowViewModel;
        private MainWindowModel _windowModel;
        private LoginPopup _loginPopup;
        private PasswordBox _passwordBox;

        private string _username;
        public string Username
        {
            get => _username;

            set
            {
                if (_username != value)
                {
                    _username = value;
                    NotifyPropertyChanged(nameof(Username));
                    NotifyPropertyChanged(nameof(LoginEnable));
                }
            }
        }

        private string _password;
        public string Password
        {
            get => _password;

            set
            {
                if (_password != value)
                {
                    _password = value;
                    NotifyPropertyChanged(nameof(Password));
                    NotifyPropertyChanged(nameof(LoginEnable));
                }
            }
        }

        private bool _staySignedChecked;
        public bool StaySignedChecked
        {
            get => _staySignedChecked;

            set
            {
                if (_staySignedChecked != value)
                {
                    _staySignedChecked = value;
                    NotifyPropertyChanged(nameof(StaySignedChecked));
                }
            }
        }

        private Dictionary<string, EndpointsEnum> _regions;
        public Dictionary<string, EndpointsEnum> Regions
        {
            get => _regions;

            set
            {
                if (_regions != value)
                {
                    _regions = value;
                    NotifyPropertyChanged(nameof(Regions));
                }
            }
        }

        private KeyValuePair<string, EndpointsEnum> _selectedRegion;
        public KeyValuePair<string, EndpointsEnum> SelectedRegion
        {
            get => _selectedRegion;

            set
            {
                _selectedRegion = value;
                GlobalManager.Region = _selectedRegion.Value;
                NotifyPropertyChanged(nameof(SelectedRegion));
            }
        }

        public bool LoginEnable => !string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(Username);

        #endregion

        #region Ctr
        public LoginPopupViewModel(MainWindowViewModel mainWindowViewModel, LoginPopup loginPopup)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _windowModel = mainWindowViewModel._windowModel;
            _loginPopup = loginPopup;
            _loginPopup.elaborationCompleted += ModelElaboration;
            Initialize();
        }
        #endregion

        #region Methods
        private void Initialize()
        {
            Regions = new Dictionary<string, EndpointsEnum>();
            Regions.Add("EU", EndpointsEnum.Europe);
            Regions.Add("NA", EndpointsEnum.NorthAmerica);
            Regions.Add("KO", EndpointsEnum.Korea);
            Regions.Add("AP", EndpointsEnum.Asia);
        }

        private void ModelElaboration(object sender, EventValArgs e)
        {
            if (e.PasswordChanged.HasValue)
            {
                if (e.PasswordChanged.Value)
                {
                    _passwordBox = _loginPopup.passwordBox;

                    Password = _passwordBox.Password;
                }
            }
        }
        #endregion

        #region Commands
        private ICommand _loginButton;
        public ICommand LoginButton
        {
            get
            {
                if (this._loginButton == null)
                {
                    this._loginButton = new RelayCommands(obj =>
                    {
                        try
                        {
                            var client = new HttpRequest();

                            if (client.PerfomLogin(Username, Password, StaySignedChecked))
                            {
                                if (_loginPopup.IsVisible)
                                {
                                    _loginPopup.Close();
                                }

                                _windowModel.GetPlayerId();
                            }
                        }
                        catch (Exception exc)
                        {

                        }
                    }, obj => true);
                }
                return this._loginButton;
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
