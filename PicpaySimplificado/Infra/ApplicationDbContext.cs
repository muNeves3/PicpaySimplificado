using Microsoft.EntityFrameworkCore;
using PicpaySimplificado.Models;

namespace PicPaySimplificado.Infra
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Carteira?> Wallets { get; set; }

        public DbSet<Transferencia> Transfers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Carteira>()
                .HasIndex(w => new { w.CPFCNPJ, w.Email })
                .IsUnique();


            modelBuilder.Entity<Carteira>()
                .Property(t => t.SaldoConta)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Carteira>()
                .Property(w => w.UserType)
                .HasConversion<string>();

            modelBuilder.Entity<Transferencia>()
                .HasKey(t => t.IdTransferencia);

            modelBuilder.Entity<Transferencia>()
                .HasOne(t => t.Sender)
                .WithMany()
                .HasForeignKey(t => t.SenderId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Transaction_Sender");

            modelBuilder.Entity<Transferencia>()
                .Property(t => t.Valor)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Transferencia>()
                .HasOne(t => t.Reciver)
                .WithMany()
                .HasForeignKey(t => t.ReciverId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Transaction_Reciver");
        }
    }
}
