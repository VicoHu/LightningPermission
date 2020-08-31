using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LightningPermission
{
    public interface IPermissionLifeCycle
    {
        /// <summary>
        /// 生命周期：获取控制器Attribute之前
        /// </summary>
        /// <param name="context">Http上下文对象</param>
        public string BeforeGetControllerAttribute(HttpContext context);

        /// <summary>
        /// 生命周期：获取控制器Attribute，并进行操作
        /// </summary>
        /// <param name="context">Http上下文对象</param>
        /// <param name="permission">该控制器上的Permission对象</param>
        /// <param name="next">管道下一中间件对象实例</param>
        /// <returns>是否通过Controller的权限检测</returns>
        public Task<Boolean> OnGetControllerAttribute(HttpContext context, Permission permission, RequestDelegate next);

        /// <summary>
        /// 生命周期：获取控制器Attribute并进行才操作之后
        /// </summary>
        /// <param name="context">Http上下文对象</param>
        public void AfterGetControllerAttribute(HttpContext context);

        /// <summary>
        /// 生命周期：在检测方法的Attribute之前
        /// </summary>
        /// <param name="context">Http上下文对象</param>
        public void BeforeGetMethodAttribute(HttpContext context);

        /// <summary>
        /// 生命周期：正在对拥有Attribute的方法进行操作
        /// </summary>
        /// <param name="context">Http上下文对象</param>
        /// <param name="permission">该控制器上的Permission对象</param>
        /// <param name="next">管道下一中间件对象实例</param>
        /// <returns>是否通过Action的权限检测</returns>
        public Task<Boolean> OnGetActionAttribute(HttpContext context, Permission permission, RequestDelegate next, bool IsControllerAllow);

        /// <summary>
        /// 生命周期：获取方法Attribute之后
        /// </summary>
        /// <param name="context">Http上下文对象</param>
        public void AfterGetMethodAttribute(HttpContext context);
    }
}
