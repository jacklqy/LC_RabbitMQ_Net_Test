using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.RabbitMQ.MessageProducer.MessageProducer
{
    public class PublishSubscribeConsumer
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
                    channel.QueueDeclare(queue: "PublishSubscrib01", durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueueDeclare(queue: "PublishSubscrib02", durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.ExchangeDeclare(exchange: "PublishSubscribExChange", type: ExchangeType.Fanout, durable: true, autoDelete: false, arguments: null);
                    channel.QueueBind(queue: "PublishSubscrib01", exchange: "PublishSubscribExChange", routingKey: string.Empty, arguments: null);
                    channel.QueueBind(queue: "PublishSubscrib02", exchange: "PublishSubscribExChange", routingKey: string.Empty, arguments: null);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("开始发布消息~~~~"); 
                    for (int i = 1; i <= 2000; i++)
                    { 
                        string message = $"广播第{i}条消息...";
                        byte[] body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish(exchange: "PublishSubscribExChange",
                                        routingKey: string.Empty,
                                        basicProperties: null,
                                        body: body); 
                        Console.WriteLine(message);
                        Thread.Sleep(200);
                    }
                }
            }
        }
    }
}
