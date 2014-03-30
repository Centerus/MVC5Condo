using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MVC5Condo.App_Auth
{
    public class CenterusAccountAuthenticationProvider : ICenterusAccountAuthenticationProvider
    {
        public async Task Authenticated(CenterusAccountAuthenticatedContext context)
        {
            await Task.Run(delegate() { return; }); // do nothing for now
        }

        public async Task ReturnEndpoint(CenterusAccountReturnEndpointContext context)
        {
            await Task.Run(delegate() { return; }); // do nothing for now
        }
    }
}