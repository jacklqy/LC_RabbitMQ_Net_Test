using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.RabbitMQ.MessageConsumer_01.MessageConsumer
{
    public class MultiProductionConsumer
    {
        public static void Show01()
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
                                var consumer = new EventingBasicConsumer(channel);
                                consumer.Received += (model, ea) =>
                                {
                                    var body = ea.Body;
                                    var message = Encoding.UTF8.GetString(body.ToArray());
                                    Console.WriteLine($"消费者01 接受消息: {message}");
                                };
                                channel.BasicConsume(queue: "MultiProducerMessage",
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

        public static void Show02()
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
                                var consumer = new EventingBasicConsumer(channel);
                                consumer.Received += (model, ea) =>
                                {
                                    var body = ea.Body;
                                    var message = Encoding.UTF8.GetString(body.ToArray());
                                    Console.WriteLine($"消费者02 接受消息: {message}");
                                };
                                channel.BasicConsume(queue: "MultiProducerMessage",
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

        public static void Show03()
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
                                var consumer = new EventingBasicConsumer(channel);
                                consumer.Received += (model, ea) =>
                                {
                                    var body = ea.Body;
                                    var message = Encoding.UTF8.GetString(body.ToArray());
                                    Console.WriteLine($"消费者03 接受消息: {message}");
                                };
                                channel.BasicConsume(queue: "MultiProducerMessage",
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
    }
}
