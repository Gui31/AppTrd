using System.Collections.Generic;
using dto.endpoint.auth.session;

namespace dto.endpoint.accountbalance
{

public class AccountBalance{
	///<Summary>
	///Balance of funds in the account
	///</Summary>
	public double? balance { get; set; }
	///<Summary>
	///Minimum deposit amount required for margins
	///</Summary>
	public double? deposit { get; set; }
	///<Summary>
	///Profit and loss amount
	///</Summary>
	public double? profitLoss { get; set; }
	///<Summary>
	///Amount available for trading
	///</Summary>
	public double? available { get; set; }
}
}
