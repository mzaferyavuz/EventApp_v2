using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventApplication.Models
{
    public enum JsonResultType : int
    {
        Success = 1,
        Error = 2
    }


    public class JsonResultModel<T>
    {
        public string Message { get; set; }
        public JsonResultType Status { get; set; }
        public IList<T> Data { get; set; }
    }
}