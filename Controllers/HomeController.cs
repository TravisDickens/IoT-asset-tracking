using IOTAssetTracking.Data;
using IOTAssetTracking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace IOTAssetTracking.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDBContext _context;

        public HomeController(ILogger<HomeController> logger, AppDBContext context )
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                ViewBag.GroupCount = await _context.groups.CountAsync();
                ViewBag.FirmwareCount = await _context.firmwares.CountAsync();
                ViewBag.DeviceCount = await _context.devices.CountAsync();
            }
            catch
            {
                ViewBag.GroupCount = ViewBag.FirmwareCount = ViewBag.DeviceCount = null;
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
