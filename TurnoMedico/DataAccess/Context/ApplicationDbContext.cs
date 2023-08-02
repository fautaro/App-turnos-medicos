using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Context
{
    public class ApplicationDbContext : DbContext
    {
        // Define propiedades DbSet para cada entidad que deseas mapear a tablas en la base de datos
        public DbSet<Profesion> Profesion { get; set; }
        public DbSet<Profesional> Profesional { get; set; }
        public DbSet<Turno> Turno { get; set; }
        public DbSet<AgendaBloqueada> AgendaBloqueada { get; set; }



        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Opcional: Si deseas personalizar cómo se mapean las entidades a las tablas en la base de datos, puedes configurarlas aquí
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<Turno>(entity =>
            {
                entity.ToTable("Turno"); // Nombre de la tabla en la base de datos
                entity.HasKey(e => e.Turno_Id); // Clave primaria
                entity.Property(e => e.Turno_Id).HasColumnName("Turno_Id").IsRequired(); // Propiedad para el campo Turno_Id
                entity.Property(e => e.FechaHora).HasColumnName("FechaHora").IsRequired(); // Propiedad para el campo FechaHora
                entity.Property(e => e.Nombre).HasColumnName("Nombre").HasMaxLength(50); // Propiedad para el campo Nombre con límite de 50 caracteres
                entity.Property(e => e.Apellido).HasColumnName("Apellido").HasMaxLength(50); // Propiedad para el campo Apellido con límite de 50 caracteres
                entity.Property(e => e.Telefono).HasColumnName("Telefono").HasMaxLength(50); // Propiedad para el campo Telefono con límite de 50 caracteres
                entity.Property(e => e.Email).HasColumnName("Email").HasMaxLength(50); // Propiedad para el campo Email con límite de 50 caracteres
                entity.Property(e => e.Activo).HasColumnName("Activo").IsRequired(); // Propiedad para el campo Activo
                entity.Property(e => e.Profesional_Id).HasColumnName("Profesional_Id").IsRequired(); // Propiedad para el campo Profesional_Id
                entity.Property(e => e.Link).HasColumnName("Link").HasMaxLength(50); // Propiedad para el campo Link con límite de 50 caracteres
            });


            modelBuilder.Entity<Profesion>(entity =>
            {
                entity.ToTable("Profesion"); // Nombre de la tabla en la base de datos
                entity.HasKey(e => e.Profesion_Id); // Clave primaria
                entity.Property(e => e.Profesion_Id).HasColumnName("Profesion_Id").ValueGeneratedOnAdd(); // Propiedad para el campo Profesion_Id con autoincremento
                entity.Property(e => e.Nombre).HasColumnName("Nombre").HasMaxLength(50); // Propiedad para el campo Nombre con límite de 50 caracteres
                entity.Property(e => e.Rubro).HasColumnName("Rubro").HasMaxLength(50); // Propiedad para el campo Rubro con límite de 50 caracteres
            });

            // Configuración de la entidad Profesional
            modelBuilder.Entity<Profesional>(entity =>
            {
                entity.ToTable("Profesional"); // Nombre de la tabla en la base de datos
                entity.HasKey(e => e.Profesional_Id); // Clave primaria
                entity.Property(e => e.Profesional_Id).HasColumnName("Profesional_Id").IsRequired(); // Propiedad para el campo Profesional_Id
                entity.Property(e => e.Nombre).HasColumnName("Nombre").HasMaxLength(50); // Propiedad para el campo Nombre con límite de 50 caracteres
                entity.Property(e => e.Apellido).HasColumnName("Apellido").HasMaxLength(50); // Propiedad para el campo Apellido con límite de 50 caracteres
                entity.Property(e => e.Alias).HasColumnName("Alias").HasMaxLength(15); // Propiedad para el campo Alias con límite de 15 caracteres
                entity.Property(e => e.Profesion_Id).HasColumnName("Profesion_Id").IsRequired(); // Propiedad para el campo Profesion_Id
                entity.Property(e => e.Usuario_Id).HasColumnName("Usuario_Id").IsRequired(); // Propiedad para el campo Usuario_Id
                entity.Property(e => e.Descripcion).HasColumnName("Descripcion").HasMaxLength(500); // Propiedad para el campo Descripcion con límite de 500 caracteres
            });


            modelBuilder.Entity<AgendaBloqueada>(entity =>
            {
                entity.ToTable("AgendaBloqueada"); // Nombre de la tabla en la base de datos
                entity.HasKey(e => e.AgendaBloqueada_Id); // Clave primaria
                entity.Property(e => e.AgendaBloqueada_Id).HasColumnName("AgendaBloqueada_Id").ValueGeneratedOnAdd(); // Propiedad para el campo AgendaBloqueada_Id con autoincremento
                entity.Property(e => e.Profesional_Id).HasColumnName("Profesional_Id").IsRequired(); // Propiedad para el campo Profesional_Id
                entity.Property(e => e.FechaDesde).HasColumnName("FechaDesde").IsRequired(); // Propiedad para el campo FechaDesde
                entity.Property(e => e.FechaHasta).HasColumnName("FechaHasta").IsRequired(); // Propiedad para el campo FechaHasta
                entity.Property(e => e.Activo).HasColumnName("Activo").IsRequired(); // Propiedad para el campo Activo
            });
        }
    }
}
