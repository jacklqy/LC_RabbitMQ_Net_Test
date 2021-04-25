using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace AspNetCore.RabbitMQ.MessageConsumer_02.ExchangeDemo
{
    public class DirectExchangeConsumerLogError
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
                    channel.QueueDeclare(queue: "DirectExchangeErrorQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);  
                    channel.ExchangeDeclare(exchange: "DirectExChange", type: ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);  
                    channel.QueueBind(queue: "DirectExchangeErrorQueue",
                              exchange: "DirectExChange",
                              routingKey: "error");

                    //消费队列中的所有消息；                                   
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body.ToArray());
                        Console.WriteLine($"【{message}】，已发送邮件通知系统管理员~~");
                    };
                    //处理消息
                    channel.BasicConsume(queue: "DirectExchangeErrorQueue",
                                         autoAck: true,
                                         consumer: consumer);
                    Console.ReadLine(); 
                }
            }
        }


      
    }
}
