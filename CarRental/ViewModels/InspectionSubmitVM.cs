using Microsoft.AspNetCore.Http;

namespace CarRental.ViewModels
{
    public class InspectionSubmitVM
    {
        public int id { get; set; }

        /// <summary>"handover" hoặc "return"</summary>
        public string? InspectionType { get; set; }

        public int? Odometer { get; set; }

        public int? FuelLevel { get; set; }

        public string? Notes { get; set; }

        public IFormFile? FrontImage { get; set; }

        public IFormFile? RearImage { get; set; }

        public IFormFile? LeftImage { get; set; }

        public IFormFile? RightImage { get; set; }
    }
}

