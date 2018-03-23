using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight.Threading;
using AppTrd.BaseLib.Common;
using AppTrd.BaseLib.Model;
using AppTrd.BaseLib.Receiver;
using AppTrd.BaseLib.Service;

namespace AppTrd.Charts.Control
{
    public class CandleGraphControl : System.Windows.Controls.Control
    {
        private class CandleGraphContext
        {
            public string DisplayFormat { get; set; } = "0";
            public int CandleDuration { get; set; } = -1;
            public Periods Period { get; set; }

            public double Min { get; set; }
            public double Max { get; set; }

            public double GraphWidth { get; set; }
            public double GraphHeigh { get; set; }

            public double MaxTextWidth { get; set; }
            public double MaxTextHeight { get; set; }
        }

        private ObservableCollection<CandleData> _candles;
        private CandleData _currentCandle = null;
        private Window _parentWindow;

        private CandleGraphContext _graphContext = new CandleGraphContext();

        public static readonly DependencyProperty CandlesProperty = DependencyProperty.Register(
            "Candles", typeof(ObservableCollection<CandleData>), typeof(CandleGraphControl), new PropertyMetadata(default(ObservableCollection<CandleData>), (o, args) => ((CandleGraphControl)o).CandlesChanged(args)));

        private void CandlesChanged(DependencyPropertyChangedEventArgs args)
        {
            if (_candles != null)
            {
                _candles.CollectionChanged -= CandlesCollectionChanged;
                InvalidateVisual();
            }

            _candles = Candles;

            if (_candles != null)
            {
                _candles.CollectionChanged += CandlesCollectionChanged;
                CandlesCollectionChanged(null, null);
            }
        }

        private void CandlesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (_currentCandle != null)
            {
                _currentCandle.PropertyChanged -= CurrentCandlePropertyChanged;
            }

            _currentCandle = _candles.LastOrDefault();

            if (_currentCandle != null)
            {
                _currentCandle.PropertyChanged += CurrentCandlePropertyChanged;
            }

            DispatcherHelper.CheckBeginInvokeOnUI(InvalidateVisual);
        }

