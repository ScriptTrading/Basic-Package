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
/// Version: 1.2.8
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://github.com/ScriptTrading/Basic-Package/blob/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    [Description("Instruments with gaps down tend to go lower.")]
    [Category("Script-Trading Basic Package")]
    [IsEntryAttribute(true)]
	[IsStopAttribute(false)]
	[IsTargetAttribute(false)]
	[OverrulePreviousStopPrice(false)]
    public class Momentum_down_gap_Condition : UserScriptedCondition
	{
        #region Variables

        private int _candles = 14;
        private double _percentage = 3.0;

        private Color _plot0color = Const.DefaultIndicatorColor;
        private int _plot0width = Const.DefaultLineWidth;
        private DashStyle _plot0dashstyle = Const.DefaultIndicatorDashStyle;

        private Stack<DateTime> lastgaps;

        #endregion

        protected override void OnInit()
		{
			IsEntry = true;
			IsStop = false;
			IsTarget = false;

            Add(new Plot(new Pen(this.Plot0Color, this.Plot0Width), PlotStyle.Line, "Occurred"));
            Add(new Plot(new Pen(this.Plot0Color, this.Plot0Width), PlotStyle.Line, "Entry"));

            IsOverlay = false;
			CalculateOnClosedBar = false;
            IsAutoAdjustableScale = true;

            this.RequiredBarsCount = 20;

            this.TimeFrame = new TimeFrame(DatafeedHistoryPeriodicity.Day, 1);
        }

		protected override void OnCalculate()
		{

            if (ProcessingBarIndex == 0)
            {
                lastgaps = new Stack<DateTime>();
            }

            if (this.RequiredBarsCount > 2)
            {
                //double gapopen = ((Open[0] - Close[1]) * 100) / Close[1];
                //double gapclose = ((Close[0] - Close[1]) * 100) / Close[1];
                double gaphighlow = ((High[0] * 100) / Low[1]) - 100;
                bool therewasagap = false;

                //if (gapopen >= this.Percentage && gapclose >= this.Percentage)
                if (gaphighlow <= (this.Percentage * (-1)))
                {
                    therewasagap = true;
                }

                if (therewasagap)
                {
                    lastgaps.Push(Time[0]);
                }

                if (lastgaps != null && lastgaps.Count > 0 && lastgaps.Peek() >= Time[this.Candles - 1])
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
                AddChartTextFixed("AlertText", String.Format(Const.DefaultStringDatafeedBarsRequiredCount, 2), TextPosition.Center, Color.Red, new Font("Arial", 30), Color.Red, Color.Red, 20);
            }


            PlotColors[0][0] = this.Plot0Color;
            Plots[0].PenStyle = this.Dash0Style;
            Plots[0].Pen.Width = this.Plot0Width;
        }


        public override string ToString()
        {
            return "Momentum down gap (C)";
        }

        public override string DisplayName
        {
            get
            {
                return "Momentum down gap (C)";
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
        [Description("The script show a signal if the gap was during the last x candles.")]
        [Category("Parameters")]
        [DisplayName("Candles")]
        public int Candles
        {
            get { return _candles; }
            set { _candles = value; }
        }


        /// <summary>
        /// </summary>
        [Description("Percentage for the up gap.")]
        [Category("Parameters")]
        [DisplayName("Percentage")]
        public double Percentage
        {
            get { return _percentage; }
            set { _percentage = value; }
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
#region AgenaTrader Automaticaly Generated Code. Do not change it manualy

namespace AgenaTrader.UserCode
{
	#region Indicator

	public partial class UserIndicator
	{
		/// <summary>
		/// Instruments with gaps down tend to go lower.
		/// </summary>
		public Momentum_down_gap_Condition Momentum_down_gap_Condition(System.Int32 candles, System.Double percentage)
        {
			return Momentum_down_gap_Condition(InSeries, candles, percentage);
		}

		/// <summary>
		/// Instruments with gaps down tend to go lower.
		/// </summary>
		public Momentum_down_gap_Condition Momentum_down_gap_Condition(IDataSeries input, System.Int32 candles, System.Double percentage)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Momentum_down_gap_Condition>(input, i => i.Candles == candles && Math.Abs(i.Percentage - percentage) <= Double.Epsilon);

			if (indicator != null)
				return indicator;

			indicator = new Momentum_down_gap_Condition
						{
							RequiredBarsCount = RequiredBarsCount,
							CalculateOnClosedBar = CalculateOnClosedBar,
							InSeries = input,
							Candles = candles,
							Percentage = percentage
						};
			indicator.SetUp();

			CachedCalculationUnits.AddIndicator2Cache(indicator);

			return indicator;
		}
	}

	#endregion

	#region Strategy

	public partial class UserStrategy
	{
		/// <summary>
		/// Instruments with gaps down tend to go lower.
		/// </summary>
		public Momentum_down_gap_Condition Momentum_down_gap_Condition(System.Int32 candles, System.Double percentage)
		{
			return LeadIndicator.Momentum_down_gap_Condition(InSeries, candles, percentage);
		}

		/// <summary>
		/// Instruments with gaps down tend to go lower.
		/// </summary>
		public Momentum_down_gap_Condition Momentum_down_gap_Condition(IDataSeries input, System.Int32 candles, System.Double percentage)
		{
			if (IsInInit && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'OnInit()' method");

			return LeadIndicator.Momentum_down_gap_Condition(input, candles, percentage);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Instruments with gaps down tend to go lower.
		/// </summary>
		public Momentum_down_gap_Condition Momentum_down_gap_Condition(System.Int32 candles, System.Double percentage)
		{
			return LeadIndicator.Momentum_down_gap_Condition(InSeries, candles, percentage);
		}

		/// <summary>
		/// Instruments with gaps down tend to go lower.
		/// </summary>
		public Momentum_down_gap_Condition Momentum_down_gap_Condition(IDataSeries input, System.Int32 candles, System.Double percentage)
		{
			return LeadIndicator.Momentum_down_gap_Condition(input, candles, percentage);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Instruments with gaps down tend to go lower.
		/// </summary>
		public Momentum_down_gap_Condition Momentum_down_gap_Condition(System.Int32 candles, System.Double percentage)
		{
			return LeadIndicator.Momentum_down_gap_Condition(InSeries, candles, percentage);
		}

		/// <summary>
		/// Instruments with gaps down tend to go lower.
		/// </summary>
		public Momentum_down_gap_Condition Momentum_down_gap_Condition(IDataSeries input, System.Int32 candles, System.Double percentage)
		{
			return LeadIndicator.Momentum_down_gap_Condition(input, candles, percentage);
		}
	}

	#endregion

}

#endregion
