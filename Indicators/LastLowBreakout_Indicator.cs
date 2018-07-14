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
    public class LastLowBreakout_Indicator : UserIndicator
	{
	
	 private bool _showarrows = true;
	 private bool _showindicatorbox = false;
	 private int _candles = 14;
     private int _period = 365;

     private Stack<DateTime> lastlows;

        private Color _color_arrow_long_signal = Const.DefaultArrowShortColor;
        private Color _color_arrow_long_echo = Const.DefaultArrowShort2Color;
        private Color _plot0color = Const.DefaultIndicatorColor;
        private int _plot0width = Const.DefaultLineWidth;
        private DashStyle _plot0dashstyle = Const.DefaultIndicatorDashStyle;
	 
		protected override void OnInit()
		{
			Add(new OutputDescriptor(new Pen(this.Plot0Color, this.Plot0Width), OutputSerieDrawStyle.Line, "LastLowBreakout_Indicator"));
			IsOverlay = true;
			CalculateOnClosedBar = true;
            IsAutoAdjustableScale = true;

            this.RequiredBarsCount = 400;

            this.TimeFrame = new TimeFrame(DatafeedHistoryPeriodicity.Day, 1);

            //if (this.Chart == null)
            //{
            //    this.ShowIndicatorBox = true;
            //}
        }
		


		protected override void OnCalculate()
		{
		if(ProcessingBarIndex == 0){
			lastlows = new Stack<DateTime>();
		}
        

            if (this.RequiredBarsCount > this.Period)
            {
                bool therewasasignal = false;

                if (GetSeriesLowestValue(Low, this.Period) == 0)
                {
                    therewasasignal = true;
                }

                if (therewasasignal)
                {
                    lastlows.Push(Time[0]);
                    if (ShowArrows)
                    {
                        AddChartArrowDown("ArrowShort_LHB" + +Bars[0].Time.Ticks, this.IsAutoAdjustableScale, 0, Bars[0].High, this.ColorArrowLongSignal);
                    }
                }

                if (lastlows != null && lastlows.Count > 0 && lastlows.Peek() >= Time[this.Candles - 1])
                {
                    if (ShowArrows && !therewasasignal)
                    {
                        AddChartArrowDown("ArrowShort_Entry" + +Bars[0].Time.Ticks, this.IsAutoAdjustableScale, 0, Bars[0].High, this.ColorArrowLongEcho);
                    }

                    if (this.Chart == null || this.ShowIndicatorBox)
                    {
                        PlotLine.Set(-1);
                    }
            }
                else
                {
                    if (this.Chart == null || this.ShowIndicatorBox)
                    {
                        PlotLine.Set(0);
                    }
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
            return "Last Low Breakout (I)";
        }

        public override string DisplayName
        {
            get
            {
                return "Last Low Breakout (I)";
            }
        }

		#region Properties

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries PlotLine
        {
			get { return Outputs[0]; }
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