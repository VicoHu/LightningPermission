using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LightningPermission.Models
{
    public class PermissionTokenContext : DbContext //继承DBContext 来自EF Core框架
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private string ConnectionString = null;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="ConnectionString">数据库连接字符串</param>
        public PermissionTokenContext(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
        }

        public PermissionTokenContext(DbContextOptions<PermissionTokenContext> options): base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(this.ConnectionString);
                // 配置SqlServer连接字符串
            }
        }

        public DbSet<PermissionToken> PermissionToken { set;get;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
