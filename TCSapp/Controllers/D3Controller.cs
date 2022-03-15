using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TCSapp.Controllers
{
    public class D3Controller : Controller
    {
        // GET: D3
        public ActionResult Index()
        {
            return View();
        }

        // GET: D3/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: D3/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: D3/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: D3/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: D3/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: D3/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: D3/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
