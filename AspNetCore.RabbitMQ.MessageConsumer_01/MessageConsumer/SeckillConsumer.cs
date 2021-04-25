using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.RabbitMQ.MessageConsumer_01.MessageConsumer
{
    public class SeckillConsumer
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
                    Console.ForegroundColor = ConsoleColor.Green;
                    try
                    {
                        var consumer = new EventingBasicConsumer(channel);
                        int count = 0;
                        consumer.Received += (model, ea) =>
                        {
                            if (count >= 10)
                            {
                                Console.WriteLine("商品已经秒杀结束~这里其实没有去数据库操作~~~~"); 
                            }
                            else
                            {
                                ///这里其实是要到数据库中去操作数据；
                                var body = ea.Body;
                                var message = Encoding.UTF8.GetString(body.ToArray());
                                Console.WriteLine($"{message} 秒杀成功~");
                                count++;
                            }  
                        };
                        channel.BasicConsume(queue: "SeckillProduct", autoAck: true, consumer: consumer); //autoAck: true 自动确认，false显示确认
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
