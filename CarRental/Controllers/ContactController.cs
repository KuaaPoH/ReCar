using CarRental.Models;
using CarRental.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace CarRental.Controllers
{
    public class ContactController : Controller
    {
        private readonly DbRenalCarContext _context;
        public ContactController (DbRenalCarContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create( string name, string email, string phone, string message )
        {
            try
            {
                // Tạo đối tượng review mới
                Contact con = new Contact
                {
                    Name = name,
                    Phone = phone,
                    Email = email,
                    CreatedDate = DateTime.Now,
                    Message = message,
                    
                };

                // Thêm vào DbSet và lưu vào cơ sở dữ liệu
                _context.Contacts.Add(con);
                await _context.SaveChangesAsync(); // Sử dụng await để đảm bảo dữ liệu được lưu

                return Json(new { status = true }); // Trả về trạng thái thành công
            }
            catch (Exception ex)
            {
                // Ghi log lỗi (nếu cần) và trả về trạng thái thất bại
                return Json(new { status = false, message = ex.Message });
            }
        }
    }
}
