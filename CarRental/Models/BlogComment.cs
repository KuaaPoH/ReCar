using System;
using System.Collections.Generic;

namespace CarRental.Models;

public partial class BlogComment
{
    public int CommentId { get; set; }

    public string? Name { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? Detail { get; set; }

    public int? BlogId { get; set; }

    public bool IsActive { get; set; }

    public string? Reply { get; set; }

    public DateTime? ReplyDate { get; set; }

    public virtual Blog? Blog { get; set; }
}
