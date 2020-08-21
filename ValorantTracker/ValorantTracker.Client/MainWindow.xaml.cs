using ModernWPF;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ValorantTracker.Client.Utilities;
using ValorantTracker.Client.ViewModel;

namespace ValorantTracker.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Prop
        private MainWindowViewModel _mainWindowViewModel;
        public EventHandler<EventValArgs> elaborationCompleted;
        public TabItem tabItem;
        #endregion

        #region Ctr
        public MainWindow()
        {
            InitializeComponent();

            // to change to dark theme
            ModernTheme.ApplyTheme(ModernTheme.Theme.Dark, Accent.Red);
            _mainWindowViewModel = new MainWindowViewModel(this);
            DataContext = _mainWindowViewModel;
        }
        #endregion

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Close();
            System.Windows.Application.Current.Shutdown();
            Environment.Exit(0);
        }

        private void mainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                tabItem = e.AddedItems[0] as TabItem;
                if (tabItem != null)
                    ElaborationCompleted(new EventValArgs { TabItemReceived = tabItem != null });
            }
        }

        #region Events
        private void ElaborationCompleted(EventValArgs args)
        {
            this.elaborationCompleted?.Invoke(this, args);
        }
        #endregion

    }
}
