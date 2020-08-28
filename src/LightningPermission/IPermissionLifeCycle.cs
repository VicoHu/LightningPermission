using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LightningPermission
{
    public interface IPermissionLifeCycle
    {
        public string BeforeGetControllerAttribute(HttpContext context);

        public Task<Boolean> OnGetControllerAttribute(HttpContext context, Permission permission, RequestDelegate next);

        public void AfterGetControllerAttribute(HttpContext context);

        public void BeforeGetMethodAttribute(HttpContext context);

        public Task<Boolean> OnGetMethodAttribute(HttpContext context, Permission permission, RequestDelegate next, bool IsControllerAllow);

        public void AfterGetMethodAttribute(HttpContext context);
    }
}
