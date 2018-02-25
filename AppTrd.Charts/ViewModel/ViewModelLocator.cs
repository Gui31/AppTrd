using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using AppTrd.BaseLib.Service;
using AppTrd.BaseLib.Service.Impl;
using AppTrd.BaseLib.ViewModel;

namespace AppTrd.Charts.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<ITradingService, TradingService>();
            SimpleIoc.Default.Register<ISettingsService, SettingsService>();

            SimpleIoc.Default.Register<BaseMainViewModel, MainViewModel>();
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<DashboardViewModel>();
            SimpleIoc.Default.Register<MarketSelectorViewModel>();
        }

        public BaseMainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<BaseMainViewModel>();
            }
        }

        public static void Cleanup()
        {
        }
    }
}