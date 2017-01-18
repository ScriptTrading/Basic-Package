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
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

/// <summary>
/// Version: 1.4.3
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// Shows the Risk Reward Ratio of your current trade on the chart.
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{


    public class rrr_resultobject {

        public string text = "flat";
        public double rrr = 0.0;
        public double rrr_price = 0.0;
        public double up = 0.0;
        public double down = 0.0;
        public double up_price = 0.0;
        public double down_price = 0.0;

        public double target_quant = 0.0;
        public double stop_quant = 0.0;

        public int target_count = 0;
        public int stop_count = 0;

        public rrr_resultobject() {

        }
    }


	[Description("Shows the Risk Reward Ratio of your current trade on the chart.")]
    [Category("Script-Trading Basic Package")]
    public class Risk_Reward_Ratio_Indicator_tool : UserIndicator
	{

        private static DateTime _lastupdate = DateTime.Now;
        private TextPosition _TextPositionRRR = TextPosition.TopRight;
        private int _FontSizeRRR = 20;
        private int _seconds = 2;
        private ITradingTrade openedtrade = null;
        private int _rounddecimal = 3;

        protected override void OnInit()
		{
			IsOverlay = true;
            CalculateOnClosedBar = false;
        }


        protected override void OnStart()
        {
            // Add event listener
            if (Chart != null)
                Chart.ChartPanelMouseMove += ChartControl_ChartPanelMouseMove;
  
            calculateannddrawdata(true);
        }
        

        private void ChartControl_ChartPanelMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //AddChartTextFixed("debug_string", "x: " + e.X + " y: "+e.Y, this.TextPositionRRR, Color.Black, new Font("Arial", this.FontSizeRRR, FontStyle.Regular), Color.Transparent, Color.Transparent);
            calculateannddrawdata();
        }

        protected override void OnCalculate()
		{
            calculateannddrawdata();
        }
        


        private void calculateannddrawdata(bool force = false) {

            int quantity = 0;
            double price = 0.0;
            PositionType marketposition = PositionType.Flat;
            IList<ITradingOrder> stopstargets = new List<ITradingOrder>();

            if (force || _lastupdate.AddSeconds(this._seconds) < DateTime.Now)
            {
                IEnumerable<ITradingOrder> _regorders = this.Root.Core.TradingManager.ActiveRegisteredOrders.Where(x => x.Instrument.Symbol == this.Instrument.Symbol);
                IEnumerable<ITradingOrder> _openorders = this.Root.Core.TradingManager.OpenedOrders.Where(x => x.Instrument.Symbol == this.Instrument.Symbol);

                
                if (this.TradeInfo != null)
                {
                    quantity = this.TradeInfo.Quantity;
                    price = TradeInfo.AvgPrice;
                    marketposition = TradeInfo.PositionType;
                }

                foreach (ITradingOrder item in _regorders)
                {
                    if (item.IsManuallyConfirmable && this.TradeInfo == null)
                    {
                        quantity = item.Quantity;
                        //price = item.Price;
                        //if (price == 0.0)
                        //{
                        //    price = item.StopPrice;
                        //}
                        price = item.GetThePrice();
                        if (item.IsLong)
                        {
                            marketposition = PositionType.Long;
                        }
                        else
                        {
                            marketposition = PositionType.Short;
                        }
                    }
                    else
                    {
                        stopstargets.Add(item);
                    }
                }

                _openorders = _openorders.Concat(stopstargets);

                rrr_resultobject resultdata = this.calculate(_openorders, quantity, price, marketposition);
               
              

                AddChartTextFixed("RRR_string", resultdata.text, this.TextPositionRRR, Color.Black, new Font("Arial", this.FontSizeRRR, FontStyle.Regular), Color.Transparent, Color.Transparent);
                _lastupdate = DateTime.Now;
            }
           
        }


        protected override void OnDispose()
        {
            // Remove event listener
            if (Chart != null)
                Chart.ChartPanelMouseMove -= ChartControl_ChartPanelMouseMove;
        }


        public rrr_resultobject calculate(IEnumerable<ITradingOrder> _openorders, int entry_quantity, double entry_price, PositionType positiontype) {
            rrr_resultobject result = new rrr_resultobject();

            if ( _openorders != null && _openorders.Count() > 0)
            {
                result.target_quant = entry_quantity;
                result.stop_quant = entry_quantity;
                //Print("Entry Price: " + entry_price);

                foreach (ITradingOrder item in _openorders)
                {
                    if ((positiontype == PositionType.Long && item.IsLong) || (positiontype == PositionType.Short && item.IsShort))
                    {
                        //
                    }
                    else
                    {
                        //double price = item.Price;
                        //if (price == 0.0)
                        //{
                        //    price = item.StopPrice;
                        //}
                        double price = item.GetThePrice();
                        //stop or target
                        if (price < entry_price && positiontype == PositionType.Long
                            || price > entry_price && positiontype == PositionType.Short)
                        {
                            result.stop_count = result.stop_count + 1;
                            result.stop_quant = result.stop_quant - item.Quantity;
                            result.down_price = result.down_price + (entry_price - price);
                            result.down = result.down + ((entry_price * item.Quantity) - (price * item.Quantity));
                            //Print("Stop Price: " + price);
                        }
                        else
                        {
                            result.target_count = result.target_count + 1;
                            result.target_quant = result.target_quant - item.Quantity;
                            result.up_price = result.up_price + (price - entry_price);
                            result.up = result.up + ((price * item.Quantity) - (entry_price * item.Quantity));
                            //Print("Target Price: " + price);
                        }
                    }
                }

            }
            

            result.rrr = result.up / result.down;
            result.rrr_price = (result.up_price / result.target_count) / (result.down_price / result.stop_count);

            if (result.target_quant < 0.0 || result.stop_quant < 0.0)
            {
                //result.text = "?";
                result.text = "?" + " / " + Math.Round(result.rrr_price, this.RoundDecimal).ToString();
            }
            else if(result.down == 0.0 && result.up != 0.0)
            {
                //text = "in risk";
                result.text = "-";
            }
            else if (result.down != 0.0 && result.up == 0.0)
            {
                //text = "in love";
                result.text = "-";
            }
            else if (result.down == 0.0 && result.up == 0.0)
            {
                if (entry_price >= Double.Epsilon)
                {
                    switch (positiontype)
                    {
                        case PositionType.Long:
                            result.text = "long";
                            break;
                        case PositionType.Short:
                            result.text = "short";
                            break;
                    }
                }
            }
            else
            {
                string textquant = "";
                if (result.target_quant > 0 && result.stop_quant > 0)
                {
                    textquant = "*";
                }
                else if (result.target_quant > 0)
                {
                    textquant = "+";
                }
                else if (result.stop_quant > 0)
                {
                    textquant = "-";
                }
                result.text = Math.Round(result.rrr, this.RoundDecimal).ToString() + textquant + " / " + Math.Round(result.rrr_price, this.RoundDecimal).ToString();
            }

            return result;
        }


        public override string ToString()
        {
            return "Risk Reward Ratio (I)";
        }

        public override string DisplayName
        {
            get
            {
                return "Risk Reward Ratio (I)";
            }
        }



        #region Properties
  
        /// <summary>
        /// </summary>
        [Description("Position of the text for your RRR.")]
        [Category("Parameters")]
        [DisplayName("TextPosition")]
        public TextPosition TextPositionRRR
        {
            get { return _TextPositionRRR; }
            set { _TextPositionRRR = value; }
        }


        /// <summary>
        /// </summary>
        [Description("Round to decimal numbers.")]
        [Category("Parameters")]
        [DisplayName("Decimal numbers")]
        public int RoundDecimal
        {
            get { return _rounddecimal; }
            set { _rounddecimal = value; }
        }

        /// <summary>
        /// </summary>
        [Description("Font size of the text for your RRR.")]
        [Category("Parameters")]
        [DisplayName("Font size")]
        public int FontSizeRRR
        {
            get { return _FontSizeRRR; }
            set { _FontSizeRRR = value; }
        }


        [Browsable(false)]
		[XmlIgnore()]
		public DataSeries MyPlot1
		{
			get { return Outputs[0]; }
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
		/// Shows the Risk Reward Ratio of your current trade on the chart.
		/// </summary>
		public Risk_Reward_Ratio_Indicator_tool Risk_Reward_Ratio_Indicator_tool()
        {
			return Risk_Reward_Ratio_Indicator_tool(InSeries);
		}

		/// <summary>
		/// Shows the Risk Reward Ratio of your current trade on the chart.
		/// </summary>
		public Risk_Reward_Ratio_Indicator_tool Risk_Reward_Ratio_Indicator_tool(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Risk_Reward_Ratio_Indicator_tool>(input);

			if (indicator != null)
				return indicator;

			indicator = new Risk_Reward_Ratio_Indicator_tool
						{
							RequiredBarsCount = RequiredBarsCount,
							CalculateOnClosedBar = CalculateOnClosedBar,
							InSeries = input
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
		/// Shows the Risk Reward Ratio of your current trade on the chart.
		/// </summary>
		public Risk_Reward_Ratio_Indicator_tool Risk_Reward_Ratio_Indicator_tool()
		{
			return LeadIndicator.Risk_Reward_Ratio_Indicator_tool(InSeries);
		}

		/// <summary>
		/// Shows the Risk Reward Ratio of your current trade on the chart.
		/// </summary>
		public Risk_Reward_Ratio_Indicator_tool Risk_Reward_Ratio_Indicator_tool(IDataSeries input)
		{
			if (IsInInit && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'OnInit()' method");

			return LeadIndicator.Risk_Reward_Ratio_Indicator_tool(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Shows the Risk Reward Ratio of your current trade on the chart.
		/// </summary>
		public Risk_Reward_Ratio_Indicator_tool Risk_Reward_Ratio_Indicator_tool()
		{
			return LeadIndicator.Risk_Reward_Ratio_Indicator_tool(InSeries);
		}

		/// <summary>
		/// Shows the Risk Reward Ratio of your current trade on the chart.
		/// </summary>
		public Risk_Reward_Ratio_Indicator_tool Risk_Reward_Ratio_Indicator_tool(IDataSeries input)
		{
			return LeadIndicator.Risk_Reward_Ratio_Indicator_tool(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Shows the Risk Reward Ratio of your current trade on the chart.
		/// </summary>
		public Risk_Reward_Ratio_Indicator_tool Risk_Reward_Ratio_Indicator_tool()
		{
			return LeadIndicator.Risk_Reward_Ratio_Indicator_tool(InSeries);
		}

		/// <summary>
		/// Shows the Risk Reward Ratio of your current trade on the chart.
		/// </summary>
		public Risk_Reward_Ratio_Indicator_tool Risk_Reward_Ratio_Indicator_tool(IDataSeries input)
		{
			return LeadIndicator.Risk_Reward_Ratio_Indicator_tool(input);
		}
	}

	#endregion

}

#endregion
