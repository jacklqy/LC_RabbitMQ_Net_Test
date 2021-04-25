using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AspNetCore.RabbitMQ.MessageProducer.MessageProducer
{
    public class ProductionColonyConsumer
    {
        public static void Show()
        {
            ConnectionFactory factory = new ConnectionFactory();
            //factory.HostName = "192.168.3.211";//RabbitMQ服务在本地运行 

            factory.Port = 5672;
            factory.UserName = "richard";//用户名
            factory.Password = "richard";//密码 
            //factory.AutomaticRecoveryEnabled = true; //如果connection挂掉是否重新连接 
            //factory.TopologyRecoveryEnabled = true;//连接恢复后，连接的交换机，队列等是否一同恢复 

            //指定虚拟主机，默认‘/’
            factory.VirtualHost = "/";
            using (IConnection connection = factory.CreateConnection(new List<string>() {

                    "192.168.3.211",
                    "192.168.3.212",
                    "192.168.3.213"

            }))
            {
                using (IModel channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "ColonyProducerMessage", durable: true, exclusive: false, autoDelete: false, arguments: null);

                    channel.ExchangeDeclare(exchange: "ColonyProducerMessageExChange", type: ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);

                    channel.QueueBind(queue: "ColonyProducerMessage", exchange: "ColonyProducerMessageExChange", routingKey: "ColonyProducerMessageExChangeKey", arguments: null);

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("生产者ColonyProducerDemo已准备就绪~~~");
                    int i = 1;
                    {
                        while (true)
                        {
                            IBasicProperties basicProperties = channel.CreateBasicProperties();
                            basicProperties.Persistent = true;
                            string message = $"消息{i}";
                            byte[] body = Encoding.UTF8.GetBytes(message);
                            channel.BasicPublish(exchange: "ColonyProducerMessageExChange",
                                            routingKey: "ColonyProducerMessageExChangeKey",
                                            basicProperties: basicProperties,
                                            body: body);
                            Console.WriteLine($"集群消息：{message} 已发送~");
                            i++;
                            Thread.Sleep(10);
                        }
                    }
                }
            }
        }
    }
}
