---
Title: Why should we care about the capacity of List<T>?
Link: Why should we care about the capacity of generic lists
Summary: Have you ever thought about generic lists under the hood? Do you know what's happening when a generic list is created? How about the internal array of generic lists? Or how do you use them efficiently? In this article, you can find answers and even more.
Keywords: GenericList, performance
CreationDate: Saturday, March 13, 2021
Author: Kaywan Salimi
ArticleId: 9a1c57fb-1fb8-48bd-b8c1-5a711qaz2wsx
DisplayPriority: 2
---

<div align="center">

##  Why should we care about the capacity of `List<T>`?

</div>
<div align="center">

  ![generic-list](/data/Images/genericList2.jpg)
  
</div>


`List<T>` is a strongly typed list of objects in C# which is pretty hands-on and straightforward. It provides many capabilities namely storing, searching and manipulating lists. The generic list is a **wrapper around an array** whose size grows dynamically and facilitates you to work on collections. Let's look inside and reveal its details.

> Generic lists comes under `System.Collections.Generic` namespace. You can see the source code in [here](https://github.com/dotnet/coreclr/blob/master/src/System.Private.CoreLib/shared/System/Collections/Generic/List.cs).

## Table of contents:

* [Constructors](#constructors)
* [`Capacity` vs `Count`](#capacity-vs-count)
* [Add a new item to the list](#add-a-new-item-to-the-list)
* [Tell me the number](#tell-me-the-number)
* [Why should you always set the capacity of `List<T>`?](#why-should-you-always-set-the-capacity-of-listt)
* [Sum up](#sum-up)

## Constructors

`List<T>` has three overloaded constructors which are described in the below table.

| Constructors | Description                                             |
|--------------|---------------------------------------------------------|
|`public List()`                       |Constructs a List. The list is initially empty and has a capacity of zero. Upon adding the first element to the list the capacity is increased to `DefaultCapacity` which is four and then increased in multiples of two as required.|
|`public List(int capacity)`            |Constructs a List with a given initial capacity. The list is initially empty but will have room for the given number of elements before any reallocations are required.|
|`public List(IEnumerable<T> collection)`|Constructs a List, copying the contents of the given collection. The size and capacity of the new list will both be equal to the size of the given collection.|

As already mentioned, **generic lists use an [internal array](https://github.com/microsoft/referencesource/blob/5697c29004a34d80acdaf5742d7e699022c64ecd/mscorlib/system/collections/generic/list.cs#L40) as a data structure to store data.** Whenever you create a new instance of `List<T>` you can specify the `Capacity` of the list in the constructor, as a result, the argument is considered as the internal array length. If you do not specify it, the `DefaultCapacity` would be chosen as a default value.

```Csharp
private const int DefaultCapacity = 4;
```
Now, two questions may pop into your mind:
* What happens if I add more than four items to the list? 
* How does `List<T>` handle the capacity of the internal array?

Before answering them, let me explain two concepts.

### `Capacity` vs `Count`
The  `Capacity` indicates the maximum amount of items that the internal array can contain while the `Count` shows the size of the internal array which means how many items exist in it.

For example in the code below, there is only one item in the `list` while its capacity is four. Since I do not specify the capacity of the `list`, the default capacity is intended.

```Csharp
List<int> list = new List<int>();
list.Add(1);
Console.WriteLine($"The count is: {list.Count}");
Console.WriteLine($"The capacity is: {list.Capacity}");

//The Result of the example is:
    //The count is: 1
    //The capacity is: 4
```

### Add a new item to the list
Let's get back to the questions. By adding a new item to the `list` if its size is greater than the capacity, the capacity of the list gets doubled. Indeed, the capacity will be increased two times from the current capacity. Having changed the capacity, the current items are copied to a **new array** with a new length as many as the doubled capacity; let's see an example.

In the code below, you can see how the capacity of the `list` would be doubled when the fifth item adds to the `list`.

```Csharp
List<int> list = new List<int>();
list.Add(1);
list.Add(2);
list.Add(3);
list.Add(4);
Console.WriteLine($"The count is: {list.Count}");
Console.WriteLine($"The capacity is: {list.Capacity}");
list.Add(5);
Console.WriteLine($"The count is: {list.Count}");
Console.WriteLine($"The capacity is: {list.Capacity}");

//The Result of the example is:
    //The count is: 4
    //The capacity is: 4
    //The count is: 5
    //The capacity is: 8
```
After adding the last item, a new internal array is created with length 8 which contains 1, 2, 3, 4, 5. If we keep continue adding items to it, as soon as the `count` gets 9, a new array will be created with length 16 and current items will be copied to it again. So, the more you add, the more arrays you dispose in memory.

### Tell me the number
Let's measure the wasted resources by BenchmarkDotNet if we don't care about capacity. It is a simple example; adding 10,000 items to a list we'll see the difference between specifying the `capacity` and the default mode.

```csharp
[MemoryDiagnoser]
public class ListCapacityBenchmark
{
    [Benchmark]
    public void RunWithoutCapacity()
    {
        List<int> list = new List<int>();
        for (int i = 0; i < 10_000; i++)
        {
            list.Add(i);
        }
    }

    [Benchmark]
    public void RunWithCapacity()
    {
        List<int> list = new List<int>(10_000);
        for (int i = 0; i < 10_000; i++)
        {
            list.Add(i);
        }
    }
}
```
Here is the result on my computer:

>**Standard disclaimer:** BenchmarkDotNet results can be very subject to the machine on which a test is run, what else is going on with that machine at the same time, and sometimes seemingly the way the wind is blowing. Your results may vary.
```
// * Summary *

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1706 (21H2)
Intel Core i5-8265U CPU 1.60GHz (Whiskey Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=5.0.400
  [Host]     : .NET 5.0.9 (5.0.921.35908), X64 RyuJIT
  DefaultJob : .NET 5.0.9 (5.0.921.35908), X64 RyuJIT


|             Method |     Mean |    Error |   StdDev |   Median |   Gen 0 | Allocated |
|------------------- |---------:|---------:|---------:|---------:|--------:|----------:|
| RunWithoutCapacity | 57.35 us | 2.506 us | 6.987 us | 59.29 us | 41.6260 |    128 KB |
|    RunWithCapacity | 40.90 us | 2.703 us | 7.667 us | 43.42 us | 12.6343 |     39 KB |
```
As you can see, not only the `RunWithCapacity` is faster the other one but also it has been allocated only 39 KB while the `RunWithoutCapacity` takes 128 KB.


## Why should you always set the capacity of `List<T>`?

We should always indicate the capacity in the constructor even if you have only a rough estimation on how many items will be added to the list because relocating the internal array into a new one with a new length is just wasting resources and the memory stuffed with many useless arrays that must be reclaimed by the GC which has a cost on the performance of your program.

## Sum up

This article goes over the generic lists and its related concepts such as `Capacity` and `Count`. We learned how data is stored in a list under the hood. Furthermore, we figured out how to use generic lists efficiently in terms of memory management.




