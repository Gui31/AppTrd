using System.Collections.Generic;
using dto.endpoint.auth.session;

namespace dto.endpoint.positions.edit.v1
{

public class EditPositionRequest{
	///<Summary>
	///Stop level
	///</Summary>
	public double? stopLevel { get; set; }
	///<Summary>
	///Limit level
	///</Summary>
	public double? limitLevel { get; set; }
}
}
