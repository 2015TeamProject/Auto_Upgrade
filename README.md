
1. 程序介绍
===========

1.1 程序的根目录
----------------

![](README_IMG/6ddc285391832c29efa8fb0b94665d1f.png)

### 1.1.1 Config目录

![](README_IMG/fe59f56824f1e7b56d4fb10e79d27a28.png)

-   用于存放软件所在主机下的历史配置文件，包括其他软件的配置文件，如下：

![](README_IMG/bee7f610f0ce0755842d87e6b37ab28b.png)

### 1.1.2 RemoteConfig目录

-   远端网站的模拟目录

![](README_IMG/a41a2718e01d8d30960ed86cb15dcab1.png)

### 1.1.3 Resources目录

-   存放软件使用的图标的目录

### 1.1.4 currentConfig.xml、RemoteConfig.xml、UrlConfig.config

![](README_IMG/30a7de5d10d3b520908d62d0f56086bd.png)

-   软件一运行，先**扫描一下程序的根目录**，生成软件的**currentConfig.xml**的配置文件

-   软件接着**根据UrlConfig.config**中保存的url，将指定的**远端配置文件下载**下来，并**命名为RemoteConfig.xml**；若UrlConfig.config中指定的远端配置文件是**不存在**的，则软件会提示指定的远端配置文件资源不存在，并会生成一个**空的RemoteConfig.xml**在本地。

### 1.1.5 UpdateClass.dll

-   软件更新模块的类库，将其封装成类库，可以提供给其他程序使用

![](README_IMG/578774940063bc27e2c65ea198bdffef.png)

### 1.1.6 update.exe

-   软件更新重启辅助小程序，用于更新重启本软件

![](README_IMG/8e125ab3f0ff87496791deb9b4560919.png)

### 1.1.7 Auto Upgrade.exe

-   软件的主程序

![](README_IMG/0517ffb1f8574e6f4efe1cc738f0c44b.png)

2. 使用
=======

2.1 程序启动时
--------------

-   软件一运行，先**扫描一下程序的根目录**，生成软件的**currentConfig.xml**的配置文件

-   软件接着**根据UrlConfig.config**中保存的url，将指定的**远端配置文件下载**下来，并**命名为RemoteConfig.xml**；若UrlConfig.config中指定的远端配置文件是**不存在**的，则软件会提示指定的远端配置文件资源不存在，并会生成一个**空的RemoteConfig.xml**在本地。

![](README_IMG/b587b336a7b767aa7e490ea30cf1200a.png)

![](README_IMG/20f18ebaf2d661b04337962b9ccff8a3.png)

-   接着软件还会对**Config**目录进行扫描，将本地存放的其他配置文件加载到软件列表中。

![](README_IMG/f666c39aea4ff3982743f3218afafee7.png)

![](README_IMG/f272c70d0efc0e33007219b98e157971.png)

2.2 新建配置文件
----------------

-   点击菜单栏“文件”-“新建配置文件”，即可新建一个新的配置文件

![](README_IMG/8b7b018d57811b68e0285c3dcb01837c.png)

![](README_IMG/c8b520ed14e138eca3ab8f332d807e07.png)

-   新建后的配置文件将保存在Config目录下

![](README_IMG/8b86a1a3bcb198c4d1ddf5778ae8042a.png)

![](README_IMG/315e89f21ee9d58c2ff2968a1340dba2.png)

2.3 查看配置文件
----------------

-   点击“查看”

![](README_IMG/56b683c0a9395a5a390822b8c1e765ec.png)

-   即可查看到该配置文件中的内容

![](README_IMG/aaffd9b37b16b4d2aa1257621e6493e7.png)

-   配置文件的实际内容如图

![](README_IMG/05c2e2e7fc4b45641b2d99766f60033b.png)

2.4 修改配置文件内容
--------------------

-   软件支持对本地的配置文件信息进行修改，包括删除配置文件中的目标文件、修改配置文件中的目标文件的更新方式、新增新的目标文件（添加进来的目标文件的md5值是软件自动计算的）；

