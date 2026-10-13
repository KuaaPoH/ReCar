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

            var query = _context.Cars.Where(c => c.IsActive).AsQueryable();

            if (typeId.HasValue && typeId.Value > 0)
            {
                query = query.Where(c => c.TypeId == typeId.Value);
            }

            return View(query.ToList());
        }
    }
}
