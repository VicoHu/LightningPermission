using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LightningPermission
{
    public class LightningMiddleware
    {
        private RequestDelegate next;
        private Type StartupType;
        private string ConnectionString;
        private IPermissionLifeCycle permissionLifeCycle = null;

        /// <summary>
        /// 使用官方配置的Lightning身份验证插件
        /// </summary>
        /// <param name="builder">IApplicationBuilder实例</param>
        /// <param name="StartupType">Startup的Type对象</param>
        /// <param name="ConnectionString">数据库连接字符串（暂时仅支持SqlServer）</param>
        /// <returns>IApplicationBuilder实例</returns>
        public LightningMiddleware(RequestDelegate next, Type StartupType, string ConnectionString)
        {
            this.next = next;
            this.StartupType = StartupType;
            this.ConnectionString = ConnectionString;
        }

        /// <summary>
        /// 使用用户自定义行为的Lightning身份验证插件
        /// </summary>
        /// <param name="builder">IApplicationBuilder实例</param>
        /// <param name="StartupType">Startup的Type对象</param>
        /// <param name="ConnectionString">数据库连接字符串（暂时仅支持SqlServer）</param>
        /// <param name="permissionLifeCycle">传入用户自定义的生命周期</param>
        /// <returns></returns>
        public LightningMiddleware(RequestDelegate next, Type StartupType, string ConnectionString, IPermissionLifeCycle permissionLifeCycle)
        {
            this.next = next;
            this.StartupType = StartupType;
            this.ConnectionString = ConnectionString;
            this.permissionLifeCycle = permissionLifeCycle;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            InternalPermissionLifeCycle InternalLifeCycle = null;
            if (permissionLifeCycle == null)
            {
                //初始化生命周期
                InternalLifeCycle = new InternalPermissionLifeCycle(new PermissionLifeCycle(this.ConnectionString), this.StartupType, context, this.next);
            }
            else
            {
                // 初始化生命周期
                InternalLifeCycle = new InternalPermissionLifeCycle(new PermissionLifeCycle(ConnectionString), StartupType, context, next);
            }
            // 运行生命周期
            if (await InternalLifeCycle.RunLifeCycle())
            {
                // 运行下一个中间件
                await next(context);
            }
        }
    }
}
