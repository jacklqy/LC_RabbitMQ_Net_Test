using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.RabbitMQ.MessageConsumer_02.ExchangeDemo
{
    public class TopicExchange
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
                    channel.ExchangeDeclare(exchange: "TopicExchange", type: ExchangeType.Topic, durable: true, autoDelete: false, arguments: null);
                    channel.QueueDeclare(queue: "newsQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueueBind(queue: "newsQueue", exchange: "TopicExchange", routingKey: "#.news", arguments: null);
                    //定义消费者                                      
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body.ToArray());
                        Console.WriteLine($"接收成功！【{message}】");
                    };

                    //处理消息
                    channel.BasicConsume(queue: "ChinaQueue",
                                         autoAck: true,
                                         consumer: consumer);

                    Console.WriteLine("对来自于美国的消息比较感兴趣的 消费者");
                }
            }
        }

    }
}