![](README_IMG/0b438fae7046a94503796d232973d374.png)

-   软件不支持直接地对本地配置文件和远端配置文件进行修改；

![](README_IMG/cfdfe4e8945c95090a8ae949acc5c421.png)

2.5 另存为新的配置文件
----------------------

-   软件提供在别的配置文件的基础上，另存为一个新的配置文件

![](README_IMG/f7854de4e5f4b4107927cd77aae09d13.png)

![](README_IMG/0c0396a9883cbacf950e24fd49b3d1f7.png)

2.6 删除配置文件
----------------

-   软件允许删除Config中的配置文件，通过点击“删除”按钮完成操作

![](README_IMG/91534d7ac9660b6d8b9aea9beb3c1774.png)

![](README_IMG/dec4a92c3106362b2f56e88592c2977c.png)

-   软件的本地配置文件以及远端配置文件是不允许被删除的

![](README_IMG/480761356e0a959bc0c611b0bebe8ac1.png)

2.7 生成版本
------------

-   点击配置文集列表中某一配置文件的“生成版本”按钮

-   生成版本即将**配置文件**和**目标文**件复制到**指定的目录**下

![](README_IMG/273f4bba4b96e8deea1cf3ffe06cca3c.png)

![](README_IMG/6a854da0f521ec98905161456eda4639.png)

![](README_IMG/6034044400a37a12b9efa0849e3d4d44.png)

2.8 设置URL
-----------

![](README_IMG/d8c8f2d9c18bbb1e219f022937d25bac.png)

![](README_IMG/2ec817bf6e6d5bed30f41e0a78567f32.png)

![](README_IMG/fc17efbce2f3e9dd737c950d77665b99.png)

2.9 自动更新
------------

-   根据2.8设置好URL，即可点击“更新”按钮进行更新

![](README_IMG/1e4bd5bb559f56f84f39409eed5151ac.png)

![](README_IMG/641a0cf122a8e659809073caef230b13.png)

-   更新过程会有如图的提示

![](README_IMG/4fbea79ef0c632eb9bdf3528f702a872.png)

![](README_IMG/9292baa8782eb40121c7561e91c70a20.png)

-   更新成功后程序会自动重启

![](README_IMG/009f8c0a7b1bc186d974e2ab90ee6204.png)

2.10 多文档编辑功能
-------------------

-   程序允许多文档进行编辑

![](README_IMG/df90c2980fc77d0f76c91a6bc7142eaa.png)

2.11 更新功能类库
-----------------

![](README_IMG/0274a131869c9aa8486217a58ad26804.png)

-   该类库可以提供给别的软件调用，实现其他软件的更新功能

-   其他软件的配置文件可由本软件进行编辑

### 2.11.1 演示

-   先通过本程序新建一个测试程序的配置文件

![](README_IMG/7d29fef2a9e781a91072813d2a3b0e46.png)

-   生成版本到RemoteConfig目录下

![](README_IMG/d2947d74a831b96efe62d661d902e4a3.png)

-   生成进来的版本

![](README_IMG/795945545d3dd7076be6e368ec1a69cc.png)

-   现有测试程序如下

![](README_IMG/d57083e21c12ab8eac5d3bc2849c2788.png)

-   运行程序，通过设置远端配置文件，即可进行调用UpdateClass.dll中的类库进行更新

![](README_IMG/26cebbe27364968876b1ebdd0167e9bc.png)

![](README_IMG/a50f3fc4ccc1b9874e915c948bf2a416.png)

-   进行更新

![](README_IMG/eb56f705b97284ec89d47849227324e8.png)

-   更新过程会有如图的提示

![](README_IMG/4fbea79ef0c632eb9bdf3528f702a872.png)

![](README_IMG/9292baa8782eb40121c7561e91c70a20.png)

![](README_IMG/457dfd3a0cf584c327b6a5895bfc20ac.png)
