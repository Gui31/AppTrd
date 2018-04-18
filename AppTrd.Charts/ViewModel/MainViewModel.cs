using System.Collections.Generic;
using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;
using AppTrd.BaseLib.ViewModel;

namespace AppTrd.Charts.ViewModel
{
    public class MainViewModel : BaseMainViewModel
    {
        private List<ChildWindow> _childWindows = new List<ChildWindow>();
        private SettingsWindow _settingsWindow;

        public override void LoginCompleted()
        {
            DisplayDashboard();
        }

        public void DisplayDashboard()
        {
            CurrentViewModel = ServiceLocator.Current.GetInstance<DashboardViewModel>();
        }

        public void SelectMarket()
        {
            CurrentViewModel = ServiceLocator.Current.GetInstance<MarketSelectorViewModel>();
        }

        public void DisplaySettings()
        {
            if (_settingsWindow != null)
                return;

            var window = new SettingsWindow();
            window.Closed += Window_Closed;

            window.DataContext = ServiceLocator.Current.GetInstance<ChartsSettingsViewModel>();

            window.Show();

            _settingsWindow = window;
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            CloseSettings();
        }

        public void CloseSettings()
        {
            var window = _settingsWindow;

            if (window == null)
                return;

            window.Closed -= Window_Closed;
            window.Close();
            _settingsWindow = null;
        }

        public void OpenMarket(string epic)
        {
            var window = new ChildWindow();

            window.DataContext = new MarketViewModel(this, epic);

            window.Show();

            _childWindows.Add(window);
        }
    }
}