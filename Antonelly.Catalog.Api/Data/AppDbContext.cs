using Microsoft.EntityFrameworkCore;
using Antonelly.Catalog.Api.Entities;
using Antonelly.Catalog.Api.Enums;

namespace Antonelly.Catalog.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Company> Companies => Set<Company>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductImage> ProductImages => Set<ProductImage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureCompany(modelBuilder);
            ConfigureCategory(modelBuilder);
            ConfigureProduct(modelBuilder);
            ConfigureProductImage(modelBuilder);
        }

        private static void ConfigureCompany(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(e => e.Slug)
                    .HasMaxLength(180)
                    .IsRequired();

                entity.Property(e => e.LogoUrl)
                    .HasMaxLength(1000);

                entity.Property(e => e.WhatsappNumber)
                    .HasMaxLength(50);

                entity.Property(e => e.InstagramUrl)
                    .HasMaxLength(500);

                entity.HasIndex(e => e.Slug)
                    .IsUnique();
            });
        }

        private static void ConfigureCategory(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(e => e.Slug)
                    .HasMaxLength(180)
                    .IsRequired();

                entity.HasOne(e => e.Company)
                    .WithMany(c => c.Categories)
                    .HasForeignKey(e => e.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.CompanyId, e.Slug })
                    .IsUnique();

                entity.HasIndex(e => new { e.CompanyId, e.DisplayOrder });
            });
        }

        private static void ConfigureProduct(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(e => e.Slug)
                    .HasMaxLength(180)
                    .IsRequired();

                entity.Property(e => e.ReferenceCode)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.Brand)
                    .HasMaxLength(100);

                entity.Property(e => e.Model)
                    .HasMaxLength(150);

                entity.Property(e => e.ShortDescription)
                    .HasMaxLength(500);

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.Currency)
                    .HasMaxLength(3)
                    .IsRequired()
                    .HasDefaultValue("COP");

                entity.Property(e => e.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20)
                    .IsRequired()
                    .HasDefaultValue(ProductStatus.Available);

                entity.Property(e => e.ShowPrice)
                    .HasDefaultValue(false);

                entity.Property(e => e.IsFeatured)
                    .HasDefaultValue(false);

                entity.Property(e => e.IsVisible)
                    .HasDefaultValue(true);

                entity.HasOne(e => e.Company)
                    .WithMany(c => c.Products)
                    .HasForeignKey(e => e.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.CompanyId, e.Slug })
                    .IsUnique();

                entity.HasIndex(e => new { e.CompanyId, e.ReferenceCode })
                    .IsUnique();

                entity.HasIndex(e => new { e.CompanyId, e.Status });

                entity.HasIndex(e => new { e.CompanyId, e.IsVisible });

                entity.HasIndex(e => new { e.CompanyId, e.IsFeatured });
            });
        }

        private static void ConfigureProductImage(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.PublicId)
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(e => e.Url)
                    .HasMaxLength(1000)
                    .IsRequired();

                entity.Property(e => e.SecureUrl)
                    .HasMaxLength(1000)
                    .IsRequired();

                entity.Property(e => e.OptimizedUrl)
                    .HasMaxLength(1000)
                    .IsRequired();

                entity.Property(e => e.OriginalFilename)
                    .HasMaxLength(255);

                entity.Property(e => e.Format)
                    .HasMaxLength(10);

                entity.Property(e => e.AltText)
                    .HasMaxLength(500);

                entity.HasOne(e => e.Company)
                    .WithMany()
                    .HasForeignKey(e => e.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Product)
                    .WithMany(p => p.Images)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.PublicId)
                    .IsUnique();

                entity.HasIndex(e => new { e.ProductId, e.DisplayOrder });

                entity.HasIndex(e => new { e.ProductId, e.IsMain })
                    .HasFilter("\"is_main\" = true");
            });
        }
    }
}
