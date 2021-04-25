using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.RabbitMQ.MessageConsumer_01.MessageConsumer
{
    class ProductionConsumer
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
                    {
                        {


                            Console.ForegroundColor = ConsoleColor.Green;
                            try
                            {

                                channel.QueueDeclare(queue: "OnlyProducerMessage", durable: true, exclusive: false, autoDelete: false, arguments: null);


                                channel.ExchangeDeclare(exchange: "OnlyProducerMessageExChange", type: ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);

                                channel.QueueBind(queue: "OnlyProducerMessage", exchange: "OnlyProducerMessageExChange", routingKey: string.Empty, arguments: null);


                                var consumer = new EventingBasicConsumer(channel);
                                consumer.Received += (model, ea) =>
                                {
                                    var body = ea.Body;
                                    var message = Encoding.UTF8.GetString(body.ToArray());
                                    Console.WriteLine($"消费者01 接受消息: {message}");
                                };
                                channel.BasicConsume(queue: "OnlyProducerMessage",
                                             autoAck: true,//autoAck: true 自动确认，false显示确认
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
    }
}
