using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace UserManagement.Models;

public partial class LibraryManagementContext : DbContext
{
    public LibraryManagementContext()
    {
    }

    public LibraryManagementContext(DbContextOptions<LibraryManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\SQLExpress;Database=LibraryManagement;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CE2450186");
            entity.Property(e => e.NameHindi).HasMaxLength(100).HasColumnType("nvarchar");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Role).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.UserName).HasMaxLength(100);
            entity.Property(e => e.IsDeleted).HasColumnType("int");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
