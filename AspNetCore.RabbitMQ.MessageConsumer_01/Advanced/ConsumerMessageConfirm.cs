using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.RabbitMQ.MessageConsumer_01.Advanced
{
    public class ConsumerMessageConfirm
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
                    #region EventingBasicConsumer
                    //定义消费者                                      
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        Console.WriteLine(Encoding.UTF8.GetString(ea.Body.ToArray()));
                    };
                    Console.WriteLine("消费者准备就绪....");
                    //处理消息 
                    //autoAck: true 自动确认； 
                    channel.BasicConsume(queue: "PriorityQueue", autoAck: true, consumer: consumer);
                    Console.ReadKey();
                    #endregion
                }
            }
        }
    }
}
