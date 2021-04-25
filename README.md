# LC_RabbitMQ_Net_Test
RabbitMq优先级
![image](https://user-images.githubusercontent.com/26539681/115987918-c4a45880-a5e9-11eb-8d25-2c9f2f2ea322.png)

发布订阅模式
![image](https://user-images.githubusercontent.com/26539681/115987930-d84fbf00-a5e9-11eb-8ada-c25b7aef2822.png)

四种交换机模式(Direct、Fanout、Topic、Header)
![image](https://user-images.githubusercontent.com/26539681/115987958-01704f80-a5ea-11eb-87b2-cd2a58671b28.png)

生产者-》队列：Tx事务模式
![image](https://user-images.githubusercontent.com/26539681/115987978-1cdb5a80-a5ea-11eb-9aa2-d389b9295617.png)

生产者-》队列：Confirm确认模式
![image](https://user-images.githubusercontent.com/26539681/115988013-41373700-a5ea-11eb-9112-2d02828877d9.png)

消费者-》队列：ACK(自动确认、显示确认)
![image](https://user-images.githubusercontent.com/26539681/115988042-65931380-a5ea-11eb-989d-8206fa10a405.png)

普通集群：比较鸡肋，无法实现高可用。有主从关系，队列是存放在主节点，其它节点只存了队列结构，只要主节点挂了，就完了。

镜像集群(镜像队列---推荐使用)：绝对的高可用，没有主从关系，所有节点都同步队列，任意节点挂了，还可以继续使用。
![image](https://user-images.githubusercontent.com/26539681/115988073-9410ee80-a5ea-11eb-939d-57bd84d09aa0.png)
![image](https://user-images.githubusercontent.com/26539681/115988143-ece08700-a5ea-11eb-918f-dc037ca80d80.png)

