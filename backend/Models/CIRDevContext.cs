using System.Collections.Generic;
using backend.Models.ColorScheme;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;


namespace backend.Models
{
    public partial class CIRDevContext : DbContext
    {
        public CIRDevContext()
        {
        }

        public CIRDevContext(DbContextOptions<CIRDevContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<ChartRule> ChartRules { get; set; } = null!;
        public virtual DbSet<File> Files { get; set; } = null!;
        public virtual DbSet<Topic> Topics { get; set; } = null!;
        public virtual DbSet<Legend> Legend { get; set; } = null!;
        public virtual DbSet<LegendRow> LegendRow { get; set; } = null!;
        public virtual DbSet<ActiveLegend> ActiveLegend { get; set; } = null!;



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=TERRE-10-217\\SQL2012;Database=CIRTest;TrustServerCertificate=True;User ID=CIRDev;Password=c4QbT$QUmHAfFaNq;");
                optionsBuilder.EnableSensitiveDataLogging();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Legend>(entity =>
            {
                entity.ToTable("Legend");

                entity.HasIndex(e => e.legendId, "AK_Legend_Id")
                    .IsUnique();

                entity.Property(e => e.legendName)
                    .HasMaxLength(50)
                    .IsUnicode(true);
            });

            modelBuilder.Entity<LegendRow>(entity =>
            {
                entity.ToTable("LegendRow");

                entity.HasIndex(e => e.LegendRowId, "AK_Legend_Id")
                    .IsUnique();             
            });

            modelBuilder.Entity<ActiveLegend>(entity =>
            {
                entity.ToTable("ActiveLegend");

                entity.HasIndex(e => e.ActiveLegendId, "AK_ActiveLegend_Id")
                    .IsUnique();
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.HasIndex(e => e.Code, "AK_Category_Code")
                    .IsUnique();

                entity.Property(e => e.Code)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ChartRule>(entity =>
            {
                entity.ToTable("ChartRule");

                entity.HasIndex(e => e.TopicId, "RuleTopic_FK");

                entity.Property(e => e.IsGeneric)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnType("smalldatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Topic)
                    .WithMany(p => p.ChartRules)
                    .HasForeignKey(d => d.TopicId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ChartRuleTopic");

                entity.HasMany(d => d.Categories)
                    .WithMany(p => p.ChartRules)
                    .UsingEntity<Dictionary<string, object>>(
                        "ChartRuleCategory",
                        l => l.HasOne<Category>().WithMany().HasForeignKey("CategoryId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ChartRuleCategoryCategory"),
                        r => r.HasOne<ChartRule>().WithMany().HasForeignKey("ChartRuleId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ChartRuleCategoryChartRule"),
                        j =>
                        {
                            j.HasKey("ChartRuleId", "CategoryId");

                            j.ToTable("ChartRuleCategory");

                            j.HasIndex(new[] { "ChartRuleId" }, "ChartRuleCategory_FK");

                            j.HasIndex(new[] { "CategoryId" }, "ChartRuleCategory_FK2");
                        });
            });

            modelBuilder.Entity<File>(entity =>
            {
                entity.HasIndex(e => e.Name, "AK_Files_Name")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(128);
            });

            modelBuilder.Entity<Topic>(entity =>
            {
                entity.ToTable("Topic");

                entity.HasIndex(e => e.Code, "AK_Topic_Code")
                    .IsUnique();

                entity.HasIndex(e => e.ParentTopicId, "TopicParent_FK");

                entity.Property(e => e.Code)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.ParentTopic)
                    .WithMany(p => p.InverseParentTopic)
                    .HasForeignKey(d => d.ParentTopicId)
                    .HasConstraintName("FK_TopicTopic");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
        public void DetachAllEntities()
        {
            var changedEntriesCopy = this.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Unchanged)
                .ToList();

            foreach (var entry in changedEntriesCopy)
                entry.State = EntityState.Detached;
        }
    }
}
