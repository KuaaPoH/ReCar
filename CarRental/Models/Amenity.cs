using System;
using System.Collections.Generic;

namespace CarRental.Models;

public partial class Amenity
{
    public int AmenityId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Icon { get; set; }

    public string? IconColor { get; set; }

    public int DisplayOrder { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    public virtual ICollection<CarAmenity> CarAmenities { get; set; } = new List<CarAmenity>();
}
