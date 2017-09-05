﻿using System;
using Core.UseCases;
using RabbitMQ.Client;

namespace Adapter.Trigger.RabbitMq
{
    public class TriggerAdapter
    {
        private OrderBookUseCaseTrigger _orderBookUseCaseTrigger;
        private bool _initialized;
        private IConnection _connection;

        public void Initialize()
        {
            var factory = new ConnectionFactory() { HostName = "127.0.0.1" };
            _connection = factory.CreateConnection();

            DeclareExchange();
            DeclareQueue();
            BindQueueToExchange();

            _initialized = true;
        }

        private void BindQueueToExchange()
        {
            using (var channel = _connection.CreateModel())
            {
                channel.QueueBind("bookrequest", "bookorder", "bookrequest");
            }
        }

        private void DeclareQueue()
        {
            using (var channel = _connection.CreateModel())
            {
                channel.QueueDeclare("bookrequest", durable:false, exclusive: false, autoDelete:true);
            }
        }

        public void Handle(OrderBookUseCase orderBookUseCase)
        {
            if (!_initialized)
                throw new InvalidOperationException("Adapter must be initialized prior to use");

            _orderBookUseCaseTrigger = new OrderBookUseCaseTrigger(orderBookUseCase, _connection);

            _orderBookUseCaseTrigger.Start();
        }

        private void DeclareExchange()
        {            
            using (var channel = _connection.CreateModel())
            {
                channel.ExchangeDeclare("bookorder", ExchangeType.Topic,
                    durable: false, autoDelete: true);
            }                                
        }

        public void Shutdown()
        {            
            _orderBookUseCaseTrigger.Stop();
            _connection.Dispose();
        }
    }
}
