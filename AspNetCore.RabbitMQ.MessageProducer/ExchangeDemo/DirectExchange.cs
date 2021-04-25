using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace AspNetCore.RabbitMQ.MessageProducer.ExchangeDemo
{
    public class DirectExchange
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
                    channel.QueueDeclare(queue: "DirectExchangeLogAllQueue", durable: true, exclusive: false, autoDelete: false, arguments: null); 

                    channel.QueueDeclare(queue: "DirectExchangeErrorQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);

                    channel.ExchangeDeclare(exchange: "DirectExChange", type: ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);

                    string[] logtypes = new string[] { "debug", "info", "warn", "error" };

                    foreach (string logtype in logtypes)
                    {
                        channel.QueueBind(queue: "DirectExchangeLogAllQueue",
                                exchange: "DirectExChange",
                                routingKey: logtype);
                    } 

                    channel.QueueBind(queue: "DirectExchangeErrorQueue",
                              exchange: "DirectExChange",
                              routingKey: "error");

                    List<LogMsgModel> logList = new List<LogMsgModel>();
                    for (int i = 1; i <=100; i++)
                    {
                        if (i % 4 == 0)
                        {
                            logList.Add(new LogMsgModel() { LogType = "info", Msg = Encoding.UTF8.GetBytes($"info第{i}条信息") });
                        }
                        if (i % 4 == 1)
                        {
                            logList.Add(new LogMsgModel() { LogType = "debug", Msg = Encoding.UTF8.GetBytes($"debug第{i}条信息") });
                        }
                        if (i % 4 == 2)
                        {
                            logList.Add(new LogMsgModel() { LogType = "warn", Msg = Encoding.UTF8.GetBytes($"warn第{i}条信息") });
                        }
                        if (i % 4 == 3)
                        {
                            logList.Add(new LogMsgModel() { LogType = "error", Msg = Encoding.UTF8.GetBytes($"error第{i}条信息") });
                        }
                    }
                     
                    Console.WriteLine("生产者发送100条日志信息");
                    //发送日志信息
                    foreach (var log in logList)
                    {
                        channel.BasicPublish(exchange: "DirectExChange",
                                            routingKey: log.LogType,
                                            basicProperties: null,
                                            body: log.Msg);
                        Console.WriteLine($"{Encoding.UTF8.GetString(log.Msg)}  已发送~~");
                    }
                     
                }
            }
        }


        public class LogMsgModel
        {
            public string LogType { get; set; }

            public byte[] Msg { get; set; }
        }
    }
}
