using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Model.MetaDados;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Agente> Agente { get; set; }
        public DbSet<Fila> Fila { get; set; }
        public DbSet<ObservacoesAgente> Observacoes { get; set; }

        private readonly IConfiguration configuration;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration) : base(options)
        {
            this.configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSnakeCaseNamingConvention();
        #if DEBUG
            optionsBuilder.EnableSensitiveDataLogging(true);
        #endif
            base.OnConfiguring(optionsBuilder);
        }

        private void ConfiguraAgente(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Agente>(etd =>
            {
                etd.ToTable("Agentes");
                etd.HasKey(a => a.Id).HasName("id");
                //etd.Property(a => a.Id).HasColumnName("id");
                //etd.Property(a => a.Nome).HasColumnName("nome").HasMaxLength(100);
                //etd.Property(a => a.Status).HasColumnName("status").HasMaxLength(30);
                //etd.Property(a => a.Fila.Nome).HasColumnName("fila").HasMaxLength(100);
            });
        }

        private void ConfiguraFila(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Fila>(etd =>
            {
                etd.ToTable("Fila");
                etd.HasKey(a => a.Id).HasName("id");
            });
        }

        private void ConfiguraObservacoes(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ObservacoesAgente>(etd =>
            {
                etd.ToTable("Observacoes");
                etd.HasKey(a => a.Id).HasName("id");
            });
        }

        //private void ConfiguraPedido(ModelBuilder construtorDeModelos)
        //{
        //    construtorDeModelos.Entity<ObservacoesAgente>(etd =>
        //    {
        //        etd.ToTable("tbPedido");
        //        etd.HasKey(p => p.Id).HasName("id");
        //        etd.Property(p => p.Id).HasColumnName("id").ValueGeneratedOnAdd();
        //        etd.Property(p => p.Total).HasColumnName("total");
        //        etd.HasOne(c => c.Cliente).WithMany(p => p.Pedidos);
        //    });
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            #region Mapeamento
            //modelBuilder.HasDefaultSchema("reportadsomos");

            ConfiguraAgente(modelBuilder);
            ConfiguraFila(modelBuilder);
            ConfiguraObservacoes(modelBuilder);
            #endregion
        }


    }
}
