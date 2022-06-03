---
Title: Inversion Of Control (IOC) in simple words
Link: Inversion Of Control in simple words
Summary: Inversion of control at first glance may seem an obscure term though when you define it in simple sentences, you would realize how much it is understandable and can bring advantages to your application. In this post, I tried explaining it as simple as and as understandable as possible for junior developers.
Keywords: IOC,Inversion Of Control, DI, dependency injection
CreationDate: Friday, June 25, 2021
Author: Kevin Salimi
ArticleId: 4a1c57fb-1r48-48bd-b8c1-5a71b7q1w2e3
DisplayPeriority: 4
---

<div align="center">

## Inversion Of Control (IOC) in simple words

</div>

<div align="center">

  ![inversion-of-control](/data/Images/inversionOfControl1.png)
  <div class="post-date" style="text-align: left;">
    <span>Designed on</span>
    <a href="https://www.canva.com/"> canva</a>
 </div>
</div>

This post is the second part of simple words in which I try to explain some concepts of programming in plain English for junior developers. We have clarified the [dependency injection](https://silentexceptions.com/article/Dependency-injection-in-simple-words) in the last one and learned some basic concepts about it. In this post, we will discuss the IOC principle.

## Table of contents:

* [Warming up](#warming-up)
* [What do you mean by `Control`?](#what-do-you-mean-by-control)
* [`IOC` as a cure](#ioc-as-a-cure)
* [What is the difference between IOC and DI?](#what-is-the-difference-between-ioc-and-di)
* [DI/IOC container](#diioc-container)
* [Conclusion](#conclusion)


### Warming up
Inversion of control at first glance may seem an obscure term though when you define it in simple sentences, you would realize how much it is understandable and can bring advantages to your application. 

### What do you mean by `Control`?
Whenever a class is responsible for instantiating its own dependency, it can be said that the class has control. What kind of control are we talking here? *control means creating a new object of a class by the `new` keyword.*

For example in the code below, class `A` has the control of instantiating a new object of class `B`.
```csharp
public class A
{
	private B _b;
	public A()
	{
		_b = new B();
	}
}

public class B
{
}
```

The IOC tends to invert the relation in a way that class `B` should no longer be created by class `A`.  Let me recap some points from [the last post](https://silentexceptions.com/article/Dependency-injection-in-simple-words) and see what is wrong with this code.

* First, class `A` strongly depends on class `B`, which indicates that `A` cannot protect itself from possible changes of `B`. 
* Second, tightly coupled code is impossible to unit test. 

 
### `IOC` as a cure
To fix the problem, we should relieve the control of a class `A`. In short, we invert the control and give it to others to create a new instance of class `B` and pass it to class `A` through the constructor. 
```csharp
public class A
{
	private B _b;
	public A(B b)
	{
		_b = b;
	}
}

```
In the preceding code, class `A` is no longer in control of creating an instance of class `B`. This technique may be familiar to you; yes! you are right! It is called *dependency injection(DI)*. In short, DI is a technique for achieving the Inversion of control between classes and their dependencies.

 
### What is the difference between `IOC` and `DI`?
Indeed, DI is a form of IOC and there are plenty of types of IOC:

* Using dependency injection pattern
* Using Service locator pattern
* Factory pattern
* and more...

the bullets would not be covered in this article, yet the main point is, **IOC is a high-level principle in programming which is implemented through different fashions.**

### DI/IOC container
As time goes by, software gradually gets grows, so writing a testable and maintainable code would not be as simple as the first starting days of the project. DI containers (a.k.a IOC container) help us handle the creation of instances all over the program, which leads to having a loosely coupled application. DI container fills this gap in the application.

 
>DI container is a framework by which we can automatically handle the dependency injection among the classes of your application.

There are alot of DI containers out there; toppin the charts are:
* [**Castle Windsor**](http://www.castleproject.org/projects/windsor/)
* [**StructureMap**](http://structuremap.github.io/)
* [**Spring.NET**](http://www.springframework.net/)
* [**Autofac**](https://autofac.org/)
* [**Unity**](http://codeplex.com/unity)
* [**Ninject**](http://ninject.org/)

### Conclusion
As we discussed, IOC is a high-level principle in programming and has different ways of implementation, one of which is the Dependency Injection. IOC brings us a loosely coupled software that every part of it could be tested and easy to maintain.
