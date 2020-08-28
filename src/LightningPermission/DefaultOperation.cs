using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightningPermission
{
    internal class DefaultOperation : UserOperation
    {
        private static string AlertMessage = "Sorry, You Don't Have Permission To Access Here";

        protected HttpContext context;
        protected Permission permission;
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

        public override async Task<Boolean> OnControllerCheck()
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

        public override async Task<Boolean> OnMethedCheck(bool IsControllerAllow)
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

        private static async Task Response_403(HttpContext context, RequestDelegate next)
        {

            context.Response.ContentType = "application/json";
            // 设置内容编码格式为JSON
            context.Response.StatusCode = 403;
            // 设置Http状态码为403
            await context.Response.Body.WriteAsync(Encoding.Default.GetBytes("{\"status\":\"403\",\"msg\":\"" + AlertMessage + "\"}"));
            //await next.Invoke(context);
        }

    }
}
