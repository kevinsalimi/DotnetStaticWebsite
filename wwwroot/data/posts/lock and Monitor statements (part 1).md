---
Title: Synchronization by lock and Monitor statements (part 1)
Link: Synchronization by lock and Monitor statements
Summary: Working through enterprise applications has various challenges, among them, multi-threading stands out. Nowadays, all applications tend to use all capabilities of modern hardware, especially CPUs in our case. Performing several jobs simultaneously...
Keywords: multithreading, lock, Monitor
CreationDate: Friday, January 1, 2021
Author: Kaywan Salimi
ArticleId: 9a1c57fb-1fb8-48bd-b8c1-5a71b7caaa3b
DisplayPeriority: 0
---
<div align="center">

## Synchronization by `lock` and `Monitor` statements (part 1)

</div>
    
<div align="center">

  ![multi-threading](/data/Images/Needle-and-thread-.png)
  
</div>

Working through enterprise applications has various challenges, among them, multi-threading stands out. Nowadays, all applications tend to use all capabilities of modern hardware, especially CPUs in our case. Performing several jobs simultaneously by multiple threads brings about more advantages for us like high processing speed, however, without having enough knowledge on this matter, our application outcomes may go toward deterioration.

Fighting for grabbing resources not only belongs to real-life environments but also is the truth on the wire. In such scenarios, we have a situation that must be addressed, otherwise, something undoubtedly happens which commonly called **race condition**. It means an undesirable situation that occurs when a device or system attempts to perform two or more operations in a time.

Our duty as a programmer is to establish certain policies in order to bring fairness and balance among resources and consumers. By doing this, the mess would be wrapped up.

## Table of contents:

