using System.Collections.Generic;
using dto.endpoint.auth.session;

namespace dto.endpoint.confirms
{

public class ConfirmsResponse{
	///<Summary>
	///Position status
	///</Summary>
	public string status { get; set; }
	///<Summary>
	///</Summary>
	public string reason { get; set; }
	///<Summary>
	///Deal status
	///</Summary>
	public string dealStatus { get; set; }
	///<Summary>
	///Instrument epic identifier
	///</Summary>
	public string epic { get; set; }
	///<Summary>
	///Instrument expiry
	///</Summary>
	public string expiry { get; set; }
	///<Summary>
	///Deal reference
	///</Summary>
	public string dealReference { get; set; }
	///<Summary>
	///Deal identifier
	///</Summary>
	public string dealId { get; set; }
	///<Summary>
	///List of affected deals
	///</Summary>
	public List<AffectedDeal> affectedDeals { get; set; }
	///<Summary>
	///Level
	///</Summary>
	public double? level { get; set; }
	///<Summary>
	///Size
	///</Summary>
	public double? size { get; set; }
	///<Summary>
	///Direction
	///</Summary>
	public string direction { get; set; }
	///<Summary>
	///Stop level
	///</Summary>
	public double? stopLevel { get; set; }
	///<Summary>
	///Limit level
	///</Summary>
	public double? limitLevel { get; set; }
	///<Summary>
	///Stop distance
	///</Summary>
	public double? stopDistance { get; set; }
	///<Summary>
	///Limit distance
	///</Summary>
	public double? limitDistance { get; set; }
	///<Summary>
	///True if guaranteed stop
	///</Summary>
	public bool guaranteedStop { get; set; }
}
}
