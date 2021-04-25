using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.RabbitMQ.MessageConsumer_02.Advanced
{
    public class ConsumptionACKConfirm
    {
        /// <summary>
        /// 自动确认(不推荐使用)
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
                    consumer.Received += (model, ea) =>
                    { 
                        var message = Encoding.UTF8.GetString(ea.Body.ToArray()); 
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                        Console.WriteLine(message); 
                    };
                   

                    {
                        //处理消息 
                        //autoAck: true 自动确认； 
                        channel.BasicConsume(queue: "ConsumptionACKConfirmQueue", autoAck: false, consumer: consumer);
                    }


                    Console.ReadKey();
                    #endregion
                }
            }
        }
    }
}
