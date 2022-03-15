using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TCSapp.Models;

namespace TCSapp.Controllers
{
    public class StockTblsController : Controller
    {
        private TCSdbEntities db = new TCSdbEntities();

        // GET: StockTbls
        public ActionResult Index()
        {
            return View(db.StockTbls.ToList());
        }

        [HttpPost]
        public FileResult ExportToExcel()
        {
            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[8] { new DataColumn("Ticker"),
                                                     new DataColumn("Description"),
                                                     new DataColumn("Date"),
                                                     new DataColumn("OpeningPrice"),
                                                     new DataColumn("High"),
                                                     new DataColumn("Low"),
                                                     new DataColumn("ClosingPrice"),
                                                     new DataColumn("Volume"),
                                                    });

            var sCertificate = from stockList in db.StockTbls select stockList;

            foreach (var s in sCertificate)
            {
                dt.Rows.Add(s.Ticker, s.Description, s.StockDate, s.OpeningPrice,
                    s.High, s.Low, s.ClosingPrice, s.Volume);
            }

            using (XLWorkbook wb = new XLWorkbook()) //Install ClosedXml from Nuget for XLWorkbook  
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream()) //using System.IO;  
                {
                    wb.SaveAs(stream);
                    string filename = "StockData_" + System.DateTime.Now+".xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                }
            }
        }

        [HttpPost]
        public ActionResult ImportFromExcel(HttpPostedFileBase postedFile)
        {

            if(postedFile==null)
            {
               
                TempData["ErrorMessage"]= "Please upload a valid file !";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                if (postedFile != null && postedFile.ContentLength > (1024 * 1024 * 50))  // 50MB limit  
                {
                    ModelState.AddModelError("postedFile", "Your file is to large. Maximum size allowed is 50MB !");
                }

                else
                {
                    string filePath = string.Empty;
                    string path = Server.MapPath("~/Uploads/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    filePath = path + Path.GetFileName(postedFile.FileName);
                    string extension = Path.GetExtension(postedFile.FileName);
                    postedFile.SaveAs(filePath);

                    string conString = string.Empty;
                    switch (extension)
                    {
                        case ".xls": //For Excel 97-03.  
                            conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                            break;
                        case ".xlsx": //For Excel 07 and above.  
                            conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                            break;
                    }

                    try
                    {
                        DataTable dt = new DataTable();
                        conString = string.Format(conString, filePath);

                        using (OleDbConnection connExcel = new OleDbConnection(conString))
                        {
                            using (OleDbCommand cmdExcel = new OleDbCommand())
                            {
                                using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                                {

                                   

                                    cmdExcel.Connection = connExcel;
                                    
                                    //Get the name of First Sheet.  
                                    connExcel.Open();
                                    DataTable dtExcelSchema;
                                    dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                    string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                                    connExcel.Close();

                                    //Read Data from First Sheet.  
                                    connExcel.Open();
                                    cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                                    odaExcel.SelectCommand = cmdExcel;
                                    odaExcel.Fill(dt);
                                    connExcel.Close();
                                }
                            }
                        }

                        conString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
                        using (SqlConnection con = new SqlConnection(conString))
                        {
                            using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                            {
                                db.StockTbls.RemoveRange(db.StockTbls.ToList());
                                db.SaveChanges();
                                //Set the database table name.  
                                sqlBulkCopy.DestinationTableName = "StockTbl";
                                con.Open();


                                // Add your column mappings here
                                sqlBulkCopy.ColumnMappings.Add("Ticker", "Ticker");
                                sqlBulkCopy.ColumnMappings.Add("Description", "Description");
                                sqlBulkCopy.ColumnMappings.Add("Date", "StockDate");
                                sqlBulkCopy.ColumnMappings.Add("OpeningPrice", "OpeningPrice");
                                sqlBulkCopy.ColumnMappings.Add("High", "High");
                                sqlBulkCopy.ColumnMappings.Add("Low", "Low");
                                sqlBulkCopy.ColumnMappings.Add("ClosingPrice", "ClosingPrice");
                                sqlBulkCopy.ColumnMappings.Add("Volume", "Volume");


                                sqlBulkCopy.WriteToServer(dt);
                                con.Close();
                                //return Json("File uploaded successfully");
                                TempData["SuccessMessage"] = "File imported successfully!";
                                return RedirectToAction("Index");
                            }
                        }
                    }

                    //catch (Exception ex)  
                    //{  
                    //    throw ex;  
                    //}  
                    catch (Exception e)
                    {
                        TempData["ErrorMessage"] = "error" + e.Message;
                        return RedirectToAction("Index");
                       
                    }
                    //return RedirectToAction("Index");  
                }
            }
            //return View(postedFile);  
            //return Json("no files were selected !");
            TempData["ErrorMessage"] = "no files were selected !";
            return RedirectToAction("Index");
        }


        // GET: StockTbls/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockTbl stockTbl = db.StockTbls.Find(id);
            if (stockTbl == null)
            {
                return HttpNotFound();
            }
            return View(stockTbl);
        }

        // GET: StockTbls/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StockTbls/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StockID,Ticker,StockDate,OpeningnPrice,High,Low,ClosingPrice,AdjustedClosing,Volume,Description,ProductTypeId,TradeId")] StockTbl stockTbl)
        {
            if (ModelState.IsValid)
            {
                db.StockTbls.Add(stockTbl);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(stockTbl);
        }

        // GET: StockTbls/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockTbl stockTbl = db.StockTbls.Find(id);
            if (stockTbl == null)
            {
                return HttpNotFound();
            }
            return View(stockTbl);
        }

        // POST: StockTbls/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StockID,Ticker,StockDate,OpeningnPrice,High,Low,ClosingPrice,AdjustedClosing,Volume,Description,ProductTypeId,TradeId")] StockTbl stockTbl)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stockTbl).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(stockTbl);
        }

        // GET: StockTbls/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockTbl stockTbl = db.StockTbls.Find(id);
            if (stockTbl == null)
            {
                return HttpNotFound();
            }
            return View(stockTbl);
        }

        // POST: StockTbls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StockTbl stockTbl = db.StockTbls.Find(id);
            db.StockTbls.Remove(stockTbl);
            db.SaveChanges();
            return RedirectToAction("Index");
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
