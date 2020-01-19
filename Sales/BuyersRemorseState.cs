using System;
using System.Collections.Generic;
using System.Text;
using NServiceBus;

namespace Sales
{
    public class BuyersRemorseState : ContainSagaData
    {
        public string OrderId { get; set; }
    }
}
