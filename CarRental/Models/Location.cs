using System;
using System.Collections.Generic;

namespace CarRental.Models;

public partial class Location
{
    public int LocationId { get; set; }

    public string CityName { get; set; } = null!;

    public string? Region { get; set; }

    public bool IsPopular { get; set; } = true;

    public int DisplayOrder { get; set; } = 0;

    public bool IsActive { get; set; } = true;
}
