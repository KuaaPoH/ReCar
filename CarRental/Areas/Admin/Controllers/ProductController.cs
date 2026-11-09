using CarRental.Models;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly DbRenalCarContext _context;

        public ProductController(DbRenalCarContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "Cars", new { area = "Admin" });
        }

        public IActionResult Create()
        {
            return RedirectToAction("Index", "Cars", new { area = "Admin" });
        }

        public IActionResult Edit(int? id)
        {
            return RedirectToAction("Index", "Cars", new { area = "Admin" });
        }

        public IActionResult Delete(int? id)
        {
            return RedirectToAction("Index", "Cars", new { area = "Admin" });
        }
    }
}
