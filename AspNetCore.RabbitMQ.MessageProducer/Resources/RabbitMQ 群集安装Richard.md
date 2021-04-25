**★★★★★★http://www.rabbitmq.com/which-erlang.html （rabbitmq和erlang版本对应表）**

# RabbitMQ 群集安装（每一台机器都操作）

## 一、环境描述

### 1、操作系统

| 主机名        | IP地址 | 操作系统版本                         | erlang 版本 | rabbitmq 版本 |
| ------------- | ------ | ------------------------------------ | ----------- | ------------- |
| 192.168.3.99  | node02 | CentOS Linux release 7.4.1708 (Core) | 21.0        | 3.7.7         |
| 192.168.3.100 | node01 | CentOS Linux release 7.4.1708 (Core) | 21.0        | 3.7.7         |
| 192.168.3.222 | node03 | CentOS Linux release 7.4.1708 (Core) | 21.0        | 3.7.7         |

### 2、设置linux静态IP+

~~~
命令：
# cd /etc/sysconfig/network-scripts/
# vim ifcfg-ens33

修改内容为：
TYPE=Ethernet
PROXY_METHOD=none

BOOTPROTO=static #静态IP

BROWSER_ONLY=no
DEFROUTE=yes

IPADDR=192.168.1.189  #调整
NETMASK=255.255.255.0  #调整
GATEWAY=192.168.1.1   #调整
DNS1=202.106.0.20    #调整

DNS2=8.8.8.8
IPV4_FAILURE_FATAL=no
IPV6INIT=yes
IPV6_AUTOCONF=yes
IPV6_DEFROUTE=yes
IPV6_FAILURE_FATAL=no
IPV6_ADDR_GEN_MODE=stable-privacy
NAME=ens33
UUID=312fb2fd-eade-4e6f-8abb-5602fc8d2da4
DEVICE=ens33

ONBOOT=yes  # 支持静态IP


修改完毕后：执行命令：
# service network restart
重启网卡：
# systemctl restart network
~~~

### 3.关闭防火墙

~~~
查看防火墙：
# systemctl status  firewalld.service
开启防火墙：
# systemctl start firewalld.service    
关闭防火墙
# systemctl stop firewalld.service            #停止firewall
# systemctl disable firewalld.service        #禁止firewall开机启动
~~~

### 3 、文件配置（每一台服务都必须配置）

修改主机名称

```
查看主机名称：
# hostname
修改主机名称：
# hostnamectl set-hostname <主机名称> ## 永久修改了主机名称
```

编辑/etc/hosts 文件配置

```
127.0.0.1   localhost localhost.localdomain localhost4 localhost4.localdomain4
::1         localhost localhost.localdomain localhost6 localhost6.localdomain6
192.168.3.220 node1
192.168.3.221 node2
192.168.3.222 node3
```

## 二、安装

### 1、安装环境依赖包

