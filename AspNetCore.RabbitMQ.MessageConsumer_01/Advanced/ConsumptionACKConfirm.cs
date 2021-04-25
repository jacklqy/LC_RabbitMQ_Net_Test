using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.RabbitMQ.MessageConsumer_01.Advanced
{
    public class ConsumptionACKConfirm
    {
        /// <summary>
        /// 自动确认(不推荐使用)
        /// </summary>
        public static void Show2()
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
                    int i = 0;
                    consumer.Received += (model, ea) =>
                    { 
                        var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                        Console.WriteLine(message);
                        ////如果在这里处理消息的手，异常了呢？ 
                        ////Console.WriteLine($"接收到消息：{message}"); ; 
                        //if (i < 50)
                        //{
                        //    //手动确认  消息正常消费  告诉Broker：你可以把当前这条消息删除掉了
                        //    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                        //    Console.WriteLine(message);
                        //}
                        //else
                        //{
                        //    //否定：告诉Broker，这个消息我没有正常消费；  requeue: true：重新写入到队列里去； false:你还是删除掉；
                        //    channel.BasicReject(deliveryTag: ea.DeliveryTag, requeue: true);
                        //}
                        //i++;
                    };
                    Console.WriteLine("消费者准备就绪...."); 
                    {
                        //处理消息 
                        //autoAck: true 自动确认； 
                        channel.BasicConsume(queue: "ConsumptionACKConfirmQueue", autoAck: true, consumer: consumer);
                    } 
                    {
                        ////处理消息 
                        ////autoAck: false  显示确认； 
                        //channel.BasicConsume(queue: "ConsumptionACKConfirmQueue", autoAck: false, consumer: consumer);
                    }


                    Console.ReadKey();
                    #endregion
                }
            }
        }

        /// <summary>
        /// 手动确认(推荐使用)
        /// </summary>
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
                    int i = 0;
                    consumer.Received += (model, ea) =>
                    {
                        var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                        //如果在这里处理消息的手，异常了呢？ 
                        //Console.WriteLine($"接收到消息：{message}"); ; 
                        if (i < 50)
                        {
                            //手动确认  消息正常消费  告诉Broker：你可以把当前这条消息删除掉了
                            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                            Console.WriteLine(message);
                        }
                        else
                        {
                            //否定：告诉Broker，这个消息我没有正常消费；  requeue: true：重新写入到队列里去，false:你还是删除掉；
                            channel.BasicReject(deliveryTag: ea.DeliveryTag, requeue: true);
                        }
                        i++;
                    };
                    Console.WriteLine("消费者准备就绪....");
                    {
                        ////处理消息 
                        ////autoAck: true 自动确认； 
                        //channel.BasicConsume(queue: "ConsumptionACKConfirmQueue", autoAck: true, consumer: consumer);
                    }
                    {
                        //处理消息 
                        //autoAck: false  显示确认； 
                        channel.BasicConsume(queue: "ConsumptionACKConfirmQueue", autoAck: false, consumer: consumer);
                    }


                    Console.ReadKey();
                    #endregion
                }
            }
        }
    }
}
