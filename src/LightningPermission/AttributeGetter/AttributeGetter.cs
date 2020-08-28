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

        public static Type GetMethodAttributes(Type StartupType, HttpContext context, RequestDelegate next, bool IsControllerAllow, Func<HttpContext, Permission, RequestDelegate, Boolean, Task<Boolean>> WillDoFunc, out bool IsActionAllow)
        {
            Type ActionType = null;
            bool IsAllow = false;
            IsActionAllow = false;
            var assembly = StartupType.Assembly.GetTypes().AsEnumerable()
            .Where(type => typeof(ControllerBase).IsAssignableFrom(type)).ToList();
            assembly.ForEach(async r =>
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

        public static Type GetMethodAttributesAfterAsync(Type StartupType, HttpContext context, Type ControllerType, RequestDelegate next, bool IsControllerAllow, Func<HttpContext, Permission, RequestDelegate, Boolean, Task<Boolean>> WillDoFunc, out bool IsActionAllow)
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
                                Console.WriteLine("IsAllow" + IsAllow);
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
