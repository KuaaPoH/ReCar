using System;
using System.Collections.Generic;

namespace CarRental.Models;

public partial class CarAmenity
{
    public int CarId { get; set; }

    public int AmenityId { get; set; }

    public virtual Car? Car { get; set; }

    public virtual Amenity? Amenity { get; set; }
}
