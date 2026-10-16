namespace CarRental.ViewModels
{
    public class CartItemsVM
    {
        public int CartId { get; set; }
        public string CarName { get; set; }
        public string Image { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal SalePrice { get; set; }
        public DateTime pickupDate { get; set; }
        public DateTime returnDate { get; set; }
        public string DriveMode { get; set; } = "Tự lái";
        public decimal DriverFee { get; set; } = 0;
        public decimal PriceTotal => (Price * Quantity) + DriverFee;
    }
}
