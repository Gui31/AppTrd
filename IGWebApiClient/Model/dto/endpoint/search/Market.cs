using System.Collections.Generic;
using dto.endpoint.auth.session;

namespace dto.endpoint.search
{

public class Market{
	///<Summary>
	///Instrument epic identifier
	///</Summary>
	public string epic { get; set; }
	///<Summary>
	///Instrument name
	///</Summary>
	public string instrumentName { get; set; }
	///<Summary>
	///Instrument type
	///</Summary>
	public string instrumentType { get; set; }
	///<Summary>
	///Instrument expiry period
	///</Summary>
	public string expiry { get; set; }
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
	///Price net change
	///</Summary>
	public double? netChange { get; set; }
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
	///Price delay time in minutes
	///</Summary>
	public int delayTime { get; set; }
	///<Summary>
	///True if streaming prices are available, i.e. if the market is tradeable and the client has appropriate permissions
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
