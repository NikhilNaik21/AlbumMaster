using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSDataLayers.BLL
{
    public class ClientResponse
    {
        public string Message { get; set; }        // ✅ property
        public bool IsSuccess { get; set; }        // ✅ property  
        public string ExceptionDetail { get; set; } // ✅ property

    }
}
