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
using System.Windows.Shapes;
using ValorantTracker.Client.Utilities;
using ValorantTracker.Client.ViewModel;

namespace ValorantTracker.Client.Popups
{
    /// <summary>
    /// Interaction logic for LoginPopup.xaml
    /// </summary>
    public partial class LoginPopup : Window
    {
        public EventHandler<EventValArgs> elaborationCompleted;
        public PasswordBox passwordBox;

        public LoginPopup(MainWindowViewModel mainWindowViewModel)
        {
            InitializeComponent();
            DataContext = new LoginPopupViewModel(mainWindowViewModel, this);
        }

        private void password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var task = new Task(async () =>
            {
                passwordBox = password;
                ElaborationCompleted(new EventValArgs{ PasswordChanged = passwordBox != null });
            });
            task.Start();
        }

        #region Events
        private void ElaborationCompleted(EventValArgs args)
        {
            this.elaborationCompleted?.Invoke(this, args);
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (SystemParameters.SwapButtons)
            {
                
            }
            else
            {
                if (password.Visibility == Visibility.Collapsed)
                    password.Visibility = Visibility.Visible;
                else
                    password.Visibility = Visibility.Collapsed;

                if (passwordText.Visibility == Visibility.Collapsed)
                    passwordText.Visibility = Visibility.Visible;
                else
                    passwordText.Visibility = Visibility.Collapsed;
            }
        }
    }
}
