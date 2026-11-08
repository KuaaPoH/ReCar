using CarRental.Models;
using CarRental.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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
            var carType = (from c in _context.CarTypes 
                       select new SelectListItem ()
                       {
                           Text = c.CarTypeName,
                           Value = c.TypeId.ToString(),
                       }).ToList();
            carType.Insert(0, new SelectListItem()
            {
                Text = "--select--",
                Value = "0"
            });
            ViewBag.carType = carType;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Car c, string filePaths)
        {
            if (ModelState.IsValid)
            {
                c.Alias = CarRental.Utilities.Function.TitleSlugGenerationAlias(c.CarName);
                _context.Cars.Add(c);
                _context.SaveChanges();

                // Xử lý danh sách filePaths (chuỗi đường dẫn, cách nhau bởi dấu phẩy)
                if (!string.IsNullOrEmpty(filePaths))
                {
                    var filePathList = filePaths.Split(',', StringSplitOptions.RemoveEmptyEntries);

                    foreach (var filePath in filePathList)
                    {
                        // Tạo bản ghi mới trong bảng CarImage
                        var carImage = new CarImage
                        {
                            CarId = c.CarId, // Liên kết với xe vừa tạo
                            Image1 = filePath.Trim(), // Đường dẫn ảnh
                            
                        };

                        _context.CarImages.Add(carImage);
                    }

                    // Lưu tất cả ảnh vào cơ sở dữ liệu
                    _context.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return View(c);
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var car = _context.Cars.Find(id);

            if (car == null)
            {
                return NotFound();
            }
            var carType = (from c in _context.CarTypes
                           select new SelectListItem()
                           {
                               Text = c.CarTypeName,
                               Value = c.TypeId.ToString(),
                           }).ToList();
            carType.Insert(0, new SelectListItem()
            {
                Text = "--select--",
                Value = string.Empty,
            });
            ViewBag.carType = carType;
            return View(car);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Car c)
        {
            if (ModelState.IsValid)
            {
                c.Alias = CarRental.Utilities.Function.TitleSlugGenerationAlias(c.CarName);
               
                _context.Cars.Update(c);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(c);
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var car = _context.Cars.Find(id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }
        [HttpPost]
        public IActionResult Delete (int id)
        {
            var deleCar = _context.Cars.Find(id);
            if (deleCar == null) {
                return NotFound();
            }
            _context.Cars.Remove(deleCar);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
