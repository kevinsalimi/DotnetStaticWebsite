---
Title: Synchronization by lock and Monitor statements (part 2)
Link: Synchronization by lock and Monitor statements part 2
Summary: In this part, we go over the Monitor class and its best practices, also a consumer-producer problem is implemented by the Monitor capabilities.
Keywords: multithreading, lock, Monitor
CreationDate: Monday, February 1, 2021
Author: Kevin Salimi
ArticleId: 9a1c57fb-1fb8-48bd-b8c1-5a71b7cbba45
DisplayPeriority: 1
---

<div align="center">

## Synchronization by lock and Monitor statements (part 2)

</div>

<div align="center">

  ![multithreading](/data/Images/multithreading.jpg)
  
</div>

[In the last post](http://silentexception.com/article/Synchronization-by-lock-and-Monitor-statements), we learned what synchronization is and why we need it in our codes. Then `lock` and `Monitor` statements are introduced, and some samples are also demonstrated. The `Monitor` functionalities are beyond `Enter` and `Exit`. In this post, we look over `Monitor` methods and implement a simple consumer-producer queue.

## Table of contents:

* [Signaling](#signaling)
* [`Monitor.Wait(object)`](#waitobject)
* [`Monitor.Wait(object, int)`](#waitobject-int)
* [`Pulse` and `PulseAll`](#pulse-and-pulseall)
* [`Pulse` and `PulseAll` efficiency comparing](#pulse-and-pulseall-efficiency-comparing)
* [producer-consumer problem](#producer-consumer-problem)
* [Sum up](#sum-up)

## Signaling
The purpose of signaling is establishing cooperation among a bunch of threads, where they can notify each other to go ahead and do something or to halt. 
Imagine a thread that needs to wait for a resource to be provided or replenished; whenever the resource is available, the thread would be informed by other threads and resume its work. It might remind you of the [producer-consumer problem](https://en.wikipedia.org/wiki/Producer%E2%80%93consumer_problem) in which we have two actors; a producer(provides data) and a consumer(processes data).

As discussed, signaling is a mechanism by which threads interact with each other. 
In .NET, One of the common ways of implementing signaling is the `Monitor` static class. 
The class provides `Wait`, `Pulse`, and `PulseAll` methods for achieving synchronization.

>Bear in mind that the above methods are designed for use within `lock` statements; otherwise, they throw exceptions.

## `Wait(object)`
When `Monitor.Wait` is called, the caller releases the ownership of the `lock` 
on the object and goes to waiting-queue until it receives a signal by `Pulse` or `PulseAll` 
from other threads (if there is one) then comes back to ready-queue and also reacquires the 
`lock` again. In other words, a thread calls `Wait` for being alert to changing states by other threads.
Note that `Wait` returns `true` if it requires the `lock` unless it never returns.

Let's look at an example:
```csharp
    class Sample
    {
        private static object locker = new object();
        static void Main(string[] args)
        {
            Thread[] threads = new Thread[3];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(WaitSample);
                threads[i].Name = $"Thread {i}";
                threads[i].Start();
            }
        }

        static void WaitSample()
        {
            lock (locker)
            {
                Console.WriteLine($"{Thread.CurrentThread.Name} has entered the critical section.");
                Monitor.Wait(locker);
            }
        }
    }
    
    //The result of the example is: 
        //Thread 0 has entered the critical section.
        //Thread 1 has entered the critical section.
        //Thread 2 has entered the critical section.
```
As you can see, when the `Thread 0` comes through the critical section that is enclosed by the `lock`, 
a message comes up on the screen that indicates `Thread 0 has entered the critical section.`. 
Next, the `Monitor.Wait` is called and the `Thread 0` releases the lock and blocks. 
Now, the `Thread 1` comes to the play and acquires the `lock`. 
The same scenario goes on also for the `Thread 1` and `Thread 2`. 
So far, we have three threads in the waiting-queue and wait forever since there is no thread to send a signal and bring them back to the ready-queue.

## `Wait(object, int)`

As long as the above sample doesn't call `Pulse` or `PulseAll` the threads would be blocked indefinitely. 
To prevent this, You should use the below constructor:
```csharp
public static bool Wait (object obj, int millisecondsTimeout);
```
The `millisecondsTimeout` indicates the duration of time in which the thread waits for a 
signal from other threads on the same `lock` object. 
After the time elapses, the thread comes back to the ready-queue and reacquires the `lock`.
The `Wait` returns `true` if the `lock` was reacquired before the specified time elapsed; 
`false` if the `lock` was reacquired after the specified time elapsed; finally, the method does not return until the lock is reacquired.

> The time-out ensures that the current thread does not block indefinitely.

> There are other constructors of `Monitor.Wait` that are not covered in this post. You can learn more about them [here](https://docs.microsoft.com/en-us/dotnet/api/system.threading.monitor.wait?view=net-5.0).

## `Pulse` and `PulseAll`
The `PulseAll` releases the entire waiting-queue of waiting threads while 
the `Pulse` releases a single thread at the head of waiting-queue. In effect, 
`PulseAll` moves threads from waiting-queue to ready-queue so they can resume in an orderly fashion.


Let's complete the previous example:
```csharp
    class Sample2
    {
        private static object locker = new object();
        static void Main(string[] args)
        {
            Thread[] threads = new Thread[3];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(WaitSample);
                threads[i].Name = $"Thread {i}";
                threads[i].Start();
            }
            
            //Simulate some jobs
            Thread.Sleep(500);

            lock (locker)
            {
                Console.WriteLine("PulseAll has just called.");
                Monitor.PulseAll(locker);
            }
        }

        static void WaitSample()
        {
            lock (locker)
            {
                Console.WriteLine($"{Thread.CurrentThread.Name} has entered the critical section.");
                Monitor.Wait(locker);
                Console.WriteLine($"{Thread.CurrentThread.Name} woken!");
            }
        }

        //The result of the example is: 
            //Thread 0 has entered the critical section.
            //Thread 1 has entered the critical section.
            //Thread 2 has entered the critical section.
            //PulseAll has just called.
            //Thread 0 woken!
            //Thread 1 woken!
            //Thread 2 woken!
    }
```
Now, what is going to happen after calling `PulseAll`?
Indeed, it tells the CLR that if there are any threads in the waiting-queue, let them resume and bring them back to the ready-queue. In the end, as you can see the result, they are woken up one after another.

So far, we have learned how exactly the `Monitor` class works. Now let's peak up a challenge and solve it by using `Monitor` class capabilities.

## Producer-consumer problem
producer-consumer problem is a pretty common scenario in software development in which there are two types of actor:

* One or more producers that provide the data 
* One or more consumers responsible for processing those data. 

Both use a queue or other data structure as a bedrock for storing data jointly. The most challenging thing is synchronization among threads since only one shared resource exists, and race conditions might have happened.

![Producer-Consumer](/data/Images/Producer-Consumer.jpg)


Now, we want to implement a producer-consumer problem by the capabilities of the `Monitor` class.
I chose a bakery as an example in which baguettes are baked then consumed by people. If bread runs out, 
people wait until trays are refilled.

```csharp
    public struct Baguette
    {
        public string Name { get; set; }
    }
    
    public class Bakery
    {
        //Shared resource
        private Queue<Baguette> _baguetteQueue;
        public Bakery()
        {
            _baguetteQueue = new Queue<Baguette>();
        }

        //Consumer
        public void BringBaguette(Action<Baguette> action)
        {
            lock (_baguetteQueue)
            {
                while (_baguetteQueue.Count == 0)
                    Monitor.Wait(_baguetteQueue);

                action.Invoke(_baguetteQueue.Dequeue());
            }
        }

        //Producer
        public void RefillTray(Baguette[] freshBaguettes)
        {
            lock (_baguetteQueue)
            {
                foreach (var item in freshBaguettes)
                    _baguetteQueue.Enqueue(item);

                Monitor.PulseAll(_baguetteQueue);
            }
        }
    }
```
Let's spell out the code. The `Bakery` class contains two methods. The first method is the `BringBaguette` that plays a consumer role and responsible for retrieving an item for processing. The second method is the `RefillTray`, a producer that accepts a collection of `Baguette` and `Enqueue` them to the `_baguetteQueue`.

The synchronization object in both methods is `_baguetteQueue` that guarantees only one of them can go ahead at the time. In other words, when the producer is going to add some data to the queue the consumer is waiting and vice versa.

> Bear in mind that In order for `Wait` to communicate with `Pulse` or `PulseAll`, the synchronizing object (_baguetteQueue, in our case) must be the same.

#### `RefillTray` method
The caller sends a collection of `Baguette` into the method. At the first step of the code, 
the caller thread grabs the `lock` then appends the collection of `Baguette` to the `_baguetteQueue`. Finally, `PulseAll` is called, where it sends a signal for releasing all threads that are stuck in the waiting-queue then they are allowed to resume.

#### `BringBaguette` method
The method informs the client by an `Action` which has been already sent as an argument.
Moreover, there is a `while` loop that iterates until the queue is empty. 
Whenever the queue is empty the current thread will be locked by calling `Monitor.Wait`. 

> This is just a sample for being familiar with the `Monitor` class and its usages. 
The producer-consumer problem has a different implementation in .NET. 
One of the elegant and practical one of which is the [BlockingCollection](https://docs.microsoft.com/en-us/dotnet/api/system.collections.concurrent.blockingcollection-1?view=net-5.0).

### `Pulse` and `PulseAll` efficiency comparing
In terms of efficiency, `Pulse` beats `PulseAll` since `Pulse` gets only one thread back to life,
and the other threads have no overhead on the process. On the other hand, `PulseAll` kicks all 
waiting threads into life. In this situation, if there is only one item in the queue,
the first thread at the head of the waiting-queue could go ahead, and the rest of them should 
get back to sleep. Therefore, As you can see the `PulseAll` works on all threads that may not 
need to wake up.

## Sum up
Signaling is one way of synchronization by which threads can communicate with each other. 
When it comes to signaling, the `Monitor` static class is a viable way for bringing synchronization
to our codes. 
Threads would go to sleep by calling `Monitor.Wait` and 
be woken up by `Monitor.Pulse` or `Monitor.PulseAll`. The `Monitor` class is a proper candidate 
the implementation of the producer-consumer problem in .NET.


