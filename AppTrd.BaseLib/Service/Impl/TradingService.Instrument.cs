using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppTrd.BaseLib.Common;
using AppTrd.BaseLib.Listener;
using AppTrd.BaseLib.Model;
using AppTrd.BaseLib.Receiver;
using dto.endpoint.marketdetails.v2;
using dto.endpoint.prices.v2;
using IGWebApiClient;
using Lightstreamer.DotNet.Client;

namespace AppTrd.BaseLib.Service.Impl
{
    public partial class TradingService
    {
        internal InstrumentModel GetInstrument(string epic)
        {
            var market = Instruments.FirstOrDefault(m => m.Epic == epic);

            if (market == null)
            {
                var detail = GetMarketDetails(epic);

                market = new InstrumentModel();

                market.DealingRuleData = detail.dealingRules;
                market.InstrumentData = detail.instrument;
                market.SnapshotData = detail.snapshot;

                market.Epic = epic;
                market.InstrumentName = market.InstrumentData.name;

                market.Offer = market.SnapshotData.offer;
                market.Bid = market.SnapshotData.bid;
                market.High = market.SnapshotData.high;
                market.Low = market.SnapshotData.low;

                Instruments.Add(market);

                SubscribeL1WatchlistPrices();
            }

            return market;
        }

        public MarketDetailsResponse GetMarketDetails(string epic)
        {
            var result = _igRestApiClient.marketDetailsV2(epic).RunSync();

            return result.Response;
        }

        public ClientSentimentModel GetClientSentiment(string epic)
        {
            var test = _igRestApiClient.getClientSentiment(epic).RunSync();

            return null;
        }

        public List<ClientSentimentModel> GetRelatedClientSentiment(string epic)
        {
            var test = _igRestApiClient.getRelatedClientSentiment(epic).RunSync();

            return null;
        }

