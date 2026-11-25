using CarRental.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRental.ViewComponents
{
    public class ProductViewComponent : ViewComponent
    {
        private readonly DbRenalCarContext _context;

        public ProductViewComponent(DbRenalCarContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke(int? typeId = null, List<Car>? items = null)
        {
            if (items != null)
            {
                return View(items);
            }

            var nowVn = CarRental.Utilities.Function.NowVn;
            var query = _context.Cars.Where(c => c.IsActive && c.CarStatus != "Chờ duyệt" && c.CarStatus != "Từ chối" && c.CarStatus != "Tạm ngừng" && c.CarStatus != "Ngưng hoạt động"
                && !_context.CarCalendarBlocks.Any(b => b.CarId == c.CarId && b.StartTime <= nowVn && b.EndTime >= nowVn)).AsQueryable();

            if (typeId.HasValue && typeId.Value > 0)
            {
                query = query.Where(c => c.TypeId == typeId.Value);
            }

            return View(query.ToList());
        }
    }
}
