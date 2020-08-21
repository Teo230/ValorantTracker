using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using ValorantTracker.Client.Models;

namespace ValorantTracker.Client.ViewModel
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        #region Prop
        private List<ViewItem> _listViewItem;
        public List<ViewItem> ListViewItem
        {
            get => _listViewItem;

            set
            {
                if(_listViewItem != value)
                {
                    _listViewItem = value;
                    NotifyPropertyChanged(nameof(ListViewItem));
                }
            }
        }

        private ViewItem _selectedViewItem;
        public ViewItem SelectedViewItem
        {
            get => _selectedViewItem;

            set
            {
                if (_selectedViewItem != value)
                {
                    _selectedViewItem = value;

                    SelectedFrame = new Page();
                    NotifyPropertyChanged(nameof(SelectedViewItem));
                }
            }
        }

        private Page _selectedFrame;
        public Page SelectedFrame
        {
            get => _selectedFrame;

            set
            {
                if (_selectedFrame != value)
                {
                    _selectedFrame = value;
                    NotifyPropertyChanged(nameof(SelectedFrame));
                }
            }
        }

        #endregion

        #region Ctr
        public SettingsViewModel()
        {
            Initialize();
        }
        #endregion

        #region Methods
        private void Initialize()
        {
            ListViewItem = new List<ViewItem>();

            ListViewItem.Add(new ViewItem()
            {
                ViewItemIndex = 1,
                ViewItemName = "Account",
                ViewItemSource = "Account.xaml"
            });
        }
        #endregion

        #region Commands

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
