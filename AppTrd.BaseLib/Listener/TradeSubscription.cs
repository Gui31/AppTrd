using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Lightstreamer.DotNet.Client;
using Microsoft.Practices.ServiceLocation;
using AppTrd.BaseLib.Messages;
using AppTrd.BaseLib.Model;
using AppTrd.BaseLib.Service;
using AppTrd.BaseLib.Service.Impl;
using IGWebApiClient;
using Newtonsoft.Json;

namespace AppTrd.BaseLib.Listener
{
    public class TradeSubscription : HandyTableListenerAdapter
    {
        private readonly TradingService _tradingService;

        public TradeSubscription(TradingService tradingService)
        {
            _tradingService = tradingService;

            if (_tradingService == null)
                throw new ApplicationException("Wrong ITradingService implementation");
        }

        public override void OnUpdate(int itemPos, string itemName, IUpdateInfo update)
        {
            try
            {
                var confirms = update.GetNewValue("CONFIRMS");
                var opu = update.GetNewValue("OPU");
                var wou = update.GetNewValue("WOU");

                if (!(String.IsNullOrEmpty(confirms)))
                {
                    ProcessConfirmsMessage(confirms);
                }

                if (!(String.IsNullOrEmpty(opu)))
                {
                    ProcessOpuMessage(opu);
                }

                if (!(String.IsNullOrEmpty(wou)))
                {
                    ProcessWouMessage(wou);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void ProcessOpuMessage(string message)
        {
            var opu = JsonConvert.DeserializeObject<LsTradeSubscriptionData>(message);

            if (opu.dealStatus.HasValue == false || opu.dealStatus.Value != StreamingDealStatusEnum.ACCEPTED)
            {
                //TODO : error message

                return;
            }

            if (opu.status.HasValue == false)
            {
                //TODO : error message

                return;
            }

            switch (opu.status.Value)
            {
                case StreamingStatusEnum.OPEN:
                    OpenPosition(opu);
                    break;
                case StreamingStatusEnum.UPDATED:
                    UpdatePosition(opu);
                    break;
                case StreamingStatusEnum.DELETED:
                    DeletePosition(opu);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OpenPosition(LsTradeSubscriptionData opu)
        {
            var position = new PositionModel();
            
            var market = _tradingService.GetInstrument(opu.epic);

            position.Instrument = market;

            position.DealId = opu.dealId;
            position.DealSize = Convert.ToDouble(opu.size);

            if (opu.direction != null)
                position.Direction = (PositionModel.Directions)opu.direction;

            position.OpenLevel = Convert.ToDouble(opu.level);
            position.StopLevel = string.IsNullOrEmpty(opu.stopLevel) ? null : (double?)Convert.ToDouble(opu.stopLevel);
            position.LimitLevel = string.IsNullOrEmpty(opu.limitLevel) ? null : (double?)Convert.ToDouble(opu.limitLevel);

            DateTime date;
            if (DateTime.TryParseExact(opu.timestamp, "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                position.CreatedDate = date.ToLocalTime();

            //if (DateTime.TryParseExact(opu.expiryTime, "yyyy/MM/dd HH:mm:ss:fff", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            //    position.Expirity = date;

            position.GuaranteedStop = opu.guaranteedStop;

            _tradingService.AddPosition(position);
        }

        private void UpdatePosition(LsTradeSubscriptionData opu)
        {
            var position = _tradingService.Positions.FirstOrDefault(p => p.DealId == opu.dealId);

            if (position == null)
            {
                //TODO : error message

                return;
            }

            position.DealSize = Convert.ToDouble(opu.size);
            position.StopLevel = string.IsNullOrEmpty(opu.stopLevel) ? null : (double?)Convert.ToDouble(opu.stopLevel);
            position.LimitLevel = string.IsNullOrEmpty(opu.limitLevel) ? null : (double?)Convert.ToDouble(opu.limitLevel);
        }

        private void DeletePosition(LsTradeSubscriptionData opu)
        {
            var position = _tradingService.Positions.FirstOrDefault(p => p.DealId == opu.dealId);

            if (position == null)
            {
                //TODO : error message

                return;
            }

            _tradingService.DeletePosition(position);
        }

        private void ProcessWouMessage(string message)
        {
            var wou = JsonConvert.DeserializeObject<LsTradeSubscriptionData>(message);

            if (wou.dealStatus.HasValue == false || wou.dealStatus.Value != StreamingDealStatusEnum.ACCEPTED)
            {
                //TODO : error message

                return;
            }

            if (wou.status.HasValue == false)
            {
                //TODO : error message

                return;
            }

            switch (wou.status.Value)
            {
                case StreamingStatusEnum.OPEN:
                    OpenOrder(wou);
                    break;
                case StreamingStatusEnum.UPDATED:
                    UpdateOrder(wou);
                    break;
                case StreamingStatusEnum.DELETED:
                    DeleteOrder(wou);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OpenOrder(LsTradeSubscriptionData wou)
        {
            var order = new OrderModel();

            var market = _tradingService.GetInstrument(wou.epic);

            order.Model = market;

            order.DealId = wou.dealId;
            order.OrderSize = double.Parse(wou.size);
            order.Direction = wou.direction.ToString();
            order.CreationDate = DateTime.Now.ToString();
            //order.GoodTillDate = wou.expiry;
            //order.OrderType = wou.;
            order.OrderLevel = double.Parse(wou.level);
            order.StopDistance = string.IsNullOrEmpty(wou.stopDistance) ? null : (double?)double.Parse(wou.stopLevel);
            order.LimitDistance = string.IsNullOrEmpty(wou.limitDistance) ? null : (double?)double.Parse(wou.limitLevel);
            //order.TimeInForce = order.workingOrderData.timeInForce;
            //order.StopLevel = string.IsNullOrEmpty(opu.stopLevel) ? null : (double?)double.Parse(opu.stopLevel);
            //order.LimitLevel = string.IsNullOrEmpty(opu.limitLevel) ? null : (double?)double.Parse(opu.limitLevel);
            //order.CreatedDate = DateTime.Now.ToShortDateString();
            //order.GuaranteedStop = opu.guaranteedStop;

            _tradingService.AddOrder(order);
        }

        private void UpdateOrder(LsTradeSubscriptionData wou)
        {
            var order = _tradingService.Orders.FirstOrDefault(p => p.DealId == wou.dealId);

            if (order == null)
            {
                //TODO : error message

                return;
            }

            order.OrderSize = double.Parse(wou.size);
            order.OrderLevel = double.Parse(wou.level);
            order.StopDistance = string.IsNullOrEmpty(wou.stopDistance) ? null : (double?)double.Parse(wou.stopLevel);
            order.LimitDistance = string.IsNullOrEmpty(wou.limitDistance) ? null : (double?)double.Parse(wou.limitLevel);
        }

        private void DeleteOrder(LsTradeSubscriptionData wou)
        {
            var order = _tradingService.Orders.FirstOrDefault(p => p.DealId == wou.dealId);

            if (order == null)
            {
                //TODO : error message

                return;
            }

            _tradingService.DeleteOrder(order);
        }

        private void ProcessConfirmsMessage(string message)
        {
            var confirms = JsonConvert.DeserializeObject<LsTradeSubscriptionData>(message);

            var model = new ConfirmModel
            {
                Accepted = confirms.dealStatus.HasValue && confirms.dealStatus.Value == StreamingDealStatusEnum.ACCEPTED,
                DealId = confirms.dealId,
                DealReference = confirms.dealReference,
                Epic = confirms.epic,
                //Reason = confirms.reason,
                Status = confirms.status.ToString(),
                Level = Convert.ToDouble(confirms.level),
                Size = Convert.ToDouble(confirms.size),
                AffectedDeals = new List<ConfirmAffectedDealModel>(confirms.affectedDeals.Select(d => new ConfirmAffectedDealModel { DealId = d.dealId, Status = d.status })),
            };

            Messenger.Default.Send(new ConfirmOrderMessage(model), model.Epic);

            //if (confirms.dealStatus.HasValue == false || confirms.dealStatus.Value != StreamingDealStatusEnum.ACCEPTED)
            //{
            //    //TODO : error message

            //    return;
            //}

            //if (confirms.status.HasValue == false)
            //{
            //    //TODO : error message

            //    return;
            //}

            //switch (confirms.status.Value)
            //{
            //    case StreamingStatusEnum.OPEN:
            //        break;
            //    case StreamingStatusEnum.OPENED:
            //        break;
            //    case StreamingStatusEnum.UPDATED:
            //        break;
            //    case StreamingStatusEnum.AMENDED:
            //        break;
            //    case StreamingStatusEnum.CLOSED:
            //        break;
            //    case StreamingStatusEnum.FULLY_CLOSED:
            //        break;
            //    case StreamingStatusEnum.PARTIALLY_CLOSED:
            //        break;
            //    case StreamingStatusEnum.DELETED:
            //        break;
            //    default:
            //        throw new ArgumentOutOfRangeException();
            //}
        }
    }
}
