using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRental.Models;

[Table("car_calendar_blocks")]
public class CarCalendarBlock
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int BlockId { get; set; }

    public int CarId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    [StringLength(30)]
    public string Kind { get; set; } = "personal"; // personal, maintenance, other

    [StringLength(500)]
    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [ForeignKey("CarId")]
    public virtual Car? Car { get; set; }
}

