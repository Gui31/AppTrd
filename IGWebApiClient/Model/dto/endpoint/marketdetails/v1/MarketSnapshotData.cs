using System.Collections.Generic;
using dto.endpoint.auth.session;

namespace dto.endpoint.marketdetails.v1
{

public class MarketSnapshotData{
	///<Summary>
	///Market status
	///</Summary>
	public string marketStatus { get; set; }
	///<Summary>
	///Net price change on the day
	///</Summary>
	public double? netChange { get; set; }
	///<Summary>
	///Percentage price change on the day
	///</Summary>
	public double? percentageChange { get; set; }
	///<Summary>
	///Price last update time (hh:mm:ss)
	///</Summary>
	public string updateTime { get; set; }
	///<Summary>
	///Price delay
	///</Summary>
	public int delayTime { get; set; }
	///<Summary>
	///Bid price
	///</Summary>
	public double? bid { get; set; }
	///<Summary>
	///Offer price
	///</Summary>
	public double? offer { get; set; }
	///<Summary>
	///Highest price on the day
	///</Summary>
	public double? high { get; set; }
	///<Summary>
	///Lowest price on the day
	///</Summary>
	public double? low { get; set; }
	///<Summary>
	///Binary odds
	///</Summary>
	public double? binaryOdds { get; set; }
	///<Summary>
	///Number of double positions for market levels
	///</Summary>
	public int doublePlacesFactor { get; set; }
	///<Summary>
	///Multiplying factor to determine actual pip value for the levels used by the instrument
	///</Summary>
	public int scalingFactor { get; set; }
	///<Summary>
	///the number of points to add on each side of the market as an additional spread when
	///placing a guaranteed stop trade.
	///</Summary>
	public double? controlledRiskExtraSpread { get; set; }
}
}
