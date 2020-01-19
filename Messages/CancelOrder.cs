using System;
using System.Collections.Generic;
using System.Text;
using NServiceBus;

namespace Messages
{
    public class CancelOrder
        : ICommand
    {
        public string OrderId { get; set; }
    }
}
