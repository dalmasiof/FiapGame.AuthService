using Entities;
using Microsoft.EntityFrameworkCore;

namespace Context
{
    public class AuthContext : DbContext
    {
        public AuthContext(DbContextOptions<AuthContext> options) : base(options)
        {
        }
        public DbSet<Login> Logins { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Login>(entity =>
            {
                entity.HasKey(e => e.IdLogin);
                entity.Property(e => e.IdConta)
                .IsRequired();
                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(150);
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(150);
                entity.HasIndex(e => e.Email)
                    .IsUnique();
                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(255);
                entity.Property(e => e.DataCriacao)
                    .IsRequired();
                entity.Property(e => e.Ativo)
                    .IsRequired();


            });
        }
    }
}
