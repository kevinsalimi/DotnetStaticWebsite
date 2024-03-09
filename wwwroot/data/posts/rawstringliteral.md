---
Title: Raw string literal feature in C# 11
Link: Raw string literal in Csharp11
Summary: Working with string data types has been improving gradually in different versions of C#. At first glance, they may seem like small changes though they help us in the way that how to deal with strings subtly and usefully. Verbatim string(`@`) and string interpolation (`$`) are a few of those facilities that C# offers during these years. Along those lines, in this short blog, we're going to look over the raw string literal in C# 11.
Keywords: C#11, raw string literal
CreationDate: Monday, Jul 7, 2022
Author: Kaywan Salimi
ArticleId: 9a1c5744-1fb8-48bd-k4c1-5a711qaz2wpp
DisplayPriority: 7
---

<div align="center">

# Raw string literal feature in C# 11

</div>
<div align="center">

  ![generic-list](/data/Images/raw-string-literal.png)
  
</div>



Working with string data types has been improving gradually in different versions of C#. At first glance, they may seem like small changes though they help us in the way that how to deal with strings subtly and usefully. Verbatim string(`@`) and string interpolation (`$`) are a few of those facilities that C# offers during these years. Along those lines, in this short blog, we're going to look over the raw string literal in C# 11. 

If you work with JSON, XML, HTML, SQL, Regex, and others in C#, this feature is the right choice for you.

> This post is already published on the [Techspire Co](https://techspire.nl/raw-string-literal-feature-in-csharp11/) blog.
## Table of contents:

* [Prior C# 11](#prior-c-11)
* [Raw string literal](#raw-string-literal)
* [Number of `$`](#number-of)
* [Number of `"`](#number-of-1)
* [Sum up](#sum-up)

## Prior C# 11

Before C# 11, if you want to have a JSON in your code, you may end up with something like this:

```csharp
string jsonVariable = "{\"FirstName\":\"K1\", \"LastName\": \"Salimi\"}";

Console.WriteLine(jsonVariable);
//output:
	//{"FirstName":"K1", "LastName": "Salimi"}

``` 

Or thanks to verbatim string, your code would be somthing like this:

```csharp
string jsonVariable = @"
    {
        ""FirstName"": ""K1"", 
        ""LastName"": ""Salimi""
    }";
    
Console.WriteLine(jsonVariable);
//output:
    //{
    //    "FirstName": "K1",
    //    "LastName": "Salimi"
    //}
```
It's more readable than the first one, but what if we want to use string interpolation in the JSON string?! In the previous examples, it's not allowed to use string interpolation, otherwise, it leads to a compilation error due to the curly brackets of the JSON.

However, there is another way to do so which is not wise of course:

```csharp
string firstName = "K1";
string lastName = "Salimi";

string jsonVariable = 
    "{ \n"
    +
        $" \"FirstName\" : \"{firstName}\", \n"
        +
        $" \"LastName\" : \"{lastName}\", \n" 
    +
    "}";

Console.WriteLine(jsonVariable);
//output:
	//{ 
	// "FirstName" : "K1", 
	// "LastName" : "Salimi", 
	//}
```

Let's see how the raw string literal can rescue us.

## Raw string literal
The raw string literal is a new feature to support multi-line string that can be interpolated by `$`. Also you can add even extra double quotes or curly brackets in your string.

```csharp
string firstName = "K1";
string lastName = "Salimi";

var jsonVariable =
$$"""
    {
        "FirstName": "{{firstName}}",
        "LastName": "{{lastName}}"
    }
""";

Console.WriteLine(jsonVariable);
//output:
    //{
    //    "FirstName": "K1",
    //    "LastName": "Salimi"
    //}
```

> When you want to use C# 11, since it's in the preview version, don't forget to use the `EnablePreviewFeatures` tag in your .csproj file. 
> `<EnablePreviewFeatures>true</EnablePreviewFeatures>`

The raw string literal `$$""" ... """` starts with at least two `$` and three `"`, also ends with three `"`. As you can see, it's very handful.

## Number of `$`
The number of `$` that prefixes the string equals the number of curly brackets that are required to indicate a nested code expression. It means that if there are two `$`, we need to use two curly brackets to specify the interpolated part of a string, likewise, for three `$`, three curly brackets are required.

For example:

```csharp
string firstName = "K1";
string lastName = "Salimi";

var jsonVariable =
$$$"""
    {{// two curly brackets can be printed because of three $
        "FirstName": "{{{firstName}}}",
        "LastName": "{{{lastName}}}"
    }
""";

Console.WriteLine(jsonVariable);
//output:
    //{{
    //    "FirstName": "K1",
    //    "LastName": "Salimi"
    //}
```
Note that, **the number of curly brackets that can be printed in the output is the number of `$` that prefixes the string minus one.**

## Number of `"`
The same rule is here for `"`. As I mentioned, the raw string literal starts with three `"` and the number of quotes that can be in the output is the number of prefix quotes minus one which means that in the previous example, we could show `""` in the output.

Let's see three double quotes in the output:
To do that first we need to stat the raw string literal with four double quotes. `""""`

```csharp
string firstName = "K1";
string lastName = "Salimi";

var jsonVariable =
$$""""
    {
        """FirstName""": "{{firstName}}",
        """LastName""": "{{lastName}}"
    }
"""";

Console.WriteLine(jsonVariable);
//output:
//    {
//        """FirstName""": "K1",
//        """LastName""": "Salimi"
//    }
```

## Sum up
In this short post, we see how to use the raw string literal in C# 11 which is useful for cases like JSON strings which need multi-line strings and support interpolation. It also improves our code in terms of readability and maintenance.


