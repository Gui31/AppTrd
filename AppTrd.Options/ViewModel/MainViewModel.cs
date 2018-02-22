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
            CurrentViewModel = ServiceLocator.Current.GetInstance<OptionsSelectorViewModel>();
        }

        public void SelectOptions()
        {
            CurrentViewModel = ServiceLocator.Current.GetInstance<OptionsSelectorViewModel>();
        }

        public void SimulateOptions()
        {
            CurrentViewModel = ServiceLocator.Current.GetInstance<OptionsSimulatorViewModel>();
        }
    }
}