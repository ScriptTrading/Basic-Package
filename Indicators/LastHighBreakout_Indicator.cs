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
/// Version: 1.3.1
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// todo
/// * ATR https://www.traderfox.de/anleitung/pivtal-chart-events/expansion-52-week-high/
/// * RSI
/// -------------------------------------------------------------------------
/// This indicator shows an arrow on a new x days. The indicator will plot 1 if there was a high in a specific range (default: 52 week high in a 14 days range). 
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://github.com/simonpucher/AgenaTrader/blob/master/Utility/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    [Description("This indicator shows an arrow on a new x days. The indicator will plot 1 if there was a high in a specific range (default: 52 week high in a 7 days range). ")]
    [Category("Script-Trading Indicators")]
    public class LastHighBreakout_Indicator : UserIndicator
	{
	
	 private bool _showarrows = true;
	 private bool _showindicatorbox = true;
	 private int _candles = 14;
     private int _period = 365;

     private Stack<DateTime> lasthighs;

        private Color _color_arrow_long = Const.DefaultArrowLongColor;
        private Color _plot0color = Const.DefaultIndicatorColor;
        private int _plot0width = Const.DefaultLineWidth;
        private DashStyle _plot0dashstyle = Const.DefaultIndicatorDashStyle;
	 
		protected override void Initialize()
		{
			Add(new Plot(new Pen(this.Plot0Color, this.Plot0Width), PlotStyle.Line, "LastHighBreakout_Indicator"));
			Overlay = false;
			CalculateOnBarClose = true;
            AutoScale = true;

            this.BarsRequired = 400;
		}
		


		protected override void OnBarUpdate()
		{
		if(CurrentBar == 0){
			lasthighs = new Stack<DateTime>();
		}

			if(HighestBar(High, this.Period) == 0) {
				lasthighs.Push(Time[0]);
				if (ShowArrows)
                {
                    DrawArrowUp("ArrowLong_LHB" + +Bars[0].Time.Ticks, this.AutoScale, 0, Bars[0].Low, this.ColorArrowLong);
                }
			}
			
			if(lasthighs != null && lasthighs.Count > 0 && lasthighs.Peek() >= Time[this.Candles - 1]) {
			    if(this.ShowIndicatorBox){
                    PlotLine.Set(1);
			    }
			} else {
			    if(this.ShowIndicatorBox){
                    PlotLine.Set(0);
			    }
			}
		}
		
		
        public override string ToString()
        {
            return "Last High Breakout (I)";
        }

        public override string DisplayName
        {
            get
            {
                return "Last High Breakout (I)";
            }
        }

		#region Properties

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries PlotLine
        {
			get { return Values[0]; }
		}
		
		  /// <summary>
        /// </summary>
        [Description("If true then arrows are drawn on the chart.")]
        [Category("Plots")]
        [DisplayName("Show arrows")]
        public bool ShowArrows
        {
            get { return _showarrows; }
            set { _showarrows = value; }
        }

	  /// <summary>
        /// </summary>
        [Description("If true then indicator box drawn on the chart.")]
        [Category("Plots")]
        [DisplayName("Show indicatorbox")]
        public bool ShowIndicatorBox
        {
            get { return _showindicatorbox; }
            set { _showindicatorbox = value; }
        }

        /// <summary>
        /// </summary>
        [Description("The script show a signal if the last breakout was during the last x candles.")]
        [Category("Parameters")]
        [DisplayName("Candles")]
        public int Candles
        {
            get { return _candles; }
            set { _candles = value; }
        }

        /// <summary>
        /// </summary>
        [Description("Period for the last high breakout. You should use 365 for one year.")]
        [Category("Parameters")]
        [DisplayName("Period")]
        public int Period
        {
            get { return _period; }
            set { _period = value; }
        }

        /// <summary>
        /// </summary>
        [Description("Select Color for the long arrows.")]
        [Category("Plots")]
        [DisplayName("Arrow Long")]
        public Color ColorArrowLong
        {
            get { return _color_arrow_long; }
            set { _color_arrow_long = value; }
        }
        // Serialize Color object
        [Browsable(false)]
        public string ColorArrowLongSerialize
        {
            get { return SerializableColor.ToString(_color_arrow_long); }
            set { _color_arrow_long = SerializableColor.FromString(value); }
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
		/// This indicator shows an arrow on a new x days. The indicator will plot 1 if there was a high in a specific range (default: 52 week high in a 7 days range). 
		/// </summary>
		public LastHighBreakout_Indicator LastHighBreakout_Indicator(System.Int32 candles, System.Int32 period)
        {
			return LastHighBreakout_Indicator(Input, candles, period);
		}

		/// <summary>
		/// This indicator shows an arrow on a new x days. The indicator will plot 1 if there was a high in a specific range (default: 52 week high in a 7 days range). 
		/// </summary>
		public LastHighBreakout_Indicator LastHighBreakout_Indicator(IDataSeries input, System.Int32 candles, System.Int32 period)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<LastHighBreakout_Indicator>(input, i => i.Candles == candles && i.Period == period);

			if (indicator != null)
				return indicator;

			indicator = new LastHighBreakout_Indicator
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input,
							Candles = candles,
							Period = period
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
		/// This indicator shows an arrow on a new x days. The indicator will plot 1 if there was a high in a specific range (default: 52 week high in a 7 days range). 
		/// </summary>
		public LastHighBreakout_Indicator LastHighBreakout_Indicator(System.Int32 candles, System.Int32 period)
		{
			return LeadIndicator.LastHighBreakout_Indicator(Input, candles, period);
		}

		/// <summary>
		/// This indicator shows an arrow on a new x days. The indicator will plot 1 if there was a high in a specific range (default: 52 week high in a 7 days range). 
		/// </summary>
		public LastHighBreakout_Indicator LastHighBreakout_Indicator(IDataSeries input, System.Int32 candles, System.Int32 period)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.LastHighBreakout_Indicator(input, candles, period);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// This indicator shows an arrow on a new x days. The indicator will plot 1 if there was a high in a specific range (default: 52 week high in a 7 days range). 
		/// </summary>
		public LastHighBreakout_Indicator LastHighBreakout_Indicator(System.Int32 candles, System.Int32 period)
		{
			return LeadIndicator.LastHighBreakout_Indicator(Input, candles, period);
		}

		/// <summary>
		/// This indicator shows an arrow on a new x days. The indicator will plot 1 if there was a high in a specific range (default: 52 week high in a 7 days range). 
		/// </summary>
		public LastHighBreakout_Indicator LastHighBreakout_Indicator(IDataSeries input, System.Int32 candles, System.Int32 period)
		{
			return LeadIndicator.LastHighBreakout_Indicator(input, candles, period);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// This indicator shows an arrow on a new x days. The indicator will plot 1 if there was a high in a specific range (default: 52 week high in a 7 days range). 
		/// </summary>
		public LastHighBreakout_Indicator LastHighBreakout_Indicator(System.Int32 candles, System.Int32 period)
		{
			return LeadIndicator.LastHighBreakout_Indicator(Input, candles, period);
		}

		/// <summary>
		/// This indicator shows an arrow on a new x days. The indicator will plot 1 if there was a high in a specific range (default: 52 week high in a 7 days range). 
		/// </summary>
		public LastHighBreakout_Indicator LastHighBreakout_Indicator(IDataSeries input, System.Int32 candles, System.Int32 period)
		{
			return LeadIndicator.LastHighBreakout_Indicator(input, candles, period);
		}
	}

	#endregion

}

#endregion
