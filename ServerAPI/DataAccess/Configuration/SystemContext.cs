using BlockchainAppAPI.Models.Configuration;
using Microsoft.EntityFrameworkCore;
using BlockchainAppAPI.Models.Search;

namespace BlockchainAppAPI.DataAccess.Configuration
{
    public class SystemContext: DbContext
    {
        public DbSet<Module> Modules { get; set; }
        public DbSet<Models.Configuration.Object> Objects { get; set; }
        public DbSet<ObjectField> ObjectFields { get; set; }
        
        public DbSet<Page> Pages { get; set; }
        public DbSet<Widget> Widgets { get; set; }
        public DbSet<WidgetTree> WidgetTrees { get; set; }
        
        public DbSet<BaseDataModel> Data { get; set; }
        
        public DbSet<SearchObject> SearchObjects { get; set; }

        public SystemContext(DbContextOptions<SystemContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            CreateConfiguration(modelBuilder);

            CreateSystem(modelBuilder);

            CreateSearch(modelBuilder);

            #region Data

            modelBuilder.Entity<BaseDataModel>().
                Property(d => d._DataBlob).
                HasColumnName("DataBlob");

            #endregion
        }

        private void CreateConfiguration(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Page>().
                HasOne(p => p.Module).
                WithMany(m => m.Pages).
                HasForeignKey(p => p.ModuleId).
                IsRequired();

            modelBuilder.Entity<Page>().
                HasOne(p => p.MainWidget).
                WithMany().
                HasForeignKey(p => p.MainWidgetId);

            modelBuilder.Entity<Widget>().
                HasOne(w => w.Module).
                WithMany(m => m.Widgets).
                HasForeignKey(w => w.ModuleId).
                IsRequired();
                
            modelBuilder.Entity<Widget>().
                Property(w => w._WidgetProperties).
                HasColumnName("WidgetProperties");
            
            modelBuilder.Entity<WidgetTree>().
                HasOne(w => w.Widget).
                WithMany(m => m.Children).
                HasForeignKey(w => w.WidgetId).
                IsRequired();
                
            modelBuilder.Entity<WidgetTree>().
                HasOne(w => w.ChildWidget).
                WithMany(m => m.Parents).
                HasForeignKey(w => w.ChildWidgetId).
                IsRequired();
        }

        private void CreateSystem(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Models.Configuration.Object>().
                HasOne(o => o.Module).
                WithMany(m => m.Objects).
                HasForeignKey(o => o.ModuleId).
                IsRequired();

            modelBuilder.Entity<Models.Configuration.Object>().
                HasOne(o => o.ParentObject).
                WithMany().
                HasForeignKey(o => o.ParentObjectId);

            modelBuilder.Entity<Models.Configuration.Object>().
                HasOne(o => o.MainObject).
                WithMany().
                HasForeignKey(o => o.MainObjectId);
                
            modelBuilder.Entity<ObjectField>().
                HasOne(f => f.Object).
                WithMany(o => o.ObjectFields).
                HasForeignKey(f => f.ObjectId).
                IsRequired();
        }

        private void CreateSearch(ModelBuilder modelBuilder) {
            modelBuilder.Entity<SearchObject>().
                Property(w => w._CompiledFieldList).
                HasColumnName("CompiledFieldList");

            modelBuilder.Entity<Selection>().
                HasOne(s => s.SearchObject).
                WithMany(so => so.Selections).
                HasForeignKey(s => s.SearchObjectId).
                IsRequired();
        }
    }
}