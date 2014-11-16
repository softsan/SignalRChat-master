using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRDemo.ViewModel
{
    public class MessageViewModel
    {
        public int MessageId { get; set; }
        public string Message { get; set; }
        public string UserName { get; set; }
    }
}