using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.RabbitMQ.MessageConsumer_01.Advanced
{
    public class PriorityQueue
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
                        string msg = Encoding.UTF8.GetString(ea.Body.ToArray());
                        Console.WriteLine(msg);
                        if (msg.Equals("消息：1"))
                        {
                            ////显示确认  消息正常消费  告诉Broker：你可以把当前这条消息删除掉了
                            channel.BasicReject(deliveryTag: ea.DeliveryTag, requeue: false); 

                            ///设置消息优先级最高 重新写入到队列中去
                            IBasicProperties props = channel.CreateBasicProperties(); 
                            props.Priority = 10; 
                            channel.BasicPublish(exchange: "PriorityQueueExchange",
                                           routingKey: "PriorityKey",
                                           basicProperties: props,
                                           body: Encoding.UTF8.GetBytes(msg));

                        }
                    };
                    Console.WriteLine("消费者准备就绪....");
                    //处理消息
                    channel.BasicConsume(queue: "PriorityQueue", autoAck: false, consumer: consumer);//autoAck: true 自动确认，false显示确认
                    Console.ReadKey();
                    #endregion
                }
            }
        }
    }
}
