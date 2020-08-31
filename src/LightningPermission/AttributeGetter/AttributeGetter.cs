using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LightningPermission.AttributeGetter
{
    public class AttributeGetter
    {
        /// <summary>
        /// 获得控制器特性，并运行自定义的操作
        /// </summary>
        /// <param name="StartupType">Startup的Type对象</param>
        /// <param name="context">Http上下文对象</param>
        /// <param name="next">管道下一个中间件的实例对象</param>
        /// <param name="WillDoFunc">将要运行的自定义方法</param>
        /// <param name="IsControllerAllow">（传出）控制器是否允许通过</param>
        /// <returns>该控制器的Permission的Type对象</returns>
        public static Type GetControllerAttributes(Type StartupType, HttpContext context, RequestDelegate next, Func<HttpContext, Permission, RequestDelegate, Task<Boolean>> WillDoFunc, out bool IsControllerAllow)
        {
            Type ControllerType = null;
            bool IsAllow = true;
            IsControllerAllow = false;
            var assembly = StartupType.Assembly.GetTypes().AsEnumerable()
            .Where(type => typeof(ControllerBase).IsAssignableFrom(type)).ToList();
            assembly.ForEach(async d =>
            {

                var PermissionAuthorize = d.GetCustomAttribute<Permission>();
                if (PermissionAuthorize != null)
                {
                    try
                    {
                        string TargetControllerName = context.Request.RouteValues["controller"] + "Controller";
                        // 获得当前访问控制器的名字
                        if (d.Name == TargetControllerName)
                        {
                            ControllerType = d;
                            IsAllow = await WillDoFunc(context, PermissionAuthorize, next);
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            });
            IsControllerAllow = IsAllow;
            return ControllerType;
        }

        /// <summary>
        /// 获得方法的特性，并运行自定义的操作
        /// </summary>
        /// <param name="StartupType">Startup的Type对象</param>
        /// <param name="context">Http上下文对象</param>
        /// <param name="next">管道下一个中间件的实例对象</param>
        /// <param name="IsControllerAllow">该Action所在控制器是否允许访问</param>
        /// <param name="WillDoFunc">将要运行的自定义方法</param>
        /// <param name="IsActionAllow">（传出）该Action是否允许通过</param>
        /// <returns>该Action的Permission的Type对象</returns>
        public static Type GetMethodAttributes(Type StartupType, HttpContext context, RequestDelegate next, bool IsControllerAllow, Func<HttpContext, Permission, RequestDelegate, Boolean, Task<Boolean>> WillDoFunc, out bool IsActionAllow)
        {
            Type ActionType = null;
            bool IsAllow = false;
            IsActionAllow = false;
            var assembly = StartupType.Assembly.GetTypes().AsEnumerable()
            .Where(type => typeof(ControllerBase).IsAssignableFrom(type)).ToList();
            assembly.ForEach(r =>
            {
                foreach (var methodInfo in r.GetMethods())
                {
                    methodInfo.GetCustomAttributes().ToList().ForEach(async attribute =>
                    {
                        if (attribute is Permission PermissionAuthorize)
                        {
                            string TargetControllerName;
                            // 获得当前访问控制器的名字
                            string TargetMethodName;
                            // 获得当前访问的Action的名字
                            try
                            {
                                TargetControllerName = context.Request.RouteValues["controller"] + "Controller";
                                // 获得当前访问控制器的名字
                                TargetMethodName = context.Request.RouteValues["action"] + "";
                                // 获得当前访问的Action的名字
                                if (TargetControllerName == r.Name && TargetMethodName == methodInfo.Name)
                                {
                                    ActionType = methodInfo.GetType();
                                    IsAllow = await WillDoFunc(context, PermissionAuthorize, next, IsControllerAllow);
                                }
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                        }
                    });
                }
            });
            IsActionAllow = IsAllow;
            return ActionType;
        }

        /// <summary>
        /// 获得Action的特性，并运行自定义的操作（异步）
        /// </summary>
        /// <param name="StartupType">Startup的Type对象</param>
        /// <param name="context">Http上下文对象</param>
        /// <param name="next">管道下一个中间件的实例对象</param>
        /// <param name="IsControllerAllow">该Action所在控制器是否允许访问</param>
        /// <param name="WillDoFunc">将要运行的自定义异步方法</param>
        /// <param name="IsActionAllow">（传出）该Action是否允许通过</param>
        /// <returns>该Action的Permission的Type对象</returns>
        public static Type GetMethodAttributesAfterAsync(Type StartupType, HttpContext context, Type ControllerType, RequestDelegate next, bool IsControllerAllow, Func<HttpContext, Permission, RequestDelegate,  Boolean, Task<Boolean>> WillDoFunc, out bool IsActionAllow)
        {
            Type ActionType = null;
            bool IsAllow = false;
            IsActionAllow = false;
            foreach (var methodInfo in ControllerType.GetMethods())
            {
                methodInfo.GetCustomAttributes().ToList().ForEach(async attribute =>
                {
                    if (attribute is Permission PermissionAuthorize)
                    {
                        try
                        {
                            string TargetControllerName = context.Request.RouteValues["controller"] + "Controller";
                            // 获得当前访问控制器的名字
                            string TargetMethodName = context.Request.RouteValues["action"].ToString();
                            // 获得当前访问的Action的名字
                            if (TargetControllerName == ControllerType.Name && TargetMethodName == methodInfo.Name)
                            {
                                ActionType = methodInfo.GetType();
                                IsAllow = await WillDoFunc(context, PermissionAuthorize, next, IsControllerAllow);
                                //Console.WriteLine("IsAllow" + IsAllow);
                            }
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                });
            }
            IsActionAllow = IsAllow;
            return ActionType;
        }
    }
}
