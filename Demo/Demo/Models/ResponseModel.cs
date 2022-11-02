using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Demo.Models
{
    public class ResponseModel<T>
    {
        public int Status { get; set; }
        public T retObj { get; set; }
        public string errMsg { get; set; }
    }
}