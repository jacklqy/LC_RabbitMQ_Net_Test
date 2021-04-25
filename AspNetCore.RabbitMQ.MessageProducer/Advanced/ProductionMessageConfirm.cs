using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.RabbitMQ.MessageProducer.Advanced
{
    public class ProductionMessageConfirm
    {
        public static void Show()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";//RabbitMQ服务在本地运行
            factory.UserName = "guest";//用户名
            factory.Password = "guest";//密码 
            using (var connection = factory.CreateConnection())
            {
                //创建通道channel
                using (var channel = connection.CreateModel())
                {
                    Console.WriteLine("生产者准备就绪....");
                    channel.QueueDeclare(queue: "ConfirmSelectQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);
                    //声明交换机exchang
                    channel.ExchangeDeclare(exchange: "ConfirmSelectQueueExchange", type: ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);
                    //绑定exchange和queue
                    channel.QueueBind(queue: "ConfirmSelectQueue", exchange: "ConfirmSelectQueueExchange", routingKey: "ConfirmSelectKey");
                    string message = "";
                    //发送消息
                    //在控制台输入消息，按enter键发送消息
                    while (!message.Equals("quit", StringComparison.CurrentCultureIgnoreCase))
                    {
                        message = Console.ReadLine();
                        var body = Encoding.UTF8.GetBytes(message);
                        try
                        {
                            //开启消息确认模式
                            channel.ConfirmSelect();
                            //发送消息
                            channel.BasicPublish(exchange: "ConfirmSelectQueueExchange", routingKey: "ConfirmSelectKey", basicProperties: null, body: body);
                            //如果一条消息或多消息都确认发送
                            if (channel.WaitForConfirms())
                            {
                                Console.WriteLine($"【{message}】发送到Broke成功！");
                            }
                            else
                            { 
                                //可以记录个日志，重试一下；
                            }

                            //channel.WaitForConfirmsOrDie();//如果所有消息发送成功 就正常执行；如果有消息发送失败；就抛出异常；
                        }
                        catch (Exception)
                        {
                            Console.WriteLine($"【{message}】发送到Broker失败！");
                            //就应该通知管理员
                            // 重新试一下
                        }
                    }
                    Console.Read();
                }
            }
        }
    }
}
