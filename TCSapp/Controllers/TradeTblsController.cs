using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TCSapp.Models;

namespace TCSapp.Controllers
{
    public class TradeTblsController : Controller
    {
        private TCSdbEntities db = new TCSdbEntities();

        // GET: TradeTbls
        public ActionResult Index(string sortOrder)
        {

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.MarketSortParm = sortOrder == "Market" ? "market_desc" : "Market";
            ViewBag.TypeSortParm = sortOrder == "TradeType" ? "tradeType_desc" : "TradeType";
            ViewBag.AmountSortParm = sortOrder == "TradeAmount" ? "tradeAmount_desc" : "TradeAmount";
            ViewBag.ProfitSortParm = sortOrder == "Profit" ? "profit_desc" : "Profit";
            var trade = from s in db.TradeTbls
                           select s;
            switch (sortOrder)
            {
                case "name_desc":
                    trade = trade.OrderByDescending(s => s.TradeMarket);
                    break;
                case "Name":
                    trade = trade.OrderBy(s => s.TradeMarket);
                    break;
                case "tradeType_desc":
                    trade = trade.OrderByDescending(s => s.TradeType);
                    break;
                case "TradeType":
                    trade = trade.OrderBy(s => s.TradeType);
                    break;
                case "tradeAmount_desc":
                    trade = trade.OrderByDescending(s => s.TradeAmount);
                    break;
                case "TradeAmount":
                    trade = trade.OrderBy(s => s.TradeAmount);
                    break;
             
                case "Date":
                    trade = trade.OrderBy(s => s.TradeDate);
                    break;
                case "date_desc":
                    trade = trade.OrderByDescending(s => s.TradeDate);
                    break;
                case "Market":
                    trade = trade.OrderBy(s => s.TradeMarket);
                    break;
                case "market_desc":
                    trade = trade.OrderByDescending(s => s.TradeMarket);
                    break;
                case "Profit":
                    trade = trade.OrderBy(s => s.TradeProfit);
                    break;
                case "profit_desc":
                    trade = trade.OrderByDescending(s => s.TradeProfit);
                    break;
                default:
                    trade = trade.OrderByDescending(s => s.TradeDate);
                    break;
            }
            return View(trade.ToList());
        }

        public ActionResult TradePortfolio()
        {
            return View();
        }

        // GET: TradeTbls/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TradeTbl tradeTbl = db.TradeTbls.Find(id);
            if (tradeTbl == null)
            {
                return HttpNotFound();
            }
            return View(tradeTbl);
        }
        //Get Start Day of week
        public  DateTime GetStartDayofWeek(DateTime tradedate)
            {

            string day =Convert.ToString(tradedate.DayOfWeek);
            DateTime startDate, EndDate;
            if(day=="Monday")
            {
                startDate = tradedate.Date;
                EndDate = tradedate.Date.AddDays(+4);
            }
            else if (day == "Tuesday")
            {
                startDate = tradedate.Date.AddDays(-1);
                EndDate = tradedate.Date.AddDays(+3);
            }
            else if (day == "Wednesday")
            {
                startDate = tradedate.Date.AddDays(-2);
                EndDate = tradedate.Date.AddDays(+2);
            }
            else if (day == "Thursday")
            {
                startDate = tradedate.Date.AddDays(-3);
                EndDate = tradedate.Date.AddDays(+1);
            }
            else if (day == "Friday")
            {
                startDate = tradedate.Date.AddDays(-4);
                EndDate = tradedate.Date;
            }
            else
            {
                startDate = tradedate.Date;
                EndDate = tradedate.Date;
            }
            ViewBag.startDate = startDate;
            ViewBag.endDate = EndDate;

            return tradedate;
            }
        //Get week Trade
        public class weekTradeDate
        {
           public DateTime initdate { get; set; }
            public DateTime finaldate { get; set; }
            public int? TradeProfit { get; set; }
        }

        private IEnumerable<SelectListItem> GetAllStocks()
        {
            IEnumerable<SelectListItem> list = db.StockTbls.Select(s => new SelectListItem
            {
                Selected = false,
                Text = s.Ticker,
                Value = s.Ticker
            }).Distinct();

            return list;
        }

