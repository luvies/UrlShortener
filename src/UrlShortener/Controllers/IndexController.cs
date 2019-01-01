using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace UrlShortener.Controllers
{
    [Route("/")]
    public class IndexController : Controller
    {
        // GET: /
        public IActionResult Index()
        {
            return View();
        }

        // GET: /_admin
        [HttpGet("_admin")]
        public IActionResult Admin()
        {
            return View("Admin");
        }

        // GET: /{forward}
        [HttpGet("{forward}")]
        public string Forward(string forward)
        {
            return forward;
        }
    }
}
