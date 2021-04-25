using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.RabbitMQ.MessageProducer.ExchangeDemo
{
    public class HeaderExchange
    {
        public static void Show()
        {
            var factory = new ConnectionFactory();
            factory.HostName = "localhost";//RabbitMQ服务在本地运行
            factory.UserName = "guest";//用户名
            factory.Password = "guest";//密码 
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "HeaderExchange", type: ExchangeType.Headers, durable: false, autoDelete: false, arguments: null);
                    channel.QueueDeclare(queue: "HeaderExchangeAllqueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueueDeclare(queue: "HeaderExchangeAnyqueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    Console.WriteLine("生产者准备就绪....");
                    channel.QueueBind(queue: "HeaderExchangeAllqueue", exchange: "HeaderExchange", routingKey: string.Empty,
                        arguments: new Dictionary<string, object> {
                                                                    { "x-match","all"},
                                                                    { "teacher","Richard"},
                                                                    { "pass","123"}});   
                    {
                        string message = "teacher和pass都相同时发送的消息";
                        var props = channel.CreateBasicProperties();
                        props.Headers = new Dictionary<string, object>() {
                                                                           { "teacher","Richard"},
                                                                           { "pass","123"}
                                                                          };
                        var body = Encoding.UTF8.GetBytes(message);
                        //基本发布
                        channel.BasicPublish(exchange: "HeaderExchange",
                                             routingKey: string.Empty,
                                             basicProperties: props,
                                             body: body);
                        Console.WriteLine($"消息【{message}】已发送");
                    }
                    {
                        string message = "teacher和pass有一个不相同时发送的消息";
                        var props = channel.CreateBasicProperties();
                        props.Headers = new Dictionary<string, object>() {
                                                                           { "teacher","Richard"},
                                                                           { "pass","234"}
                                                                          };
                        var body = Encoding.UTF8.GetBytes(message); 
                        channel.BasicPublish(exchange: "HeaderExchange",
                                             routingKey: string.Empty,
                                             basicProperties: props,
                                             body: body);
                        Console.WriteLine($"消息【{message}】已发送");
                    }
                    Console.WriteLine("**************************************************************");
                    {
                        channel.QueueBind(queue: "HeaderExchangeAnyqueue",  exchange: "HeaderExchange", routingKey: string.Empty,
                        arguments: new Dictionary<string, object> {
                                            { "x-match","any"},
                                            { "teacher","Richard"},
                                            { "pass","123"},});

                        string msg = "teacher和pass完全相同时发送的消息";
                        var props = channel.CreateBasicProperties();
                        props.Headers = new Dictionary<string, object>() {
                                                 { "teacher","Richard"},
                                                 { "pass","123"} 
                                            };
                        var body = Encoding.UTF8.GetBytes(msg); 
                        channel.BasicPublish(exchange: "HeaderExchange",
                                             routingKey: string.Empty,
                                             basicProperties: props,
                                             body: body);
                        Console.WriteLine($"消息【{msg}】已发送"); 
                    }

                    { 
                        string msg = "teacher和pass有一个不相同时发送的消息";
                        var props = channel.CreateBasicProperties();
                        props.Headers = new Dictionary<string, object>() {
                                                 { "teacher","Richard"},
                                                 { "pass","234"}
                                            };
                        var body = Encoding.UTF8.GetBytes(msg);
                        channel.BasicPublish(exchange: "HeaderExchange",
                                             routingKey: string.Empty,
                                             basicProperties: props,
                                             body: body);
                        Console.WriteLine($"消息【{msg}】已发送");
                    }

                }
            }
            Console.ReadKey();
        }
    }
}