        private void CurrentCandlePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(InvalidateVisual);
        }

        public ObservableCollection<CandleData> Candles
        {
            get { return (ObservableCollection<CandleData>)GetValue(CandlesProperty); }
            set { SetValue(CandlesProperty, value); }
        }

        public static readonly DependencyProperty DoublePlacesFactorProperty = DependencyProperty.Register(
            "DoublePlacesFactor", typeof(int), typeof(CandleGraphControl), new PropertyMetadata(default(int), (o, args) => ((CandleGraphControl)o).DoublePlacesFactorChanged()));

        private void DoublePlacesFactorChanged()
        {
            var format = "0.";

            for (int i = 0; i < Math.Max(1, DoublePlacesFactor); i++)
                format += "0";

            _graphContext.DisplayFormat = format;
        }

        public int DoublePlacesFactor
        {
            get { return (int)GetValue(DoublePlacesFactorProperty); }
            set { SetValue(DoublePlacesFactorProperty, value); }
        }

        public static readonly DependencyProperty ScalingFactorProperty = DependencyProperty.Register(
            "ScalingFactor", typeof(double), typeof(CandleGraphControl), new PropertyMetadata(default(double), ((o, args) => ((CandleGraphControl)o).ScalingFactorChanged())));

        private void ScalingFactorChanged()
        {
            var format = "0.";

            for (int i = 0; i < Math.Max(1, ScalingFactor.ToString().Count(c => c == '0') + 1); i++)
                format += "0";

            _graphContext.DisplayFormat = format;
        }

        public double ScalingFactor
        {
            get { return (double)GetValue(ScalingFactorProperty); }
            set { SetValue(ScalingFactorProperty, value); }
        }

        public static readonly DependencyProperty PositionsProperty = DependencyProperty.Register(
            "Positions", typeof(ObservableCollection<PositionModel>), typeof(CandleGraphControl), new PropertyMetadata(default(ObservableCollection<PositionModel>)));

        public ObservableCollection<PositionModel> Positions
        {
            get { return (ObservableCollection<PositionModel>)GetValue(PositionsProperty); }
            set { SetValue(PositionsProperty, value); }
        }

        public static readonly DependencyProperty PeriodProperty = DependencyProperty.Register(
            "Period", typeof(Periods), typeof(CandleGraphControl), new PropertyMetadata(default(Periods)));

        public Periods Period
        {
            get { return (Periods)GetValue(PeriodProperty); }
            set { SetValue(PeriodProperty, value); }
        }

        public static readonly DependencyProperty HideTimeGridProperty = DependencyProperty.Register(
            "HideTimeGrid", typeof(bool), typeof(CandleGraphControl), new PropertyMetadata(default(bool)));

        public bool HideTimeGrid
        {
            get { return (bool)GetValue(HideTimeGridProperty); }
            set { SetValue(HideTimeGridProperty, value); }
        }

        public CandleGraphControl()
        {
            Loaded += CandleGraphControl_Loaded;
        }

        private void CandleGraphControl_Loaded(object sender, RoutedEventArgs e)
        {
            _parentWindow = GetParentWindow(this);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (_candles == null)
                return;

            if (_parentWindow != null)
            {
                var transform = TransformToAncestor(_parentWindow);
                var point = transform.Transform(new Point(0, 0));

                var xOffset = point.X % 1;
                var yOffset = point.Y % 1;

                var translate = new TranslateTransform(0.5 - xOffset, 0.5 - yOffset);

                drawingContext.PushTransform(translate);
            }

            var maxDatas = (int)Math.Round(ActualWidth / 6);

            var datas = _candles.Reverse().Take(maxDatas).ToList();

            if (datas.Count <= 0)
                return;

            _graphContext.Period = Period;
            _graphContext.CandleDuration = (int)Period;

            if (_graphContext.CandleDuration < 0 && datas.Count >= 2)
            {
                _graphContext.CandleDuration = (int)datas[0].Time.Subtract(datas[1].Time).TotalSeconds;
            }

            var min = double.MaxValue;
            var max = double.MinValue;

            foreach (var data in datas)
            {
                if (data.IsEmpty)
                    continue;

                if (data.Low < min)
                    min = data.Low;

                if (data.High > max)
                    max = data.High;
            }

            var margin = Math.Max((max - min) * 0.05, max * 0.0005);
            margin = max * 0.00025;

            _graphContext.Min = min - margin;
            _graphContext.Max = max + margin;

            RenderGrid(drawingContext);

            var index = 0;
            foreach (var candleData in datas)
            {
                if (HideTimeGrid == false)
                    RenderMinute(drawingContext, candleData, index);

                RenderCandle(drawingContext, candleData, index++);
            }

            RenderLastPrice(drawingContext, datas.FirstOrDefault());

            if (Positions != null)
            {
                var positions = Positions.ToList();

                foreach (var position in positions)
                {
                    RenderPosition(drawingContext, position, datas.FirstOrDefault());
                }
            }
        }

        private void RenderMinute(DrawingContext drawingContext, CandleData data, int index)
        {
            bool ok = false;
            string format = "HH:mm";

            switch (_graphContext.Period)
            {
                case Periods.OneSecond:
                    ok = data.Time.Second % 15 == 0;
                    format = "HH:mm:ss";
                    break;
                case Periods.OneMinute:
                    ok = data.Time.Minute % 5 == 0;
                    break;
                case Periods.TwoMinutes:
                    ok = data.Time.Minute % 10 == 0;
                    break;
                case Periods.ThreeMinutes:
                    ok = data.Time.Minute % 15 == 0;
                    break;
                case Periods.FiveMinutes:
                    ok = data.Time.Minute % 30 == 0;
                    break;
                case Periods.TenMinutes:
                    ok = data.Time.Minute == 0;
                    break;
                case Periods.FifteenMinutes:
                    ok = data.Time.Minute == 0;
                    break;
                case Periods.ThirtYMinutes:
                    ok = data.Time.Hour % 2 == 0 && data.Time.Minute == 0;
                    break;
                case Periods.OneHour:
                    ok = data.Time.Hour % 6 == 0;
                    break;
                case Periods.TwoHours:
                    ok = data.Time.Hour % 6 == 0;
                    format = "dd HH";
                    break;
                case Periods.ThreeHours:
                    ok = data.Time.Hour % 8 == 0;
                    format = "dd HH";
                    break;
                case Periods.FourHours:
                    ok = data.Time.Hour % 6 == 0;
                    format = "dd HH";
                    break;
                case Periods.OneDay:
                    ok = data.Time.DayOfWeek == DayOfWeek.Friday;
                    format = "dd/MM";
                    break;
                case Periods.OneWeek:
                    ok = data.Time.DayOfYear % 28 == 0;
                    format = "dd/MM";
                    break;
                case Periods.OneMonth:
                    ok = data.Time.Month % 4 == 0;
                    format = "MM/yy";
                    break;
            }

            if (ok)
            {
                var x = _graphContext.GraphWidth - 4 - 6 * index;
                var pen = new Pen(Brushes.Black, 1);

                drawingContext.PushOpacity(0.25);
                drawingContext.DrawLine(pen, new Point(x, 0), new Point(x, _graphContext.GraphHeigh));
                drawingContext.Pop();

                var typeface = new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
                var text = new FormattedText(data.Time.ToString(format), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, 10, Brushes.Black);

                drawingContext.DrawText(text, new Point(x - text.Width / 2, _graphContext.GraphHeigh + 2));
            }
        }

        private void RenderCandle(DrawingContext drawingContext, CandleData data, int index)
        {
            if (data.IsEmpty)
                return;

            var x = _graphContext.GraphWidth - 4 - 6 * index;
            var h = _graphContext.Max - _graphContext.Min;

            var open = Math.Round(_graphContext.GraphHeigh - ((data.Open - _graphContext.Min) / h * _graphContext.GraphHeigh));
            var close = Math.Round(_graphContext.GraphHeigh - ((data.Close - _graphContext.Min) / h * _graphContext.GraphHeigh));
            var low = Math.Round(_graphContext.GraphHeigh - ((data.Low - _graphContext.Min) / h * _graphContext.GraphHeigh));
            var high = Math.Round(_graphContext.GraphHeigh - ((data.High - _graphContext.Min) / h * _graphContext.GraphHeigh));

            var pen = new Pen(Brushes.Black, 1);
            var fill = data.Close < data.Open ? Brushes.LightCoral : Brushes.LightGreen;

            drawingContext.DrawLine(pen, new Point(x, low), new Point(x, high));
            drawingContext.DrawRectangle(fill, pen, new Rect(new Point(x - 2, open), new Point(x + 2, close)));
        }

        private void RenderGrid(DrawingContext drawingContext)
        {
            var interval = LinearAxis.CalculateActualInterval(_graphContext.GraphHeigh, _graphContext.Min, _graphContext.Max, 4);
            var ticks = LinearAxis.GetMajorValues(_graphContext.GraphHeigh, _graphContext.Min, _graphContext.Max, interval).ToList();

            var typeface = new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);

            _graphContext.MaxTextWidth = 0;
            _graphContext.MaxTextHeight = 0;

            foreach (var tick in ticks)
            {
                var text = new FormattedText(tick.ToString(_graphContext.DisplayFormat), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, 11, Brushes.Black);

                var width = Math.Floor(text.Width) + 1.5;
                if (width > _graphContext.MaxTextWidth)
                    _graphContext.MaxTextWidth = width;

                var height = Math.Floor(text.Height) + 1.5;
                if (height > _graphContext.MaxTextHeight)
                    _graphContext.MaxTextHeight = height;
            }

            _graphContext.MaxTextHeight = Math.Ceiling(_graphContext.MaxTextHeight);
            _graphContext.MaxTextWidth = Math.Ceiling(_graphContext.MaxTextWidth);

            _graphContext.GraphWidth = Math.Floor(ActualWidth - _graphContext.MaxTextWidth);
            _graphContext.GraphHeigh = Math.Floor(ActualHeight - _graphContext.MaxTextHeight);

            foreach (var tick in ticks)
            {
                var y = Math.Round(_graphContext.GraphHeigh - ((tick - _graphContext.Min) / (_graphContext.Max - _graphContext.Min) * _graphContext.GraphHeigh));

                var text = new FormattedText(tick.ToString(_graphContext.DisplayFormat), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, 10, Brushes.Black);
                drawingContext.DrawText(text, new Point(_graphContext.GraphWidth, y - text.Height / 2));

                var pen = new Pen(Brushes.Black, 1);
                drawingContext.PushOpacity(0.25);
                drawingContext.DrawLine(pen, new Point(0, y), new Point(_graphContext.GraphWidth - 1, y));
                drawingContext.Pop();
            }
        }

        private void RenderPosition(DrawingContext drawingContext, PositionModel position, CandleData lastCandle)
        {
            var length = DateTime.Now.Subtract(position.CreatedDate).TotalSeconds / _graphContext.CandleDuration * 6;

            var x1 = _graphContext.GraphWidth - 4 - length;
            var x2 = _graphContext.GraphWidth;
            var open = Math.Round(_graphContext.GraphHeigh - ((position.OpenLevel - _graphContext.Min) / (_graphContext.Max - _graphContext.Min) * _graphContext.GraphHeigh));

            var brush = Brushes.Blue;

            brush = position.Pnl > 0 ? Brushes.Lime : Brushes.Red;

            var pen = new Pen(brush, 1);

            if (x1 > 0)
            {
                drawingContext.DrawRectangle(brush, null, new Rect(new Point(x1 - 1, open - 1), new Size(3, 3)));
                drawingContext.DrawLine(pen, new Point(x1, open), new Point(x2, open));
            }
            else
                drawingContext.DrawLine(pen, new Point(0, open), new Point(x2, open));

            var pnl = position.Pnl;

            if (lastCandle.HasBidAsk && position.Direction == PositionModel.Directions.BUY)
                pnl = (lastCandle.Bid - position.OpenLevel) * position.DealSize * position.ContractSize;

            if (lastCandle.HasBidAsk && position.Direction == PositionModel.Directions.SELL)
                pnl = (position.OpenLevel - lastCandle.Ask) * position.DealSize * position.ContractSize;

            var dir = position.Direction == PositionModel.Directions.BUY ? "↑" : "↓";

            var typeface = new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            var text = new FormattedText($"{dir} {pnl:0.##}", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, 11, Brushes.White);

            drawingContext.DrawRectangle(Brushes.Black, null, new Rect(_graphContext.GraphWidth, open - text.Height / 2, text.Width, text.Height));
            drawingContext.DrawText(text, new Point(_graphContext.GraphWidth, open - text.Height / 2));
        }

        private void RenderLastPrice(DrawingContext drawingContext, CandleData lastCandle)
        {
            var y = Math.Round(_graphContext.GraphHeigh - ((lastCandle.LastPrice - _graphContext.Min) / (_graphContext.Max - _graphContext.Min) * _graphContext.GraphHeigh));

            var brush = Brushes.Black;

            var pen = new Pen(brush, 1);

            drawingContext.DrawLine(pen, new Point(0, y), new Point(_graphContext.GraphWidth, y));

            var typeface = new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            var text = new FormattedText($"{lastCandle.LastPrice:0.##}", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, 11, Brushes.White);

            drawingContext.DrawRectangle(Brushes.Black, null, new Rect(_graphContext.GraphWidth, y - text.Height / 2, text.Width, text.Height));
            drawingContext.DrawText(text, new Point(_graphContext.GraphWidth, y - text.Height / 2));
        }

        private Window GetParentWindow(DependencyObject child)
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null)
            {
                return null;
            }

            Window parent = parentObject as Window;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return GetParentWindow(parentObject);
            }
        }
    }
}
