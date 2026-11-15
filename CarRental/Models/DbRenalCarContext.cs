using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Models;

public partial class DbRenalCarContext : DbContext
{
    public DbRenalCarContext()
    {
    }

    public DbRenalCarContext(DbContextOptions<DbRenalCarContext> options)
        : base(options)
    {
    }

public virtual DbSet<Blog> Blogs { get; set; }

    public virtual DbSet<BlogComment> BlogComments { get; set; }

    public virtual DbSet<Car> Cars { get; set; }

    public virtual DbSet<ChatMessage> ChatMessages { get; set; }

    public virtual DbSet<CarImage> CarImages { get; set; }

    public virtual DbSet<CarRentalOrder> CarRentalOrders { get; set; }

    public virtual DbSet<CarReview> CarReviews { get; set; }

    public virtual DbSet<CarType> CarTypes { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<News> News { get; set; }

    public virtual DbSet<NewsComment> NewsComments { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<PartnerPayout> PartnerPayouts { get; set; }

    public virtual DbSet<WalletTransaction> WalletTransactions { get; set; }

    public virtual DbSet<CarOwnerRegistration> CarOwnerRegistrations { get; set; }

    public virtual DbSet<Bank> Banks { get; set; }

    public virtual DbSet<CarBrand> CarBrands { get; set; }

    public virtual DbSet<Amenity> Amenities { get; set; }

    public virtual DbSet<CarAmenity> CarAmenities { get; set; }

    public virtual DbSet<VehicleInspection> VehicleInspections { get; set; }

    public virtual DbSet<CarCalendarBlock> CarCalendarBlocks { get; set; }

    public virtual DbSet<SystemSetting> SystemSettings { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

modelBuilder.Entity<Blog>(entity =>
        {
            entity.ToTable("Blog");

            entity.Property(e => e.Alias).HasMaxLength(250);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(4000);
            entity.Property(e => e.Image).HasMaxLength(500);
            entity.Property(e => e.ModifiedBy).HasMaxLength(150);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.SeoDescription).HasMaxLength(500);
            entity.Property(e => e.SeoKeywords).HasMaxLength(250);
            entity.Property(e => e.SeoTitle).HasMaxLength(250);
            entity.Property(e => e.Title).HasMaxLength(250);
            entity.Property(e => e.Views).HasDefaultValue(0);
            entity.Property(e => e.IsFeatured).HasDefaultValue(false);
            entity.Property(e => e.Badge).HasMaxLength(50);

            entity.Property(e => e.CustomerId).HasColumnName("CustomerId");
            entity.HasOne(d => d.Customer).WithMany(p => p.Blogs)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_Blog_Customers");
        });

        modelBuilder.Entity<BlogComment>(entity =>
        {
            entity.HasKey(e => e.CommentId);

            entity.ToTable("BlogComment");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Detail).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Reply).HasColumnType("longtext");
            entity.Property(e => e.ReplyDate).HasColumnType("datetime");

            entity.HasOne(d => d.Blog).WithMany(p => p.BlogComments)
                .HasForeignKey(d => d.BlogId)
                .HasConstraintName("FK_BlogComment_Blog");
        });

        modelBuilder.Entity<Car>(entity =>
        {
            entity.Property(e => e.CarId).HasColumnName("CarID");
            entity.Property(e => e.Alias).HasMaxLength(50);
            entity.Property(e => e.CarBrand).HasMaxLength(50);
            entity.Property(e => e.CarName).HasMaxLength(100);
            entity.Property(e => e.Color).HasMaxLength(50);
            entity.Property(e => e.LicensePlate).HasMaxLength(20);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SalePrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TypeId).HasColumnName("TypeID");
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.CarStatus).HasMaxLength(50);

            entity.HasOne(d => d.Type).WithMany(p => p.Cars)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("FK_Cars_CarTypes");

