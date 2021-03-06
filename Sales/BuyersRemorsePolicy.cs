﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Messages;
using NServiceBus;
using NServiceBus.Logging;

namespace Sales
{
    class BuyersRemorsePolicy : Saga<BuyersRemorseState>,
        IAmStartedByMessages<PlaceOrder>,
        IHandleMessages<CancelOrder>,
        IHandleTimeouts<BuyersRemorseIsOver>
    {
        static ILog log = LogManager.GetLogger<BuyersRemorsePolicy>();

        public async Task Handle(PlaceOrder message, IMessageHandlerContext context)
        {
            log.Info($"Received PlaceOrder, OrderId = {message.OrderId}");
            Data.OrderId = message.OrderId;

            log.Info($"Starting cool down period for order #{Data.OrderId}.");
            await RequestTimeout(context, TimeSpan.FromSeconds(20), new BuyersRemorseIsOver());
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<BuyersRemorseState> mapper)
        {
            mapper.ConfigureMapping<PlaceOrder>(p => p.OrderId).ToSaga(s => s.OrderId);
            mapper.ConfigureMapping<CancelOrder>(p => p.OrderId).ToSaga(s => s.OrderId);
        }

        public async Task Timeout(BuyersRemorseIsOver timeout, IMessageHandlerContext context)
        {
            log.Info($"Cooling down period for order #{Data.OrderId} has elapsed.");

            var orderPlaced = new OrderPlaced
            {
                OrderId = Data.OrderId
            };

            // TODO: Save order state in database?

            await context.Publish(orderPlaced);

            MarkAsComplete();
        }

        public Task Handle(CancelOrder message, IMessageHandlerContext context)
        {
            log.Info($"Order #{message.OrderId} was cancelled.");

            //TODO: Possibly publish an OrderCancelled event?

            MarkAsComplete();

            return Task.CompletedTask;
        }
    }

    class BuyersRemorseIsOver
    {
    }


}
