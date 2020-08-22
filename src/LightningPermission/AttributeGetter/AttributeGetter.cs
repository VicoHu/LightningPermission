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

        public static Type GetControllerAttributes(Type StartupType, HttpContext context, Func<Task> next, Func<HttpContext, Permission, Func<Task>, Boolean> WillDoFunc)
        {
            Type ControllerType = null;
            var assembly = StartupType.Assembly.GetTypes().AsEnumerable()
            .Where(type => typeof(ControllerBase).IsAssignableFrom(type)).ToList();
            assembly.ForEach(d =>
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
                            WillDoFunc(context, PermissionAuthorize, next);
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            });
            return ControllerType;
        }

        public static Type GetMethodAttributes(Type StartupType, HttpContext context, Func<Task> next, Func<HttpContext, Permission, Func<Task>, Boolean> WillDoFunc)
        {
            Type ActionType = null;
            var assembly = StartupType.Assembly.GetTypes().AsEnumerable()
            .Where(type => typeof(ControllerBase).IsAssignableFrom(type)).ToList();
            assembly.ForEach(r =>
            {
                foreach (var methodInfo in r.GetMethods())
                {
                    foreach (Attribute attribute in methodInfo.GetCustomAttributes())
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
                                    WillDoFunc(context, PermissionAuthorize, next);
                                }
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                        }
                    }
                }
            });
            return ActionType;
        }

        public static Type GetMethodAttributesAfter(Type StartupType, HttpContext context, Type ControllerType, Func<Task> next, Func<HttpContext, Permission, Func<Task>, Boolean> WillDoFunc)
        {
            Type ActionType = null;
            foreach (var methodInfo in ControllerType.GetMethods())
            {
                foreach (Attribute attribute in methodInfo.GetCustomAttributes())
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
                                WillDoFunc(context, PermissionAuthorize, next);
                            }
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                }
            }
            return ActionType;
        }
    }
}
