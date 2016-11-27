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
    [Category("Script-Trading Conditions")]
    [IsEntryAttribute(true)]
	[IsStopAttribute(false)]
	[IsTargetAttribute(false)]
	[OverrulePreviousStopPrice(false)]
    public class LastHighBreakout_Condition : UserScriptedCondition
	{
        #region Variables

        private int _candles = 14;
        private int _period = 365;

        private Color _plot0color = Const.DefaultIndicatorColor;
        private int _plot0width = Const.DefaultLineWidth;
        private DashStyle _plot0dashstyle = Const.DefaultIndicatorDashStyle;


        #endregion

        protected override void Initialize()
		{
			IsEntry = true;
			IsStop = false;
			IsTarget = false;

            Add(new Plot(new Pen(this.Plot0Color, this.Plot0Width), PlotStyle.Line, "Occurred"));
            Add(new Plot(new Pen(this.Plot0Color, this.Plot0Width), PlotStyle.Line, "Entry"));

            Overlay = false;
            CalculateOnBarClose = true;
            AutoScale = true;

            this.BarsRequired = 400;
        }

		protected override void OnBarUpdate()
		{
           Occurred.Set(LeadIndicator.LastHighBreakout_Indicator(this.Candles, this.Period)[0]);

            PlotColors[0][0] = this.Plot0Color;
            Plots[0].PenStyle = this.Dash0Style;
            Plots[0].Pen.Width = this.Plot0Width;

        }


        public override string ToString()
        {
            return "Last High Breakout (C)";
        }

        public override string DisplayName
        {
            get
            {
                return "Last High Breakout (C)";
            }
        }

        #region Properties

        [Browsable(false)]
		[XmlIgnore()]
		public DataSeries Occurred
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Entry
		{
			get { return Values[1]; }
		}

		public override IList<DataSeries> GetEntries()
		{
			return new[]{Entry};
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
		public LastHighBreakout_Condition LastHighBreakout_Condition(System.Int32 candles, System.Int32 period)
        {
			return LastHighBreakout_Condition(Input, candles, period);
		}

		/// <summary>
		/// This indicator shows an arrow on a new x days. The indicator will plot 1 if there was a high in a specific range (default: 52 week high in a 7 days range). 
		/// </summary>
		public LastHighBreakout_Condition LastHighBreakout_Condition(IDataSeries input, System.Int32 candles, System.Int32 period)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<LastHighBreakout_Condition>(input, i => i.Candles == candles && i.Period == period);

			if (indicator != null)
				return indicator;

			indicator = new LastHighBreakout_Condition
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
		public LastHighBreakout_Condition LastHighBreakout_Condition(System.Int32 candles, System.Int32 period)
		{
			return LeadIndicator.LastHighBreakout_Condition(Input, candles, period);
		}

		/// <summary>
		/// This indicator shows an arrow on a new x days. The indicator will plot 1 if there was a high in a specific range (default: 52 week high in a 7 days range). 
		/// </summary>
		public LastHighBreakout_Condition LastHighBreakout_Condition(IDataSeries input, System.Int32 candles, System.Int32 period)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.LastHighBreakout_Condition(input, candles, period);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// This indicator shows an arrow on a new x days. The indicator will plot 1 if there was a high in a specific range (default: 52 week high in a 7 days range). 
		/// </summary>
		public LastHighBreakout_Condition LastHighBreakout_Condition(System.Int32 candles, System.Int32 period)
		{
			return LeadIndicator.LastHighBreakout_Condition(Input, candles, period);
		}

		/// <summary>
		/// This indicator shows an arrow on a new x days. The indicator will plot 1 if there was a high in a specific range (default: 52 week high in a 7 days range). 
		/// </summary>
		public LastHighBreakout_Condition LastHighBreakout_Condition(IDataSeries input, System.Int32 candles, System.Int32 period)
		{
			return LeadIndicator.LastHighBreakout_Condition(input, candles, period);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// This indicator shows an arrow on a new x days. The indicator will plot 1 if there was a high in a specific range (default: 52 week high in a 7 days range). 
		/// </summary>
		public LastHighBreakout_Condition LastHighBreakout_Condition(System.Int32 candles, System.Int32 period)
		{
			return LeadIndicator.LastHighBreakout_Condition(Input, candles, period);
		}

		/// <summary>
		/// This indicator shows an arrow on a new x days. The indicator will plot 1 if there was a high in a specific range (default: 52 week high in a 7 days range). 
		/// </summary>
		public LastHighBreakout_Condition LastHighBreakout_Condition(IDataSeries input, System.Int32 candles, System.Int32 period)
		{
			return LeadIndicator.LastHighBreakout_Condition(input, candles, period);
		}
	}

	#endregion

}

#endregion
