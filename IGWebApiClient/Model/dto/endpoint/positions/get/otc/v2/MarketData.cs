using System.Collections.Generic;
using dto.endpoint.auth.session;

namespace dto.endpoint.positions.get.otc.v2
{

public class MarketData{
	///<Summary>
	///Instrument name
	///</Summary>
	public string instrumentName { get; set; }
	///<Summary>
	///Instrument expiry period
	///</Summary>
	public string expiry { get; set; }
	///<Summary>
	///Instrument epic identifier
	///</Summary>
	public string epic { get; set; }
	///<Summary>
	///Instrument type
	///</Summary>
	public string instrumentType { get; set; }
	///<Summary>
	///Instrument lot size
	///</Summary>
	public double? lotSize { get; set; }
	///<Summary>
	///High price
	///</Summary>
	public double? high { get; set; }
	///<Summary>
	///Low price
	///</Summary>
	public double? low { get; set; }
	///<Summary>
	///Price percentage change
	///</Summary>
	public double? percentageChange { get; set; }
	///<Summary>
	///Price net change
	///</Summary>
	public double? netChange { get; set; }
	///<Summary>
	///Bid
	///</Summary>
	public double? bid { get; set; }
	///<Summary>
	///Offer
	///</Summary>
	public double? offer { get; set; }
	///<Summary>
	///Last instrument price update time
	///</Summary>
	public string updateTime { get; set; }
	///<Summary>
	///Instrument price delay (minutes)
	///</Summary>
	public int delayTime { get; set; }
	///<Summary>
	///True if streaming prices are available, i.e. the market is tradeable and the client has appropriate permissions
	///</Summary>
	public bool streamingPricesAvailable { get; set; }
	///<Summary>
	///Market status
	///</Summary>
	public string marketStatus { get; set; }
	///<Summary>
	///multiplying factor to determine actual pip value for the levels used by the instrument
	///</Summary>
	public int scalingFactor { get; set; }
}
}
