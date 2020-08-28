using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LightningPermission
{
    public abstract class UserOperation
    {
        public abstract Task<Boolean> OnControllerCheck();

        public abstract Task<Boolean> OnMethedCheck(bool IsControllerAllow);
    }
}
