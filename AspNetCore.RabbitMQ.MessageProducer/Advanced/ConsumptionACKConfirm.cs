using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AspNetCore.RabbitMQ.MessageProducer.Advanced
{
    public class ConsumptionACKConfirm
    {
        public static void Show()
        {
            var factory = new ConnectionFactory();
            factory.HostName = "localhost";//RabbitMQ服务在本地运行
            factory.UserName = "guest";//用户名
            factory.Password = "guest";//密码 
            using (var connection = factory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "ConsumptionACKConfirmQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);
                    //声明交换机exchang
                    channel.ExchangeDeclare(exchange: "ConsumptionACKConfirmQueueExchange", type: ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);
                    //绑定exchange和queue
                    channel.QueueBind(queue: "ConsumptionACKConfirmQueue", exchange: "ConsumptionACKConfirmQueueExchange", routingKey: "ConsumptionACKConfirmKey");

                    for (int i = 1; i <= 1000; i++)
                    {
                        string message = $"消息{i}";
                        channel.BasicPublish(exchange: "ConsumptionACKConfirmQueueExchange",
                                         routingKey: "ConsumptionACKConfirmKey",
                                         basicProperties: null,
                                         body: Encoding.UTF8.GetBytes(message));

                        Thread.Sleep(300);
                        Console.WriteLine($"【{message}】 已发送~~~");
                    }



                    Console.Read();

                }
            }
        }
    }
}

