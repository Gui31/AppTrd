using System.Collections.Generic;
using dto.endpoint.auth.session;

namespace dto.endpoint.marketdetails.v2
{

public class DepositBand{
	///<Summary>
	///Band minimum
	///</Summary>
	public double min { get; set; }
	///<Summary>
	///Band maximum
	///</Summary>
	public double max { get; set; }
	///<Summary>
	///Margin Percentage
	///</Summary>
	public double? margin { get; set; }
	///<Summary>
	///the currency for this currency band factor calculation
	///</Summary>
	public string currency { get; set; }
}
}
