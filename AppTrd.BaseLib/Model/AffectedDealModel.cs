using GalaSoft.MvvmLight;

namespace AppTrd.BaseLib.Model
{
    public class AffectedDealModel : ViewModelBase
    {
        private string _affectedDeal_Status;
        public string AffectedDeal_Status
        {
            get { return _affectedDeal_Status; }
            set
            {
                if (_affectedDeal_Status != value)
                {
                    _affectedDeal_Status = value;
                    RaisePropertyChanged("AffectedDeal_Status");
                }
            }
        }

        private string _affectedDeal_Id;
        public string AffectedDeal_Id
        {
            get { return _affectedDeal_Id; }
            set
            {
                if (_affectedDeal_Id != value)
                {
                    _affectedDeal_Id = value;
                    RaisePropertyChanged("AffectedDeal_Id");
                }
            }
        }
    }
}