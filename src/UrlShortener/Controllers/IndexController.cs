using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Services;

namespace UrlShortener.Controllers
{
    [Route("/")]
    public class IndexController : Controller
    {
        readonly IForwardDb _forwardDb;

        public IndexController(IForwardDb forwardDb)
        {
            _forwardDb = forwardDb;
        }

        // GET: /
        public IActionResult Index()
        {
            return View();
        }

        // GET: /_admin
        [HttpGet("_admin")]
        public async Task<IActionResult> Admin()
        {
            return View("Admin", await _forwardDb.ListAllForwards());
        }

        // GET: /{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Forward(string id)
        {
            try
            {
                return Redirect(await _forwardDb.ProcessForward(id));
            }
            catch (KeyNotFoundException)
            {
                return View("NotFound", id);
            }
        }
    }
}
