Richard......

# 无法远程访问RabbitMQ Web管理

## 一、检查

当在 Linux 上配置好 Rabbitmq服务器后，如果从主机中无法访问到 Linux 中的Rabbitmq服务器时，需要做如下的检查：

### 1、**Rabbitmq是否启动成功**

~~~
在控制台输入：
ps -ef | grep rabbitmq
命令含义：从当前所有进程中查找是否含有rabbitmq进程
如果有内容显示，则说明 Rabbitmq启动成功
~~~

![image-20200708195926430](C:\Users\Richard\AppData\Roaming\Typora\typora-user-images\image-20200708195926430.png)

~~~
否则，重新启动 Rabbitmq
~~~

### 2、**检查能否从 Linux 本地中访问到 Rabbitmq**

~~~
从控制台输入命令：
wget http://localhost:15672
~~~

![image-20200708200102504](C:\Users\Richard\AppData\Roaming\Typora\typora-user-images\image-20200708200102504.png)

~~~
否则，检查 Rabbitmq端口号是否正确
~~~

### 3.**检查 Rabbitmq启动端口号**

~~~
Rabbitmq 默认的启动端口号是 15672，如果你没有对 Rabbitmq 的配置文件做修改的话应该是没有问题的
输入命令：
ps -ef | grep rabbitmq
命令含义：查看 Rabbitmq进程信息
~~~

![image-20200708200154848](C:\Users\Richard\AppData\Roaming\Typora\typora-user-images\image-20200708200154848.png)

~~~
查看进程号（图中红色框位置）
接着输入命令：
netstat -apn | grep 9810
~~~



### 4 、**检查远程访问的** **ip** **地址是否正确**

```
如果从 Linux 本地可以成功访问 Rabbitmq服务器，而从 Windows（主机） 上无法访问，那么首先检查远程访问的 ip 地址是否正确
在 Liunx 控制台上输入命令:
ifconfig
```

![image-20200708200410560](C:\Users\Richard\AppData\Roaming\Typora\typora-user-images\image-20200708200410560.png)

```
图中位置即是 Linux 的 ip 地址，若此处没出现ens33的ip地址，可能是网络配置没配置好
```

### 5、**检查 Linux 防火墙是否开放 Rabbitmq端口号**

```
当window能够ping通linux的ip，而还是不能通过ip在windows上访问linux的一些服务，如tomcat、mysql、nginx、rabbitmq等服务，最可能的原因是linux的防火墙问题。
如果你没有修改过 Linux 防火墙配置的话，那么 Rabbitmq 端口号一定是被禁用了 ，因为 Linux 防火墙默认只开启 22 号端口。
你需要设置防火墙配置，开放 Rabbitmq的端口号 （注：网上有其他解决方法说直接关闭防火墙，这种方法很不可取）
我的 Linux 版本是 CentOS 7 ，在CentOS 7或RHEL 7或Fedora中防火墙由firewalld来管理，如果要添加范围例外端口 如 1000-2000
语法命令如下：启用区域端口和协议组合

firewall-cmd [--zone=<zone>] --add-port=<port>[-<port>]/<protocol> [--timeout=<seconds>]
此举将启用端口和协议的组合。端口可以是一个单独的端口 <port> 或者是一个端口范围 <port>-<port> 。协议可以是 tcp 或 udp。
实际命令如下：
　　添加
firewall-cmd --zone=public --add-port=80/tcp --permanent （--permanent永久生效，没有此参数重启后失效）

firewall-cmd --zone=public --add-port=1000-2000/tcp --permanent 
　　重新载入
firewall-cmd --reload
　　查看
firewall-cmd --zone=public --query-port=80/tcp
　　删除
firewall-cmd --zone=public --remove-port=80/tcp --permanent
此处的解决方案是开放 15672端口号只需输入命令：(注：若python无法连接Rabbitmq，同理，则需要开放5672端口号)
firewall-cmd --zone=public --add-port=15672/tcp --permanent
然后重启防火墙，即可解决：
firewall-cmd --reload
 成功访问rabbitmq管理页面


```

> 