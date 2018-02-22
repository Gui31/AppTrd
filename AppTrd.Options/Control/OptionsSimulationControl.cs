using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AppTrd.BaseLib.Common;
using AppTrd.Options.Helper;
using AppTrd.Options.ViewModel.Item;

namespace AppTrd.Options.Control
{
    public class OptionsSimulationControl : System.Windows.Controls.Control
    {
        public class RenderingContext
        {
            public DrawingContext DrawingContext { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }

            public int SimulationWidth { get; set; }
            public int SimulationHeight { get; set; }

            public List<OptionItem> Options { get; set; }

            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }

            public double MinPrice { get; set; }
            public double MaxPrice { get; set; }

            public double CurrentPrice { get; set; }

            public List<double> HorizontalTicks { get; set; }
            public List<double> VerticalTicks { get; set; }

            public double[] Values { get; set; }
            public double MinValue { get; set; }
            public double MaxValue { get; set; }

            public bool DisplayMouse { get; set; }
            public int MouseX { get; set; }
            public int MouseY { get; set; }
        }

        public static readonly DependencyProperty OptionsProperty = DependencyProperty.Register(
            "Options", typeof(List<OptionItem>), typeof(OptionsSimulationControl), new PropertyMetadata(default(List<OptionItem>)));

        public List<OptionItem> Options
        {
            get { return (List<OptionItem>)GetValue(OptionsProperty); }
            set { SetValue(OptionsProperty, value); }
        }

        public static readonly DependencyProperty EndDateProperty = DependencyProperty.Register(
            "EndDate", typeof(DateTime), typeof(OptionsSimulationControl), new PropertyMetadata(default(DateTime)));

        public DateTime EndDate
        {
            get { return (DateTime)GetValue(EndDateProperty); }
            set { SetValue(EndDateProperty, value); }
        }

        public static readonly DependencyProperty MinPriceProperty = DependencyProperty.Register(
            "MinPrice", typeof(double), typeof(OptionsSimulationControl), new PropertyMetadata(default(double)));

        public double MinPrice
        {
            get { return (double)GetValue(MinPriceProperty); }
            set { SetValue(MinPriceProperty, value); }
        }

        public static readonly DependencyProperty MaxPriceProperty = DependencyProperty.Register(
            "MaxPrice", typeof(double), typeof(OptionsSimulationControl), new PropertyMetadata(default(double)));

        public double MaxPrice
        {
            get { return (double)GetValue(MaxPriceProperty); }
            set { SetValue(MaxPriceProperty, value); }
        }

        public static readonly DependencyProperty LastUpdateProperty = DependencyProperty.Register(
            "LastUpdate", typeof(DateTime), typeof(OptionsSimulationControl), new PropertyMetadata(default(DateTime), (o, args) => ((OptionsSimulationControl)o).LastUpdateChanged()));


        private void LastUpdateChanged()
        {
            _invalidateContext = true;
            InvalidateVisual();
        }

        public DateTime LastUpdate
        {
            get { return (DateTime)GetValue(LastUpdateProperty); }
            set { SetValue(LastUpdateProperty, value); }
        }

        public static readonly DependencyProperty CurrentPriceProperty = DependencyProperty.Register(
            "CurrentPrice", typeof(double), typeof(OptionsSimulationControl), new PropertyMetadata(default(double)));

        public double CurrentPrice
        {
            get { return (double)GetValue(CurrentPriceProperty); }
            set { SetValue(CurrentPriceProperty, value); }
        }

        private readonly Typeface _typeface;
        private readonly List<int> _greenCollection;
        private readonly List<int> _redCollection;

        private bool _invalidateContext;
        private RenderingContext _context;

        public OptionsSimulationControl()
        {
            IsManipulationEnabled = false;

            _typeface = new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);

            _greenCollection = new List<int>();
            _redCollection = new List<int>();

            _greenCollection.Add(GetColorData(0, 255, 150));
            _redCollection.Add(GetColorData(255, 0, 150));

