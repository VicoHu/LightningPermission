using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;

namespace LightningPermission
{
    internal class DefaultOperation : IOperation
    {
        /// <summary>
        /// 无法访问时的提示信息
        /// </summary>
        protected static string AlertMessage = "Sorry, You Don't Have Permission To Access Here";

        /// <summary>
        /// Http上下文对象
        /// </summary>
        protected HttpContext context;

        /// <summary>
        /// Permission对象实例
        /// </summary>
        protected Permission permission;

        /// <summary>
        /// 管道下一个中间件对象实例
        /// </summary>
        protected RequestDelegate next;

        /// <summary>
        /// 权限字符串
        /// </summary>
        protected string RoleStr = null;

        public DefaultOperation(HttpContext context, Permission permission, RequestDelegate next, string RoleStr)
        {
            this.context = context;
            this.permission = permission;
            this.next = next;
            this.RoleStr = RoleStr;
        }

        /// <summary>
        /// Controller层访问权限检查
        /// </summary>
        /// <returns>是否有权限访问</returns>
        public async Task<Boolean> OnControllerCheck()
        {
            if (permission.Method == Permission.CheckMethod.Allow)
            {
                // 只允许模式
                if (((IList)permission.AllowRoles).Contains(RoleStr))
                {
                    // 如果权限标识在允许的权限列表内
                    return true;
                }
                else
                {
                    // 如果权限标识不在允许的权限列表内
                    await Response_403(context, next);
                    return false;
                }
            }
            else if (permission.Method == Permission.CheckMethod.DisAllow)
            {
                // 不允许模式
                if (((IList)permission.DisAllowRoles).Contains(RoleStr))
                {
                    // 如果权限标识在 不允许的权限列表内
                    await Response_403(context, next);
                    return false;
                }
                else
                {
                    // 如果权限标识不在不允许的权限列表内
                    return true;
                }
            }
            else
            {
                await Response_403(context, next);
                return false;
            }
        }

        /// <summary>
        /// Action层访问访问权限检查
        /// </summary>
        /// <param name="IsControllerAllow"></param>
        /// <returns>是否有权限访问</returns>
        public async Task<Boolean> OnActionCheck(bool IsControllerAllow)
        {
            if (!IsControllerAllow)
            {
                return false;
            }
            if (permission.Method == Permission.CheckMethod.Allow)
            {
                // 只允许模式
                if (((IList)permission.AllowRoles).Contains(RoleStr))
                {
                    // 如果权限标识在允许的权限列表内
                    return true;
                }
                else
                {
                    // 如果权限标识不在允许的权限列表内
                    await Response_403(context, next);
                    return false;
                }
            }
            else if (permission.Method == Permission.CheckMethod.DisAllow)
            {
                // 不允许模式
                if (((IList)permission.DisAllowRoles).Contains(RoleStr))
                {
                    // 如果权限标识在不允许的权限列表内
                    await Response_403(context, next);
                    return false;
                }
                else
                {
                    // 如果权限标识不在不允许的权限列表内
                    return true;
                }
            }
            else
            {
                await Response_403(context, next);
                return false;
            }

        }

        /// <summary>
        /// 无权限访问时，默认的返回信息
        /// </summary>
        /// <param name="context">Http上下文对象</param>
        /// <param name="next">管道下一个中间件的对象实例</param>
        private static async Task Response_403(HttpContext context, RequestDelegate next)
        {
            context.Response.ContentType = "application/json";
            // 设置内容编码格式为JSON
            context.Response.StatusCode = 403;
            // 设置Http状态码为403
            await context.Response.Body.WriteAsync(Encoding.Default.GetBytes("{\"status\":\"403\",\"msg\":\"" + AlertMessage + "\"}"));
            // 设置返回的信息
        }

    }
}
