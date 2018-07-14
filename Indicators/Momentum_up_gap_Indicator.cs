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
        private double _percentage = 3.0;
        
        private bool _showarrows = true;
        private bool _showindicatorbox = false;

        private Color _color_arrow_long_signal = Const.DefaultArrowLongColor;
        private Color _color_arrow_long_echo = Const.DefaultArrowLong2Color;
        private Color _plot0color = Const.DefaultIndicatorColor;
        private int _plot0width = Const.DefaultLineWidth;
        private DashStyle _plot0dashstyle = Const.DefaultIndicatorDashStyle;

        private Stack<DateTime> lastgaps;
        private IBar _candlebeforegap = null;

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void OnInit()
        {
            Add(new OutputDescriptor(new Pen(this.Plot0Color, this.Plot0Width), OutputSerieDrawStyle.Line, " Momentum_up_gap_Indicator"));

            CalculateOnClosedBar = false;
            IsOverlay = true;
            IsAutoAdjustableScale = true;

            //Because of Backtesting reasons if we use the advanced mode we need at least two bars
            this.RequiredBarsCount = 20;

            this.TimeFrame = new TimeFrame(DatafeedHistoryPeriodicity.Day, 1);

            //if (this.Chart == null)
            //{
            //    this.ShowIndicatorBox = true;
            //}
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
                double gaphighlow = ((Low[0] * 100) / High[1]) - 100;
                bool therewasagap = false;

                //if (gapopen >= this.Percentage && gapclose >= this.Percentage)
                if (gaphighlow >= this.Percentage)
                {
                    therewasagap = true;
                    _candlebeforegap = Bars[1];
                }

                if (therewasagap)
                {
                    lastgaps.Push(Time[0]);
                    if (ShowArrows)
                    {
                        AddChartArrowUp("ArrowLong_Entry" + +Bars[0].Time.Ticks, this.IsAutoAdjustableScale, 0, Bars[0].Low, this.ColorArrowLongSignal);
                    }
                }

                if (lastgaps != null && lastgaps.Count > 0 && lastgaps.Peek() >= Time[this.Candles - 1])
                {
                    if (ShowArrows && !therewasagap)
                    {
                        AddChartArrowUp("ArrowLong_Entry" + +Bars[0].Time.Ticks, this.IsAutoAdjustableScale, 0, Bars[0].Low, this.ColorArrowLongEcho);
                    }
                    if (this.Chart == null || this.ShowIndicatorBox)
                    {
                        PlotLine.Set(1);
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
                AddChartTextFixed("AlertText", String.Format(Const.DefaultStringDatafeedBarsRequiredCount, 2), TextPosition.Center, Color.Red, new Font("Arial", 30), Color.Red, Color.Red, 20);
            }


           

            PlotColors[0][0] = this.Plot0Color;
            OutputDescriptors[0].PenStyle = this.Dash0Style;
            OutputDescriptors[0].Pen.Width = this.Plot0Width;

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
            get { return Outputs[0]; }
        }

        [Browsable(false)]
        [XmlIgnore()]
        public IBar CandleBeforeGap
        {
            get { return this._candlebeforegap; }
        }

        /// <summary>
        /// </summary>
        [Description("The script show a signal if the gap was during the last x candles.")]
        [InputParameter]
        [DisplayName("Candles")]
        public int Candles
        {
            get { return _candles; }
            set { _candles = value; }
        }

        /// <summary>
        /// </summary>
        [Description("Percentage for the up gap.")]
        [InputParameter]
        [DisplayName("Percentage")]
        public double Percentage
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