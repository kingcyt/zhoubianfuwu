﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace EntitiesModel
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class APeripheralServicesEntities : DbContext
    {
        public APeripheralServicesEntities()
            : base("name=APeripheralServicesEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AroundUserFinance> AroundUserFinance { get; set; }
        public virtual DbSet<AroundUserFinanceLog> AroundUserFinanceLog { get; set; }
        public virtual DbSet<BackUserRole> BackUserRole { get; set; }
        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<KeyWordUpHomePag> KeyWordUpHomePag { get; set; }
        public virtual DbSet<LinkMailboxQuery> LinkMailboxQuery { get; set; }
        public virtual DbSet<Notice> Notice { get; set; }
        public virtual DbSet<OrderMailboxQuery> OrderMailboxQuery { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<RoleNavigationBar> RoleNavigationBar { get; set; }
        public virtual DbSet<BackUser> BackUser { get; set; }
        public virtual DbSet<NavigationBar> NavigationBar { get; set; }
        public virtual DbSet<AroundUser> AroundUser { get; set; }
        public virtual DbSet<DrawMoney> DrawMoney { get; set; }
        public virtual DbSet<BadEvaluate> BadEvaluate { get; set; }
        public virtual DbSet<HomePagNoBadEvaluate> HomePagNoBadEvaluate { get; set; }
        public virtual DbSet<NavBySubmeun> NavBySubmeun { get; set; }
        public virtual DbSet<BugQuery> BugQuery { get; set; }
        public virtual DbSet<AsinKeyWord> AsinKeyWord { get; set; }
        public virtual DbSet<ShopReport> ShopReport { get; set; }
        public virtual DbSet<ShopCpcReport> ShopCpcReport { get; set; }
        public virtual DbSet<AddShopCart> AddShopCart { get; set; }
        public virtual DbSet<Wishs> Wishs { get; set; }
        public virtual DbSet<Different> Different { get; set; }
        public virtual DbSet<Evaluate> Evaluate { get; set; }
    }
}
