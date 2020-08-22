using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LightningPermission.Models;

namespace LightningPermission
{
    public class PermissionLifeCycle : IPermissionLifeCycle
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private string ConnectionString;

        public PermissionLifeCycle(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
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
        public virtual bool OnGetMethodAttribute(HttpContext context, Permission permission, Func<Task> next)
        {
            this.MainControl(context, permission,next);
            return true;
        }

        /// <summary>
        /// 生命周期：获取方法Attribute之后
        /// </summary>
        /// <param name="context">Http上下文对象</param>
        public virtual void AfterGetMethodAttribute(HttpContext context)
        {
        }

        /// <summary>
        /// 生命周期：获取控制器Attribute之前
        /// </summary>
        /// <param name="context">Http上下文对象</param>
        public virtual void BeforeGetControllerAttribute(HttpContext context)
        {
            using (var db = new PermissionTokenContext(this.ConnectionString))
            {
                // 检测是否存在数据库，如果存在，则不作操作，如果 不存在，则生成数据库
                db.Database.EnsureCreated();
            }
        }

        /// <summary>
        /// 生命周期：获取控制器Attribute，并进行操作
        /// </summary>
        /// <param name="context">Http上下文对象</param>
        /// <param name="StartupType">Startup的Type对象</param>
        public virtual bool OnGetControllerAttribute(HttpContext context, Permission permission, Func<Task> next)
        {
            this.MainControl(context, permission,next);
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
        /// 检测到接口有权限控制后进行的主要操作
        /// </summary>
        /// <param name="context">Http上下文对象</param>
        /// <param name="permission">permission实例</param>
        private async void MainControl(HttpContext context, Permission permission, Func<Task> next)
        {
            TokenStore tokenStore = new TokenStore(this.ConnectionString);
            string role = tokenStore.GetRoleByTokenStr(context.Request.Headers["ltoken"]);
            if (role == null || !((IList)permission.AllowRoles).Contains(role))
            {
                context.Response.ContentType = "application/json";
                // 设置内容编码格式为JSON
                context.Response.StatusCode = 403;
                // 设置Http状态码为403
                await context.Response.Body.WriteAsync(Encoding.Default.GetBytes("{\"status\":\"403\",\"msg\":\"Sorry, You Don't Have Permission To Access Here\"}"));
                // 异步写入JSON内容
            }
            else {
                await next.Invoke();
                // 进入下一个中间件
            }
        }
    }
}