* [Synchronization](#synchronization)
* [`lock` statement](#lock-statement)
    * [Choosing the right synchronization object](#choosing-the-right-synchronization-object)
    * [Under the hood](#under-the-hood) 
* [The `Monitor` statement](#the-monitor-statement)
    * [Lock on a Reference Type, Not a Value Type](#lock-on-a-reference-type-not-a-value-type)
    * [`Monitor` Sample](#monitor-sample)
    * [`Monitor` static methods](#monitor-static-methods)
* [Conclusion](#conclusion)
    * [Next mission](#next-mission)

## Synchronization
Whenever two or more threads tend to reach out to a shared-resource at a time, the system needs a synchronization mechanism to ensure that only one thread has exclusive access to the resource. Synchronizing access to data is a safe way to prevent race conditions from happening along with the system.
There are plenty of mechanisms to synchronize an application in .NET. This blog post goes over `lock` and `Monitor` statements.

## `lock` statement
The `lock` is widely accepted due to its simplicity. However, that advantage has a downside: programmers use it without really considering what it does and how to use it efficiently. Knowing it and best practices bring you proper insight when coding in concurrent scenarios.

It ensures that only one thread can enter a critical section of codes. Other threads that try to be the owner of the `lock` would be suspended until the first thread releases the `lock`.


```csharp
static object lockerObject = new object();
int counter = 0;

lock(lockerObject)
{
    counter++;
}
```
As you can see, the `lock` guarantees that only one thread could increment the value of the `counter`. The `lockerObject` is a **synchronizing object** that can be locked by only one thread at a time.

#### Choosing the right synchronization object
Although any object can be used as a synchronization object, there is one hard rule, "it must be a reference type". The synchronization object is typically a private and a static or an instance field. A private member helps to encapsulate the locking object. As a result, it can prevent deadlock, since unrelated code could choose the same object to lock on for different purposes.

Also, you can use `this` object as a synchronizing object. For example:
```csharp
lock(this)
{
    ...
}
```
As mentioned, locking in this way has a serious issue that is not encapsulating the locking logic and can prone to deadlock.

Briefly, the most reliable way for choosing a synchronizing object is to consider a private object in the class by which you can have precise control over it. 

#### Under the hood
The `lock` statement is translated to the following code in C# 3.0:
(The code explained in the [Monitor](#the-monitor-statement) section.)

```csharp
static object locker = new object();
Monitor.Enter(locker);
try
{
    ...
}
finally
{
    Monitor.Exit(locker);
}
```
There is a vulnerability in this code. If an exception is being thrown between `Monitor.Enter()` and `try` due to for example `OutOfMemoryException` or the thread is being aborted, also if `lock` is being taken, it is never released because the thread never gets into the `try` block and causes **leaked lock**.

This danger has been fixed in C# 4.0. the following overload is added to `Monitor.Enter`.
```csharp
public static void Enter(object obj, ref bool lockTaken)
```

Eventually, from C# 4.0 onward the `lock` statement is translated to the following code. As you see, the vulnerability that we saw before has healed.     
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
In short, the `lock` is shorthand for utilizing the `Monitor` statement. Let's take a closer look at it.

## The `Monitor` statement

The `Monitor` is another way of synchronization in .NET with which a lock object has exclusive access to a particular block of code. It is a static class that provides several methods to operate on an object that controls access to the critical section. An object can grab the ownership of the Monitor by calling `Monitor.Enter`, and release it by `Monitor.Exit`. While a thread owns the lock for an object, no other thread can acquire that lock.

The `Monitor` never be released unless the owner of `Monitor` must pass to `Exit`. Therefore, if you pass an object to `Exit` which is not the owner of the Monitor, you will get the `SynchronizationLockException` exception with the message "Object synchronization method was called from an unsynchronized block of code."

#### Lock on a Reference Type, Not a Value Type
Despite the `lock` that only accepts reference types as a lock object, `Enter` accepts reference types and value types. Bear in mind that using value types as a lock object leads to automatic "boxing" (i.e. wrapping), then a reference type would be the ownership. Automatic boxing creates a new object for each invocation of `Enter`, so `Exit` attempts to find the lock for the box object but finds nothing and throws the mentioned exception. That leads to a leaked lock since the owner object never releases the `Monitor`.

The below codes show the point:
```csharp
static int locker;

static void WriteToFile()
{
  Monitor.Enter(locker);
  try
  {
    ...
  }
  finally
  {
    Monitor.Exit(locker);
  }
}
```
If we box the `locker` then passed through the `Enter` the above exception never happens. For example:
```csharp
static int locker;
object lockerObj = (object) locker;

static void WriteToFile()
{
  Monitor.Enter(lockerObj);
  try
  {
    ...
  }
  finally
  {
    Monitor.Exit(lockerObj);
  }
}
```

#### `Monitor` Sample
So far, we have learned concepts and fundamental principles of the `Monitor`. It is time to practice, so let's dive through a short sample.

The below code lunches ten tasks, each of which sleeps for 200 milliseconds and increments the value of the `counter` variable. The critical section enclosed by `Enter` and `Exit` methods so that Our program works well and displays the expected result on the screen.

```csharp
class Example
    {
        private static object locker = new object();
        static void Main(string[] args)
        {
            int taskListLength = 10;
            int counter = 0;

            List<Task> tasks = new List<Task>(taskListLength);
            for (int i = 0; i < taskListLength; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    //Simulate some jobs
                    Thread.Sleep(200);

                    Monitor.Enter(locker);
                    try
                    {
                        //Critical section
                        counter += 1;
                    }
                    finally
                    {
                        Monitor.Exit(locker);
                    }
                }));
            }
            Task.WaitAll(tasks.ToArray());
            Console.WriteLine("The counter value is: {0}", counter);            
        }
    }
    //The Result of the example is:
    //  The counter value is: 10
```

#### `Monitor` static methods
The below table contains static methods of the `Monitor` class with a short description.

| Methods        | Description                                                                   |
|----------------|-------------------------------------------------------------------------------|
| Enter, TryEnter | Acquires a lock for an object. It indicates the start point of the critical section. |
| Wait           | Releases the lock on an object and blocks the current thread until it reacquires the lock.|
| Pulse, PulseAll | While a thread owns the lock for an object, no other thread can acquire that lock.|
| Exit           | Releases the lock on an object. It indicates the endpoint of the critical section.|

## Conclusion
The `lock` statement is an easy way to bring thread safety to your code and allowing only one thread to synchronize access to a particular block of code. We see that the `lock` stems from `Enter/Exit` methods of the `Monitor` class. Also discussed best practices that help us to gain a deeper insight for moving toward thread safety. 

Here is not the end of our story; in the next post, we will go through some practical samples and see how `lock` is used in conjunction with other `Monitor` methods for a more advanced form of thread coordination.


#### Next mission
The post paves the way for the [next mission](http://silentexception.com/article/Synchronization-by-lock-and-Monitor-statements-part-2) in which we will dive into details of `Wait`, `Pulse`, and `PulseAll` then will implement a simple <mark> ***producer-consumer problem***</mark> by the capabilities of `Monitor` static class.
