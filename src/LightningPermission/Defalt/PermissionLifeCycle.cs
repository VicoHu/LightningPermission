using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using LightningPermission.Models;

namespace LightningPermission
{
    public class PermissionLifeCycle : IPermissionLifeCycle
    {
        /// <summary>
        /// 权限字符串
        /// </summary>
        protected string RoleStr = null;

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected string ConnectionString = null;

        public PermissionLifeCycle(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
        }

        /// <summary>
        /// 生命周期：获取控制器Attribute之前
        /// </summary>
        /// <param name="context">Http上下文对象</param>
        public virtual string BeforeGetControllerAttribute(HttpContext context)
        {
            using (var db = new PermissionTokenContext(this.ConnectionString))
            {
                // 检测是否存在数据库，如果存在，则不作操作，如果 不存在，则生成数据库
                db.Database.EnsureCreated();
            }

            // 根据头部Token字符串(ltoken)获取当前权限
            if (context.Request.Headers.ContainsKey("ltoken"))
            {
                // 判断是否能从Header中获得到ltoken
                TokenStore tokenStore = new TokenStore(this.ConnectionString);
                // 获得当前token字符串的权限字符串
                this.RoleStr = tokenStore.GetRoleByTokenStr(context.Request.Headers["ltoken"]);
            }
            // 根据url参数中，获得Token字符串(ltoken)，再获取当前权限（不推荐）
            else if (context.Request.Query.ContainsKey("ltoken")) 
            {
                // 判断是否能从Query中获得到ltoken
                TokenStore tokenStore = new TokenStore(this.ConnectionString);
                // 获得当前token字符串的权限字符串
                this.RoleStr = tokenStore.GetRoleByTokenStr(context.Request.Query["ltoken"]);
            }
            /*
             * 尝试在Query中uid
             * 绝大部分情况下，为了安全着想
             * 请尽量不要使用这种获取验证的方式，推荐头部附带Token字符串进行身份验证
             */
            //else if (context.Request.Query.ContainsKey("uid"))
            //{
            //    // 判断是否获得到uid
            //    TokenStore tokenStore = new TokenStore(this.ConnectionString);
            //    // 获得当前token字符串的权限字符串
            //    this.RoleStr = tokenStore.GetRoleByUid(context.Request.Query["uid"]);
            //}
            else
            {
                this.RoleStr = null;
            }
            return this.RoleStr;
        }

        /// <summary>
        /// 生命周期：获取控制器Attribute，并进行操作
        /// </summary>
        /// <param name="context">Http上下文对象</param>
        /// <param name="permission">该控制器上的Permission对象</param>
        /// <param name="next">管道下一中间件对象实例</param>
        /// <returns>是否通过Controller的权限检测</returns>
        public virtual async Task<bool> OnGetControllerAttribute(HttpContext context, Permission permission, RequestDelegate next)
        {
            if (permission != null)
            {
                DefaultOperation operation = new DefaultOperation(context, permission, next, this.RoleStr);
                return await operation.OnControllerCheck();
            }
            return true;
        }

        /// <summary>
        /// 生命周期：获取控制器Attribute并进行才操作之后
        /// </summary>
        /// <param name="context">Http上下文对象</param>
        public virtual void AfterGetControllerAttribute(HttpContext context)
        {
        }

        /// <summary>
        /// 生命周期：在检测方法的Attribute之前
        /// </summary>
        /// <param name="context">Http上下文对象</param>
        public virtual void BeforeGetMethodAttribute(HttpContext context)
        {
        }

        /// <summary>
        /// 生命周期：正在对拥有Attribute的方法进行操作
        /// </summary>
        /// <param name="context">Http上下文对象</param>
        /// <param name="permission">读取到的</param>
        /// <returns></returns>
        public virtual async Task<Boolean> OnGetActionAttribute(HttpContext context, Permission permission, RequestDelegate next, bool IsControllerAllow)
        {
            if (permission != null)
            {
                DefaultOperation operation = new DefaultOperation(context, permission, next, this.RoleStr);
                return await operation.OnActionCheck(IsControllerAllow);
            }
            return true;
        }

        /// <summary>
        /// 生命周期：获取方法Attribute之后
        /// </summary>
        /// <param name="context">Http上下文对象</param>
        public virtual void AfterGetMethodAttribute(HttpContext context)
        {
        }
    }
}
