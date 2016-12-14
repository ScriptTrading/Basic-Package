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
/// Version: 1.2.4
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

    [Description("Instruments with gaps up tend to go higher.")]
    [Category("Script-Trading Basic Package")]
    public class Momentum_up_gap_Indicator : UserIndicator
    {

        //input
        private int _candles = 14;
        private int _percentage = 3;
        
        private bool _showarrows = true;
        private bool _showindicatorbox = false;

        private Color _color_arrow_long_signal = Const.DefaultArrowLongColor;
        private Color _color_arrow_long_echo = Const.DefaultArrowLong2Color;
        private Color _plot0color = Const.DefaultIndicatorColor;
        private int _plot0width = Const.DefaultLineWidth;
        private DashStyle _plot0dashstyle = Const.DefaultIndicatorDashStyle;

        private Stack<DateTime> lastgaps;

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(new Pen(this.Plot0Color, this.Plot0Width), PlotStyle.Line, "Plot_Line"));

            CalculateOnBarClose = false;
            Overlay = true;
            AutoScale = true;

            //Because of Backtesting reasons if we use the advanced mode we need at least two bars
            this.BarsRequired = 20;
        }

      

        protected override void OnBarUpdate()
        {
            if (CurrentBar == 0)
            {
                lastgaps = new Stack<DateTime>();
            }

            if (this.BarsRequired > 2)
            {
                //double gapopen = ((Open[0] - Close[1]) * 100) / Close[1];
                //double gapclose = ((Close[0] - Close[1]) * 100) / Close[1];
                double gaphighlow = ((Low[0] * 100) / High[1]) - 100;
                bool therewasagap = false;

                //if (gapopen >= this.Percentage && gapclose >= this.Percentage)
                if (gaphighlow >= this.Percentage)
                {
                    therewasagap = true;
                }

                if (therewasagap)
                {
                    lastgaps.Push(Time[0]);
                    if (ShowArrows)
                    {
                        DrawArrowUp("ArrowLong_Entry" + +Bars[0].Time.Ticks, this.AutoScale, 0, Bars[0].Low, this.ColorArrowLongSignal);
                    }
                }

                if (lastgaps != null && lastgaps.Count > 0 && lastgaps.Peek() >= Time[this.Candles - 1])
                {
                    if (ShowArrows && !therewasagap)
                    {
                        DrawArrowUp("ArrowLong_Entry" + +Bars[0].Time.Ticks, this.AutoScale, 0, Bars[0].Low, this.ColorArrowLongEcho);
                    }
                    if (this.ShowIndicatorBox)
                    {
                        PlotLine.Set(1);
                    }
                }
                else
                {
                    if (this.ShowIndicatorBox)
                    {
                        PlotLine.Set(0);
                    }
                }
            }
            else
            {
                DrawTextFixed("AlertText", String.Format(Const.DefaultStringDatafeedBarsRequiredCount, 2), TextPosition.Center, Color.Red, new Font("Arial", 30), Color.Red, Color.Red, 20);
            }


           

            PlotColors[0][0] = this.Plot0Color;
            Plots[0].PenStyle = this.Dash0Style;
            Plots[0].Pen.Width = this.Plot0Width;

        }


        public override string ToString()
        {
            return "Momentum up gap (I)";
        }

        public override string DisplayName
        {
            get
            {
                return "Momentum up gap (I)";
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
        public int Percentage
        {
            get { return _percentage; }
            set { _percentage = value; }
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
        [Description("Select Color for the long arrows signal.")]
        [Category("Plots")]
        [DisplayName("Arrow Signal")]
        public Color ColorArrowLongSignal
        {
            get { return _color_arrow_long_signal; }
            set { _color_arrow_long_signal = value; }
        }
        // Serialize Color object
        [Browsable(false)]
        public string ColorArrowLongSignalSerialize
        {
            get { return SerializableColor.ToString(_color_arrow_long_signal); }
            set { _color_arrow_long_signal = SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [Description("Select Color for the long arrows echo.")]
        [Category("Plots")]
        [DisplayName("Arrow Echo")]
        public Color ColorArrowLongEcho
        {
            get { return _color_arrow_long_echo; }
            set { _color_arrow_long_echo = value; }
        }
        // Serialize Color object
        [Browsable(false)]
        public string ColorArrowLongEchoSerialize
        {
            get { return SerializableColor.ToString(_color_arrow_long_echo); }
            set { _color_arrow_long_echo = SerializableColor.FromString(value); }
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
		/// Instruments with gaps up tend to go higher.
		/// </summary>
		public Momentum_up_gap_Indicator Momentum_up_gap_Indicator(System.Int32 candles, System.Int32 percentage)
        {
			return Momentum_up_gap_Indicator(Input, candles, percentage);
		}

		/// <summary>
		/// Instruments with gaps up tend to go higher.
		/// </summary>
		public Momentum_up_gap_Indicator Momentum_up_gap_Indicator(IDataSeries input, System.Int32 candles, System.Int32 percentage)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Momentum_up_gap_Indicator>(input, i => i.Candles == candles && i.Percentage == percentage);

			if (indicator != null)
				return indicator;

			indicator = new Momentum_up_gap_Indicator
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input,
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
		/// Instruments with gaps up tend to go higher.
		/// </summary>
		public Momentum_up_gap_Indicator Momentum_up_gap_Indicator(System.Int32 candles, System.Int32 percentage)
		{
			return LeadIndicator.Momentum_up_gap_Indicator(Input, candles, percentage);
		}

		/// <summary>
		/// Instruments with gaps up tend to go higher.
		/// </summary>
		public Momentum_up_gap_Indicator Momentum_up_gap_Indicator(IDataSeries input, System.Int32 candles, System.Int32 percentage)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.Momentum_up_gap_Indicator(input, candles, percentage);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Instruments with gaps up tend to go higher.
		/// </summary>
		public Momentum_up_gap_Indicator Momentum_up_gap_Indicator(System.Int32 candles, System.Int32 percentage)
		{
			return LeadIndicator.Momentum_up_gap_Indicator(Input, candles, percentage);
		}

		/// <summary>
		/// Instruments with gaps up tend to go higher.
		/// </summary>
		public Momentum_up_gap_Indicator Momentum_up_gap_Indicator(IDataSeries input, System.Int32 candles, System.Int32 percentage)
		{
			return LeadIndicator.Momentum_up_gap_Indicator(input, candles, percentage);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Instruments with gaps up tend to go higher.
		/// </summary>
		public Momentum_up_gap_Indicator Momentum_up_gap_Indicator(System.Int32 candles, System.Int32 percentage)
		{
			return LeadIndicator.Momentum_up_gap_Indicator(Input, candles, percentage);
		}

		/// <summary>
		/// Instruments with gaps up tend to go higher.
		/// </summary>
		public Momentum_up_gap_Indicator Momentum_up_gap_Indicator(IDataSeries input, System.Int32 candles, System.Int32 percentage)
		{
			return LeadIndicator.Momentum_up_gap_Indicator(input, candles, percentage);
		}
	}

	#endregion

}

#endregion
