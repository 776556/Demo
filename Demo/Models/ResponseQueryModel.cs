using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Demo.Models
{
    public class ResponseQueryModel
    {
        public NoetHeadModel Head { get; set; }

        public List<NoetBodyModel> Body { get; set; }
    }
}