using AspNetCore.RabbitMQ.MessageProducer.Advanced;
using AspNetCore.RabbitMQ.MessageProducer.ExchangeDemo;
using AspNetCore.RabbitMQ.MessageProducer.MessageProducer;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace AspNetCore.RabbitMQ.MessageProducer
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
                    /////多生产者多消费者
                    //IConfigurationRoot config = new ConfigurationBuilder()
                    // .SetBasePath(Directory.GetCurrentDirectory())
                    // .AddCommandLine(args)//支持命令行参数
                    // .Build();

                    //string strMinute = config["minute"];  //什么时候开始执行
                    //string No = config["no"]; //生产者编号 
                    //int minute = Convert.ToInt32(strMinute); 
                    //bool flg = true;
                    //while (flg)
                    //{ 
                    //    if (DateTime.Now.Minute == minute)
                    //    {
                    //        Console.WriteLine($"到{strMinute}分钟，开始写入消息。。。");
                    //        flg = false;
                    //        MultiProductionConsumer.Show(No);
                    //    }
                    //}
                }
                {
                    //互为生产者消费者
                    //Task.Run(() => { MutualProductionConsumer.ShowProductio(); });
                    //Task.Run(() => { MutualProductionConsumer.ShowConsumer(); });
                }
                {
                    ////秒杀
                    //IConfigurationRoot config = new ConfigurationBuilder()
                    // .SetBasePath(Directory.GetCurrentDirectory())
                    // .AddCommandLine(args)//支持命令行参数
                    // .Build();
                    //string strMinute = config["minute"];  //什么时候开始执行 
                    //int minute = Convert.ToInt32(strMinute);

                    //bool flg = true;
                    //while (flg)
                    //{
                    //    Console.WriteLine($"到{strMinute}分钟，开始写入消息。。。");
                    //    if (DateTime.Now.Minute == minute)
                    //    {
                    //        flg = false;
                    //        SeckillConsumer.Show();
                    //    }
                    //}
                }
                {
                    //////优先级 
                    PriorityQueue.Show();
                }
                {
                    //////发布订阅模式
                    PublishSubscribeConsumer.Show();
                }

                #endregion

                #region 架构师-2 四种交换机模式
                {
                    ////处理路由键。需要将一个队列绑定到交换机上，要求该消息与一个特定的路由键完全匹配
                    //DirectExchange.Show();
                }
                {
                    ////不处理路由键。你只需要简单的将队列绑定到交换机上。一个发送到交换机的消息都会被转发到与该交换机绑定的所有队列上。很像子网广播，每台子网内的主机都获得了一份复制的消息
                    //FanoutExchange.Show();
                }
                {
                    ////将路由键和某模式进行匹配。此时队列需要绑定要一个模式上。符号“#”匹配一个或多个词，符号“”匹配不多不少一个词。因此“abc.#”能够匹配到“abc.def.ghi”，但是“abc.” 只会匹配到“abc.def”。
                    //TopicExchange.Show();
                }
                {
                    ////不处理路由键。而是根据发送的消息内容中的headers属性进行匹配。在绑定Queue与Exchange时指定一组键值对；当消息发送到RabbitMQ时会取到该消息的headers与Exchange绑定时指定的键值对进行匹配；如果完全匹配则消息会路由到该队列，否则不会路由到该队列。headers属性是一个键值对，可以是Hashtable，键值对的值可以是任何类型。而fanout，direct，topic 的路由键都需要要字符串形式的。
                    ////匹配规则x - match有下列两种类型：
                    ////x - match = all ：表示所有的键值对都匹配才能接受到消息
                    ////x - match = any ：表示只要有键值对匹配就能接受到消息
                    //HeaderExchange.Show();
                }
                #endregion

                #region 架构师-3 

                #region 生产者-》队列
                {
                    ////生产者-》队列
                    ////Tx事务模式(同步)：可以让信道设置成一个带事务的信道，分为三步：开启事务、提交事务、支持回滚。在事务提交之前，是不能继续发送消息的。
                    ProductionMessageTx.Show();
                }
                {
                    ////生产者-》队列；效率比Tx事务模式要高些
                    ////Confirm确认模式(异步)：生产者发送消息给队列，队列收到后做出响应，告诉生产者消息已收到。本质就是应答模式。在应答之前，可以继续发送消息。单条发消息、批量发消息
                    ProductionMessageConfirm.Show();
                }
                #endregion

                #region 消费者-》队列
                {
                    ////消费者-》队列
                    ////自动确认(性能高)：客户端消费消息的时候，只要是收到消息，就直接回执给RabbitMQ，Ok没问题，直接总览了所有。如果只是消费成功了一条消息，RabbitMQ也会认为你是全部成功了，会把所有消息从队列移除，这样会导致消息的丢失。
                    ////显示确认(性能稍微低些，推荐使用)：又称为手动确认，客户端消费者消费一条，就回执给RabbitMQ一条消息，RabbitMQ只删除这一条消息，相当于是消费了一条，删除一条消息。
                    ConsumptionACKConfirm.Show();
                } 
                #endregion

                {
                    //如何保证消息一定会从生产端到消费端呢？---》分布式事务，基于本地消息表做分布式事务的一种手段。
                    //1、消息持久化
                    //2、生产端消息确认
                    //3、消费端消息确认

                }
                {
                    //集群
                    //普通集群：比较鸡肋，无法实现高可用。有主从关系，队列是存放在主节点，其它节点只存了队列结构，只要主节点挂了，就完了。
                    //镜像集群(镜像队列---推荐使用)：绝对的高可用，没有主从关系，所有节点都同步队列，任意节点挂了，还可以继续使用。
                    ProductionColonyConsumer.Show();
                }
                #endregion

                Console.Read();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
