using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.RabbitMQ.MessageProducer.ExchangeDemo
{
    public class TopicExchange
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
                    channel.ExchangeDeclare(exchange: "TopicExchange", type: ExchangeType.Topic, durable: true, autoDelete: false, arguments: null); 

                    channel.QueueDeclare(queue: "ChinaQueue", durable: true, exclusive: false, autoDelete: false, arguments: null); 
                    channel.QueueDeclare(queue: "newsQueue", durable: true, exclusive: false, autoDelete: false,  arguments: null); 

                    channel.QueueBind(queue: "ChinaQueue", exchange: "TopicExchange", routingKey: "China.#", arguments: null); 
                    channel.QueueBind(queue: "newsQueue", exchange: "TopicExchange", routingKey: "#.news", arguments: null);
                    {
                        string message = "来自中国的新闻消息。。。。";
                        var body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish(exchange: "TopicExchange", routingKey: "China.news", basicProperties: null, body: body);
                        Console.WriteLine($"消息【{message}】已发送到队列");
                    }

                    {
                        string message = "来自中国的天气消息。。。。";
                        var body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish(exchange: "TopicExchange", routingKey: "China.weather", basicProperties: null, body: body);
                        Console.WriteLine($"消息【{message}】已发送到队列");
                    }
                    {
                        string message = "来自美国的新闻消息。。。。";
                        var body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish(exchange: "TopicExchange", routingKey: "usa.news", basicProperties: null, body: body);
                        Console.WriteLine($"消息【{message}】已发送到队列");
                    } 
                    {
                        string message = "来自美国的天气消息。。。。";
                        var body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish(exchange: "TopicExchange", routingKey: "usa.weather", basicProperties: null, body: body);
                        Console.WriteLine($"消息【{message}】已发送到队列");
                    } 
                }
            }
        }
    }
}
