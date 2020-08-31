/*
 * Author: VicoHu
 * CreateTime: 2020/8/13
 * Email: vicohu@163.com
 */
using System;

namespace LightningPermission
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class Permission : Attribute
    {
        /// <summary>
        /// 检测权限的方式 (只允许 或 只不允许)
        /// </summary>
        public enum CheckMethod
        {
            /// <summary>
            /// 只允许
            /// </summary>
            Allow,
            /// <summary>
            /// 只不允许
            /// </summary>
            DisAllow
        }
        /// <summary>
        /// 允许的权限字符串数组
        /// </summary>
        public string[] AllowRoles = null;

        /// <summary>
        /// 不允许的权限字符串数组
        /// </summary>
        public string[] DisAllowRoles = null;

        /// <summary>
        /// 检测权限的方式
        /// </summary>
        public CheckMethod Method;

        /// <summary>
        /// 默认为 只允许 的检测模式
        /// </summary>
        /// <param name="AllowRoles">允许的权限</param>
        public Permission(string[] AllowRoles)
        {
            this.Method = CheckMethod.Allow;
            this.AllowRoles = AllowRoles;
        }

        /// <summary>
        /// 不做任何设置的Permission特性，需要传入检测模式 Method，以及AllowRoles和DisAllowRoles的其中一个字符串 数组
        /// </summary>
        public Permission()
        {
        }
    }
}
