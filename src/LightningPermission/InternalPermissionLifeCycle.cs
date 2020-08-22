using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LightningPermission
{
    internal class InternalPermissionLifeCycle
    {
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
        private Func<Task> next;

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
        public InternalPermissionLifeCycle(IPermissionLifeCycle CustomLifeCycle, Type StartupType, HttpContext context, Func<Task> next)
        {
            this.CustomLifeCycle = CustomLifeCycle;
            this.StartupType = StartupType;
            this.context = context;
            this.next = next;
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
        public virtual void OnGetMethodAttribute()
        {
            if (this.ControllerType != null)
            {
                this.ActionType = AttributeGetter.AttributeGetter.GetMethodAttributesAfter(this.StartupType, this.context, this.ControllerType, this.next, CustomLifeCycle.OnGetMethodAttribute);
            }
            else
            {
                this.ActionType = AttributeGetter.AttributeGetter.GetMethodAttributes(this.StartupType, this.context, this.next, CustomLifeCycle.OnGetMethodAttribute);
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
        /// 生命周期：获取控制器Attribute之前
        /// </summary>
        public virtual void BeforeGetControllerAttribute()
        {
            CustomLifeCycle.BeforeGetControllerAttribute(this.context);
        }

        /// <summary>
        /// 生命周期：获取控制器Attribute，并进行操作
        /// </summary>
        public virtual void OnGetControllerAttribute()
        {
            this.ControllerType = AttributeGetter.AttributeGetter.GetControllerAttributes(this.StartupType, this.context, this.next, CustomLifeCycle.OnGetControllerAttribute);
        }

        /// <summary>
        /// 生命周期：获取控制器Attribute并进行才操作之后
        /// </summary>
        public virtual void AfterGetControllerAttribute()
        {
            CustomLifeCycle.AfterGetControllerAttribute(this.context);
        }

        /// <summary>
        /// 按顺序运行生命周期函数
        /// </summary>
        public void RunLifeCycle()
        {
            // 获取控制器Attribute之前
            this.BeforeGetControllerAttribute();
            // 对获取的控制器Attribute进行操作
            this.OnGetControllerAttribute();
            // 获取控制器Attribute之后
            this.AfterGetControllerAttribute();
            // 获取方法Attribute之前
            this.BeforeGetMethodAttribute();
            // 获取方法Attribute, 并进行操作
            this.OnGetMethodAttribute();
            // 获取方法Attribute之后
            this.AfterGetMethodAttribute();
        }
    }
}
