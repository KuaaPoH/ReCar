using System;

namespace CarRental.ViewModels
{
    public class TopOwnerVM
    {
        public int AccountId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int CarCount { get; set; }
        public int Trips { get; set; }
        public decimal Revenue { get; set; }
        public double Rating { get; set; } = 4.9;
    }

    public class SettlementVM
    {
        public string SettlementCode { get; set; } = string.Empty;
        public int OwnerId { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public string OwnerPhone { get; set; } = string.Empty;
        public string Bank { get; set; } = string.Empty;
        public string Period { get; set; } = "Tháng 10/2026";
        public int Trips { get; set; }
        public decimal TotalGmv { get; set; }
        public decimal Fee { get; set; }
        public decimal NetPayout { get; set; }
        public bool IsSettled { get; set; }
    }
}
