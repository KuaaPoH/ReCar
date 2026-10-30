using CarRental.Controllers;
using CarRental.Models;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.ViewComponents
{
    public class BlogVIewComponent : ViewComponent
    {
        private readonly DbRenalCarContext _context;
        public BlogVIewComponent(DbRenalCarContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var items = _context.Blogs
                .Where(i => i.IsActive == true)
                .OrderByDescending(i => i.CreatedDate)
                .ToList();

            foreach (var curated in BlogController.GetCuratedBlogs())
            {
                if (!items.Any(b => b.BlogId == curated.BlogId || b.Alias == curated.Alias))
                {
                    items.Add(curated);
                }
            }

            return await Task.FromResult<IViewComponentResult>(View(items));
        }
    }
}
