using CarRental.Controllers;
using CarRental.Models;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.ViewComponents
{
    public class NewsViewComponent : ViewComponent
    {
        private readonly DbRenalCarContext _context;

        public NewsViewComponent(DbRenalCarContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var items = _context.News
                .Where(m => m.IsActive == true)
                .OrderByDescending(m => m.CreatedDate)
                .ToList();

            foreach (var curated in NewsController.GetCuratedNews())
            {
                if (!items.Any(n => n.NewsId == curated.NewsId || n.Alias == curated.Alias))
                {
                    items.Add(curated);
                }
            }

            return await Task.FromResult<IViewComponentResult>(View(items));
        }
    }
}
