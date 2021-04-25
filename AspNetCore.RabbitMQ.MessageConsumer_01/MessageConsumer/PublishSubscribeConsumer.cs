using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.RabbitMQ.MessageConsumer_01.MessageConsumer
{
    /// <summary>
    /// 发布订阅
    /// </summary>
    public class PublishSubscribeConsumer
    {
        public static void Show()
        {
            var factory = new ConnectionFactory();
            factory.HostName = "localhost";//RabbitMQ服务在本地运行
            factory.UserName = "Richard02";//用户名
            factory.Password = "123456";//密码 
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                { 
                    Console.ForegroundColor = ConsoleColor.Green;
                    channel.QueueDeclare(queue: "PublishSubscrib01", durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueueDeclare(queue: "PublishSubscrib02", durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.ExchangeDeclare(exchange: "PublishSubscribExChange", type: ExchangeType.Fanout, durable: true, autoDelete: false, arguments: null);
                    channel.QueueBind(queue: "PublishSubscrib01", exchange: "PublishSubscribExChange", routingKey: string.Empty, arguments: null);
                    channel.QueueBind(queue: "PublishSubscrib02", exchange: "PublishSubscribExChange", routingKey: string.Empty, arguments: null);
                     
                    Console.WriteLine("订阅者01 已经准备就绪~~");
                    try
                    {
                        var consumer = new EventingBasicConsumer(channel); 
                        consumer.Received += (model, ea) =>
                        { 
                                var body = ea.Body;
                                var message = Encoding.UTF8.GetString(body.ToArray());
                                Console.WriteLine($"订阅者01收到消息:{message} ~");  
                        };
                        channel.BasicConsume(queue: "PublishSubscrib01", autoAck: true, consumer: consumer); //autoAck: true 自动确认，false显示确认
                        Console.ReadLine();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    } 
                }
            }
        }
    }
}