        // GET: TradeTbls/Create
        public ActionResult Create()
        {
            //var stocklist = db.StockTbls.Select(a => a.Ticker).Distinct().ToList();
            //List<SelectListItem> newlist = new List<SelectListItem>();
            //if (stocklist != null)
            //{
            //    foreach(var i in stocklist)
            //    {
            //        newlist.Add(new SelectListItem
            //        {
            //            Text = i.ToString(),
            //            Value = i.ToString(),
            //            Selected = false
            //        });

            //    }

            //}
            try
            {


                ViewBag.stocklist = GetAllStocks();



                string datenow = DateTime.Now.ToString("dd/MM/yyyy");
                var order = db.TradeTbls.Where(a => a.TradeDate == datenow).ToList();
                // (dt.Day + ((int)dt.DayOfWeek)) / 7 + 1;
                GetStartDayofWeek(DateTime.Now);
                DateTime startDate = Convert.ToDateTime(ViewBag.startDate);
                DateTime endDate = Convert.ToDateTime(ViewBag.endDate);
                var totalOrder = db.TradeTbls.ToList().Select(o => new weekTradeDate
                {
                    initdate = DateTime.ParseExact(o.TradeDate, "dd/MM/yyyy", null),
                    TradeProfit = Convert.ToInt32(o.TradeProfit)
                });
                var weekorder = totalOrder.Where(a => a.initdate >= startDate && a.initdate <= endDate).ToList();

                var OrderNum = order.Count();
                int SuccessOrder = 0;
                int todayProfit = 0;
                foreach (var n in order)
                {
                    if (n.TradeProfit != null)
                    {
                        if (Convert.ToInt32(n.TradeProfit) > 0)
                        {
                            SuccessOrder += 1;
                        }
                        else
                        {
                            SuccessOrder -= 1;
                        }

                        todayProfit += Convert.ToInt32(n.TradeProfit);
                    }
                }
                int SuccessWeek = 0;
                int weekProfit = 0;
                foreach (var n in weekorder)
                {
                    if (n.TradeProfit != null)
                    {
                        if (Convert.ToInt32(n.TradeProfit) > 0)
                        {
                            SuccessWeek += 1;
                        }
                        else
                        {
                            SuccessWeek -= 1;
                        }

                        weekProfit += Convert.ToInt32(n.TradeProfit);
                    }
                }

                ViewBag.OrderNumber = OrderNum;
                ViewBag.SuccessOrder = SuccessOrder;
                ViewBag.todayProfit = todayProfit;
                ViewBag.weekProfit = weekProfit;

                if (SuccessOrder < -1)
                {
                    ViewBag.Message = String.Format("Maximum LOSS limit for today has reached !");

                }
            }
            catch
            {
                ViewBag.Message= String.Format("Connection Issue has occured !");
            }

            return View();
        }

        // POST: TradeTbls/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TradeID,TradeName,TradeDescription,TradeDate,TradeQuantity,TradePrice,TradeValue,TradeMarket,TradeType,TradeDeposit,TradeAmount,TradeSL,TradeTP,TradeLoss,TradeProfit,TradeZone,TradeLimit,UserID")] TradeTbl tradeTbl)
        {
            if (ModelState.IsValid)
            {
                db.TradeTbls.Add(tradeTbl);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tradeTbl);
        }

        // GET: TradeTbls/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TradeTbl tradeTbl = db.TradeTbls.Find(id);
            if (tradeTbl == null)
            {
                return HttpNotFound();
            }
            return View(tradeTbl);
        }

        // POST: TradeTbls/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TradeID,TradeAmount,TradeProfit,TradeDate,TradeMarket,TradeType")] TradeTbl tradeTbl)
        {
            if (tradeTbl.TradeProfit == null)
            {
                TempData["ErrorMessage"] = "Please Enter Profit or Loss Amount";
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var abb = db.TradeTbls.FirstOrDefault(a => a.TradeID == tradeTbl.TradeID);
                    abb.TradeProfit = tradeTbl.TradeProfit;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
           
            return View(tradeTbl);
        }

        // GET: TradeTbls/Delete/5
        public ActionResult Delete(int? id)
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //TradeTbl tradeTbl = db.TradeTbls.Find(id);
            //if (tradeTbl == null)
            //{
            //    return HttpNotFound();
            //}
            //return View(tradeTbl);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeleteConfirmed(id.Value);
            return RedirectToAction("Index");
        }

        // POST: TradeTbls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TradeTbl tradeTbl = db.TradeTbls.Find(id);
            db.TradeTbls.Remove(tradeTbl);
            db.SaveChanges();
            TempData["ErrorMessage"] = "Record is deleted successfully";
            return null;
            //return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
