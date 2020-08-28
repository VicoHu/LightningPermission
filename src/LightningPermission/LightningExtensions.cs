using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LightningPermission
{
    public static class LightningExtensions
    {
        /// <summary>
        /// 使用用户自定义行为的Lightning身份验证插件
        /// </summary>
        /// <param name="builder">IApplicationBuilder实例</param>
        /// <param name="StartupType">Startup的Type对象</param>
        /// <param name="ConnectionString">数据库连接字符串（暂时仅支持SqlServer）</param>
        /// <param name="permissionLifeCycle">传入用户自定义的生命周期</param>
        /// <returns></returns>
        public static IApplicationBuilder UseLightningPermission(this IApplicationBuilder builder, Type StartupType, string ConnectionString, IPermissionLifeCycle permissionLifeCycle)
        {
            return builder.UseMiddleware<LightningMiddleware>(StartupType, ConnectionString, permissionLifeCycle);
        }

        /// <summary>
        /// 使用官方配置的Lightning身份验证插件
        /// </summary>
        /// <param name="builder">IApplicationBuilder实例</param>
        /// <param name="StartupType">Startup的Type对象</param>
        /// <param name="ConnectionString">数据库连接字符串（暂时仅支持SqlServer）</param>
        /// <returns>IApplicationBuilder实例</returns>
        public static IApplicationBuilder UseLightningPermission(this IApplicationBuilder builder, Type StartupType, string ConnectionString)
        {
            return builder.UseMiddleware<LightningMiddleware>(StartupType, ConnectionString);
        }


    }
}
