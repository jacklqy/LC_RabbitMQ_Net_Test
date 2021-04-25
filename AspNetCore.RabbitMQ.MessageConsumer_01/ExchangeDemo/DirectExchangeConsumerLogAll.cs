using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace AspNetCore.RabbitMQ.MessageConsumer_01.ExchangeDemo
{
    public class DirectExchangeConsumerLogAll
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
                    //channel.QueueDeclare(queue: "DirectExchangeLogAllQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);   
                    //channel.ExchangeDeclare(exchange: "DirectExChange", type: ExchangeType.Direct, durable: true, autoDelete: false, arguments: null); 
                    //string[] logtypes = new string[] { "debug", "info", "warn", "error" }; 
                    //foreach (string logtype in logtypes)
                    //{
                    //    channel.QueueBind(queue: "DirectExchangeLogAllQueue",
                    //            exchange: "DirectExChange",
                    //            routingKey: logtype);
                    //}

                    //消费队列中的所有消息；                                   
                    var consumer = new EventingBasicConsumer(channel); 
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body.ToArray()); 
                        Console.WriteLine($"【{message}】，写入文本~~");
                    }; 
                    //处理消息
                    channel.BasicConsume(queue: "DirectExchangeLogAllQueue",
                                         autoAck: true,
                                         consumer: consumer); //autoAck: true 自动确认，false显示确认
                    Console.ReadLine(); 
                }
            }
        } 
    }
}