        public BrowseModel BrowseRoot()
        {
            try
            {
                var response = _igRestApiClient.browseRoot().RunSync();
                if (response)
                {
                    var browseResult = new BrowseModel();

                    if (response.Response.nodes != null)
                        browseResult.Nodes = response.Response.nodes.Select(n => new BrowseNodeModel
                        {
                            Id = n.id,
                            Name = n.name
                        }).ToList();

                    if (response.Response.markets != null)
                        browseResult.Markets = response.Response.markets.Select(m => new BrowseMarketModel
                        {
                            Epic = m.epic,
                            expiry = m.expiry,
                            InstrumentName = m.instrumentName,
                            InstrumentType = m.instrumentType,
                            MarketStatus = m.marketStatus
                        }).ToList();

                    return browseResult;
                }
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        public BrowseModel BrowseNode(string nodeId)
        {
            try
            {
                var response = _igRestApiClient.browse(nodeId).RunSync();
                if (response)
                {
                    var browseResult = new BrowseModel();

                    if (response.Response.nodes != null)
                        browseResult.Nodes = response.Response.nodes.Select(n => new BrowseNodeModel
                        {
                            Id = n.id,
                            Name = n.name
                        }).ToList();

                    if (response.Response.markets != null)
                        browseResult.Markets = response.Response.markets.Select(m => new BrowseMarketModel
                        {
                            Epic = m.epic,
                            expiry = m.expiry,
                            InstrumentName = m.instrumentName,
                            InstrumentType = m.instrumentType,
                            MarketStatus = m.marketStatus,
                            netChange = m.netChange,
                            bid = m.bid,
                            offer = m.offer
                        }).ToList();

                    return browseResult;
                }
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        public List<WatchlistModel> GetWatchlists()
        {
            try
            {
                var response = _igRestApiClient.listOfWatchlists().RunSync();
                if (response && (response.Response.watchlists != null))
                {
                    var watchlists = new List<WatchlistModel>();

                    foreach (var wl in response.Response.watchlists)
                    {
                        var wm = new WatchlistModel();
                        wm.WatchlistId = wl.id;
                        wm.WatchlistName = wl.name;
                        wm.Editable = wl.editable;
                        wm.Deletable = wl.deleteable;

                        watchlists.Add(wm);
                    }

                    return watchlists;
                }
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        public List<WatchlistMarketModel> GetWatchlistMarkets(WatchlistModel watchlist)
        {
            try
            {
                var response = _igRestApiClient.instrumentsForWatchlist(watchlist.WatchlistId).RunSync();
                if (response != null)
                {
                    if (response && (response.Response.markets != null) && (response.Response.markets.Count > 0))
                    {
                        var watchlistMarkets = new List<WatchlistMarketModel>();
                        foreach (var wl in response.Response.markets)
                        {
                            var wim = new WatchlistMarketModel();
                            wim.Model = new InstrumentModel();
                            wim.Model.High = wl.high;
                            wim.Model.Low = wl.low;
                            wim.Model.Epic = wl.epic;
                            wim.Model.Bid = wl.bid;
                            wim.Model.Offer = wl.offer;
                            wim.Model.PctChange = wl.percentageChange;
                            wim.Model.NetChange = wl.netChange;
                            wim.Model.InstrumentName = wl.instrumentName;
                            wim.Model.StreamingPricesAvailable = wl.streamingPricesAvailable;
                            wim.Model.MarketStatus = wl.marketStatus;

                            watchlistMarkets.Add(wim);
                        }

                        return watchlistMarkets;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        public PriceList GetPriceList(string epic, Periods period, int count)
        {
            var result = _igRestApiClient.priceSearchByNumV2(epic, _periodsNames[period], count.ToString()).RunSync();

            return result;
        }

        public SubscribedTableKey SubscribeToChartTick(string epic, ChartTickSubscription chartTickSubscription)
        {
            if (CurrentAccount != null)
            {
                // TODO : Save STK

                return _igStreamApiClient.SubscribeToChartTicks(new[] { epic }, chartTickSubscription);
            }

            return null;
        }

        public SubscribedTableKey SubscribeToChartCandle(string epic, ChartPeriods period, ChartCandleSubscription chartCandleSubscription)
        {
            if (CurrentAccount != null)
            {
                // TODO : Save STK

                return _igStreamApiClient.SubscribeToChartCandleData(new[] { epic }, (ChartScale)period, chartCandleSubscription);
            }

            return null;
        }

        public void UnsubscribeToChartData(SubscribedTableKey subscribedTableKey)
        {
            _igStreamApiClient.UnsubscribeTableKey(subscribedTableKey);
        }

        private bool _hasNewReceiver = false;

        public CandleReceiver GetCandleReceiver(string epic, Periods period)
        {
            var receiver = CandleReceivers.OfType<CandleDataReceiver>().FirstOrDefault(c => c.Epic == epic && c.Period == period);

            if (receiver == null)
            {
                receiver = new CandleDataReceiver(epic, period);

                CandleReceivers.Add(receiver);

                _hasNewReceiver = true;
            }

            return receiver;
        }

        public CandleReceiver GetCandleReceiver(string epic, int ticksCount)
        {
            var receiver = CandleReceivers.OfType<TicksDataReceiver>().FirstOrDefault(c => c.Epic == epic && c.TicksCount == ticksCount);

            if (receiver == null)
            {
                receiver = new TicksDataReceiver(epic, ticksCount);

                CandleReceivers.Add(receiver);

                _hasNewReceiver = true;
            }

            return receiver;
        }

        public void SubscribeToChartCandle()
        {
            if (CurrentAccount == null)
                return;

            if (_hasNewReceiver == false)
                return;

            if (_candleMultiSubKey != null)
            {
                UnsubscribeToChartData(_candleMultiSubKey);
                _candleMultiSubKey = null;
            }

            var candleInfos = CandleReceivers.OfType<CandleDataReceiver>().Select(c => new ChartCandleInfo(c.Epic, (ChartScale)c.Period)).ToArray();

            if (candleInfos.Length > 0)
            {
                _candleMultiSubKey = _igStreamApiClient.SubscribeToChartCandleData(candleInfos, _candleMultiSub);
            }

            var tickInfos = CandleReceivers.OfType<TicksDataReceiver>().Select(c => c.Epic).Distinct().ToArray();

            if (tickInfos.Length > 0)
            {
                _candleMultiSubKey = _igStreamApiClient.SubscribeToChartTicks(tickInfos, _tickMultiSub);
            }

            _hasNewReceiver = false;
        }
    }
}
