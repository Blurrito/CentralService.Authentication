using CentralService.Authentication.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralService.Authentication.Core.Factories
{
    public static class AuthenticationManagerFactory
    {
        public static IAuthenticationManager GetManager() => new AuthenticationManager();
    }
}
