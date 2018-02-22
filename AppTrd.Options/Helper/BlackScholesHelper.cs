using System;

namespace AppTrd.Options.Helper
{
    public static class BlackScholesHelper
    {
        public static double Prime(bool isCall, double price, double strike, double interest, double volatility, double time)
        {
            var d1 = (Math.Log(price / strike) + (interest + volatility * volatility / 2.0) * time) / (volatility * Math.Sqrt(time));
            var d2 = d1 - volatility * Math.Sqrt(time);

            if (isCall)
                return price * CND(d1) - strike * Math.Exp(-interest * time) * CND(d2);

            return strike * Math.Exp(-interest * time) * CND(-d2) - price * CND(-d1);
        }

        public static double ImpliedVolatility(bool isCall, double price, double strike, double time, double interest, double prime, double e = 0.3)
        {
            var s = 1e-5;
            var o = e;
            var h = 1;
            var c = isCall ? "c" : "p";

            while (true)
            {
                var l = BlackScholes(c, price, strike, time, interest, o);
                var v = o - s;
                var y = BlackScholes(c, price, strike, time, interest, v);
                var a = (y - l) / s;

                if (Math.Abs(a) < 1e-5 || h == 100)
                    break;

                o = o - (prime - l) / a;
                h++;
            }

            return o;
        }

        private static double BlackScholes(string n, double t, double i, double r, double u, double f)
        {
            var e = CalculateD1(t, i, f, r, u);
            var o = e - f * Math.Sqrt(r);

            if (n == "p")
                return i * Math.Exp(-u * r) * CND(-o) - t * CND(-e);

            return t * CND(e) - i * Math.Exp(-u * r) * CND(o);
        }

        private static double CalculateD1(double n, double t, double i, double r, double u)
        {
            return (Math.Log(n / t) + (u + i * i / 2) * r) / (i * Math.Sqrt(r));
        }

        private static double CND(double n)
        {
            if (n < 0)
                return 1 - CND(-n);

            const double i = .31938153;
            const double r = -.356563782;
            const double u = 1.781477937;
            const double f = -1.821255978;
            const double e = 1.330274429;
            var t = 1 / (1 + .2316419 * n);

            return (1 - Math.Exp(-n * n / 2) / Math.Sqrt(2 * Math.PI) * t * (i + t * (r + t * (u + t * (f + t * e)))));
        }
    }
}
