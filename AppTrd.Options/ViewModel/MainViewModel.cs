using System.Collections.Generic;
using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;
using AppTrd.BaseLib.Common;
using System;
using AppTrd.BaseLib.ViewModel;

namespace AppTrd.Options.ViewModel
{
    public class MainViewModel : BaseMainViewModel
    {
        public override void LoginCompleted()
        {
            MainMenu();
        }

        public void MainMenu()
        {
            CurrentViewModel = ServiceLocator.Current.GetInstance<MainMenuViewModel>();
        }

        public void SelectOptions()
        {
            CurrentViewModel = ServiceLocator.Current.GetInstance<OptionsSelectorViewModel>();
        }

        public void SimulateOptions(List<string> epics)
        {
            var vm = ServiceLocator.Current.GetInstance<OptionsSimulatorViewModel>();

            vm.Prepare(epics);

            CurrentViewModel = vm;
        }

        public void SimulateOptions(string marketId)
        {
            var vm = ServiceLocator.Current.GetInstance<OptionsSimulatorViewModel>();

            vm.Prepare(marketId);

            CurrentViewModel = vm;
        }
    }
}