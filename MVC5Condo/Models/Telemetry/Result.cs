using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MVC5Condo.Models.Telemetry
{
    public class ResultBase
    {
        public string STATUS;
        public string When;
        public string Code;
        public string Msg;
        public string Description;

        public virtual ResultBase Deserialize(string JsonStr)
        {
            return JsonConvert.DeserializeObject<ResultBase>(JsonStr);
        }
    }
}