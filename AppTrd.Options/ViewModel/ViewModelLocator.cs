using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using AppTrd.BaseLib.Service;
using AppTrd.BaseLib.Service.Impl;
using AppTrd.BaseLib.ViewModel;

namespace AppTrd.Options.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<ITradingService, TradingService>();
            SimpleIoc.Default.Register<ISettingsService, SettingsService>();

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<BaseMainViewModel>(() => ServiceLocator.Current.GetInstance<MainViewModel>());
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<OptionsSelectorViewModel>();
            SimpleIoc.Default.Register<OptionsSimulatorViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}