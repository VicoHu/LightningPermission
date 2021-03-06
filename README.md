<h2 align="center">LightningPermission</h2>

#### 作用范围

**LightningPermission** 是一个为 **.Net Core 3 Web应用** 开发的一个**用于控制接口权限的中间件插件。**



#### 它有什么作用？

**LightningPermission** 主要用于对 WebAPI 应用 和 ASP.NET 应用，提供接口级和控制器级的权限控制。



#### 它的优势是什么？

- **将复杂的权限控制，简化到仅需要配置一个特性，仅需一行简单代码**
- **可以实现控制器级批量权限控制，也可以实现某一接口的权限控制**

- **对访问权限控制和业务逻辑进行解耦**
  - 开发者只需要关心某个Web接口的业务逻辑和数据处理，而不需要关心权限的访问权限问题
  - 权限可以交由其他人进行更改和调整，只需要修改允许或拒绝的权限身份字段，而不需要写任何业务代码，既可以完成权限的控制
- **高度自由化设计**
  - 开发者可以自己定义中间件从请求中获取权限的方式
  - 开发者可以自己定义中间件在获取该控制器或者方法的权限范围后的具体操作，例如：定义拒绝请求时返回的内容
  - 开发者可以继承 PermissionLifeCycle 生命周期类 后，对任意一个生命周期方法进行重写
- **提供一系列的权限获取、增加、修改的方法**
  - 开发者只需要传入数据库连接字符串，中间件就会自动完成对数据库权限表的创建
  - 开发者可以利用内置的权限获取或者操作方法，增加某用户对应权限，或者获取或修改某用户的唯一 Token 字符串



#### 为什么开发这款中间件插件

解耦访问权限控制和业务逻辑，解决程序员既要完成业务逻辑和数据处理，又要完成权限控制时，业务逻辑散乱且复杂的痛点。

