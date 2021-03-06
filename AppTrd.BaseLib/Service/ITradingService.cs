﻿using System;
using System.Collections.Generic;
using dto.endpoint.marketdetails.v2;
using dto.endpoint.prices.v2;
using Lightstreamer.DotNet.Client;
using AppTrd.BaseLib.Listener;
using AppTrd.BaseLib.Model;
using AppTrd.BaseLib.Receiver;

namespace AppTrd.BaseLib.Service
{
    public interface ITradingService : IDisposable
    {
        bool IsLogged { get; }
        bool IsConnected { get; }

        List<AccountModel> Accounts { get; }
        AccountModel CurrentAccount { get; }

        List<InstrumentModel> Instruments { get; }
        List<PositionModel> Positions { get; }
        List<OrderModel> Orders { get; }

        bool Login(string username, string password, string apiKey, bool useDemo);
        bool SelectAccount(AccountModel account);
        void Logout(bool stayConnected);

        //List<WatchlistModel> GetWatchlists();
        //List<WatchlistMarketModel> GetWatchlistMarkets(WatchlistModel watchlist);
        
        InstrumentModel GetInstrument(string epic);

        MarketDetailsResponse GetMarketDetails(string epic);

        List<PivotPointModel> GetPivotPoints(string epic);

        PriceList GetPriceList(string epic, Periods period, int count);

        CandleReceiver GetCandleReceiver(string epic, Periods period, int averageOpen);
        CandleReceiver GetCandleReceiver(string epic, int ticksCount, int averageOpen, int maxSeconds);
        void SubscribeToChartCandle();

        SubscribedTableKey SubscribeToChartTick(string epic, ChartTickSubscription chartTickSubscription);
        SubscribedTableKey SubscribeToChartCandle(string epic, ChartPeriods period, ChartCandleSubscription chartCandleSubscription);
        void UnsubscribeToChartData(SubscribedTableKey subscribedTableKey);

        ClientSentimentModel GetClientSentiment(string epic);
        List<ClientSentimentModel> GetRelatedClientSentiment(string epic);

        BrowseModel BrowseRoot();
        BrowseModel BrowseNode(string nodeId);
    }
}
