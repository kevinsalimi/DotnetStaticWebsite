---
Title: Dependency injection in simple words
Link: Dependency injection in simple words
Summary: In this article I tried to make DI as simple and as understandable as possible not only for juniors but also for those who feel uncomfortable with dependency injection.
Keywords: DI, dependency injection, testability
CreationDate: Tuesday, May 4, 2021
Author: Kaywan Salimi
ArticleId: 9a1c57fb-1fb8-48bd-b8c1-5a71b7q1w2e3
DisplayPeriority: 3
---

<div align="center">

## Dependency injection in simple words

</div>

<div align="center">

  ![dependency-injection](/data/Images/Dependency-Injection.png)
  
</div>

When I was a junior developer, some concepts were hard for me to grasp such as **DI(dependency injection)**, **IOC(inversion of control)**, and so on. The more I wrapped my head around these topics, the more questions popped into my mind. I stuck in the definition, let alone their pros and cons. Unfortunately, the top google articles are not as obvious as possible for junior programmers so that in this article I tried to make DI ([IOC may next time!](https://silentexception.com/article/Inversion-Of-Control-in-simple-words)) as simple and as understandable as possible not only for juniors but also for those who feel uncomfortable with DI.


## Table of contents:

* [Warming up](#warming-up)
* [What is DI?](#what-is-di)
* [Example](#example)
* [Different types of DI](#different-types-of-di)
    * [Constructor Injection](#constructor-injection)
    * [Property Injection](#property-injection)
    * [Method Injection](#method-injection)
* [Testability](#testability)
* [Conclusion](#conclusion)

### Warming up
Coding skill is not enough to make you a professional developer. After learning Topics like DI and other principles of programming, then your puzzle pieces should fall into place and give you a bird-eyes view of programming and more confidence to make software better. Let’s make the story short and get started.


### What is DI?
**Dependency injection is a simple concept and all about giving an object its instance variables.** Just like that!
 
Let’s take one step back and look at the whole story. A system is formed by many components that subtly cooperate; those components can also be decomposed into plenty of classes that have internal connectivity with each other. This situation leads to getting inevitable coupling among classes that should be handled. Otherwise, the code would not be maintainable. Dependency injection solves and manages this problem, then facilitates our code to have testability.
 
### Example
Let’s assume that we have two classes `A` and `B`. Class `A` consumes `B` through the constructor. In short, they say `_b` is a dependency of class `A`;

```csharp
class A
{
    private B _b;
    public A()
    {
        _b = new B();
    }
}

class B
{
}

```

In the preceding code, we can see that the class `A`  depends directly on the class `B`. You may ask, so What? Which is not out of the question.
 
What happens if I add a parameter to the constructor of class `B`? Like below:

```csharp
class B
{
    public B(string data)
    {

    }
}

```
After that, we got a compile error in the constructor of class `A`, which states that `There is no argument given that corresponds to the required formal parameter 'data'`. The point is, *the change happened in class `B` though we feel it in class `A`*. This simple example shows how changes can be problematic for scenarios in which dependencies are created directly in the consumer class. (the class `A` in our example)
 
DI comes to our rescue. DI says **“give classes whatever they want.”** Let’s change class `A`.


```csharp
class A
{
    private B _b;
    public A(B b)
    {
        _b = b;
    }
}

```
Now, class `A` does not depend on class `B` directly and the change that we have done on the constructor of class `B` constructor has nothing to do with class `A` . In other words, class `A` is no longer responsible for creating its required objects. If we get back to the short definition of DI above, an object is an instance of class `A` and its variable is `_b` that is provided from outside class `A`.
```csharp
public class Program
{
    public static void Main(string[] args)
    {
        B b = new B("Sample data");
        A a = new A(b);
    }
}
```

>The major benefit of using Dependency Injection is that the client class does not need to be aware of how to create the dependencies. All the client class needs to know is how to interact with those dependencies. 


### Different types of DI
#### Constructor Injection
This type of injection uses the constructor to pass the dependencies of a class. People take this type of injection over other types yet you should choose it based on some reasons. If your class has a dependency that without which the class gets dysfunctional, you should use constructor injection. In other words, if your class cannot work without a dependency, then inject it via the constructor. Moreover, stay alert that if the dependency has a lifetime longer than a method, you can go with constructor Injection.
 
#### Property Injection
It’s also known as *setter injection*. This type uses a property to pass a dependency. Since there is no guarantee that the dependency will be injected when the class is instantiated. It’s considered an optional injection and hides the dependency.
 
#### Method Injection
It is useful when the dependency is different for each operation or it has many implementations. Furthermore, It’s mainly used when you want to supply dependencies to an existing consumer. [(learn more)](https://freecontent.manning.com/understanding-method-injection/)

### Testability
DI has plenty of advantages, among which testability stands out. Since you pass the class all its requirements, you can create mock and fake implementations as an argument for testing classes without modifying the code. That doesn't mean that you should inject every dependency a class has since it must help you in making the class more testable and the system more maintainable. So you have got to ask yourself whether a dependency helps from a testing perspective to inject it from the outside or if it helps make your application more flexible.

>Bear in mind that this article is a starting point for you and DI is so beyond this short article. [(learn more)](https://www.youtube.com/watch?v=QtDTfn8YxXg)

### Conclusion 
DI is a powerful and useful technique that helps you have flexible, maintainable, loosely coupled, and testable codes. As you saw there are different types of DI that you should pick one given to the necessity. Besides, we learned how DI makes room for testability, which is difficult to ignore in software development. Note that the article is a short introduction to DI and still there is a lot of information out there around the topic that you should know as a developer.