这里虚拟机系统为Centos7，采用的安装方式是yum安装，为了简单，这里直接使用官方提供的erlang和RabbitMQ-server的自动安装脚本([官方安装文档](https://www.rabbitmq.com/install-rpm.html))，逐行执行下边的代码就可以安装完成erlang和RabbitMQ。

```
安装socat
# yum install socat
```

### 2、安装 erlang

rabbitMQ 是用 erlang 语言写的，所以需要先安装 erlang。

```
# curl -s https://packagecloud.io/install/repositories/rabbitmq/erlang/script.rpm.sh | sudo bash
# yum -y install erlang

测试查看版本：
# erl

```

### 3、安装 rabbitmq

```
# curl -s https://packagecloud.io/install/repositories/rabbitmq/rabbitmq-server/script.rpm.sh | sudo bash
# yum -y install rabbitmq-server

测试查看版本：rabbitmqctl version
```

### 4.启动rabbitmq服务

~~~
启动：
# systemctl start rabbitmq-server
停止：
# rabbitmqctl stop_app
~~~

### 5.添加web管理插件

~~~
# rabbitmq-plugins enable rabbitmq_management
~~~



## 三、设置 rabbitMQ

### 1、开启 rabbitMQ web 页面访问

```
# rabbitmq-plugins enable rabbitmq_management
也可以直接将开启的插件配置写入配置文件
# echo "[rabbitmq_management]." > /usr/local/rabbitmq/etc/rabbitmq/enabled_plugins
```

### 2、启动 rabbitmq-server

```
# rabbitmq-server start    # 前台启动

  ##  ##
  ##  ##      RabbitMQ 3.7.7. Copyright (C) 2007-2018 Pivotal Software, Inc.
  ##########  Licensed under the MPL.  See http://www.rabbitmq.com/
  ######  ##
  ##########  Logs: /usr/local/rabbitmq/var/log/rabbitmq/rabbit@node03.log
                    /usr/local/rabbitmq/var/log/rabbitmq/rabbit@node03_upgrade.log

              Starting broker...
 completed with 3 plugins.    # 说明 web 管理插件已经启动


# rabbitmq-server  -detached    # 后台启动，不占用终端，推荐
Warning: PID file not written; -detached was passed.7

# rabbitmq-server start_app
# rabbitmq-server stop_app

默认有一个属于localhost的用户guest 密码guest，不支持远程访问，怎么解决?
1、使用rabbitmqctl来管理用户
./rabbitmqctl add_user rabbit rabbit ## 添加用户
./rabbitmqctl set_permissions rabbit -p / ".*" ".*" ".*"
./rabbitmqctl set_user_tags rabbit administrator
2、添加端口
# firewall-cmd --add-port=5672/tcp --permanent ## rabbitmq端口
# firewall-cmd --add-port=15672/tcp --permanent ## web管理界面端口
# firewall-cmd --reload ## 刷新放行列表
3、通过http://ip:15672
输入rabbit/rabbit 进行web管界面
```

### 3、 rabbitMQ 端口

**★这里有一个需要注意**，记得配置下hosts，在hosts里加上本机的名称。erlang进程需要host来进行连接，所以它会检查你的hosts配置。还需要设置下防火墙，三个端口要打开。**15672**是管理界面用的，**25672**是集群之间使用的端口，4369是erlang进程epmd用来做node连接的。

```
1.查看放行端口：
# firewall-cmd --list-port
2、添加端口
# firewall-cmd --add-port=5672/tcp --permanent ## rabbitmq端口
# firewall-cmd --add-port=15672/tcp --permanent ## web管理界面端口
# firewall-cmd --add-port=4369/tcp --permanent ##node通信端口
# firewall-cmd --reload ## 刷新放行列表
```

## 四、群集配置

### 1、准备工作

| IP地址        | 主机名称 | 操作系统版本                         | erlang 版本 | rabbitmq 版本 |
| ------------- | -------- | ------------------------------------ | ----------- | ------------- |
| 192.168.3.211 | node01   | CentOS Linux release 7.4.1708 (Core) | V11.0.2     | 3.8.5         |
| 192.168.3.212 | node02   | CentOS Linux release 7.4.1708 (Core) | V11.0.2     | 3.8.5         |
| 192.168.3.213 | node03   | CentOS Linux release 7.4.1708 (Core) | V11.0.2     | 3.8.5         |

编辑/etc/hosts 文件配置映射

~~~
命令指定IP地址复制文件：
# scp /etc/hosts root@192.168.3.213:/etc/hosts  


修改为以下内容：
127.0.0.1   localhost localhost.localdomain localhost4 localhost4.localdomain4
::1         localhost localhost.localdomain localhost6 localhost6.localdomain6
192.168.3.211 node1
192.168.3.212 node2
192.168.3.213 node3
192.168.3.214 node4
192.168.3.215 node5
~~~



~~~

命令执行启动前台启动rabbitmq服务：
# rabbitmq-server start

命令执行启动后台启动rabbitmq服务：
# rabbitmq-server  -detached

命令执行停止RabbitMq服务： 
# rabbitmqctl stop

命令执行启用插件：
# rabbitmq-plugins enable rabbitmq_management

查看rabbitMq进程：
# ps -ef | grep rabbitmq



1.查看放行端口：
# firewall-cmd --list-port

2关闭端口：
# firewall-cmd --permanent --zone=public --remove-port=5672/tcp
# firewall-cmd --permanent --zone=public --remove-port=15672/tcp
# firewall-cmd --permanent --zone=public --remove-port=4369/tcp
# firewall-cmd --permanent --zone=public --remove-port=25672/tcp

3、添加端口
# firewall-cmd --add-port=5672/tcp --permanent ## rabbitmq端口
# firewall-cmd --add-port=15672/tcp --permanent ## web管理界面端口
# firewall-cmd --add-port=4369/tcp --permanent ##node erlang通信端口
# firewall-cmd --add-port=25672/tcp --permanent ##node 集群节点通信

# firewall-cmd --reload ## 刷新放行列表
~~~



> 集群管理：没有明显的主从，主要是 disk 和 ram 节点的区别
>
> 集群要求：不支持跨网段(erlang 限制)【局域网】
>
> 集群类型：普通通集群、镜像集群
>
> - 普通集群：结构同步，消息实体只存在一个节点中，但 consumer 在非消息节点获取时，节点间存在消息拉取，易产生性能瓶颈。
> - 镜像集群：集群中一个 master，负责调试，处理消息实体，其他节点保存一份数据到本地；性能主要靠 master 承载。
>
> 持久化，分两部分：
>
> - Rabbitmq 服务器配置持久化：默认的就是持久化(disc类型);
> - 代码持久化：默认情况下，代码创建的消息队列和存放在队列里的消息都是非持久化的，需要在建产队列时指定

**在配置群集前，必须保证各节点之间的主机名能够相互解析**

RabbitMQ 节点使用域名相互寻址，因此所有群集成员的主机名必须能够从所有群集节点解析，可以修改 `/etc/hosts` 文件或者使用 DNS 解析

如果要使用节点名称的完整主机名（RabbitMQ 默认为短名称），并且可以使用DNS解析完整的主机名，则可能需要调查设置环境变量  `RABBITMQ_USE_LONGNAME = true`

一个群集的组成可以动态改变，所有的 RabbitMQ 开始作为单个节点运行，这些节点可以加入到群集，然后也可以再次脱离群集转加单节点。

RabbitMQ 群集可以容忍单个节点的故障。节点可以随意启动和停止，只要它们在关闭时能和群集成员节点联系。

**节点可以是 Disk 节点或 RAM 节点**

RAM 节点将内部数据库表存储在 RAM 中。这不包括消息，消息存储索引，队列索引和其他节点状态，在 90％ 以上的情况下，您希望所有节点都是磁盘节点;

RAM 节点是一种特殊情况，可用于改善高排队，交换或绑定流失的性能集群。RAM 节点不提供有意义的更高的消息速率。

由于 RAM 节点仅将内部数据库表存储在 RAM 中，因此它们必须在启动时从对等节点同步它们。这意味着**群集必须至少包含一个磁盘节点。因此无法手动删除集群中剩余的最后一个磁盘节点**

### 1、设置节点相互信任：Erlang Cookie

RabbitMQ 节点和 CLI 工具（例如 rabbitmqctl ）使用 cookie 来确定它们是否被允许相互通信，要使两个节点能够通信，它们必须具有相同的共享密钥，称为 Erlang Cookie。 Cookie 只是一个字符串，最多可以有 255 个字符。它通常存储在本地文件中。**该文件必须只能由所有者访问（400 权限）**。每个集群节点必须具有相同的 cookie，文件位置（rpm 安装） /var/lib/rabbitmq/.erlang.cookie，如果是源码安装的 .erlang.cookie 文件在**启动用户的家目录中**。把 rabbit2、rabbit3 设置成和 rabbit1 一样的即可，权限是 400 ，或者直接复制一份过去即可。

```
这里采用复制的方式

命令Copy文件：scp /var/lib/rabbitmq/.erlang.cookie root@192.168.3.212:/var/lib/rabbitmq


采用源码安装的 rabbitmq .erlang.cookie 文件在 /root 目录下
# scp /root/.erlang.cookie node02:/root/
root@node02's password: 
.erlang.cookie   100%   20     2.3KB/s   00:00    
# scp /root/.erlang.cookie node03:/root/
root@node03's password: 
.erlang.cookie   100%   20     7.5KB/s   00:00 
```

### 2、正常方式启动所有节点

```
# rabbitmq-server -detached   #后台启动  在所有节点上启动 rabbitmq-server
```

### 3、查看群集状态

```
# nod01 上
# rabbitmqctl cluster_status
Cluster status of node rabbit@node01 ...
[{nodes,[{disc,[rabbit@node01]}]},
 {running_nodes,[rabbit@node01]},
 {cluster_name,<<"rabbit@node01">>},
 {partitions,[]},
 {alarms,[{rabbit@node01,[]}]}]
 
 # node02 上
 # ./rabbitmqctl cluster_status
Cluster status of node rabbit@node02 ...
[{nodes,[{disc,[rabbit@node02]}]},
 {running_nodes,[rabbit@node02]},
 {cluster_name,<<"rabbit@node02">>},
 {partitions,[]},
 {alarms,[{rabbit@node02,[]}]}]
 
 # node03 上
 # ./rabbitmqctl cluster_status
Cluster status of node rabbit@node03 ...
[{nodes,[{disc,[rabbit@node03]}]},
 {running_nodes,[rabbit@node03]},
 {cluster_name,<<"rabbit@node03">>},
 {partitions,[]},
 {alarms,[{rabbit@node03,[]}]}]
```

### 4、将 node02、node03 加入 rabbit@node01 群集

#### a. 停止 node02 的 rabbitmq 应用程序

```
# 在其余 2 个节点上操作
关闭所有rabbitmq进程：  pkill rabbitmq
根据进程ID关闭进程: kill -9 9038

# rabbitmqctl stop_app
Stopping rabbit application on node rabbit@node02 ...
```

#### b. 加入 rabbit@node01 群集1

```
# 在其余 2 个节点上操作
# rabbitmqctl join_cluster rabbit@node01   
# 如果这一步报错的话，请在所有节点打开相应的端口，打开 4369 端口 （Refuse Access）,解决方式
# firewall-cmd --add-port=4369/tcp --permanent
# firewall-cmd --reload
Clustering node rabbit@node02 with rabbit@node01
```

#### c. 启动 rabbitMQ 程序

```
# 在其余 2 个节点上操作
#  rabbitmqctl start_app
Starting node rabbit@node02 ...
 completed with 3 plugins.
```

#### b. 查看群集状态

```
在群集任何一个节点上都可以查看到群集的状态
# rabbitmqctl cluster_status
Cluster status of node rabbit@node01 ...
[{nodes,[{disc,[rabbit@node01,rabbit@node02,rabbit@node03]}]},
 {running_nodes,[rabbit@node03,rabbit@node02,rabbit@node01]},
 {cluster_name,<<"rabbit@node01">>},
 {partitions,[]},
 {alarms,[{rabbit@node03,[]},{rabbit@node02,[]},{rabbit@node01,[]}]}]
```

通过上面的步骤，我们可以在群集运行的同时随时向群集添加新节点

已加入群集的节点可以随时停止，也可以崩溃。在这两种情况下，群集的其余部分都会继续运行，并且节点在再次启动时，会自动 ”跟上“（同步）其它群集节点。

> 注意：
>
> 当整个集群关闭时，最后一个关闭的节点必须是第一个启动的节点，如果不是这样，节点会等待 30s 等待最后的磁盘节点恢复状态，然后失败。如果最后下线的节点不能上线，可以使用 forget_cluster_node 命令将其从群集中删除。如果所有的节点不受控制的同时宕机，比如掉电，会进入所有的节点都会认为其他节点比自己宕机的要晚，即自己先宕机，这种情况下可以使用 force_boot 指令来启动一个节点。

#### d. 设置群集模式为"镜像队列"模式

```
# rabbitmqctl set_policy ha-all "^" '{"ha-mode":"all","ha-sync-mode":"automatic"}'

rabbitmqctl set_policy [-p Vhost] Name Pattern Definition [Priority]
-p Vhost： 可选参数，针对指定vhost下的queue进行设置
Name: policy的名称
Pattern: queue的匹配模式(正则表达式)
Definition：镜像定义，包括三个部分ha-mode, ha-params, ha-sync-mode
	ha-mode:指明镜像队列的模式，有效值为 all/exactly/nodes
		all：表示在集群中所有的节点上进行镜像
		exactly：表示在指定个数的节点上进行镜像，节点的个数由ha-params指定
		nodes：表示在指定的节点上进行镜像，节点名称通过ha-params指定
	ha-params：ha-mode模式需要用到的参数
	ha-sync-mode：进行队列中消息的同步方式，有效值为automatic和manual
priority：可选参数，policy的优先级
```

> ```
> ha-sync-mode`： 如果此节点不进行设置，在其中一台服务器宕机再启动后 会报  Unsynchronised Mirrors XXXX  错误。这时候在队列详细信息页面需要手动点击同步队列，或者用命令行执行命令 `rabbitmqctl sync_queue name
> ```

## 五、群集移除节点

当节点不再是节点的一部分时，需要从群集中明确地删除节点。

将 rabbit@node02 从群集中删除，回到独立模式：

```
在 rabbit@node02 上操作：
1、停止 RabbitMQ 应用程序。
# rabbitmqctl stop_app
Stopping rabbit application on node rabbit@node02 ...

2、重置节点。
# rabbitmqctl reset
Resetting node rabbit@node02 ...

3、重新启动 RabbitMQ 应用程序。
# rabbitmqctl start_app
Starting node rabbit@node02 ...
 completed with 3 plugins.
 
4、在节点上运行 cluster_status 命令，确认 rabbit@node02 现在已经不再是群集的一部分，并独立运行
# rabbitmqctl cluster_status
Cluster status of node rabbit@node02 ...
[{nodes,[{disc,[rabbit@node02]}]},
 {running_nodes,[rabbit@node02]},
 {cluster_name,<<"rabbit@node02">>},
 {partitions,[]},
 {alarms,[{rabbit@node02,[]}]}]
```

也可以远程删除节点，例如，在处理无响应的节点时，这很有用。

例如，在节点 rabbit@node01 上把 rabbit@node03 从群集中移除

```
1、先在 rabbit@node03 上将 RabbitMQ 应用停掉
# rabbitmqctl stop_app
Stopping rabbit application on node rabbit@node03 ...

2、在 rabbit@node01 上远程将 rabbit@node03 删除
# rabbitmqctl forget_cluster_node rabbit@node03
Removing node rabbit@node03 from the cluster

3、请注意，这时，rabbit@node03 仍然认为它还在 rabbit@node01 的群集里面，并试图启动它，这将会导致错误。我们需要将 rabbit@node03 重新设置才能重新启动它。(在 rabbit@node03 上操作)
# rabbitmqctl reset
Resetting node rabbit@node03 ...

4、重新启动 rabbit@node03
# rabbitmqctl start_app
Starting node rabbit@node03 ...
 completed with 3 plugins.
```

现在，三个节点都是作为独立的节点在运行。

> 注意：此时，rabbit@node01 保留了簇的剩余状态，而 rabbit@node02 和 rabbit@node03 是刚刚初始化的 RabbitMQ。如果想重新初始化 rabbit@node01 的话，需要按照与其它节点相同的步骤进行即可：
>
> 1、停止 RabbitMQ 应用程序
>
> 2、 重置 RabbitMQ
>
> 3、启动 RabbitMQ 应用程序

## 六、RabbitMQ 管理

### 1、主机名更改

RabbitMQ 节点使用主机名相互通信。因此，所有节点名称必须能够解析所有群集对应的名称。像 rabbitmqctl 这样的工具也是如此。除此之外，**默认情况下 RabbitMQ 使用系统的当前主机名来命名数据库目录。如果主机名更改，则会创建一个新的空数据库。**为了避免数据丢失，建立一个固定和可解析的主机名至关重要。每当主机名更改时，应该重新启动 RabbitMQ。如果要使用节点名称的完整主机名（RabbitMQ 默认为短名称），并且可以使用 DNS 解析完整的主机名，则需要修改设置环境变量 `RABBITMQ_USE_LONGNAME=true`

### 2、RAM 节点的群集

RAM 节点只将其元数据保存在内存中。 只有 RAM 节点的集群是脆弱的， 如果群集停止，将无法再次启动， 并将丢失所有数据。

**创建 RAM 节点**

我们可以在首次加入集群时将节点声明为 RAM 节点。像之前一样，我们使用 rabbitmqctl join_cluster 来完成此 操作，但传递 --ram 标志

```
[root@node03 escript]# rabbitmqctl stop_app
Stopping rabbit application on node rabbit@node03 ...

[root@node03 escript]# rabbitmqctl join_cluster --ram rabbit@node01
Clustering node rabbit@node03 with rabbit@node01

[root@node03 escript]# rabbitmqctl start_app
Starting node rabbit@node03 ...
 completed with 3 plugins.
 
[root@node03 escript]# rabbitmqctl cluster_status
Cluster status of node rabbit@node03 ...
[{nodes,[{disc,[rabbit@node01]},{ram,[rabbit@node03]}]},
 {running_nodes,[rabbit@node01,rabbit@node03]},
 {cluster_name,<<"rabbit@node01">>},
 {partitions,[]},
 {alarms,[{rabbit@node01,[]},{rabbit@node03,[]}]}]
```

**更改节点类型**

可以将节点的类型 RAM 更改为 disc，反之亦然。

使用 `change_cluster_node_type` 命令。

```
[root@node03 escript]# ./rabbitmqctl stop_app
Stopping rabbit application on node rabbit@node03 ...

[root@node03 escript]# ./rabbitmqctl change_cluster_node_type disc
Turning rabbit@node03 into a disc node

[root@node03 escript]# ./rabbitmqctl start_app
Starting node rabbit@node03 ...
 completed with 3 plugins.
 
[root@node03 escript]# ./rabbitmqctl cluster_status
Cluster status of node rabbit@node03 ...
[{nodes,[{disc,[rabbit@node01,rabbit@node03]}]},
 {running_nodes,[rabbit@node01,rabbit@node03]},
 {cluster_name,<<"rabbit@node01">>},
 {partitions,[]},
 {alarms,[{rabbit@node01,[]},{rabbit@node03,[]}]}]
```

**常用管理命令**

- 用户权限管理

  RabbitMQ 有一个默认的用户'guest',密码也是"guest",这个用户默认只能通过本机访问，如：`http://localhost:15672`，在通过 http 访问之前记得启用 management 插件。

  要让其他机器可以访问，需要创建一个新用户，并为其分配权限。

  1. 用户管理

     > rabbitmqctl list_users	# 列出所有用户
     >
     > rabbitmqctl add_user {username} {password}	# 添加用户
     >
     > rabbitmqctl delete_user {username}	# 删除用户
     >
     > rabbitmqctl change_password {username} {newpassword}	# 修改密码
     >
     > rabbitmqctl authenticate_user {username} ｛password}	# 用户认证
     >
     > rabbitmqctl clear_password {username}	# 删除密码，密码删除后就不能访问了。
     >
     > rabbitmqctl set_user_tags {username} {tag ...}	# 为用户设置角色，tag 可以是 0 个，一个，或多个。如：`rabbitmqctl set_user_tags chris administrator`，设置为管理员;`rabbitmqctl set_user_tags chris` ，清除 chris 与角色的关联。

  2. 权限管理

     RabbitMQ 客户端连接到一个服务端的时候，在它的操作指令中指定了一个虚拟主机。服务端首先检查是否有访问该虚拟主机的权限，没有权限的会拒绝连接。

     对于 exchanges 和 queues 等资源，位于某个虚拟主机内；不同虚拟主机内即便名称相同也代表不同的资源。当特定操作在资源上执行时第二级访问控制开始生效。

     RabbitMQ 在某个资源上区分了配置、写和读操作。配置操作创建或者销毁资源，或者更改资源的行为。写操作将消息注入进资源之中。读操作从资源中获取消息。

     要执行特定操作用户必须授予合适的权限。

     > rabbitmqctl list_vhosts [vhost info item ...]	# 获取 vhosts 列表
     >
     > rabbitmqctl add_vhost {vhost}	# 添加 vhosts
     >
     > rabbitmqctl delete_vhost ｛hosts}	# 删除 vhosts
     >
     > rabbitmqctl set_permissions [-p vhost] {user} {conf} {write} {read}	# 给用户分配对应的 vhost 上分配相应的权限。如：`rabbitmqctl set_permissions -p /myvhost chris "^chris-.*" ".*" ".*"`
     >
     > rabbitmqctl clear_permissions [-p vhost] {username}	# 清除权限
     >
     > rabbitmqctl list_permissions [-p vhost]	# 清除权限列表
     >
     > rabbitmqctl list_user_permissions {username}	# user 权限列表
     >
     > `rabbitmqctl set_permissions -p / chris ".*" ".*" ".*"` 此时用户chris才有访问队列资源的权限

