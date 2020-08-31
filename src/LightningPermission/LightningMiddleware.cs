using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LightningPermission
{
    public class LightningMiddleware
    {
        /// <summary>
        /// 
        /// </summary>
        private RequestDelegate next;

        /// <summary>
        /// Startup的Type对象实例
        /// </summary>
        private Type StartupType;
        
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private string ConnectionString;

        /// <summary>
        /// 自定义生命周期
        /// </summary>
        private PermissionLifeCycle permissionLifeCycle = null;

        /// <summary>
        /// 使用官方配置的Lightning身份验证插件
        /// </summary>
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
        /// <param name="StartupType">Startup的Type对象</param>
        /// <param name="ConnectionString">数据库连接字符串（暂时仅支持SqlServer）</param>
        /// <param name="permissionLifeCycle">传入用户自定义的生命周期</param>
        public LightningMiddleware(RequestDelegate next, Type StartupType, string ConnectionString, PermissionLifeCycle permissionLifeCycle)
        {
            this.next = next;
            this.StartupType = StartupType;
            this.ConnectionString = ConnectionString;
            this.permissionLifeCycle = permissionLifeCycle;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 声明内部内部生命周期
            InternalPermissionLifeCycle InternalLifeCycle = null;

            // 判断初始化时，是否初始化了自定义生命周期
            if (this.permissionLifeCycle == null)
            {
                // 如果没有，则使用默认的自定义生命周期，初始化内部生命周期
                InternalLifeCycle = new InternalPermissionLifeCycle(new PermissionLifeCycle(this.ConnectionString), this.StartupType, context, this.next);
            }
            else
            {
                // 如果有，则使用用户传入的自定义生命周期，初始化内部生命周期
                InternalLifeCycle = new InternalPermissionLifeCycle(this.permissionLifeCycle, StartupType, context, next);
            }
            // 运行生命周期，并判断是否有权限进入访问接口
            if (await InternalLifeCycle.RunLifeCycle())
            {
                // 运行下一个中间件
                await next(context);
            }
        }
    }
}
