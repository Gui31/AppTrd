using System.Collections.Generic;
using dto.endpoint.auth.session;

namespace dto.endpoint.prices.v1
{

public class Price{
	///<Summary>
	///Bid price
	///</Summary>
	public double? bid { get; set; }
	///<Summary>
	///Ask price
	///</Summary>
	public double? ask { get; set; }
	///<Summary>
	///Last traded price.  This will generally be null for non exchange-traded instruments
	///</Summary>
	public double? lastTraded { get; set; }
}
}
