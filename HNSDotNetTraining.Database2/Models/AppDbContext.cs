using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HNSDotNetTraining.Database2.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblProduct> TblProducts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblProduct>(entity =>
        {
            entity.Property(e => e.ProductCode).HasComputedColumnSql("('P'+right('0000'+CONVERT([varchar](10),[ProductId]),(4)))", true);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
