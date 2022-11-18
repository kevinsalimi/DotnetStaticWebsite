---
Title: Why should you use Span<T> in .NET?
Link: why-should-you-use-spant-in-dotnet
Summary: Going over Span<T>, teaches us how we can have a higher performance without memory allocation code and how it is a better alternative to the Substring API. Span<T> reduces the load on GC by minimizing managed memory allocations. Span<T> has some restriction of usages, to overcome them Memory<T> has briefly introduced.
Keywords: Span<T>, Memory<T>, performace, performace in .NET, substring
CreationDate: Monday, October 25, 2021
Author: Kaywan Salimi
ArticleId: 5b1c57fb-1r48-48bd-b8c1-5a71b7q1w2e3
DisplayPeriority: 5
---
<div align="center">

# Why should you use `Span<T>` in .NET?

</div>

<div align="center">

![dotnetnetPerformance<t>](/data/Images/dotnetnetPerformance.png)

</div>

`Span<T>` helps you to have a higher performance without memory allocations. Please bear in mind that the more knowledge you have on performance topics, the more you will become a unique developer. 

Please enjoy ;)

## Table of contents:

* [Warming up](#warming-up)
* [`Span<T>`](#spant)
    * [Slicing](#slicing)
    * [`ReadOnlySpan<T>`](#readonlyspant)
    * [`Span<T>` instead of Substring](#spant-instead-of-substring)
    * [Using `Span<T>` for other types of memory](#using-spant-for-other-types-of-memory)
    * [`Span<T>` Limitations](#spant-limitations)
* [`Memory<T>`](#memoryt)  
* [Conclusion](#conclusion)

## Warming up
Before diving into the `Span<T>`, I want to look over an example then we will optimize it along with the article. 

Let's suppose that we have a string, which is a time of the day (Ex: "192035"). It contains hour, minute, and second. Now, I want to extract those data and make a `TimeSpan` eventually. The option here is using the `Substring` API of the `String` class.
	
```csharp
.
ExtractTimeSpanWithSubstring("192035");
.
.
.
public TimeSpan ExtractTimeSpanWithSubstring(string time)
{
    int hour = int.Parse(time.Substring(0, 2));
    int minute = int.Parse(time.Substring(2, 2));
    int secind = int.Parse(time.Substring(4, 2));

    return new TimeSpan(hour, minute, secind);
}
```
> You may think that the code is a bit verbose though I tend to make it readable as much as possible for junior developers.

The code above works fine though when it comes to performance, there is a matter, which is not trivial. Let's measure it with BenchmarkDotNet.	

Here is the result on my machine:
>**Standard disclaimer:** BenchmarkDotNet results can be very subject to the machine on which a test is run, what else is going on with that machine at the same time, and sometimes seemingly the way the wind is blowing. Your results may vary.
```
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1237 (21H1/May2021Update)
Intel Core i5-8265U CPU 1.60GHz (Whiskey Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=5.0.400
  [Host]     : .NET 5.0.9 (5.0.921.35908), X64 RyuJIT
  DefaultJob : .NET 5.0.9 (5.0.921.35908), X64 RyuJIT


|                       Method |     Mean |    Error |   StdDev |  Gen 0 | Allocated |
|----------------------------- |---------:|---------:|---------:|-------:|----------:|
| ExtractTimeSpanWithSubstring | 73.15 ns | 1.205 ns | 1.068 ns | 0.0305 |      96 B |
```
As you can see, it takes 73.15 ns, and 96B managed memory allocated. The allocation boils down to the Substring function. Substring returns a new string object that would be allocated on the heap memory.

>As you know string data type is an immutable type. It means, they cannot be changed after they have been created. When you change a string, a new string object will be returned.

	
Now, the question is how do you solve this conundrum? the answer is `Span<T>` which will discuss in the following.

	
## `Span<T>`
`Span<T>` is a value type for representing contiguous arbitrary memory. In other words, it is a facade over the array, string, and any contiguous memory almost with no overhead. The main purpose of `Span<T>` is writing low-allocation codes, which leads to cutting down managed memory allocations and so reduces the load on GC.  

There are three types of memory in .NET:
	
* Managed memory (handled by GC)
* Unmanaged memory (Invisible to GC)
* Stack allocated memory

It can wrap all these types into a `Span<T>` and have access to it safely.

![Span<t>](/data/Images/SpanT.png)

example:
```csharp  
byte[] array = new byte[10];
Span<byte> bytes = array; // Implicit cast from byte[] to Span<byte>
```
In the preceding code, the `bytes` points to whole indexes of `array`. By utilizing the `Slice` method of `Span` you can point to a portion of the array without creating a copy.

```csharp
byte[] array = new byte[7];
Span<byte> bytesSpan = array;
bytesSpan[0] = 32; // The first index of array set to 32
bytesSpan[2] = 64; // The third index of array set to
Span<byte> sliceSpan = bytesSpan.Slice(2, 4);
Assert.Equal(sliceSpan[0] == bytesSpan[2]); // OK
sliceSpan[0] = 65;
sliceSpan[5] = 120; //Throws IndexOutOfRangeException
```
`Span<T>` allows you to access and modify the data. It looks like you are working on the original array. As you can see, an array of the `byte` is created and wrapped into a `Span<byte>` called `byteSpan`. From there, it is possible to work with `byteSpan` exactly like the original array. Then, by the `Slice` method, a window is created over index 2 to 5, without making a copy. 

A picture is worth a thousand words:

![Span<t>](/data/Images/SliceSpanT.png)
	
As it is obvious the value of the first index of `sliceSpan` and the third index of `bytesSpan` are the same. In the last second line, the value of the first index of The `sliceSpan` is changed to `65` and The `sliceSpan` has no index of 5, thereby it throws an `IndexOutOfRangeException` exception.

### Slicing
Slicing is the main feature of Span that exposes reusable subsections of Span without making copies. It creates a new Span with a different pointer and length. If you remember from the first example, in .NET whenever we want to substring a string, it allocates a new string and copies the desired result to it. So far, you may feel that how much Span is powerful in such scenarios.

It has two different overloads; the first one specifies only the start index and creates a window from there to the rest of an array. However, the second overload makes room for specifying the length of your Span.
```csharp
public Span<T> Slice(int start);
public Span<T> Slice(int start, int length);
```
	
### `ReadOnlySpan<T>`
It is the other related type of Span that provides a read-only view to work with strings or other immutable types.

```csharp
ReadOnlySpan<char> sample = "This is a string".AsSpan();
```
From there, you can use the `sample` variable in a way you would work with an array.

### `Span<T>` instead of Substring
So far, we learned that what is Span and how we can use it. It is time to get back to our example and rewrite it by using Span. We will benchmark both solutions by BenchmarkDotNet and see how much Span can optimize our code.

```csharp
.
ExtractTimeSpanWithSpan("192035".AsSpan());
.
.
.
public TimeSpan ExtractTimeSpanWithSpan(ReadOnlySpan<char> spanTime)
{
    int hour = int.Parse(spanTime.Slice(0, 2));
    int minute = int.Parse(spanTime.Slice(2, 2));
    int secind = int.Parse(spanTime.Slice(4, 2));

    return new TimeSpan(hour, minute, secind);
}
```
As you can see, there is no big change here. The parameter now is `ReadOnlySpan<char>` and instead of `Substring`, we used `Slice`. Though the performance result is significantly improved.

```csharp
[MemoryDiagnoser]
public class TimeSpanParser
{
    private string time;

    [GlobalSetup]
    public void Setup() => time = "191545";

    [Benchmark]
    public TimeSpan ExtractTimeSpanWithSubstring()
    {
        int hour = int.Parse(time.Substring(0, 2));
        int minute = int.Parse(time.Substring(2, 2));
        int second = int.Parse(time.Substring(4, 2));

        return new TimeSpan(hour, minute, second);
    }

    [Benchmark]
    public TimeSpan ExtractTimeSpanWithSpan()
    {
        ReadOnlySpan<char> spanTime = time.AsSpan();
        int hour = int.Parse(spanTime.Slice(0, 2));
        int minute = int.Parse(spanTime.Slice(2, 2));
        int secind = int.Parse(spanTime.Slice(4, 2));
        
        return new TimeSpan(hour, minute, secind);
    }              
}

```

The benchmark result on my machine is:
```
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1237 (21H1/May2021Update)
Intel Core i5-8265U CPU 1.60GHz (Whiskey Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=5.0.400
  [Host]     : .NET 5.0.9 (5.0.921.35908), X64 RyuJIT
  DefaultJob : .NET 5.0.9 (5.0.921.35908), X64 RyuJIT


|                       Method |     Mean |    Error |   StdDev |  Gen 0 | Allocated |
|----------------------------- |---------:|---------:|---------:|-------:|----------:|
| ExtractTimeSpanWithSubstring | 71.17 ns | 0.581 ns | 0.515 ns | 0.0305 |      96 B |
|      ExtractTimeSpanWithSpan | 46.21 ns | 0.189 ns | 0.177 ns |      - |         - |
```
Obviously, `ExtractTimeSpanWithSpan` (46.21 ns) is almost **54 percent** faster than `ExtractTimeSpanWithSubstring` (71.17 ns) plus without any allocation on the managed memory. The result is tremendously persuasive to use Span in our codes.

> Note that our scenario is a simple one. In complicated scenarios, the performance difference is pretty significant.

### Using `Span<T>` for other types of memory
Because `Span<T>` is an abstraction over an arbitrary block of memory, methods of the `Span<T>` type and methods with `Span<T>` parameters operate on any `Span<T>` object regardless of the kind of memory it encapsulates. "It can be said that, `Span<T>` is an array-like interface over different memories in .NET", [Albahari said](http://www.albahari.com/nutshell/).

In below you see that how can wrap a contiguous unmanaged and stack allocated memory into a Span:

```csharp
IntPtr unmanagedMemory = Marshal.AllocHGlobal(128); 
unsafe
{
    Span<byte> unmanaged = new Span<byte>(unmanagedMemory.ToPointer(), 128); 
    unmanaged[0] = 32; //OK
}
Marshal.FreeHGlobal(unmanagedMemory);
```
On stack memory:
```
Span<byte> stackMemory = stackalloc byte[128];
stackMemory[0] = 32; //OK
```

### `Span<T>` Limitations
`Span<T>` is a `ref struct`. What is `ref struct`?! It is a Struct that would always live on stack memory not on the heap. There is may a confusion that `struct` is a value type and can only live on the stack, yet in some occasions `struct` can be allocated on the heap; for example:

* When boxing happens. It means, if a `struct` or generally any of the value types cast into a reference type, it would be allocated on the heap memory.
```csharp
struct MyStruct { }
static void Main(string[] args)
{
    MyStruct myStruct = new MyStruct();
    object boxed = myStruct; //boxing
}
```
* When `struct` declared as a member of a class.
```csharp
struct MyStruct { }
class MyClass
{
    public MyStruct StructProperty { get; set; }
}
```
In the case of `ref struct`, it guarantees that the struct only lives on stack. This is a enough reason to have some limitation to work with `Span<T>`:

* `Span<T>` cannot be a member of a class.
* `Span<T>` cannot be used in an `async` method. The result is whenever `async & await` are used, an `AsyncMethodBuilder` is created. The builder creates an asynchronous state machine, and in some situations might put parameters of the method on the heap so that `Span<T>` violates this rule.

There are many situation that you can read more in [here](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/struct#ref-struct).

## `Memory<T>`
.NET has a facility to overcome those limitations. `Memory<T>` acts quite like `Span<T>` except it can be placed on the managed heap because it is not a `ref struct`. That means, it can be used in an async method or be a member of a class. Besides, it has another related data type `ReadOnlyMemory<T>` to work with immutable data types like string. 

>This article would not cover `Memory<T>`, you can learn more in [here](https://docs.microsoft.com/en-us/dotnet/api/system.memory-1?view=net-5.0).

## Conclusion

Going over `Span<T>`, teaches us how we can have a higher performance without memory allocation code and how it is a better alternative to the `Substring` API. `Span<T>` reduces the load on GC by minimizing managed memory allocations. `Span<T>` has some restriction of usages, to overcome them `Memory<T>` has briefly introduced. 

I hope the post works for you, and persuades you to use Span in your future code. 