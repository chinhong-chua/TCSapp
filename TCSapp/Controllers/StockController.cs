using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace TCSapp.Controllers
{
    public class StockController : System.Web.Mvc.Controller
    {

        public IActionResult StockChart()
        {
            return (IActionResult)View();
        }
    }
}
