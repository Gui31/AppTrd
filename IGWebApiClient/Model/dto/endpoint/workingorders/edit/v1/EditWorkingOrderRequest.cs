using System.Collections.Generic;
using dto.endpoint.auth.session;

namespace dto.endpoint.workingorders.edit.v1
{

public class EditWorkingOrderRequest{
	///<Summary>
	///Time in force type
	///</Summary>
	public string timeInForce { get; set; }
	///<Summary>
	///Good until date
	///</Summary>
	public string goodTillDate { get; set; }
	///<Summary>
	///Stop level
	///</Summary>
	public double? stopLevel { get; set; }
	///<Summary>
	///Stop distance
	///</Summary>
	public double? stopDistance { get; set; }
	///<Summary>
	///Limit level
	///</Summary>
	public double? limitLevel { get; set; }
	///<Summary>
	///Limit distance
	///</Summary>
	public double? limitDistance { get; set; }
	///<Summary>
	///Deal type
	///</Summary>
	public string type { get; set; }
	///<Summary>
	///Limit level
	///</Summary>
	public double? level { get; set; }
}
}