## 七、高可用配置

![1646a2dbbc08e9c6](assets/1646a2dbbc08e9c6.png)



### 7.1、安装配置HaProxy

 yum install haproxy

### 7.2、安装配置Keepalived

#### HAproxy负载

> 在`192.168.1.219`和`192.168.1.220`做负载

- 安装HAproxy

```
[root@rabbit1 tmp] yum install haproxy
// 编辑配置文件
[root@rabbit1 tmp] vim /etc/haproxy/haproxy.cfg
```

在最后新增配置信息：

```
#######################HAproxy监控页面#########################
listen http_front
        bind 0.0.0.0:1080           #监听端口
        stats refresh 30s           #统计页面自动刷新时间
        stats uri /haproxy?stats    #统计页面url
        stats realm Haproxy Manager #统计页面密码框上提示文本
        stats auth admin:Guo*0820      #统计页面用户名和密码设置
        #stats hide-version         #隐藏统计页面上HAProxy的版本信息

#####################我把RabbitMQ的管理界面也放在HAProxy后面了###############################
listen rabbitmq_admin
    bind 0.0.0.0:15673
    server node1 192.168.3.220:15672
    server node2 192.168.3.221:15672
    server node3 192.168.3.222:15672

#####################RabbitMQ服务代理###########################################
listen rabbitmq_cluster 0.0.0.0:5673
    mode tcp
    stats enable
    balance roundrobin
    #option tcpka
    #option tcplog
    timeout client 3h
    timeout server 3h
    timeout connect 3h
    #balance url_param userid
    #balance url_param session_id check_post 64
    #balance hdr(User-Agent)
    #balance hdr(host)
    #balance hdr(Host) use_domain_only
    #balance rdp-cookie
    #balance leastconn
    #balance source //ip
    server   node1 192.168.3.220:5672 check inter 5s rise 2 fall 3   #check inter 5000 是检测心跳频率，rise 2是2次正确认为服务器可用，fall 3是3次失败认为服务器不可用
    server   node2 192.168.1.221:5672 check inter 5s rise 2 fall 3
    server   node3 192.168.1.222:5672 check inter 5s rise 2 fall 3
```

