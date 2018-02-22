using System.Collections.Generic;
using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;
using AppTrd.BaseLib.ViewModel;

namespace AppTrd.Charts.ViewModel
{
    public class MainViewModel : BaseMainViewModel
    {
        private List<ChildWindow> _childWindows = new List<ChildWindow>();

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

        public void OpenMarket(string epic)
        {
            var window = new ChildWindow();

            window.DataContext = new CandleViewModel(this, epic);

            window.Show();

            _childWindows.Add(window);
        }
    }
}