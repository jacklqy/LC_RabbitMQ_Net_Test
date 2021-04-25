using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AspNetCore.RabbitMQ.MessageConsumer_01.Advanced
{
    public class ConsumptionColonyACKConfirm
    {
        public static void Show()
        {
            var factory = new ConnectionFactory();
            //factory.HostName = "192.168.3.212";//RabbitMQ服务在本地运行
            factory.Port = 5672;
            factory.UserName = "richard";//用户名
            factory.Password = "richard";//密码 
            using (var connection = factory.CreateConnection(new List<string>() {
                    "192.168.3.211",
                    "192.168.3.212",
                    "192.168.3.213"
            }))
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
                        if (true)//根据实际业务逻辑判断
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
                        Console.WriteLine(message);
                        Thread.Sleep(2000);
                    };
                    //autoAck: false  显示确认； 
                    channel.BasicConsume(queue: "ColonyProducerMessage", autoAck: false, consumer: consumer); 
                    Console.ReadKey();
                    #endregion
                }
            }
        }
    }
}
