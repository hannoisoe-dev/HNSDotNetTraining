using HNSDotNetTraining.ConsoleApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HNSDotNetTraining.ConsoleApp
{
    internal class AppDBContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=HNSDotNetTraining;User ID=sa;Password=sasa@123;TrustServerCertificate=true");
            }
        }

        
        // Spa Management System DbSets
        public DbSet<SpaServiceDataModel> SpaServices { get; set; }
        public DbSet<CustomerDataModel> Customers { get; set; }
        public DbSet<TherapistDataModel> Therapists { get; set; }
        public DbSet<AppointmentDataModel> Appointments { get; set; }
        public DbSet<PaymentDataModel> Payments { get; set; }
    }
}
