using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Demo.Models
{
    public class RequestUpdateModel
    {
        public NoetHeadModel head { get; set; }
        public List<NoetBodyModel> body { get; set; }
    }
}