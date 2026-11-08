using System;
using System.Collections.Generic;

namespace CarRental.Models;

public partial class Menu
{
    public int MenuId { get; set; }

    public string? Title { get; set; }

    public string? Alias { get; set; }

    public string? Description { get; set; }

    public int? Levels { get; set; }

    public int? ParentId { get; set; }

    public int? Position { get; set; }

    public bool? IsActive { get; set; }

    public string? Icon { get; set; } = "bi-link-45deg";

    public string Subsystem { get; set; } = "Customer"; // Customer | CarOwner | Admin

    public string? Url { get; set; }
}
