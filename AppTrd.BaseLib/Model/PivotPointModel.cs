using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTrd.BaseLib.Model
{
    public class PivotPointModel
    {
        public string Period { get; set; }

        public double Pivot { get; set; }

        public double S1 { get; set; }
        public double S2 { get; set; }
        public double S3 { get; set; }

        public double R1 { get; set; }
        public double R2 { get; set; }
        public double R3 { get; set; }
    }
}