            for (int i = 1; i < 256; i++)
            {
                var val = (byte)i;

                _greenCollection.Add(GetColorData(val, 255, val));
                _redCollection.Add(GetColorData(255, val, val));
            }
        }

        private int GetColorData(byte red, byte green, byte blue)
        {
            int colorData = red << 16;
            colorData |= green << 8;
            colorData |= blue << 0;

            return colorData;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            _invalidateContext = true;
            InvalidateVisual();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            var context = _context;

            if (context == null)
                return;

            var position = e.GetPosition(this);

            context.MouseX = (int)position.X;
            context.MouseY = (int)position.Y;
            context.DisplayMouse = position.X < context.SimulationWidth && position.Y < context.SimulationHeight;

            InvalidateVisual();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            var context = _context;

            if (context == null)
                return;

            context.DisplayMouse = false;

            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (Options == null)
                return;

            var context = _context;

            if (_invalidateContext || context == null)
            {
                context = new RenderingContext
                {
                    DrawingContext = drawingContext,
                    Width = ActualWidth,
                    Height = ActualHeight
                };

                if (context.Height < 1 || context.Width < 1)
                    return;

                context.SimulationWidth = (int)Math.Floor(context.Width);
                context.SimulationHeight = (int)Math.Floor(context.Height);

                context.StartDate = DateTime.Now;
                context.EndDate = EndDate;

                context.MinPrice = MinPrice;
                context.MaxPrice = MaxPrice;

                context.CurrentPrice = CurrentPrice;

                context.Options = Options.ToList();

                CalculateGrid(context);

                Simulate(context);

                _context = context;
            }
            else
            {
                context.DrawingContext = drawingContext;
            }

            Render(context);

            _context = context;
            _invalidateContext = false;
        }

        private void CalculateGrid(RenderingContext context)
        {
            var format = MaxPrice > 10 ? "0" : "0.0000";

            var typeface = new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            var testText = new FormattedText(context.MaxPrice.ToString(format), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, 10, Brushes.Black);

            context.SimulationHeight = (int)(context.Height - testText.Height - 2);
            context.SimulationWidth = (int)(context.Width - testText.Width - 2);

            var intervalHorizontal = LinearAxis.CalculateActualInterval(context.SimulationHeight, context.MinPrice, context.MaxPrice, 4);
            context.HorizontalTicks = LinearAxis.GetMajorValues(context.SimulationHeight, context.MinPrice, context.MaxPrice, intervalHorizontal).ToList();

            var maxTime = context.EndDate.Subtract(context.StartDate.Date).TotalDays;
            var delta = context.StartDate.Subtract(context.StartDate.Date).TotalDays;
            var intervalVertical = LinearAxis.CalculateActualInterval(context.SimulationWidth, 0, maxTime, 4);
            context.VerticalTicks = LinearAxis.GetMajorValues(context.SimulationWidth, 0, maxTime, intervalVertical).Select(t => t - delta).ToList();
        }

        private void Simulate(RenderingContext context)
        {
            var values = new double[context.SimulationWidth * context.SimulationHeight];
            var minValue = double.MaxValue;
            var maxValue = double.MinValue;

            var deltaTime = (context.EndDate.Subtract(context.StartDate).TotalDays) / context.SimulationWidth;
            var deltaPrice = (context.MaxPrice - context.MinPrice) / context.SimulationHeight;

            double baseValue = 0;

            //for (int i = 0; i < context.SimulationHeight; i++)
            Parallel.For(0, context.SimulationHeight, i =>
            {
                var price = context.MinPrice + (i * deltaPrice);

                for (int j = 0; j < context.SimulationWidth; j++)
                {
                    var time = context.StartDate.AddDays(j * deltaTime);
                    double value = 0;

                    foreach (var option in context.Options)
                    {
                        if (option.Action == OptionActions.None)
                            continue;

                        var isCall = option.Directions == OptionDirections.Call;
                        var timeToExpiry = option.Expiry.Subtract(time).TotalDays / 365;

                        if (option.Action == OptionActions.Buy)
                        {
                            value -= option.CurrentPrime * option.Quantity;
                            var prime = BlackScholesHelper.Prime(isCall, price, option.Strike, option.InterestRate / 100, option.Volatility / 100, timeToExpiry);
                            value += Math.Max(0, prime * option.Quantity);

                            value -= option.Spread;
                        }

                        if (option.Action == OptionActions.Sell)
                        {
                            value += option.CurrentPrime * option.Quantity;
                            var prime = BlackScholesHelper.Prime(isCall, price, option.Strike, option.InterestRate / 100, option.Volatility / 100, timeToExpiry);
                            value -= Math.Max(0, prime * option.Quantity);

                            value -= option.Spread;
                        }
                    }

                    minValue = Math.Min(minValue, value);
                    maxValue = Math.Max(maxValue, value);

                    values[(context.SimulationHeight - i - 1) * context.SimulationWidth + j] = value;
                }
            });

            context.Values = values;
            context.MinValue = minValue;
            context.MaxValue = maxValue;
        }

        private void Render(RenderingContext context)
        {
            RenderSimulation(context);

            RenderGrid(context);

            RenderInfo(context);

            if (context.DisplayMouse)
                RenderMouse(context);
        }

        private void RenderSimulation(RenderingContext context)
        {
            var writeableBitmap = new WriteableBitmap(
                context.SimulationWidth,
                context.SimulationHeight,
                96,
                96,
                PixelFormats.Bgr32,
                null);

            writeableBitmap.Lock();

            unsafe
            {
                for (int i = 0; i < context.SimulationHeight; i++)
                {
                    for (int j = 0; j < context.SimulationWidth; j++)
                    {
                        var pBackBuffer = (int)writeableBitmap.BackBuffer + (i * writeableBitmap.BackBufferStride) + (j * 4);

                        var value = context.Values[i * context.SimulationWidth + j];
                        int colorData = 0x00FFFFFF;

                        if (value > 0)
                        {
                            var val = (byte)(255 - (value / context.MaxValue) * 255);

                            colorData = _greenCollection[val];
                        }

                        if (value < 0)
                        {
                            var val = (byte)(255 - (value / context.MinValue) * 255);

                            colorData = _redCollection[val];
                        }

                        *((int*)pBackBuffer) = colorData;
                    }
                }
            }

            writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, context.SimulationWidth, context.SimulationHeight));

            writeableBitmap.Unlock();

            context.DrawingContext.DrawImage(writeableBitmap, new Rect(new Point(0, 0), new Size(context.SimulationWidth, context.SimulationHeight)));
        }

        private void RenderGrid(RenderingContext context)
        {
            var y = context.SimulationHeight - (context.CurrentPrice - context.MinPrice) / (context.MaxPrice - context.MinPrice) * context.SimulationHeight;
            var pen = new Pen(Brushes.Blue, 1);
            context.DrawingContext.DrawLine(pen, new Point(0, y), new Point(context.SimulationWidth, y));

            pen = new Pen(Brushes.DarkGray, 1);
            var format = MaxPrice > 10 ? "0" : "0.0000";

            foreach (var tick in context.HorizontalTicks)
            {
                y = context.SimulationHeight - (tick - context.MinPrice) / (context.MaxPrice - context.MinPrice) * context.SimulationHeight;

                if (y < 0 || y >= context.SimulationHeight)
                    continue;

                context.DrawingContext.DrawLine(pen, new Point(0, y), new Point(context.SimulationWidth, y));

                var priceText = new FormattedText(tick.ToString(format), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, _typeface, 10, Brushes.Black);
                context.DrawingContext.DrawText(priceText, new Point(context.SimulationWidth + 1, y - priceText.Height / 2));
            }

            var duration = context.EndDate.Subtract(context.StartDate).TotalDays;
            foreach (var tick in context.VerticalTicks)
            {
                var x = tick / duration * context.SimulationWidth;

                if (x < 0 || x >= context.SimulationWidth)
                    continue;

                context.DrawingContext.DrawLine(pen, new Point(x, 0), new Point(x, context.SimulationHeight));

                var date = context.StartDate.AddDays(tick);

                var dateText = new FormattedText(date.ToString("dd/MM/yy"), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, _typeface, 10, Brushes.Black);
                context.DrawingContext.DrawText(dateText, new Point(x - dateText.Width / 2, context.SimulationHeight + 1));
            }
        }

        public void RenderInfo(RenderingContext context)
        {
            context.DrawingContext.DrawRectangle(Brushes.White, null, new Rect(new Point(0, 0), new Size(50, 28)));

            context.DrawingContext.DrawRectangle(Brushes.Lime, null, new Rect(new Point(2, 2), new Size(10, 10)));
            context.DrawingContext.DrawRectangle(Brushes.Red, null, new Rect(new Point(2, 16), new Size(10, 10)));

            var winText = new FormattedText(context.MaxValue.ToString("#"), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, _typeface, 10, Brushes.Black);
            context.DrawingContext.DrawText(winText, new Point(14, 0));

            var looseText = new FormattedText(context.MinValue.ToString("#"), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, _typeface, 10, Brushes.Black);
            context.DrawingContext.DrawText(looseText, new Point(14, 14));
        }

        public void RenderMouse(RenderingContext context)
        {
            var deltaTime = (context.EndDate.Subtract(context.StartDate).TotalDays) / context.SimulationWidth;
            var deltaPrice = (context.MaxPrice - context.MinPrice) / context.SimulationHeight;

            var date = context.StartDate.AddDays(context.MouseX * deltaTime);
            var price = context.MaxPrice - context.MouseY * deltaPrice;
            var prime = context.Values[context.MouseY * context.SimulationWidth + context.MouseX];

            var format = MaxPrice > 10 ? "0" : "0.0000";
            var pen = new Pen(Brushes.Black, 1);

            context.DrawingContext.DrawLine(pen, new Point(0, context.MouseY + 0.5), new Point(context.SimulationWidth, context.MouseY + 0.5));

            var priceText = new FormattedText(price.ToString(format), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, _typeface, 10, Brushes.Black);
            var point = new Point(context.SimulationWidth, Math.Max(0, context.MouseY - priceText.Height / 2 - 1));
            context.DrawingContext.DrawRectangle(Brushes.White, pen, new Rect(point, new Size(priceText.Width + 2, priceText.Height + 2)));
            point.Offset(1, 1);
            context.DrawingContext.DrawText(priceText, point);

            context.DrawingContext.DrawLine(pen, new Point(context.MouseX + 0.5, 0), new Point(context.MouseX + 0.5, context.SimulationHeight));

            var dateText = new FormattedText(date.ToString("dd/MM/yy HH:mm"), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, _typeface, 10, Brushes.Black);
            point = new Point(Math.Max(0, context.MouseX - dateText.Width / 2 - 1), context.SimulationHeight);
            context.DrawingContext.DrawRectangle(Brushes.White, pen, new Rect(point, new Size(dateText.Width + 2, dateText.Height + 2)));
            point.Offset(1, 1);
            context.DrawingContext.DrawText(dateText, point);

            var primeText = new FormattedText(prime.ToString("0"), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, _typeface, 10, Brushes.Black);
            point = new Point(context.MouseX - primeText.Width - 3, context.MouseY - primeText.Height - 3);

            if (point.X < 0)
                point = new Point(context.MouseX + 1, point.Y);

            if (point.Y < 0)
                point = new Point(point.X, context.MouseY + 1);

            context.DrawingContext.DrawRectangle(Brushes.White, pen, new Rect(point, new Size(primeText.Width + 2, primeText.Height + 2)));
            point.Offset(1, 1);
            context.DrawingContext.DrawText(primeText, new Point(point.X, point.Y));
        }
    }
}
