using System.Collections.Generic;
using dto.endpoint.auth.session;

namespace AppTrd.BaseLib.Model
{
    public class BrowseModel
    {
        ///<Summary>
        ///Child market hierarchy nodes
        ///</Summary>
        public List<BrowseNodeModel> Nodes { get; set; }
        ///<Summary>
        ///List of markets (applicable only to leaf nodes of the hierarchy tree)
        ///</Summary>
        public List<BrowseMarketModel> Markets { get; set; }
    }
}
