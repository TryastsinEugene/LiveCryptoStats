using LiveCryptoStats.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

		//Navigation Commands
		public ICommand HomeCommand { get; set; }
		public ICommand SettingsCommand { get; set; }

		//Window Commands
		public ICommand CloseCommand { get; set; }
		public ICommand MinimizeCommand { get; set; }
		public ICommand MaximizeCommand { get; set; }

		private void Settings(object view) => CurrentView = new SettingVM();

		public NavigationVM()
		{
			var homeVM = new HomeVM();
			homeVM.NavigateToCurrencyDetailsAction = currency =>
			{
				CurrentView = new CurrencyDetailsVM(currency);
			};

			HomeCommand = new RelayCommand(_ => CurrentView = homeVM);
			SettingsCommand = new RelayCommand(Settings);

			CurrentView = homeVM;

			CloseCommand = new RelayCommand(param => { Application.Current.Shutdown(); });
			MinimizeCommand = new RelayCommand(param => { Application.Current.MainWindow.WindowState = WindowState.Minimized; });
			MaximizeCommand = new RelayCommand(param =>
			{
				if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
					Application.Current.MainWindow.WindowState = WindowState.Normal;
				else
					Application.Current.MainWindow.WindowState = WindowState.Maximized;

			});
		}
	}
}