- 启动HAproxy负载

```
[root@rabbit1 tmp] haproxy -f /etc/haproxy/haproxy.cfg
```

- HAproxy负载完毕

#### Keepalived安装

> 利用keepalived做主备，避免单点问题，实现高可用
>  在 `192.168.1.219`和`192.168.1.220`做主备，前者主，后者备

- 安装Keepalived

```
[root@rabbit1 tmp] yum -y install keepalived
```

- 配置Keepalived生成VIP

```
[root@rabbit1 tmp] vim /etc/keepalived/keepalived.conf
```

部分配置信息（只显示使用到的）：

```
global_defs {
   notification_email {
     acassen@firewall.loc
     failover@firewall.loc
     sysadmin@firewall.loc
   }
   
   notification_email_from Alexandre.Cassen@firewall.loc
   smtp_server 192.168.200.1
   smtp_connect_timeout 30
   router_id LVS_DEVEL
   vrrp_skip_check_adv_addr
   # vrrp_strict    # 注释掉，不然访问不到VIP
   vrrp_garp_interval 0
   vrrp_gna_interval 0
}

# 检测任务
vrrp_script check_haproxy {
    # 检测HAProxy监本
    script "/etc/keepalived/script/check_haproxy.sh"
    # 每隔两秒检测
    interval 2
    # 权重
    weight 2
}

# 虚拟组
vrrp_instance haproxy {
    state MASTER # 此处为`主`，备机是 `BACKUP`
    interface enp4s0f0 # 物理网卡，根据情况而定
    mcast_src_ip 192.168.1.219 # 当前主机ip
    virtual_router_id 51 # 虚拟路由id，同一个组内需要相同
    priority 100 # 主机的优先权要比备机高
    advert_int 1 # 心跳检查频率，单位：秒
    authentication { # 认证，组内的要相同
        auth_type PASS
        auth_pass 1111
    }
    # 调用脚本
    track_script {
        check_haproxy
    }
    # 虚拟ip，多个换行
    virtual_ipaddress {
        192.168.1.222
    }
}
```

