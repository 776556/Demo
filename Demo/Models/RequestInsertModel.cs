using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Demo.Models
{
    public class RequestInsertModel
    {
        public RequestInsertHeadModel head { get; set; }
        public List<RequestInsertBodyModel> body { get; set; }
    }
}