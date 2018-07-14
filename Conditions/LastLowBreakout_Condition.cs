using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using AgenaTrader.API;
using AgenaTrader.Custom;
using AgenaTrader.Plugins;
using AgenaTrader.Helper;

/// <summary>
/// Version: 1.8.0
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// todo
/// * ATR https://www.traderfox.de/anleitung/pivtal-chart-events/expansion-52-week-high/
/// * RSI
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://github.com/simonpucher/AgenaTrader/blob/master/Utility/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    [Description("This indicator shows an arrow on a new x days low. The indicator will plot 1 if there was a low in a specific range (default: 52 week low in a 14 days range).")]
    [Category("Script-Trading Basic Package")]
    [IsEntryAttribute(true)]
	[IsStopAttribute(false)]
	[IsTargetAttribute(false)]
	[OverrulePreviousStopPrice(false)]
    public class LastLowBreakout_Condition : UserScriptedCondition
	{
        #region Variables

        private int _candles = 14;
        private int _period = 365;

        private Stack<DateTime> lastlows;

        private Color _plot0color = Const.DefaultIndicatorColor;
        private int _plot0width = Const.DefaultLineWidth;
        private DashStyle _plot0dashstyle = Const.DefaultIndicatorDashStyle;


        #endregion

        protected override void OnInit()
		{
			IsEntry = true;
			IsStop = false;
			IsTarget = false;

            Add(new OutputDescriptor(new Pen(this.Plot0Color, this.Plot0Width), OutputSerieDrawStyle.Line, "Occurred"));
            Add(new OutputDescriptor(new Pen(this.Plot0Color, this.Plot0Width), OutputSerieDrawStyle.Line, "Entry"));

            IsOverlay = false;
            CalculateOnClosedBar = true;
            IsAutoAdjustableScale = true;

            this.RequiredBarsCount = 400;

            this.TimeFrame = new TimeFrame(DatafeedHistoryPeriodicity.Day, 1);
        }

		protected override void OnCalculate()
		{

            if (ProcessingBarIndex == 0)
            {
                lastlows = new Stack<DateTime>();
            }


            if (this.RequiredBarsCount > this.Period)
            {
                if (GetSeriesLowestValue(Low, this.Period) == 0)
                {
                    lastlows.Push(Time[0]);

                }

                if (lastlows != null && lastlows.Count > 0 && lastlows.Peek() >= Time[this.Candles - 1])
                {
                    Occurred.Set(-1);
                }
                else
                {
                    Occurred.Set(0);
                }
            }
            else
            {
                AddChartTextFixed("AlertText", String.Format(Const.DefaultStringDatafeedBarsRequiredCount, this.Period + 1), TextPosition.Center, Color.Red, new Font("Arial", 30), Color.Red, Color.Red, 20);
            }


            PlotColors[0][0] = this.Plot0Color;
            OutputDescriptors[0].PenStyle = this.Dash0Style;
            OutputDescriptors[0].Pen.Width = this.Plot0Width;

        }


        public override string ToString()
        {
            return "Last Low Breakout (C)";
        }

        public override string DisplayName
        {
            get
            {
                return "Last Low Breakout (C)";
            }
        }

        #region Properties

        [Browsable(false)]
		[XmlIgnore()]
		public DataSeries Occurred
		{
			get { return Outputs[0]; }
		}

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Entry
		{
			get { return Outputs[1]; }
		}

		public override IList<DataSeries> GetEntries()
		{
			return new[]{Entry};
		}

        /// <summary>
        /// </summary>
        [Description("The script show a signal if the last breakout was during the last x candles.")]
        [InputParameter]
        [DisplayName("Candles")]
        public int Candles
        {
            get { return _candles; }
            set { _candles = value; }
        }

        /// <summary>
        /// </summary>
        [Description("Period for the last high breakout. You should use 365 for one year.")]
        [InputParameter]
        [DisplayName("Period")]
        public int Period
        {
            get { return _period; }
            set { _period = value; }
        }

        /// <summary>
        /// </summary>
        [Description("Select Color for the indicator.")]
        [Category("Plots")]
        [DisplayName("Plot Color")]
        public Color Plot0Color
        {
            get { return _plot0color; }
            set { _plot0color = value; }
        }
        // Serialize Color object
        [Browsable(false)]
        public string Plot0ColorSerialize
        {
            get { return SerializableColor.ToString(_plot0color); }
            set { _plot0color = SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [Description("Line width for indicator.")]
        [Category("Plots")]
        [DisplayName("Plot Line width")]
        public int Plot0Width
        {
            get { return _plot0width; }
            set { _plot0width = Math.Max(1, value); }
        }

        /// <summary>
        /// </summary>
        [Description("DashStyle for indicator.")]
        [Category("Plots")]
        [DisplayName("Plot DashStyle")]
        public DashStyle Dash0Style
        {
            get { return _plot0dashstyle; }
            set { _plot0dashstyle = value; }
        }
        

        #endregion
    }
}