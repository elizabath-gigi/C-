using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OrderManagement.Models;

public partial class LibraryContext : DbContext
{
    public LibraryContext()
    {
    }

    public LibraryContext(DbContextOptions<LibraryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Library> Libraries { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }
 //warningTo protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
       // => optionsBuilder.UseSqlServer("Server=DESKTOP-04AG7NB\\SQLEXPRESS; Database=Library; User Id=DESKTOP-04AG7NB\\P10;TrustServerCertificate=True;Trusted_Connection=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Library>(entity =>
        {
            entity.HasKey(e => e.BookId);

            entity.ToTable("library");

            entity.Property(e => e.BookId)
                .ValueGeneratedNever()
                .HasColumnName("BookId");
            entity.Property(e => e.BookAuthor)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BookAuthor");
            entity.Property(e => e.BookName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BookName");
            entity.Property(e => e.NoOfBook)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("NoOfBook");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
