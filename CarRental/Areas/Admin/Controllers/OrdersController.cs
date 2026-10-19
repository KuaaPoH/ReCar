using CarRental.Models;
using CarRental.Utilities;
using CarRental.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace CarRental.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly DbRenalCarContext _context;

        public OrdersController(DbRenalCarContext context)
        {
            _context = context;
        }

        private bool CheckAdmin()
        {
            return Function.IsLogin(HttpContext.Session) && Function.IsAdmin(HttpContext.Session);
        }

        public IActionResult Index(int? filterStatusId, string? q)
        {
            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            var query = _context.CarRentalOrders
                .Include(o => o.Customer)
                .Include(o => o.Status)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Car)
                .OrderByDescending(o => o.OrderId)
                .AsQueryable();

            if (filterStatusId.HasValue && filterStatusId.Value > 0)
            {
                query = query.Where(o => o.StatusId == filterStatusId.Value);
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                string searchKey = q.Trim().ToLower();
                query = query.Where(o => o.OrderId.ToString().Contains(searchKey)
                                      || (o.Customer != null && o.Customer.Name != null && o.Customer.Name.ToLower().Contains(searchKey))
                                      || (o.Customer != null && o.Customer.PhoneNumber != null && o.Customer.PhoneNumber.Contains(searchKey)));
            }

            ViewBag.FilterStatusId = filterStatusId;
            ViewBag.FilterQ = q;
            ViewBag.OrderStatuses = _context.OrderStatuses.ToList();

            return View(query.ToList());
        }

        public IActionResult Edit(int? id)
        {
            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            if (id == null || id == 0)
            {
                return NotFound();
            }

            var order = _context.CarRentalOrders
                .Include(o => o.Customer)
                .Include(o => o.Status)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Car)
                .FirstOrDefault(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            ViewBag.OrderStatuses = new SelectList(_context.OrderStatuses.ToList(), "StatusId", "StatusDescription", order.StatusId);
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CarRentalOrder o)
        {
            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Login", new { area = "Admin" });
            }

            var existingOrder = _context.CarRentalOrders
                .Include(order => order.Customer)
                .Include(c => c.OrderDetails)
                .FirstOrDefault(order => order.OrderId == o.OrderId);

            if (existingOrder == null)
            {
                return NotFound();
            }

            existingOrder.StatusId = o.StatusId;
            existingOrder.Notes = o.Notes;

            _context.CarRentalOrders.Update(existingOrder);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
