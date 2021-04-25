using AspNetCore.RabbitMQ.MessageConsumer_01.Advanced;
using AspNetCore.RabbitMQ.MessageConsumer_01.ExchangeDemo;
using AspNetCore.RabbitMQ.MessageConsumer_01.MessageConsumer;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.RabbitMQ.MessageConsumer_01
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                #region 架构师-1
                {
                    ////生产者消费者
                    //ProductionConsumer.Show();
                }

                {
                    //多生产消费者
                    //Task.Run(() => { MultiProductionConsumer.Show01(); });
                    //Task.Run(() => { MultiProductionConsumer.Show02(); });
                    //Task.Run(() => { MultiProductionConsumer.Show03(); });
                }
                {
                    //互为生产消费者
                    //Task.Run(() => { MutualProductionConsumer.ShowProductio(); });
                    //Task.Run(() => { MutualProductionConsumer.ShowConsumer(); });
                }
                {
                    ////秒杀
                    //SeckillConsumer.Show();
                }

                {
                    ////发布订阅模式
                    //PublishSubscribeConsumer.Show();
                }
                {
                    ////优先级队列
                    //PriorityQueue.Show();
                }

                #endregion

                #region 架构师-2 四种交换机模式
                {
                    //DirectExchangeConsumerLogAll.Show();
                }
                {
                    //FanoutExchange.Show();
                }
                {
                    //TopicExchange.Show();
                }
                #endregion

                #region 架构师-3 
                {
                    //消费者-》队列
                    //自动确认(性能高)：客户端消费消息的时候，只要是收到消息，就直接回执给RabbitMQ，Ok没问题，直接总览了所有。如果只是消费成功了一条消息，RabbitMQ也会认为你是全部成功了，会把所有消息从队列移除，这样会导致消息的丢失。
                    //显示确认(性能稍微低些，推荐使用)：又称为手动确认，客户端消费者消费一条，就回执给RabbitMQ一条消息，RabbitMQ只删除这一条消息，相当于是消费了一条，删除一条消息。
                    ConsumptionACKConfirm.Show();
                }
                {
                    ////集群
                    ConsumptionColonyACKConfirm.Show();
                }
                #endregion

                Console.Read();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }
    }
}
