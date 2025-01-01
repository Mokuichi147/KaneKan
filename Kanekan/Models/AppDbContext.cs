using Microsoft.EntityFrameworkCore;

namespace KaneKan.Models
{
    public class AppDbContext : DbContext
    {
        private string filename = Path.Join("Data", "KaneKan.sqlite3");
        private string connectionString;

        public DbSet<Payment> Payments { get; set; } = null!;

        public AppDbContext(bool isReadOnly)
        {
            connectionString = $"Data Source={filename}";
            if (isReadOnly)
            {
                connectionString += ";Mode=ReadOnly";
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(connectionString);
        }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("payments");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id").IsRequired();
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
                entity.Property(e => e.PaymentAt).HasColumnName("payment_at");
                entity.Property(e => e.IsPaid).HasColumnName("is_paid").IsRequired();
                entity.Property(e => e.Name).HasColumnName("name").IsRequired();
                entity.Property(e => e.Amount).HasColumnName("amount").IsRequired();
                entity.Property(e => e.PaymentMethod).HasColumnName("payment_method");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.CategoriesJson).HasColumnName("categories_json");
            });
        }
    }
}