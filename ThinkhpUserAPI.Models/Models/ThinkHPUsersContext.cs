using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ThinkhpUserAPI.Models.Models;

public partial class ThinkHPUsersContext : DbContext
{
    public ThinkHPUsersContext()
    {
    }

    public ThinkHPUsersContext(DbContextOptions<ThinkHPUsersContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CityMaster> CityMasters { get; set; }

    public virtual DbSet<ParaglidingTicketPurchase> ParaglidingTicketPurchases { get; set; }

    public virtual DbSet<ParaglidingTicketPurchaseDetail> ParaglidingTicketPurchaseDetails { get; set; }

    public virtual DbSet<ParaglidingTicketStatus> ParaglidingTicketStatuses { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<StateMaster> StateMasters { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserLogInToken> UserLogInTokens { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CityMaster>(entity =>
        {
            entity.HasKey(e => e.CityId).HasName("PK_Table_1");

            entity.ToTable("CityMaster");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<ParaglidingTicketPurchase>(entity =>
        {
            entity.HasKey(e => e.PurchasedTicketId);

            entity.ToTable("ParaglidingTicketPurchase");

            entity.Property(e => e.AmountPerTicket).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Cgstamount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("CGSTAmount");
            entity.Property(e => e.Cgstpercentage)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("CGSTPercentage");
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NetAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Sgstamount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("SGSTAmount");
            entity.Property(e => e.Sgstpercentage)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("SGSTPercentage");

            entity.HasOne(d => d.User).WithMany(p => p.ParaglidingTicketPurchases)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_ParaglidingTicketPurchase_User");
        });

        modelBuilder.Entity<ParaglidingTicketPurchaseDetail>(entity =>
        {
            entity.HasKey(e => e.PurchasedTicketDetailId);

            entity.ToTable("ParaglidingTicketPurchaseDetail");

            entity.Property(e => e.NetAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.PurchasedTicket).WithMany(p => p.ParaglidingTicketPurchaseDetails)
                .HasForeignKey(d => d.PurchasedTicketId)
                .HasConstraintName("FK_ParaglidingTicketPurchaseDetail_ParaglidingTicketPurchase");

            entity.HasOne(d => d.User).WithMany(p => p.ParaglidingTicketPurchaseDetails)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_ParaglidingTicketPurchaseDetail_User");
        });

        modelBuilder.Entity<ParaglidingTicketStatus>(entity =>
        {
            entity.HasKey(e => e.TicketStatusId);

            entity.ToTable("ParaglidingTicketStatus");

            entity.Property(e => e.TicketStatusId).ValueGeneratedNever();
            entity.Property(e => e.PurchasedTicketDetailId).ValueGeneratedOnAdd();

            entity.HasOne(d => d.PurchasedTicketDetail).WithMany(p => p.ParaglidingTicketStatuses)
                .HasForeignKey(d => d.PurchasedTicketDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ParaglidingTicketStatus_ParaglidingTicketPurchaseDetail");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.RoleName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<StateMaster>(entity =>
        {
            entity.HasKey(e => e.StateId).HasName("PK_StateMst");

            entity.ToTable("StateMaster");

            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Posno)
                .HasMaxLength(50)
                .HasColumnName("POSNo");
            entity.Property(e => e.StateCode).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.Address)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.AlternateMobileNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.InsertedOn).HasColumnType("datetime");
            entity.Property(e => e.LastName)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.MobileNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
            entity.Property(e => e.UserName)
                .HasMaxLength(250)
                .IsUnicode(false);

            entity.HasOne(d => d.City).WithMany(p => p.Users)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("FK_User_CityMaster");

            entity.HasOne(d => d.State).WithMany(p => p.Users)
                .HasForeignKey(d => d.StateId)
                .HasConstraintName("FK_User_StateMaster");
        });

        modelBuilder.Entity<UserLogInToken>(entity =>
        {
            entity.HasKey(e => e.UserTokenId);

            entity.ToTable("UserLogInToken");

            entity.Property(e => e.InsertedOn).HasColumnType("datetime");
            entity.Property(e => e.TokenExpireTime).HasColumnType("datetime");
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("UserRole");

            entity.Property(e => e.InsertedOn).HasColumnType("datetime");
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_UserRole_Role");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserRole_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
