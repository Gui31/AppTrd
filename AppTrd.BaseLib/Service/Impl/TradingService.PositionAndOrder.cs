using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppTrd.BaseLib.Common;
using AppTrd.BaseLib.Messages;
using AppTrd.BaseLib.Model;
using dto.endpoint.positions.close.v1;
using dto.endpoint.positions.create.otc.v2;
using dto.endpoint.positions.edit.v2;
using GalaSoft.MvvmLight.Messaging;

namespace AppTrd.BaseLib.Service.Impl
{
    public partial class TradingService
    {
        internal void AddPosition(PositionModel model)
        {
            Positions.Add(model);

            var message = new PositionAddedMessage(model);

            Messenger.Default.Send(message);
            Messenger.Default.Send(message, model.Instrument.Epic);
        }

        internal void DeletePosition(PositionModel model)
        {
            Positions.Remove(model);

            var message = new PositionDeletedMessage(model);

            Messenger.Default.Send(message);
            Messenger.Default.Send(message, model.Instrument.Epic);
        }

        internal void AddOrder(OrderModel model)
        {
            Orders.Add(model);
        }
        internal void DeleteOrder(OrderModel model)
        {
            Orders.Remove(model);
        }

        private void LoadPositions()
        {
            var positionsResponse = _igRestApiClient.getOTCOpenPositionsV2().RunSync();
            if (positionsResponse && (positionsResponse.Response != null))
            {
                Positions.Clear();

                if (positionsResponse.Response.positions.Count != 0)
                {
                    foreach (var position in positionsResponse.Response.positions)
                    {
                        var model = new PositionModel();

                        var market = GetInstrument(position.market.epic);

                        model.Instrument = market;

                        model.DealId = position.position.dealId;
                        model.DealSize = (double)position.position.size;
                        model.Direction = (PositionModel.Directions)Enum.Parse(typeof(PositionModel.Directions), position.position.direction);
                        model.OpenLevel = (double)position.position.level;
                        model.StopLevel = (double?)position.position.stopLevel;
                        model.LimitLevel = (double?)position.position.limitLevel;

                        DateTime date;
                        if (DateTime.TryParseExact(position.position.createdDate, "yyyy/MM/dd HH:mm:ss:fff", CultureInfo.CurrentCulture, DateTimeStyles.None, out date))
                            model.CreatedDate = date;

                        model.GuaranteedStop = position.position.controlledRisk;

                        Positions.Add(model);
                    }
                }
            }
        }

        private void LoadOrders()
        {
            var response = _igRestApiClient.workingOrdersV2().RunSync();

            if (response && (response.Response != null) && (response.Response.workingOrders != null))
            {
                Orders.Clear();

                if (response.Response.workingOrders.Count != 0)
                {
                    foreach (var order in response.Response.workingOrders)
                    {
                        var model = new OrderModel();

                        var market = GetInstrument(order.marketData.epic);

                        model.Model = market;

                        model.OrderSize = order.workingOrderData.orderSize;
                        model.Direction = order.workingOrderData.direction;
                        model.DealId = order.workingOrderData.dealId;
                        model.CreationDate = order.workingOrderData.createdDate;
                        model.GoodTillDate = order.workingOrderData.goodTillDate;
                        model.OrderType = order.workingOrderData.orderType;
                        model.StopDistance = order.workingOrderData.stopDistance;
                        model.LimitDistance = order.workingOrderData.limitDistance;
                        model.TimeInForce = order.workingOrderData.timeInForce;

                        Orders.Add(model);
                    }
                }
            }
        }

        public string CreateOrder(string direction, string epic, string currency, double size, double? level, double? stop, double? limit, bool gStop)
        {
            var request = new CreatePositionRequest()
            {
                currencyCode = currency,
                direction = direction,
                expiry = "-",
                epic = epic,
                forceOpen = (stop.HasValue || limit.HasValue),
                guaranteedStop = gStop,
                level = level,
                limitDistance = limit,
                limitLevel = null,
                orderType = level.HasValue ? "LIMIT" : "MARKET",
                quoteId = null,
                size = (double?)size,
                stopDistance = stop,
                stopLevel = null,
                trailingStop = false,
                trailingStopIncrement = null
            };

            var result = _igRestApiClient.createPositionV2(request).RunSync();

            return result.Response != null ? result.Response.dealReference : null;
        }

        public void CloseOrder(PositionModel position)
        {
            var request = new ClosePositionRequest()
            {
                expiry = "-",
                dealId = position.DealId,
                direction = position.Direction == PositionModel.Directions.BUY ? "SELL" : "BUY",
                size = (double?)position.DealSize,
                orderType = "MARKET",
                epic = null,
                level = null,
                quoteId = null,
            };

            var result = _igRestApiClient.closePosition(request);
        }

        public void CloseOrder(PositionModel.Directions direction, string epic, double size)
        {
            var request = new ClosePositionRequest()
            {
                expiry = "-",
                dealId = null,
                direction = direction == PositionModel.Directions.BUY ? "SELL" : "BUY",
                size = (double?)size,
                orderType = "MARKET",
                epic = epic,
                level = null,
                quoteId = null,
            };

            var result = _igRestApiClient.closePosition(request);
        }

        public void CloseOrder(PositionModel position, double size, double? level)
        {
            var request = new ClosePositionRequest()
            {
                expiry = "-",
                dealId = position.DealId,
                direction = position.Direction == PositionModel.Directions.BUY ? "SELL" : "BUY",
                size = (double?)size,
                orderType = level.HasValue ? "LIMIT" : "MARKET",
                epic = null,
                level = level,
                quoteId = null,
            };

            var result = _igRestApiClient.closePosition(request);
        }

        public void UpdateOrder(PositionModel position, double? stopLevel, double? limitLevel)
        {
            var request = new EditPositionRequest()
            {
                limitLevel = limitLevel,
                stopLevel = stopLevel,
                trailingStop = false,
                trailingStopDistance = null,
                trailingStopIncrement = null,
            };

            var result = _igRestApiClient.editPositionV2(position.DealId, request);
        }
    }
}
