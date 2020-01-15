using System;
using System.Collections.Generic;
using System.Text;
using NServiceBus;

namespace Messages
{
    public class OrderPlaced : IEvent
    {

        public string OrderId { get; set; }
    }
}
