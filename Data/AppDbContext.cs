using Microsoft.EntityFrameworkCore;

using shipping_service_backend.Models;

namespace shipping_service_backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Shipment> Shipments => Set<Shipment>();
        public DbSet<TrackingEvent> TrackingEvents => Set<TrackingEvent>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Shipment>(entity =>
            {
                entity.HasKey(s => s.ShipmentId);

                entity.Property(s => s.ShipmentId)
                      .UseIdentityColumn();

                entity.Property(s => s.OrderNumber)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.HasIndex(s => s.OrderNumber)
                      .IsUnique();

                entity.Property(s => s.CustomerEmail)
                      .IsRequired()
                      .HasMaxLength(255);

                entity.Property(s => s.TrackingNumber)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.HasIndex(s => s.TrackingNumber)
                      .IsUnique();

                entity.Property(s => s.Carrier)
                      .HasMaxLength(100);

                // Store enum as string so it's human-readable in the DB
                // (consistent with how Java services store enums)
                entity.Property(s => s.Status)
                      .HasConversion<string>()
                      .HasMaxLength(30)
                      .HasDefaultValue(ShipmentStatus.Pending);

                entity.Property(s => s.ShippingAddress)
                      .HasColumnType("nvarchar(max)");

                entity.Property(s => s.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()")
                      .ValueGeneratedOnAdd();

       
                entity.Property(s => s.UpdatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.HasMany(s => s.TrackingEvents)
                      .WithOne(t => t.Shipment)
                      .HasForeignKey(t => t.shipmentId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TrackingEvent>(entity =>
            {
                entity.HasKey(t => t.EventId);

                entity.Property(t => t.EventId)
                      .UseIdentityColumn();

                entity.Property(t => t.Status)
                      .HasConversion<string>()
                      .HasMaxLength(30)
                      .IsRequired();

                entity.Property(t => t.Location)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(t => t.Notes)
                      .HasMaxLength(500);

                entity.Property(t => t.OccurredAt)
                      .IsRequired();
            });
        


    }
    }
}
