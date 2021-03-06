﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using AppTrd.BaseLib.Common;
using AppTrd.BaseLib.ViewModel;
using AppTrd.BaseLib.Model;
using AppTrd.BaseLib.Service;

namespace AppTrd.Options.ViewModel
{
    public class OptionsSelectorViewModel : BaseViewModel
    {
        private readonly ITradingService _tradingService;
        private string _marketId;

        private readonly Stack<string> _browseHistory = new Stack<string>();

        public ObservableCollection<BrowseMarketModel> SelectedMarkets { get; } = new ObservableCollection<BrowseMarketModel>();

        private BrowseModel _browse;
        public BrowseModel Browse
        {
            get { return _browse; }
            set
            {
                if (_browse == value)
                    return;

                _browse = value;
                RaisePropertyChanged(() => Browse);
            }
        }

        public RelayCommand<string> BrowseNodeCommand { get; }
        public RelayCommand BrowseBackCommand { get; }
        public RelayCommand<BrowseMarketModel> AddMarketCommand { get; }
        public RelayCommand<BrowseMarketModel> RemoveMarketCommand { get; }
        public RelayCommand ValidateCommand { get; }

        public OptionsSelectorViewModel(ITradingService tradingService)
        {
            _tradingService = tradingService;

            Title = "Select options";

            BrowseNodeCommand = new RelayCommand<string>(BrowseNode);
            BrowseBackCommand = new RelayCommand(BrowseBack);
            AddMarketCommand = new RelayCommand<BrowseMarketModel>(AddMarket);
            RemoveMarketCommand = new RelayCommand<BrowseMarketModel>(RemoveMarket);
            ValidateCommand = new RelayCommand(Validate, CanValidate);
        }

        public override void Init()
        {
            SelectedMarkets.Clear();
            _browseHistory.Clear();

            _marketId = null;

            Browse = BrowseRoot();
        }

        public void BrowseNode(string nodeId)
        {
            Browse = _tradingService.BrowseNode(nodeId);

            _browseHistory.Push(nodeId);
        }

        private void BrowseBack()
        {
            if (_browseHistory.Count > 0)
                _browseHistory.Pop();

            if (_browseHistory.Count > 0)
                Browse = _tradingService.BrowseNode(_browseHistory.Peek());
            else
                Browse = BrowseRoot();
        }

        private BrowseModel BrowseRoot()
        {
            var root = _tradingService.BrowseRoot();

            root.Nodes = root.Nodes.Where(n => n.Name.StartsWith("Options")).ToList();

            return root;
        }

        private void AddMarket(BrowseMarketModel item)
        {
            var detail = _tradingService.GetMarketDetails(item.Epic);

            if (_marketId == null)
                _marketId = detail.instrument.marketId;

            if (_marketId != detail.instrument.marketId)
            {
                MessageBox.Show($"Add option based on {_marketId} market.", "Wrong market");
                return;
            }

            SelectedMarkets.Add(item);

            ValidateCommand.RaiseCanExecuteChanged();
        }

        private void RemoveMarket(BrowseMarketModel item)
        {
            SelectedMarkets.Remove(item);

            if (SelectedMarkets.Count == 0)
                _marketId = null;

            ValidateCommand.RaiseCanExecuteChanged();
        }

        private void Validate()
        {
            var mainViewModel = ServiceLocator.Current.GetInstance<MainViewModel>();

            mainViewModel.SimulateOptions(SelectedMarkets.Select(m => m.Epic).ToList());
        }

        private bool CanValidate()
        {
            return SelectedMarkets != null && SelectedMarkets.Count > 0;
        }
    }
}
