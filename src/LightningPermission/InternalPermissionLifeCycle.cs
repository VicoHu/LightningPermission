using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace LightningPermission
{
    internal class InternalPermissionLifeCycle
    {
        /// <summary>
        /// controller层级是否允许了该权限，默认值为false
        /// </summary>
        private bool IsControllerAllow = false;

        /// <summary>
        /// Action层级是否允许了该权限，默认值为false
        /// </summary>
        private bool IsActionAllow = false;

        /// <summary>
        /// 权限字符串
        /// </summary>
        private string Role = null;

        /// <summary>
        /// Startup的Type对象
        /// </summary>
        private Type StartupType;

        /// <summary>
        /// 符合筛选条件的方法的Type对象
        /// </summary>
        private Type ActionType;

        /// <summary>
        /// 符合筛选条件的控制器Type对象
        /// </summary>
        private Type ControllerType;

        /// <summary>
        /// 管道控制方法
        /// </summary>
        private RequestDelegate next;

        /// <summary>
        /// Http上下文对象
        /// </summary>
        private HttpContext context;

        // 用户传入的生命周期
        private IPermissionLifeCycle CustomLifeCycle;

        /// <summary>
        /// 初始化内部生命周期
        /// </summary>
        /// <param name="CustomLifeCycle">用户自定义生命周期</param>
        /// <param name="StartupType">Startup的Type对象</param>
        public InternalPermissionLifeCycle(IPermissionLifeCycle CustomLifeCycle, Type StartupType, HttpContext context, RequestDelegate next)
        {
            this.CustomLifeCycle = CustomLifeCycle;
            this.StartupType = StartupType;
            this.context = context;
            this.next = next;
        }

        /// <summary>
        /// 生命周期：获取控制器Attribute之前
        /// </summary>
        public virtual void BeforeGetControllerAttribute()
        {
            this.Role = CustomLifeCycle.BeforeGetControllerAttribute(this.context);
        }

        /// <summary>
        /// 生命周期：获取控制器Attribute，并进行操作
        /// </summary>
        public virtual void OnGetControllerAttribute()
        {
            this.ControllerType = AttributeGetter.AttributeGetter.GetControllerAttributes(this.StartupType, this.context, this.next, CustomLifeCycle.OnGetControllerAttribute, out this.IsControllerAllow);
        }

        /// <summary>
        /// 生命周期：获取控制器Attribute并进行才操作之后
        /// </summary>
        public virtual void AfterGetControllerAttribute()
        {
            CustomLifeCycle.AfterGetControllerAttribute(this.context);
        }

        /// <summary>
        /// 生命周期：在检测方法的Attribute之前
        /// </summary>
        public virtual void BeforeGetMethodAttribute()
        {
            CustomLifeCycle.BeforeGetMethodAttribute(this.context);
        }

        /// <summary>
        /// 生命周期：正在对拥有Attribute的方法进行操作
        /// </summary>
        public virtual void OnGetActionAttribute()
        {
            //Console.WriteLine("IsControllerAllow: " + IsControllerAllow);
            if (this.IsControllerAllow)
            {
                if (this.ControllerType != null)
                {
                    this.ActionType = AttributeGetter.AttributeGetter.GetMethodAttributesAfterAsync(this.StartupType, this.context, this.ControllerType, this.next, this.IsControllerAllow, CustomLifeCycle.OnGetActionAttribute, out this.IsActionAllow);
                }
                else
                {
                    this.ActionType = AttributeGetter.AttributeGetter.GetMethodAttributes(this.StartupType, this.context, this.next, this.IsControllerAllow, CustomLifeCycle.OnGetActionAttribute, out this.IsActionAllow);
                }
            }
        }

        /// <summary>
        /// 生命周期：获取方法Attribute之后
        /// </summary>
        public virtual void AfterGetMethodAttribute()
        {
            CustomLifeCycle.AfterGetMethodAttribute(this.context);

        }

        /// <summary>
        /// 按顺序运行生命周期函数
        /// </summary>
        public async Task<Boolean> RunLifeCycle()
        {
            // 获取控制器Attribute之前
            this.BeforeGetControllerAttribute();
            //Console.WriteLine("BeforeGetControllerAttribute");

            // 对获取的控制器Attribute进行操作
            this.OnGetControllerAttribute();
            //Console.WriteLine("OnGetControllerAttribute");

            // 获取控制器Attribute之后
            this.AfterGetControllerAttribute();
            //Console.WriteLine("AfterGetControllerAttribute");

            // 获取方法Attribute之前
            this.BeforeGetMethodAttribute();
            //Console.WriteLine("BeforeGetMethodAttribute");

            // 获取方法Attribute, 并进行操作
            this.OnGetActionAttribute();
            //Console.WriteLine("OnGetMethodAttribute");

            // 获取方法Attribute之后
            this.AfterGetMethodAttribute();
            //Console.WriteLine("AfterGetMethodAttribute\n--------------------------------------");

            return this.IsControllerAllow && this.IsActionAllow;
            //await this.next.Invoke(context);
        }
    }
}
