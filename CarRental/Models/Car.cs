using System;
using System.Collections.Generic;

namespace CarRental.Models;

public partial class Car
{
    public int CarId { get; set; }

    public string? CarName { get; set; }

    public int? Seat { get; set; }

    public string? LicensePlate { get; set; }

    public decimal? Price { get; set; }

    public string? Color { get; set; }

    public int? Model { get; set; }

    public double? Rate { get; set; }

    public string? CarBrand { get; set; }

    public string? Image { get; set; }

    public decimal? SalePrice { get; set; }

    public string? Alias { get; set; }

    public bool IsSale { get; set; }

    public bool IsActive { get; set; }

    public string? Details { get; set; }

    public string? Descriptions { get; set; }

    public int? TypeId { get; set; }

    public int? OwnerId { get; set; }

    public string? Transmission { get; set; }

    public string? Fuel { get; set; }

    public string? Location { get; set; }

    public int? Trips { get; set; }

    public string? Badge { get; set; }

    public string? CarStatus { get; set; } = "Sẵn sàng";

    public string? RegistrationImage { get; set; }

    public string? InspectionImage { get; set; }

    public string? InsuranceImage { get; set; }

    public virtual ICollection<CarReview> CarReviews { get; set; } = new List<CarReview>();

    public virtual ICollection<CarAmenity> CarAmenities { get; set; } = new List<CarAmenity>();

    public virtual CarType? Type { get; set; }

    public virtual Account? Owner { get; set; }
}
