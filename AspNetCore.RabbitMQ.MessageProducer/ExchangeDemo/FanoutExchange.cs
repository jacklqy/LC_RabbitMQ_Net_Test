using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AspNetCore.RabbitMQ.MessageProducer.ExchangeDemo
{
    public class FanoutExchange
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
                    channel.QueueDeclare(queue: "FanoutExchangeZhaoxi001", durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueueDeclare(queue: "FanoutExchangeZhaoxi002", durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.ExchangeDeclare(exchange: "FanoutExchange", type: ExchangeType.Fanout, durable: true, autoDelete: false, arguments: null);
                    channel.QueueBind(queue: "FanoutExchangeZhaoxi001", exchange: "FanoutExchange", routingKey: string.Empty, arguments: null);
                    channel.QueueBind(queue: "FanoutExchangeZhaoxi002", exchange: "FanoutExchange", routingKey: string.Empty, arguments: null);
                    //在控制台输入消息，按enter键发送消息
                    int i = 1;
                    while (true)
                    {
                        //Console.WriteLine("请输入通知~~");
                        //var message = Console.ReadLine();
                        IBasicProperties basicProperties = channel.CreateBasicProperties();
                        basicProperties.Persistent = true;//消息持久化
                        var message = $"通知{i}";
                        var body = Encoding.UTF8.GetBytes(message);
                        //基本发布
                        channel.BasicPublish(exchange: "FanoutExchange",
                                             routingKey: string.Empty,
                                             basicProperties: null,
                                             body: body);
                        Console.WriteLine($"通知【{message}】已发送到队列");
                        Thread.Sleep(2000);
                        i++;
                    }

                }
            }
        }
    }
}
