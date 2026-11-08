using CarRental.Models;
using CarRental.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CustomerController : Controller
    {
        private readonly DbRenalCarContext _context;
        public CustomerController(DbRenalCarContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return RedirectToAction("Index", "UserManagement", new { area = "Admin" });
        }
        [HttpGet]
        public IActionResult Details (int? id)
        {
            return RedirectToAction("Details", "UserManagement", new { area = "Admin", id = id });
        }
    }
}
