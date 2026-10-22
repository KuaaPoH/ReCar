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
    public class CarTypesController : Controller
    {
        private readonly DbRenalCarContext _context;

        public CarTypesController(DbRenalCarContext context)
        {
            _context = context;
        }

        // GET: Admin/CarTypes
        public async Task<IActionResult> Index()
        {
            if (!Function.IsLogin())
            {
                return RedirectToAction("Index", "Login");
            }

            var carTypes = await _context.CarTypes
                .Include(t => t.Cars)
                .OrderBy(t => t.TypeId)
                .ToListAsync();

            return View(carTypes);
        }

        // GET: Admin/CarTypes/Create
        public IActionResult Create()
        {
            if (!Function.IsLogin())
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }

        // POST: Admin/CarTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarType carType)
        {
            if (!Function.IsLogin())
            {
                return RedirectToAction("Index", "Login");
            }

            if (ModelState.IsValid)
            {
                _context.Add(carType);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã tạo mới kiểu xe '{carType.CarTypeName}' thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(carType);
        }

        // GET: Admin/CarTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!Function.IsLogin())
            {
                return RedirectToAction("Index", "Login");
            }

            if (id == null) return NotFound();

            var carType = await _context.CarTypes.FindAsync(id);
            if (carType == null) return NotFound();

            return View(carType);
        }

        // POST: Admin/CarTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CarType carType)
        {
            if (!Function.IsLogin())
            {
                return RedirectToAction("Index", "Login");
            }

            if (id != carType.TypeId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carType);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Đã cập nhật kiểu xe '{carType.CarTypeName}' thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.CarTypes.Any(e => e.TypeId == carType.TypeId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(carType);
        }

        // POST: Admin/CarTypes/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!Function.IsLogin())
            {
                return RedirectToAction("Index", "Login");
            }

            var carType = await _context.CarTypes.Include(t => t.Cars).FirstOrDefaultAsync(t => t.TypeId == id);
            if (carType == null) return NotFound();

            if (carType.Cars != null && carType.Cars.Any())
            {
                TempData["Error"] = $"Không thể xóa kiểu xe '{carType.CarTypeName}' vì đang có {carType.Cars.Count} xe thuộc loại này!";
                return RedirectToAction(nameof(Index));
            }

            _context.CarTypes.Remove(carType);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Đã xóa kiểu xe '{carType.CarTypeName}' thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
