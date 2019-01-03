using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Models;
using UrlShortener.Services;

namespace UrlShortener.Controllers
{
    [Route("/")]
    [Authorize]
    public class IndexController : Controller
    {
        readonly IForwardDb _forwardDb;
        readonly IAuther _auther;

        public IndexController(IForwardDb forwardDb, IAuther auther)
        {
            _forwardDb = forwardDb;
            _auther = auther;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(string errorMessage = null, string successMessage = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return View(
                    "Index",
                    (await _forwardDb.ListAllForwards(), errorMessage, successMessage)
                );
            }
            return View("Login", false);
        }

        [HttpPost]
        [AllowAnonymous]
        [RequireParameter("key")]
        public IActionResult IndexLogin([FromForm]LoginBody login)
        {
            if (_auther.AttemptAuth(login.Key))
            {
                return RedirectToAction("Index");
            }
            return View("Login", true);
        }

        [HttpPost(Order = 1)]
        public async Task<IActionResult> IndexAdd([FromForm]ForwardItem forward)
        {
            try
            {
                await _forwardDb.AddForward(forward);
                return await Index(successMessage: "Forward added");
            }
            catch (ArgumentException)
            {
                return await Index("Forward data invalid");
            }
            catch (InvalidOperationException ex)
            {
                return await Index($"Failed to add forward, reason: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
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

        [HttpGet("{id}/edit")]
        public async Task<IActionResult> ForwardItem(string id, string errorMessage = null, string successMessage = null)
        {
            try
            {
                return View(
                    "ForwardItem",
                    (await _forwardDb.GetForward(id), errorMessage, successMessage)
                );
            }
            catch (KeyNotFoundException)
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost("{id}/edit")]
        public async Task<IActionResult> ForwardItemUpdate(string id, [FromForm]ForwardItemUpdate forward)
        {
            try
            {
                await _forwardDb.UpdateForward(id, forward);
                return await ForwardItem(id, successMessage: "Forward updated");
            }
            catch (ArgumentException)
            {
                return await ForwardItem(id, "Forward data invalid");
            }
            catch (KeyNotFoundException)
            {
                return RedirectToAction("Index");
            }
        }
    }
}
