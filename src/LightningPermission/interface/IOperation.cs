using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LightningPermission
{
    public interface IOperation
    {
        /// <summary>
        /// Controller层访问权限检查
        /// </summary>
        /// <returns>是否有权限访问</returns>
        public Task<Boolean> OnControllerCheck();

        /// <summary>
        /// Action层访问访问权限检查
        /// </summary>
        /// <param name="IsControllerAllow"></param>
        /// <returns>是否有权限访问</returns>
        public Task<Boolean> OnActionCheck(bool IsControllerAllow);
    }
}
