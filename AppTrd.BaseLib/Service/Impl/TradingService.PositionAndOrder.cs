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
    }
}
