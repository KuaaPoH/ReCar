using System;
using System.Collections.Generic;

namespace CarRental.Models;

public partial class CarReview
{
    public int CarReviewId { get; set; }

    public string? Name { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? Detail { get; set; }

    public int? CarId { get; set; }

    public int? Star { get; set; }

    public bool? IsActive { get; set; }
    public int? OrderId { get; set; }

    public virtual Car? Car { get; set; }
}
