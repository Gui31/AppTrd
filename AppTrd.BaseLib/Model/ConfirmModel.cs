using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTrd.BaseLib.Model
{
    public class ConfirmModel
    {
        public string Epic;
        public string DealId;
        public string DealReference;
        public bool Accepted;
        public string Reason;
        public string Status;

        public double Level;
        public double Size;

        public List<ConfirmAffectedDealModel> AffectedDeals;
    }

    public class ConfirmAffectedDealModel
    {
        public string DealId;
        public string Status;
    }
}
