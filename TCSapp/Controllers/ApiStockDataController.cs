
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Web.UI;
using TCSapp.Models;
using YahooFinanceApi;


namespace TCSapp.Controllers
{
   
    // GET api/<controller>
    [Produces("application/json")]
        public class ApiStockDataController : System.Web.Mvc.Controller
    {
        private TCSdbEntities db = new TCSdbEntities();
        [Route("~/api/ApiStockData/{ticker}/{start}/{end}/{period}")]
            [HttpGet]
            public async Task<string> GetStockData(string ticker, string start,
                string end, string period)
            {
                var p = Period.Daily;
            int sid = Convert.ToInt32(ticker);
            ticker = db.StockTbls.FirstOrDefault(a => a.StockID == sid).Ticker;
                if (period.ToLower() == "weekly") p = Period.Weekly;
          

            var startDate = DateTime.Parse(start);
                var endDate = DateTime.Parse(end);

            var hist = await Yahoo.GetHistoricalAsync(ticker, startDate, endDate, p);

                List<StockPriceModel> models = new List<StockPriceModel>();
                foreach (var r in hist)
                {
                    models.Add(new StockPriceModel
                    {
                        Ticker = ticker,
                        Date = r.DateTime.ToString("yyyy-MM-dd"),
                        Open = r.Open,
                        High = r.High,
                        Low = r.Low,
                        Close = r.Close,
                        AdjustedClose = r.AdjustedClose,
                        Volume = r.Volume
                    });
                }

            

            var serializer = new JavaScriptSerializer();
            var Json = serializer.Serialize(models);
            ViewBag.MyList = 222;
            var script = "var stockL = " + Json + ";";
           // ScriptManager.RegisterClientScriptBlock(null, GetType(), "none", "<script>" + script + ";</script>", false);
            return serializer.Serialize( models);
            }
        }
    }