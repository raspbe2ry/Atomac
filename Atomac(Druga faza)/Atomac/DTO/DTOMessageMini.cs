using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atomac.DTO
{
    public class DTOMessageMini
    {
        public string Text { get; set; }
        public bool LinkTag { get; set; }
        public string Time { get; set; }
        public string Sender { get; set; }
    }
}