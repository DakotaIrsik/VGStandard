using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VGStandard.Common.Web.Responses
{
    public class ServiceStandardResponse
    {
        public int StatusCode { get; set; }

        public string ErrorMessage { get; set; }

        public object Result { get; set; }
    }
}
