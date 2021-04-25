using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AspNetCore.RabbitMQ.MessageConsumer_01.MessageConsumer
{
    public class MutualProductionConsumer
    {
        /// <summary>
        /// 互为生产消费者--生产者
        /// </summary>
        public static void ShowProductio()
        {
            var factory = new ConnectionFactory();
            factory.HostName = "localhost";//RabbitMQ服务在本地运行
            factory.UserName = "guest";//用户名
            factory.Password = "guest";//密码 
            using (var connection = factory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    //生产者
                    channel.QueueDeclare(queue: "MutualProductionConsumerMessage02", durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.ExchangeDeclare(exchange: "MutualProductionConsumerMessageExChange02", type: ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);
                    channel.QueueBind(queue: "MutualProductionConsumerMessage02", exchange: "MutualProductionConsumerMessageExChange02", routingKey: string.Empty, arguments: null);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("生产者已准备就绪~~~");
                    for (int i = 0; i < 1000; i++)
                    {
                        string message = $"生产者02------消息{i}";
                        byte[] body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish(exchange: "MutualProductionConsumerMessageExChange02",
                                        routingKey: string.Empty,
                                        basicProperties: null,
                                        body: body);
                        Console.WriteLine($"{message} 已发送~");
                        Thread.Sleep(300);
                    }
                }
            }
        }

        /// <summary>
        /// 互为生产消费者--消费者
        /// </summary>
        public static void ShowConsumer()
        {
            var factory = new ConnectionFactory();
            factory.HostName = "localhost";//RabbitMQ服务在本地运行
            factory.UserName = "guest";//用户名
            factory.Password = "guest";//密码 
            using (var connection = factory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    //生产者
                    channel.QueueDeclare(queue: "MutualProductionConsumerMessage01", durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.ExchangeDeclare(exchange: "MutualProductionConsumerMessageExChange01", type: ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);
                    channel.QueueBind(queue: "MutualProductionConsumerMessage01", exchange: "MutualProductionConsumerMessageExChange01", routingKey: string.Empty, arguments: null);
                    //消费者
                    try
                    {
                        var consumer = new EventingBasicConsumer(channel);
                        consumer.Received += (model, ea) =>
                        {
                            var body = ea.Body;
                            var message = Encoding.UTF8.GetString(body.ToArray());
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"消费者02 接受消息: {message}");
                        };
                        channel.BasicConsume(queue: "MutualProductionConsumerMessage01",
                                     autoAck: true,
                                     consumer: consumer);
                        Console.WriteLine(" Press [enter] to exit.");
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
