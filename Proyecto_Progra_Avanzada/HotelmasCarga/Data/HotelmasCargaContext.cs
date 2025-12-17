using Microsoft.EntityFrameworkCore;
using HotelmasCarga.Models;

namespace HotelmasCarga.Data
{
    public class HotelmasCargaContext : DbContext
    {
        public HotelmasCargaContext(DbContextOptions<HotelmasCargaContext> options) : base(options)
        {
        }

        public DbSet<TipoHabitacion> TiposHabitacion { get; set; } = null!;
        public DbSet<Habitacion> Habitaciones { get; set; } = null!;
        public DbSet<Cliente> Clientes { get; set; } = null!;
        public DbSet<Reserva> Reservas { get; set; } = null!;
        public DbSet<DisponibilidadHabitacion> Disponibilidades { get; set; } = null!;
        public DbSet<ColaEspera> ColasEspera { get; set; } = null!;
        public DbSet<HistorialReserva> HistorialesReserva { get; set; } = null!;
        public DbSet<Hotel> Hoteles { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure decimal precision if not using Column attribute
            modelBuilder.Entity<TipoHabitacion>().Property(t => t.Precio).HasColumnType("decimal(18,2)");

            // Configure relationships explicitly when helpful
            modelBuilder.Entity<Habitacion>()
                .HasOne(h => h.TipoHabitacion)
                .WithMany(t => t.Habitaciones)
                .HasForeignKey(h => h.IdTipoHabitacion)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Cliente)
                .WithMany(c => c.Reservas)
                .HasForeignKey(r => r.IdCliente)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Habitacion)
                .WithMany(h => h.Reservas)
                .HasForeignKey(r => r.IdHabitacion)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DisponibilidadHabitacion>()
                .HasOne(d => d.Habitacion)
                .WithMany(h => h.Disponibilidades)
                .HasForeignKey(d => d.IdHabitacion)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ColaEspera>()
                .HasOne(c => c.Cliente)
                .WithMany(cl => cl.ColaEspera)
                .HasForeignKey(c => c.IdCliente)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ColaEspera>()
                .HasOne(c => c.TipoHabitacion)
                .WithMany(t => t.ColaEspera)
                .HasForeignKey(c => c.IdTipoHabitacion)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HistorialReserva>()
                .HasOne(h => h.Reserva)
                .WithMany(r => r.Historial)
                .HasForeignKey(h => h.IdReserva)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint: one availability record per room per date
            modelBuilder.Entity<DisponibilidadHabitacion>()
                .HasIndex(d => new { d.IdHabitacion, d.Fecha })
                .IsUnique();
        }
    }
}
