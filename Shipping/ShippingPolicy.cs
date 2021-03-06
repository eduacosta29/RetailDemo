﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Messages;
using NServiceBus;
using NServiceBus.Logging;

namespace Shipping
{
    public class ShippingPolicy : Saga<ShippingPolicyData>, IAmStartedByMessages<OrderPlaced>, IAmStartedByMessages<OrderBilled>
    {
        static ILog log = LogManager.GetLogger<ShippingPolicy>();

        public Task Handle(OrderBilled message, IMessageHandlerContext context)
        {
            log.Info($"Received OrderBilled, OrderId = {message.OrderId}");
            Data.IsOrderBilled = true;
            return ProcessOrder(context);
        }

        public Task Handle(OrderPlaced message, IMessageHandlerContext context)
        {
            log.Info($"Received OrderPlaced, OrderId = {message.OrderId}");
            Data.IsOrderPlaced = true;
            return ProcessOrder(context);
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ShippingPolicyData> mapper)
        {
            mapper.ConfigureMapping<OrderPlaced>(message => message.OrderId)
                  .ToSaga(sagadata => sagadata.OrderId);
            mapper.ConfigureMapping<OrderBilled>(message => message.OrderId)
                  .ToSaga(sagaData => sagaData.OrderId);
        }

        private async Task ProcessOrder(IMessageHandlerContext context)
        {
            if (Data.IsOrderPlaced && Data.IsOrderBilled)
            {
                await context.SendLocal(new ShipOrder() { OrderId = Data.OrderId });
                MarkAsComplete();
            }
        }
    }
}
