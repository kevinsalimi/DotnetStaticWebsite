---
Title: Synchronization in CSharp by the lock statement
Summary: this is under development
Keywords: multithreading, lock, monitor, mutex
CreationDate: Tuesday, Decamber 1, 2020
Author: Kevin Salimi
ArticleId: 9a1c57fb-1fb8-48bd-b8c1-5a71b7caaa3b
---

<div align="center">

  ![multi-threading](/data/Images/Needle-and-thread-862x574.jpg)
  
</div>

## Introduction

If you have worked on an enterprise project, concurrency is one of the common scenarios that is inevitable and you have to deal with,
in which several threads attempt to access a shared resource.
When it comes to sharing resources among multiple consumers, the program would run into unexpected problems that must be safely handled.

Fighting for resources ownership is not only belongs to the real life environment but also is true on the wire. 
A problematic situation to control, so-called race condition.
In other words, it's an undesirable situation that occurs when a device or system attempts to perform two or 
more operations at the same time. That's prone to developing a dysfunctional program if not handled in the program. 

Our duty as a programmer is establishing fairness and balancing among resources and consumers in order to wrap the mess up. 

## Synchronization
Whenever two or more threads tend to reaching out a shared resource at the same time, the system needs a synchronization mechanism to ensure that only one thread owns and uses the resource at a time. 
There are plenty of mechanisms to establish a synchronization in .NET, in this article I go over `lock` and `mutex` statements.

### `lock` statement
The `lock` keyword ensures that only one thread can enter a critical or particular section of code. If a thread acquires a `lock`, the second thread that wants to acquire that `lock` is suspended until the first thread releases the `lock`.

```csharp
//Details are removed to be brief
lock(lockerObject)
{
    //statements
    variable++;
}
```
In the above sample code, it can guarantee that only one thread can increment the value of the `variable`. The `lockerObject` is a **synchronizing object** that can be locked by only on thread in a time.
#### Choosing the synchronization object
You can use any object as a synchronization object but there is one hard rule which is ***"It must be a reference type"***. The synchronization object is typically a private and a static or an instance field. A private member helps to encapsulate the locking object. As a result, it can prevent deadlock, since no one from outside of the class has access to it.

In addition, you can use `this` object as a synchronizing object. For example:
```csharp
lock(this){...}
```
Locking in this way has a serious issue that it is not encapsulating the locking logic
also can prone to deadlock.

Simply put, the safest way to choosing a synchronizing object is considering a private object in the class by which you can have a precise control over it. 

#### Under the hood
Actually, `lock` statement would be translated to the following code in C# 3.0:
```csharp
private static object locker = new object();
Monitor.Enter(locker);
try{...}
finally
{
    Monitor.Exit(locker);
}
```
There is a vulnerability in this code. If an exception being thrown between `Monitor.Enter` and `try` due to `OutOfMemoryException` or the thread being aborted, plus if `lock` is taken, it never released because the thread never gets to try block and it causes **leaked lock**.

This danger has fixed in C# 4.0. the following overload is added to `Monitor.Enter`.
```csharp
public static void Enter(object obj, ref bool lockTaken)
```

Eventually, from C# 4.0 onward the `lock` statement is translated to the following code. As you see, the vulnerability that we saw before is healed.     
```csharp
private static object locker = new object();
bool lockTaken = false;
try
{
    Monitor.Enter(locker, ref lockTaken);
    //Statements
}
finally
{
    if(lockTaken)
        Monitor.Exit(locker);
}
```
### `mutex` statement
