using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LightningPermission
{
    public interface IPermissionLifeCycle
    {
        public void BeforeGetControllerAttribute(HttpContext context);

        public bool OnGetControllerAttribute(HttpContext context, Permission permission, Func<Task> next);

        public void AfterGetControllerAttribute(HttpContext context);

        public void BeforeGetMethodAttribute(HttpContext context);

        public bool OnGetMethodAttribute(HttpContext context, Permission permission, Func<Task> next);

        public void AfterGetMethodAttribute(HttpContext context);
    }
}
