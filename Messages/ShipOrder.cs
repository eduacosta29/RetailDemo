using System;
using System.Collections.Generic;
using System.Text;
using NServiceBus;

namespace Messages
{
    public class ShipOrder : ICommand
    {
        public string OrderId { get; set; }
    }
}
