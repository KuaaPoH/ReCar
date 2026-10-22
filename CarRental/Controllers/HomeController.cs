using CarRental.Models;
using CarRental.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CarRental.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DbRenalCarContext _context;

        public HomeController(DbRenalCarContext context, ILogger<HomeController> logger)
        {
            _logger = logger;
            _context = context;
        }
        
        public IActionResult Index()
        {
            
            ViewBag.productFeatured = _context.Cars.Where(i => i.IsActive == true).OrderBy(i => i.CarId).ToList();
            ViewBag.productDealofDay = _context.Cars.Where(i => i.IsSale == true && i.IsActive == true).OrderByDescending(i=>i.CarId).ToList();
            ViewBag.Blog = _context.Blogs.Where(i => i.IsActive == true).ToList();
            ViewBag.CarTypes = _context.CarTypes.ToList();
            ViewBag.Locations = _context.Locations.Where(l => l.IsActive).OrderBy(l => l.DisplayOrder).ToList();
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
