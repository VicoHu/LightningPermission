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
                // 判断是否获得到ltoken
                TokenStore tokenStore = new TokenStore(this.ConnectionString);
                // 获得当前token字符串的权限字符串
                this.RoleStr = tokenStore.GetRoleByTokenStr(context.Request.Headers["ltoken"]);
            }
            else
            {
                // 尝试在query中uid
                if (context.Request.Query.ContainsKey("uid"))
                {
                    // 判断是否获得到uid
                    TokenStore tokenStore = new TokenStore(this.ConnectionString);
                    // 获得当前token字符串的权限字符串
                    this.RoleStr = tokenStore.GetRoleByUid(context.Request.Query["uid"]);
                }
            }
            return this.RoleStr;
        }

        /// <summary>
        /// 生命周期：获取控制器Attribute，并进行操作
        /// </summary>
        /// <param name="context">Http上下文对象</param>
        /// <param name="StartupType">Startup的Type对象</param>
        public virtual async Task<bool> OnGetControllerAttribute(HttpContext context, Permission permission, RequestDelegate next)
        {
            if (permission != null)
            {
                //Console.WriteLine("*******Controller*******");
                //await next.Invoke(context);
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
        public virtual async Task<Boolean> OnGetMethodAttribute(HttpContext context, Permission permission, RequestDelegate next, bool IsControllerAllow)
        {
            if (permission != null)
            {
                //Console.WriteLine("*******Method*******");
                //await next.Invoke(context);

                DefaultOperation operation = new DefaultOperation(context, permission, next, this.RoleStr);
                return await operation.OnMethedCheck(IsControllerAllow);
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

        /// <summary>
        /// 检测到接口有权限控制后进行的主要操作
        /// </summary>
        /// <param name="context">Http上下文对象</param>
        /// <param name="permission">permission实例</param>
        //private async Task<Boolean> MainControl(HttpContext context, Permission permission, RequestDelegate next)
        //{
        //    Console.WriteLine("HasStarted: " + context.Response.HasStarted);
        //    //if (context.Response.HasStarted)
        //    //{
        //    //    await next.Invoke(context);
        //    //}

        //    bool status = true;

        //    Console.WriteLine("this.RoleStr: " + this.RoleStr);
        //    Console.WriteLine("permission.Mothod: " + permission.Method);
        //    Console.WriteLine("permission.AllowRoles: " + permission.AllowRoles);
        //    Console.WriteLine("permission.DisAllowRoles: " + permission.DisAllowRoles);

        //    // 判断是否存在于被拒绝的权限列表内，存在则返回403
        //    if (this.RoleStr != null && permission.Method == Permission.CheckMethod.DisAllow && ((IList)permission.DisAllowRoles).Contains(this.RoleStr))
        //    {
        //        Console.WriteLine(1);
        //        context.Response.ContentType = "application/json";
        //        // 设置内容编码格式为JSON
        //        context.Response.StatusCode = 403;
        //        // 设置Http状态码为403
        //        await context.Response.Body.WriteAsync(Encoding.Default.GetBytes("{\"status\":\"403\",\"msg\":\"" + this.AlertMessage + "\"}"));
        //        // 异步写入JSON内容
        //        status = false;
        //    }
        //    else if (this.RoleStr != null && permission.Method == Permission.CheckMethod.DisAllow && !((IList)permission.DisAllowRoles).Contains(this.RoleStr))
        //    {
        //        Console.WriteLine(2);
        //        await next.Invoke(context);
        //        // 终止当前中间件，进入下一个中间件
        //    }
        //    else
        //    {
        //        // 判断是否存在于被允许的权限列表内，不存在则返回403
        //        if (permission.AllowRoles == null || this.RoleStr == null || !((IList)permission.AllowRoles).Contains(this.RoleStr))
        //        {
        //            Console.WriteLine(3);
        //            context.Response.ContentType = "application/json";
        //            // 设置内容编码格式为JSON
        //            context.Response.StatusCode = 403;
        //            // 设置Http状态码为403
        //            await context.Response.Body.WriteAsync(Encoding.Default.GetBytes("{\"status\":\"403\",\"msg\":\"" + this.AlertMessage + "\"}"));
        //            // 异步写入JSON内容
        //            Console.WriteLine("AfterHasStarted: " + context.Response.HasStarted);
        //            status = false;
        //        }
        //        else if (permission.AllowRoles != null && ((IList)permission.AllowRoles).Contains(this.RoleStr))
        //        {
        //            Console.WriteLine(4);
        //            await next.Invoke(context);
        //            // 终止当前中间件，进入下一个中间件
        //        }
        //        else
        //        {
        //            Console.WriteLine(5);
        //            await next.Invoke(context);
        //            // 终止当前中间件，进入下一个中间件
        //        }
        //    }
        //    //await next.Invoke(context);
        //    return status;
        //}
    }
}
