using _036_MoviesMvcWissen.Models.LogDemo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _036_MoviesMvcWissen.Controllers
{
    public class LogDemoController : Controller
    {
        private ILogger _logger;
        public LogDemoController(ILogger logger)
        {
            _logger = logger;
        }
        // GET: LogDemo
        public ActionResult Index()
        {
            //_logger = new DatabaseLogger(); dependincy inj yapılmaması gereken
            _logger.Log("Home Controller -> Index Action executed.");
            return Content("Home Controller->Index Action executed.");
        }
    }
}