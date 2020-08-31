using System;
using System.Linq;
using LightningPermission.Models;

namespace LightningPermission
{
    class TokenStore
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private string ConnectionString;

        /// <summary>
        /// 数据库连接对象
        /// </summary>
        private PermissionTokenContext context;

        public TokenStore(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
            this.context = new PermissionTokenContext(ConnectionString);
        }

        /// <summary>
        /// 创建一个新的用户权限实例，并储存到数据库
        /// </summary>
        /// <param name="Uid">用户唯一Id</param>
        /// <param name="Role">权限码</param>
        /// <returns>是否完成创建</returns>
        public bool CreateUser(string Uid, string Role)
        {
            using (var transation = this.context.Database.BeginTransaction())
            {
                context.PermissionToken.Add(new PermissionToken()
                {
                    Uid = Uid,
                    Role = Role,
                    TokenStr = Guid.NewGuid().ToString().Replace("-", "")
                });
                // 添加用户
                int EffectLineCount = context.SaveChanges();
                // 保存变化
                if (EffectLineCount > 0)
                    transation.Commit();
                else
                    transation.Rollback();
                return EffectLineCount > 0;
            }
        }

        /// <summary>
        /// 根据Uid，更新权限
        /// </summary>
        /// <param name="Uid">用户Id</param>
        /// <param name="Role">权限</param>
        /// <returns>是否完成更新</returns>
        public bool UpdateRoleByUid(string Uid, string Role)
        {
            using (var transation = this.context.Database.BeginTransaction())
            {
                var user = context.PermissionToken.Where(p => p.Uid == Uid).FirstOrDefault();
                // 找到该用户
                if (user == null)
                {
                    // 如果不存在该用户
                    return false;
                }
                user.Role = Role;
                context.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                int EffectLineCount = context.SaveChanges();
                // 保存变化
                if (EffectLineCount > 0)
                    transation.Commit();
                else
                    transation.Rollback();
                return EffectLineCount > 0;
            }
        }


        /// <summary>
        /// 根据Tokne字符串，更新权限
        /// </summary>
        /// <param name="TokenStr">Tokne字符串</param>
        /// <param name="Role">权限</param>
        /// <returns>是否完成更新</returns>
        public bool UpdateRoleByTokenStr(string TokenStr, string Role)
        {
            using (var transation = this.context.Database.BeginTransaction())
            {
                var user = context.PermissionToken.Where(p => p.TokenStr == TokenStr).FirstOrDefault();
                // 找到该用户
                if (user == null)
                {
                    // 如果不存在该用户
                    return false;
                }
                user.Role = Role;
                context.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                int EffectLineCount = context.SaveChanges();
                // 保存变化
                if (EffectLineCount > 0)
                    transation.Commit();
                else
                    transation.Rollback();
                return EffectLineCount > 0;
            }
        }

        /// <summary>
        /// 根据Uid获得Role
        /// </summary>
        /// <param name="Uid">用户Id</param>
        /// <returns></returns>
        public string GetRoleByUid(string Uid)
        {
            PermissionToken permissionToken = context.PermissionToken.Find(Uid);
            return permissionToken != null ? permissionToken.Role : null;
        }

        /// <summary>
        /// 根据Uid获得Token字符串
        /// </summary>
        /// <param name="Uid">用户Id</param>
        /// <returns></returns>
        public string GetTokenStrByUid(string Uid)
        {
            PermissionToken permissionToken = context.PermissionToken.Find(Uid);
            return permissionToken != null ? permissionToken.TokenStr : null;
        }

        /// <summary>
        /// 根据Token字符串获得Role权限
        /// </summary>
        /// <param name="TokenStr">Token字符串</param>
        /// <returns></returns>
        public string GetRoleByTokenStr(string TokenStr)
        {
            PermissionToken permissionToken = context.PermissionToken.Where(P => P.TokenStr == TokenStr).FirstOrDefault();
            return permissionToken != null ? permissionToken.Role : null;
        }

        /// <summary>
        /// 根据Token字符串获得Uid
        /// </summary>
        /// <param name="TokenStr">Token字符串</param>
        /// <returns></returns>
        public string GetUidByTokenStr(string TokenStr)
        {
            PermissionToken permissionToken = context.PermissionToken.Where(P => P.TokenStr == TokenStr).FirstOrDefault();
            return permissionToken != null ? permissionToken.Uid : null;
        }

        /// <summary>
        /// 根据Token字符串获得PermissionToken对象
        /// </summary>
        /// <param name="TokenStr">Token字符串</param>
        /// <returns></returns>
        public PermissionToken GetPermissionTokenByTokenStr(string TokenStr)
        {
            PermissionToken permissionToken = context.PermissionToken.Where(P => P.TokenStr == TokenStr).FirstOrDefault();
            return permissionToken;
        }

        /// <summary>
        /// 根据Uid获得PermissionToken对象
        /// </summary>
        /// <param name="Uid">用户Id</param>
        /// <returns></returns>
        public PermissionToken GetPermissionTokenByUid(string Uid)
        {
            PermissionToken permissionToken = context.PermissionToken.Where(P => P.Uid == Uid).FirstOrDefault();
            return permissionToken;
        }

        /// <summary>
        /// 初始化数据库
        /// </summary>
        public void InitDatabase()
        {
            using (var db = new PermissionTokenContext(this.ConnectionString))
            {
                // 检测是否存在数据库，如果存在，则不作操作，如果 不存在，则生成数据库
                db.Database.EnsureCreated();
            }
        }
    }
}
