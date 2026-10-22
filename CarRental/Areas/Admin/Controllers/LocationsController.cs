using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRental.Models;
using CarRental.Utilities;

namespace CarRental.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LocationsController : Controller
    {
        private readonly DbRenalCarContext _context;

        public LocationsController(DbRenalCarContext context)
        {
            _context = context;
        }

        // GET: Admin/Locations
        public async Task<IActionResult> Index()
        {
            if (!Function.IsLogin())
            {
                return RedirectToAction("Index", "Login");
            }

            var locations = await _context.Locations
                .OrderBy(l => l.DisplayOrder)
                .ThenBy(l => l.LocationId)
                .ToListAsync();

            return View(locations);
        }

        // GET: Admin/Locations/Create
        public IActionResult Create()
        {
            if (!Function.IsLogin())
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }

        // POST: Admin/Locations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Location location)
        {
            if (!Function.IsLogin())
            {
                return RedirectToAction("Index", "Login");
            }

            if (ModelState.IsValid)
            {
                _context.Locations.Add(location);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã thêm địa điểm '{location.CityName}' vào CSDL thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(location);
        }

        // GET: Admin/Locations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!Function.IsLogin())
            {
                return RedirectToAction("Index", "Login");
            }

            if (id == null) return NotFound();

            var location = await _context.Locations.FindAsync(id);
            if (location == null) return NotFound();

            return View(location);
        }

        // POST: Admin/Locations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Location location)
        {
            if (!Function.IsLogin())
            {
                return RedirectToAction("Index", "Login");
            }

            if (id != location.LocationId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(location);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Đã cập nhật địa điểm '{location.CityName}' thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Locations.Any(e => e.LocationId == location.LocationId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(location);
        }

        // POST: Admin/Locations/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!Function.IsLogin())
            {
                return RedirectToAction("Index", "Login");
            }

            var location = await _context.Locations.FindAsync(id);
            if (location == null) return NotFound();

            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Đã xóa địa điểm '{location.CityName}' khỏi hệ thống!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Locations/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            if (!Function.IsLogin())
            {
                return RedirectToAction("Index", "Login");
            }

            var location = await _context.Locations.FindAsync(id);
            if (location != null)
            {
                location.IsActive = !location.IsActive;
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã {(location.IsActive ? "kích hoạt" : "tạm ẩn")} địa điểm '{location.CityName}'!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
