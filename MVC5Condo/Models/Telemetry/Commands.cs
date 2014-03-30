using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MVC5Condo.Models.Telemetry
{
    public class CommandBase
    {
        public string Command;
        public string Parameter;

        public virtual string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}