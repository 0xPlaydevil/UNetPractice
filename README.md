# UNetPractice
A practice on Unity3d network.

*简介*

有一个项目需要用到网络，这是提前学习并做的练习。该练习的目标是尝试理解UNet的设计思路和使用方式。

*使用方法*

项目根路径下有一个**配置文件**，其中可以设置是否作服务器/IP等基本配置。本机测试将IP设为127.0.0.1,同时跑两个程序实例，一个作客户端，一个做服务端，配置文件里对应字段填写false/true。**先运行服务端**，再运行客户端。连通时两个程序里都会出现Player。  
角色控制：wsad+鼠标,同第三人称游戏操作方式。鼠标左中右键可以以不同的方式发射不同的子弹。F键可以乘坐载具或从载具下来。载具wsad控制。
