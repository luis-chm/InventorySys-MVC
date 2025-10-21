using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace InventorySys.Models;

public partial class InventorySysContext : DbContext
{
    public InventorySysContext()
    {
    }

    public InventorySysContext(DbContextOptions<InventorySysContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblAuditLog> TblAuditLogs { get; set; }

    public virtual DbSet<TblCollection> TblCollections { get; set; }

    public virtual DbSet<TblDetailMovement> TblDetailMovements { get; set; }

    public virtual DbSet<TblFiniture> TblFinitures { get; set; }

    public virtual DbSet<TblFormat> TblFormats { get; set; }

    public virtual DbSet<TblMaterial> TblMaterials { get; set; }

    public virtual DbSet<TblMaterialTransaction> TblMaterialTransactions { get; set; }

    public virtual DbSet<TblRole> TblRoles { get; set; }

    public virtual DbSet<TblSite> TblSites { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }

    public virtual DbSet<VwCriticalStockMaterial> VwCriticalStockMaterials { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblAuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditLogId).HasName("PK__tbl_Audi__EB5F6CDD52A1DB4A");

            entity.ToTable("tbl_AuditLog");

            entity.Property(e => e.AuditLogId).HasColumnName("AuditLogID");
            entity.Property(e => e.NewValues).IsUnicode(false);
            entity.Property(e => e.OldValues).IsUnicode(false);
            entity.Property(e => e.Operation)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.OperationDateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RecordId).HasColumnName("RecordID");
            entity.Property(e => e.TableName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.TblAuditLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_AuditLog_Users");
        });

        modelBuilder.Entity<TblCollection>(entity =>
        {
            entity.HasKey(e => e.CollectionId).HasName("PK__tbl_Coll__7DE6BC24D7EFB30E");

            entity.ToTable("tbl_Collections");

            entity.Property(e => e.CollectionId).HasColumnName("CollectionID");
            entity.Property(e => e.CollectionEffect)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.CollectionName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TblDetailMovement>(entity =>
        {
            entity.HasKey(e => e.DetailMovId).HasName("PK__tbl_Deta__6A5CA8016F8F9139");

            entity.ToTable("tbl_DetailMovements");

            entity.Property(e => e.DetailMovId).HasColumnName("DetailMovID");
            entity.Property(e => e.DetCantEntry).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DetCantExit).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DetCurrentBalance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DetInitBalance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MaterialTransactionId).HasColumnName("MaterialTransactionID");

            entity.HasOne(d => d.MaterialTransaction).WithMany(p => p.TblDetailMovements)
                .HasForeignKey(d => d.MaterialTransactionId)
                .HasConstraintName("FK_DetailMovements_MaterialTransactions");
        });

        modelBuilder.Entity<TblFiniture>(entity =>
        {
            entity.HasKey(e => e.FinitureId).HasName("PK__tbl_Fini__09B41C91F4F3B814");

            entity.ToTable("tbl_Finitures");

            entity.Property(e => e.FinitureId).HasColumnName("FinitureID");
            entity.Property(e => e.FinitureCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FinitureName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TblFormat>(entity =>
        {
            entity.HasKey(e => e.FormatId).HasName("PK__tbl_Form__5D3DCB79C89DFB83");

            entity.ToTable("tbl_Formats");

            entity.Property(e => e.FormatId).HasColumnName("FormatID");
            entity.Property(e => e.FormatName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FormatSizeCm)
                .HasColumnType("decimal(8, 2)")
                .HasColumnName("FormatSizeCM");
        });

        modelBuilder.Entity<TblMaterial>(entity =>
        {
            entity.HasKey(e => e.MaterialId).HasName("PK__tbl_Mate__C5061317176755BA");

            entity.ToTable("tbl_Materials", tb =>
                {
                    tb.HasTrigger("trg_Materials_Delete");
                    tb.HasTrigger("trg_Materials_Insert");
                    tb.HasTrigger("trg_Materials_Update");
                });

            entity.Property(e => e.MaterialId).HasColumnName("MaterialID");
            entity.Property(e => e.CollectionId).HasColumnName("CollectionID");
            entity.Property(e => e.FinitureId).HasColumnName("FinitureID");
            entity.Property(e => e.FormatId).HasColumnName("FormatID");
            entity.Property(e => e.MaterialCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaterialDescription)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.MaterialImg)
                .IsUnicode(false)
                .HasColumnName("MaterialIMG");
            entity.Property(e => e.MaterialStock).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RecordInsertDateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SiteId).HasColumnName("SiteID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Collection).WithMany(p => p.TblMaterials)
                .HasForeignKey(d => d.CollectionId)
                .HasConstraintName("FK_Materials_Collections");

            entity.HasOne(d => d.Finiture).WithMany(p => p.TblMaterials)
                .HasForeignKey(d => d.FinitureId)
                .HasConstraintName("FK_Materials_Finitures");

            entity.HasOne(d => d.Format).WithMany(p => p.TblMaterials)
                .HasForeignKey(d => d.FormatId)
                .HasConstraintName("FK_Materials_Formats");

            entity.HasOne(d => d.Site).WithMany(p => p.TblMaterials)
                .HasForeignKey(d => d.SiteId)
                .HasConstraintName("FK_Materials_Sites");

            entity.HasOne(d => d.User).WithMany(p => p.TblMaterials)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Materials_Users");
        });

        modelBuilder.Entity<TblMaterialTransaction>(entity =>
        {
            entity.HasKey(e => e.MaterialTransactionId).HasName("PK__tbl_Mate__47D0464342C90309");

            entity.ToTable("tbl_MaterialTransactions");

            entity.Property(e => e.MaterialTransactionId).HasColumnName("MaterialTransactionID");
            entity.Property(e => e.MaterialId).HasColumnName("MaterialID");
            entity.Property(e => e.MaterialTransactionDate).HasColumnType("datetime");
            entity.Property(e => e.MaterialTransactionQuantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MaterialTransactionType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Material).WithMany(p => p.TblMaterialTransactions)
                .HasForeignKey(d => d.MaterialId)
                .HasConstraintName("FK_MaterialID");

            entity.HasOne(d => d.User).WithMany(p => p.TblMaterialTransactions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_MaterialTransactions_Users");
        });

        modelBuilder.Entity<TblRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__tbl_Role__8AFACE3A1BF5B222");

            entity.ToTable("tbl_Roles");

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TblSite>(entity =>
        {
            entity.HasKey(e => e.SiteId).HasName("PK__tbl_Site__B9DCB90365B219E5");

            entity.ToTable("tbl_Sites");

            entity.Property(e => e.SiteId).HasColumnName("SiteID");
            entity.Property(e => e.SiteLocation)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.SiteName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TblUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__tbl_User__1788CCAC7343652F");

            entity.ToTable("tbl_Users");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UserActive).HasDefaultValue(true);
            entity.Property(e => e.UserEmail)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.UserEncryptedPassword)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.TblUsers)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_Users_Roles");
        });

        modelBuilder.Entity<VwCriticalStockMaterial>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_CriticalStockMaterials");

            entity.Property(e => e.CollectionName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.MaterialCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaterialDescription)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.MaterialId).HasColumnName("MaterialID");
            entity.Property(e => e.MaterialStock).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SiteName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.StockStatus)
                .HasMaxLength(9)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
