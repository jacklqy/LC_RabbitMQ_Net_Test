using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AspNetCore.RabbitMQ.MessageProducer.MessageProducer
{
    public class ProductionConsumer
    {
        public static void Show()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";//RabbitMQ服务在本地运行
            factory.UserName = "guest";//用户名
            factory.Password = "guest";//密码 
            //factory.VirtualHost = "/Richard";
            using (IConnection connection = factory.CreateConnection())
            {  
                using (IModel channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "OnlyProducerMessage", durable: true, exclusive: false, autoDelete: false, arguments: null);


                    channel.ExchangeDeclare(exchange: "OnlyProducerMessageExChange", type: ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);

                    channel.QueueBind(queue: "OnlyProducerMessage", exchange: "OnlyProducerMessageExChange", routingKey: string.Empty, arguments: null);
                   

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("生产者ProducerDemo已准备就绪~~~");
                    int i = 1;
                    {
                        while (true)
                        {
                            IBasicProperties basicProperties = channel.CreateBasicProperties();
                            basicProperties.Persistent = true; //消息持久化
                            //basicProperties.DeliveryMode = 2;
                            string message = $"消息{i}";
                            byte[] body = Encoding.UTF8.GetBytes(message);
                            channel.BasicPublish(exchange: "OnlyProducerMessageExChange",
                                            routingKey: string.Empty,
                                            basicProperties: basicProperties,
                                            body: body);
                            Console.WriteLine($"消息消息消息消息消息消息消息消息消息消息消息消息消息消息：{message} 已发送~");
                            i++;
                            //Thread.Sleep(10);
                        }
                    }
                }
            }
        }
    }
}