-  `/etc/keepalived/script/check_haproxy.sh`内容

```
#!/bin/bash
LOGFILE="/var/log/keepalived-haproxy-status.log"
date >> $LOGFILE
if [ `ps -C haproxy --no-header |wc -l` -eq 0 ];then
    echo "warning: restart haproxy" >> $LOGFILE
    haproxy -f /etc/haproxy/haproxy.cfg
    sleep 2

    if [ `ps -C haproxy --no-header |wc -l` -eq 0 ];then
        echo "fail: check_haproxy status" >> $LOGFILE
        systemctl stop keepalived
    fi
else
    echo "success: check_haproxy status" >> $LOGFILE
fi
```

**解释说明：**

> `Keepalived`组之间的心跳检查并不能察觉到`HAproxy`负载是否正常，所以需要使用此脚本。
>  在`Keepalived`主机上，开启此脚本检测`HAproxy`是否正常工作，如正常工作，记录日志。
>  如进程不存在，则尝试重启`HAproxy`，两秒后检测，如果还没有则关掉主`Keepalived`，此时备`Keepalived`检测到主`Keepalive`挂掉，接管VIP，继续服务

## 常用命令附录

### 常用命令

~~~
查看rabbitMq进程：
# ps -ef | grep rabbitmq

后台启动Rabbitmq Server
# rabbitmq-server -detached   # 在所有节点上启动

查看放行端口：
# firewall-cmd --list-port

命令执行启用插件：
# rabbitmq-plugins enable rabbitmq_management

设置为内存节点：
# rabbitmqctl change_cluster_node_type ram
~~~