            entity.HasOne(d => d.Owner).WithMany(p => p.Cars)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("FK_Cars_Customers");
        });

        modelBuilder.Entity<CarImage>(entity =>
        {
            entity.HasKey(e => e.ImageId);

            entity.Property(e => e.ImageId).HasColumnName("ImageID");
            entity.Property(e => e.CarId).HasColumnName("CarID");
        });

        modelBuilder.Entity<CarRentalOrder>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__CarRenta__C3905BAF612B80AC");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.Deposit).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Notes).HasMaxLength(50);
            entity.Property(e => e.Payment).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PlatformFeeRate).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.PlatformFee).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");

            entity.HasOne(d => d.Customer).WithMany(p => p.CarRentalOrders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CarRental__Custo__2D27B809");

            entity.HasOne(d => d.Status).WithMany(p => p.CarRentalOrders)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__CarRental__Statu__2E1BDC42");
        });

        modelBuilder.Entity<CarReview>(entity =>
        {
            entity.Property(e => e.CarReviewId).HasColumnName("CarReviewID");
            entity.Property(e => e.CarId).HasColumnName("CarID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Detail).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.OrderId).HasColumnName("OrderId");
            entity.Property(e => e.Reply).HasColumnType("longtext");
            entity.Property(e => e.ReplyDate).HasColumnType("datetime");

            entity.HasOne(d => d.Car).WithMany(p => p.CarReviews)
                .HasForeignKey(d => d.CarId)
                .HasConstraintName("FK_CarReviews_Cars");
        });

        modelBuilder.Entity<CarType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__CarTypes__2B2E84BDA5A9E5DA");

            entity.Property(e => e.TypeId).HasColumnName("TypeID");
            entity.Property(e => e.CarTypeName).HasMaxLength(100);
            entity.Property(e => e.Manufacturer).HasMaxLength(100);
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.ToTable("Contact");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.ModifiedBy).HasMaxLength(150);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Phone).HasMaxLength(50);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64B8D5A7BBA1");

            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.AccountNumber).HasMaxLength(50);
            entity.Property(e => e.Avartar).HasMaxLength(50);
            entity.Property(e => e.Bank).HasMaxLength(50);
            entity.Property(e => e.CompanyName).HasMaxLength(100);
            entity.Property(e => e.DateofBirth).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.Idcard)
                .HasMaxLength(20)
                .HasColumnName("IDCard");
            entity.Property(e => e.LicenseNumber).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Username).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.RoleId).HasDefaultValue(2);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastLogin).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Customers)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_Customers_Role");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.ToTable("Menu");

            entity.Property(e => e.MenuId).HasColumnName("MenuID");
            entity.Property(e => e.Alias).HasMaxLength(150);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Title).HasMaxLength(150);
            entity.Property(e => e.Icon).HasMaxLength(100);
            entity.Property(e => e.Subsystem).HasMaxLength(50).HasDefaultValue("Customer");
            entity.Property(e => e.Url).HasMaxLength(255);
        });

        modelBuilder.Entity<News>(entity =>
        {
            entity.Property(e => e.Alias).HasMaxLength(250);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(4000);
            entity.Property(e => e.Image).HasMaxLength(500);
            entity.Property(e => e.ModifiedBy).HasMaxLength(150);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.SeoDescription).HasMaxLength(500);
            entity.Property(e => e.SeoKeywords).HasMaxLength(250);
            entity.Property(e => e.SeoTitle).HasMaxLength(250);
            entity.Property(e => e.Title).HasMaxLength(250);
            entity.Property(e => e.Views).HasDefaultValue(0);
            entity.Property(e => e.IsFeatured).HasDefaultValue(false);
            entity.Property(e => e.Badge).HasMaxLength(50);
            entity.Property(e => e.Views).HasDefaultValue(0);
            entity.Property(e => e.IsFeatured).HasDefaultValue(false);
            entity.Property(e => e.Badge).HasMaxLength(50);
        });

        modelBuilder.Entity<NewsComment>(entity =>
        {
            entity.HasKey(e => e.CommentId);

            entity.ToTable("NewsComment");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Detail).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Reply).HasColumnType("longtext");
            entity.Property(e => e.ReplyDate).HasColumnType("datetime");

            entity.HasOne(d => d.News).WithMany(p => p.NewsComments)
                .HasForeignKey(d => d.NewsId)
                .HasConstraintName("FK_NewsComment_News");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__D3B9D30C2C8C4FE9");

            entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");
            entity.Property(e => e.CarId).HasColumnName("CarID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.PickupDate).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ReturnDate).HasColumnType("datetime");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderDeta__Order__33D4B598");

            entity.HasOne(d => d.Car).WithMany()
                .HasForeignKey(d => d.CarId)
                .HasConstraintName("FK_OrderDetails_Cars");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__OrderSta__C8EE20433338C4B8");

            entity.ToTable("OrderStatus");

            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.StatusDescription).HasMaxLength(100);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.RoleId).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(50);
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.HasKey(e => e.MessageId);
            entity.ToTable("ChatMessages");

            entity.Property(e => e.MessageId).HasColumnName("MessageId");
            entity.Property(e => e.CarId).HasColumnName("CarId");
            entity.Property(e => e.SenderId).HasColumnName("SenderId");
            entity.Property(e => e.SenderName).HasMaxLength(100);
            entity.Property(e => e.SenderRole).HasMaxLength(50);
            entity.Property(e => e.ReceiverId).HasColumnName("ReceiverId");
            entity.Property(e => e.ReceiverName).HasMaxLength(100);
            entity.Property(e => e.MessageText).HasColumnType("text");
            entity.Property(e => e.SentAt).HasColumnType("datetime");

            entity.HasOne(d => d.Car).WithMany()
                .HasForeignKey(d => d.CarId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(d => d.Sender).WithMany(p => p.ChatMessagesSent)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(d => d.Receiver).WithMany(p => p.ChatMessagesReceived)
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.LocationId);
            entity.ToTable("locations");

            entity.Property(e => e.LocationId).HasColumnName("LocationID");
            entity.Property(e => e.CityName).HasMaxLength(100);
            entity.Property(e => e.Region).HasMaxLength(50);
            entity.Property(e => e.IsPopular).HasDefaultValue(true);
            entity.Property(e => e.DisplayOrder).HasDefaultValue(0);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Amenity>(entity =>
        {
            entity.HasKey(e => e.AmenityId);
            entity.ToTable("amenities");

            entity.Property(e => e.AmenityId).HasColumnName("AmenityId");
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Icon).HasMaxLength(50);
            entity.Property(e => e.IconColor).HasMaxLength(30);
            entity.Property(e => e.DisplayOrder).HasDefaultValue(0);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<CarAmenity>(entity =>
        {
            entity.HasKey(e => new { e.CarId, e.AmenityId });
            entity.ToTable("car_amenities");

            entity.HasOne(d => d.Car)
                .WithMany(p => p.CarAmenities)
                .HasForeignKey(d => d.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Amenity)
                .WithMany(p => p.CarAmenities)
                .HasForeignKey(d => d.AmenityId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<VehicleInspection>(entity =>
        {
            entity.HasKey(e => e.InspectionId);
            entity.ToTable("vehicle_inspections");

            entity.HasIndex(e => new { e.OrderId, e.InspectionType }).IsUnique();

            entity.Property(e => e.InspectionType).HasMaxLength(20).IsRequired();
            entity.Property(e => e.FrontImage).HasMaxLength(255);
            entity.Property(e => e.RearImage).HasMaxLength(255);
            entity.Property(e => e.LeftImage).HasMaxLength(255);
            entity.Property(e => e.RightImage).HasMaxLength(255);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Order)
                .WithMany()
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Car)
                .WithMany()
                .HasForeignKey(d => d.CarId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CarCalendarBlock>(entity =>
        {
            entity.HasKey(e => e.BlockId);
            entity.ToTable("car_calendar_blocks");

            entity.HasIndex(e => new { e.CarId, e.StartTime, e.EndTime });

            entity.Property(e => e.Kind).HasMaxLength(30).IsRequired();
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Car)
                .WithMany()
                .HasForeignKey(d => d.CarId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SystemSetting>(entity =>
        {
            entity.HasKey(e => e.SettingId);
            entity.ToTable("system_settings");

            entity.Property(e => e.SettingId).HasColumnName("SettingId");
            entity.Property(e => e.ExtraKmPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Hotline).HasMaxLength(50);
            entity.Property(e => e.SupportEmail).HasMaxLength(100);
            entity.Property(e => e.PlatformName).HasMaxLength(150);
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId);
            entity.ToTable("notifications");

            entity.Property(e => e.NotificationId).HasColumnName("NotificationId");
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.LinkUrl).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
