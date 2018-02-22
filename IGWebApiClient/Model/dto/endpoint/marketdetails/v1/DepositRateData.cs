using System.Collections.Generic;
using dto.endpoint.auth.session;

namespace dto.endpoint.marketdetails.v1
{

public class DepositRateData{
	///<Summary>
	///Unit
	///</Summary>
	public string unit { get; set; }
	///<Summary>
	///Value
	///</Summary>
	public double? value { get; set; }
}
}