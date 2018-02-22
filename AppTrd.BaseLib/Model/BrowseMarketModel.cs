using System;
using System.Collections.Generic;
using dto.endpoint.auth.session;

namespace AppTrd.BaseLib.Model
{
    public class BrowseMarketModel
    {
        ///<Summary>
        ///Price delay time in minutes
        ///</Summary>
        public int delayTime { get; set; }
        ///<Summary>
        ///Instrument epic identifier
        ///</Summary>
        public string Epic { get; set; }
        ///<Summary>
        ///Price net change
        ///</Summary>
        public double? netChange { get; set; }
        ///<Summary>
        ///Instrument lot size
        ///</Summary>
        public int lotSize { get; set; }
        ///<Summary>
        ///Instrument expiry period
        ///</Summary>
        public string expiry { get; set; }
        ///<Summary>
        ///Instrument type
        ///</Summary>
        public string InstrumentType { get; set; }
        ///<Summary>
        ///Instrument name
        ///</Summary>
        public string InstrumentName { get; set; }
        ///<Summary>
        ///Highest price of the day
        ///</Summary>
        public double? high { get; set; }
        ///<Summary>
        ///Lowest price of the day
        ///</Summary>
        public double? low { get; set; }
        ///<Summary>
        ///Percentage price change on the day
        ///</Summary>
        public double? percentageChange { get; set; }
        ///<Summary>
        ///Time of last price update
        ///</Summary>
        public string updateTime { get; set; }
        ///<Summary>
        ///Bid price
        ///</Summary>
        public double? bid { get; set; }
        ///<Summary>
        ///Offer price
        ///</Summary>
        public double? offer { get; set; }
        ///<Summary>
        ///True if OTC tradeable
        ///</Summary>
        public bool otcTradeable { get; set; }
        ///<Summary>
        ///True if streaming prices are available, i.e. the market is tradeable and the client holds the necessary access permissions
        ///</Summary>
        public bool streamingPricesAvailable { get; set; }
        ///<Summary>
        ///Market status
        ///</Summary>
        public string MarketStatus { get; set; }
        ///<Summary>
        ///multiplying factor to determine actual pip value for the levels used by the instrument
        ///</Summary>
        public int scalingFactor { get; set; }

        public string OptionName
        {
            get
            {
                if (InstrumentType.StartsWith("OPT") == false)
                    return InstrumentName;

                return $"{InstrumentName} (price: {netChange:0.0}) (prime: {bid}|{offer})";
            }
        }
    }
}
