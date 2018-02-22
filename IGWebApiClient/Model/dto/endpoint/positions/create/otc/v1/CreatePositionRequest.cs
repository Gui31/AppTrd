using System.Collections.Generic;
using dto.endpoint.auth.session;

namespace dto.endpoint.positions.create.otc.v1
{

public class CreatePositionRequest{
	///<Summary>
	///Instrument epic identifier
	///</Summary>
	public string epic { get; set; }
	///<Summary>
	///Instrument expiry
	///</Summary>
	public string expiry { get; set; }
	///<Summary>
	///Deal direction
	///</Summary>
	public string direction { get; set; }
	///<Summary>
	///Deal size
	///</Summary>
	public double? size { get; set; }
	///<Summary>
	///Deal level
	///</Summary>
	public double? level { get; set; }
	///<Summary>
	///Order type
	///</Summary>
	public string orderType { get; set; }
	///<Summary>
	///True if a guaranteed stop is required
	///</Summary>
	public bool guaranteedStop { get; set; }
	///<Summary>
	///Stop level
	///</Summary>
	public double? stopLevel { get; set; }
	///<Summary>
	///Stop distance
	///</Summary>
	public double? stopDistance { get; set; }
	///<Summary>
	///True if force open is required
	///</Summary>
	public bool forceOpen { get; set; }
	///<Summary>
	///Limit level
	///</Summary>
	public double? limitLevel { get; set; }
	///<Summary>
	///Limit distance
	///</Summary>
	public double? limitDistance { get; set; }
	///<Summary>
	///Lightstreamer price quote identifier
	///</Summary>
	public string quoteId { get; set; }
	///<Summary>
	///Currency
	///</Summary>
	public string currencyCode { get; set; }
}
}
