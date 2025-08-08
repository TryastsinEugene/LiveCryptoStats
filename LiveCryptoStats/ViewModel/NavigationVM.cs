using LiveCryptoStats.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LiveCryptoStats.ViewModel
{
	class NavigationVM : ViewModelBase
	{
		private object? _currentView;
		public object CurrentView
		{
			get => _currentView!;
			set
			{
				if (_currentView != value)
				{
					_currentView = value;
					OnPropertyChanged();
				}
			}
		}

		public ICommand HomeCommand { get; set; }
		public ICommand SettingsCommand { get; set; }

		private void Home(object view) => CurrentView = new HomeVM();
		private void Settings(object view) => CurrentView = new SettingVM();

		public NavigationVM()
		{
			HomeCommand = new RelayCommand(Home);
			SettingsCommand = new RelayCommand(Settings);

			CurrentView = new HomeVM();
		}
	}
}
