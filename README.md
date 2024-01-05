<div align="center">

<img src="assets/icon.png" alt="drawing" width="700px"/></br>

[![NuGet](https://img.shields.io/nuget/v/erroror.svg)](https://www.nuget.org/packages/erroror)

[![Build](https://github.com/amantinband/error-or/actions/workflows/build.yml/badge.svg)](https://github.com/amantinband/error-or/actions/workflows/build.yml) [![publish ErrorOr to nuget](https://github.com/amantinband/error-or/actions/workflows/publish.yml/badge.svg)](https://github.com/amantinband/error-or/actions/workflows/publish.yml)

[![GitHub contributors](https://img.shields.io/github/contributors/amantinband/error-or)](https://GitHub.com/amantinband/error-or/graphs/contributors/) [![GitHub Stars](https://img.shields.io/github/stars/amantinband/error-or.svg)](https://github.com/amantinband/error-or/stargazers) [![GitHub license](https://img.shields.io/github/license/amantinband/error-or)](https://github.com/amantinband/error-or/blob/main/LICENSE)
[![codecov](https://codecov.io/gh/amantinband/error-or/branch/main/graph/badge.svg?token=DR2EBIWK7B)](https://codecov.io/gh/amantinband/error-or)
---

### A simple, fluent discriminated union of an error or a result.

`dotnet add package ErrorOr`

</div>

- [Give it a star ⭐!](#give-it-a-star-)
- [Getting Started 🏃](#getting-started-)
  - [Replace throwing exceptions with `ErrorOr<T>`](#replace-throwing-exceptions-with-errorort)
  - [Return Multiple Errors When Needed](#return-multiple-errors-when-needed)
  - [`ErrorOr<T>` in ASP.NET](#errorort-in-aspnet)
  - [Organizing your errors](#organizing-your-errors)
- [Mediator + FluentValidation + `ErrorOr` 🤝](#mediator--fluentvalidation--erroror-)
- [Usage 🛠️](#usage-️)
  - [Creating an `ErrorOr<TResult>`](#creating-an-errorortresult)
    - [From Value, using implicit conversion](#from-value-using-implicit-conversion)
    - [From Value, using `ErrorOrFactory.From`](#from-value-using-errororfactoryfrom)
    - [From Single Error](#from-single-error)
    - [From List of Errors, using implicit conversion](#from-list-of-errors-using-implicit-conversion)
    - [Accessing the List of Errors (`result.Errors`)](#accessing-the-list-of-errors-resulterrors)
    - [Accessing the First Error (`result.FirstError`)](#accessing-the-first-error-resultfirsterror)
    - [Accessing the Errors or an empty list (`result.ErrorsOrEmptyList`)](#accessing-the-errors-or-an-empty-list-resulterrorsoremptylist)
  - [Performing actions based on the `ErrorOr<result>` result](#performing-actions-based-on-the-errororresult-result)
    - [`Match` / `MatchAsync`](#match--matchasync)
    - [`MatchFirst` / `MatchFirstAsync`](#matchfirst--matchfirstasync)
    - [`Switch` / `SwitchAsync`](#switch--switchasync)
    - [`SwitchFirst` / `SwitchFirstAsync`](#switchfirst--switchfirstasync)
    - [`Then` / `ThenAsync`](#then--thenasync)
    - [`Else` / `ElseAsync`](#else--elseasync)
  - [Error Types](#error-types)
    - [Built-in Error Types](#built-in-error-types)
    - [Custom error types](#custom-error-types)
    - [Why would I want to categorize my errors?](#why-would-i-want-to-categorize-my-errors)
  - [Built in result types](#built-in-result-types)
- [How Is This Different From `OneOf<T0, T1>` or `FluentResults`? 🤔](#how-is-this-different-from-oneoft0-t1-or-fluentresults-)
- [Contribution 🤲](#contribution-)
- [Credits 🙏](#credits-)
- [License 🪪](#license-)

# Give it a star ⭐!

Loving it? Show your support by giving this project a star!

# Getting Started 🏃

## Replace throwing exceptions with `ErrorOr<T>`

This 👇

```cs
try
{
    var result = Divide(4, 2);
    Console.WriteLine(result * 2); // 4
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
    return;
}

public float Divide(int a, int b)
{
    if (b == 0)
    {
        throw new Exception("Cannot divide by zero");
    }

    return a / b;
}
```

Turns into this 👇

```cs
var result = Divide(4, 2);

if (result.IsError)
{
    Console.WriteLine(result.FirstError.Description);
    return;
}

Console.WriteLine(result.Value * 2); // 4

public ErrorOr<float> Divide(int a, int b)
{
    if (b == 0)
    {
        return Error.Unexpected(description: "Cannot divide by zero");
    }

    return a / b;
}
```

Or, using [Then](#then--thenasync)/[Else](#else--elseasync) and [Switch](#switch--switchasync)/[Match](#match--matchasync), you can do this 👇

```cs

Divide(4, 2)
    .Then(val => val * 2) // you can chain multiple `Then` methods. They will only be invoked if the result is not an error
    .SwitchFirst(
        onValue: Console.WriteLine, // 4
        onFirstError: error => Console.WriteLine(error.Description));

public ErrorOr<float> Divide(int a, int b); // same as above
```

## Return Multiple Errors When Needed

Internally, the `ErrorOr` object has a list of `Error`s, so if you have multiple errors, you don't need to compromise and have only the first one.

```csharp
public class User(string _name)
{
    public static ErrorOr<User> Create(string name)
    {
        List<Error> errors = [];

        if (name.Length < 2)
        {
            errors.Add(Error.Validation(description: "Name is too short"));
        }

        if (name.Length > 100)
        {
            errors.Add(Error.Validation(description: "Name is too long"));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            errors.Add(Error.Validation(description: "Name cannot be empty or whitespace only"));
        }

        if (errors.Count > 0)
        {
            return errors;
        }

        return new User(name);
    }
}
```

##  `ErrorOr<T>` in ASP.NET

```csharp
[HttpGet("{id:guid}")]
public async Task<IActionResult> GetUser(Guid Id)
{
    var getUserQuery = new GetUserQuery(Id);

    ErrorOr<User> getUserResponse = await _mediator.Send(getUserQuery);

    return getUserResponse
        .Then(user => _mapper.Map<UserResponse>(user)) // will only be invoked if the result is not an error
        .Match(onValue: Ok, onError: Problem);
}
```

## Organizing your errors

A nice approach, is creating a static class with the expected errors. For example:

```csharp
public static partial class DivisionErrors
{
    public static Error CannotDivideByZero = Error.Unexpected(
        code: "Division.CannotDivideByZero",
        description: "Cannot divide by zero.");
}
```

Which can later be used as following 👇

```csharp
public ErrorOr<float> Divide(int a, int b)
{
    if (b == 0)
    {
        return DivisionErrors.CannotDivideByZero;
    }

    return a / b;
}
```

Then, in an outer layer, you can use the `Error.Match` method to return the appropriate HTTP status code.

```csharp
return Divide(a, b)
    .MatchFirst(
        onValue: user => CreatedAtRoute("GetUser", new { id = user.Id }, user),
        onError: error => error is DivisionErrors.CannotDivideByZero ? BadRequest() : InternalServerError());
```

# [Mediator](https://github.com/jbogard/MediatR) + [FluentValidation](https://github.com/FluentValidation/FluentValidation) + `ErrorOr` 🤝

A common approach when using `MediatR` is to use `FluentValidation` to validate the request before it reaches the handler.

Usually, the validation is done using a `Behavior` that throws an exception if the request is invalid.

Using `ErrorOr`, we can create a `Behavior` that returns an error instead of throwing an exception.

This plays nicely when the project uses `ErrorOr`, as the layer invoking the `Mediator`, similar to other components in the project, simply receives an `ErrorOr` and can handle it accordingly.

Here is an example of a `Behavior` that validates the request and returns an error if it's invalid 👇

```csharp
public class ValidationBehavior<TRequest, TResponse>(IValidator<TRequest>? validator = null)
    : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : IErrorOr
{
    private readonly IValidator<TRequest>? _validator = validator;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validator is null)
        {
            return await next();
        }

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (validationResult.IsValid)
        {
            return await next();
        }

        var errors = validationResult.Errors
            .ConvertAll(error => Error.Validation(
                code: error.PropertyName,
                description: error.ErrorMessage));

        return (dynamic)errors;
    }
}
```

# Usage 🛠️

## Creating an `ErrorOr<TResult>`

There are implicit converters from `TResult`, `Error`, `List<Error>` to `ErrorOr<TResult>`

### From Value, using implicit conversion

```csharp
ErrorOr<int> result = 5;
```

```csharp
public ErrorOr<int> GetValue()
{
    return 5;
}
```

### From Value, using `ErrorOrFactory.From`

```csharp
ErrorOr<int> result = ErrorOrFactory.From(5);
```

```csharp
public ErrorOr<int> GetValue()
{
    return ErrorOrFactory.From(5);
}
```

### From Single Error

```csharp
ErrorOr<int> result = Error.Unexpected(me);
```

```csharp
public ErrorOr<int> GetValue()
{
    return Error.Unexpected();
}
```

### From List of Errors, using implicit conversion

```csharp
ErrorOr<int> result = new List<Error> { Error.Unexpected(), Error.Validation() };
```

```csharp
public ErrorOr<int> GetValue()
{
    return new List<Error>
    {
        Error.Unexpected(),
        Error.Validation()
    };
}
```

### Accessing the List of Errors (`result.Errors`)

```csharp
ErrorOr<int> result = new List<Error> { Error.Unexpected(), Error.Validation() };

List<Error> value = result.Errors; // List<Error> { Error.Unexpected(), Error.Validation() }
```

```csharp
ErrorOr<int> result = Error.Unexpected();

List<Error> value = result.Errors; // List<Error> { Error.Unexpected() }
```

### Accessing the First Error (`result.FirstError`)

```csharp
ErrorOr<int> result = new List<Error> { Error.Unexpected(), Error.Validation() };

Error value = result.FirstError; // Error.Unexpected()
```

```csharp
ErrorOr<int> result = Error.Unexpected();

Error value = result.FirstError; // Error.Unexpected()
```

### Accessing the Errors or an empty list (`result.ErrorsOrEmptyList`)

```csharp
ErrorOr<int> result = new List<Error> { Error.Unexpected(), Error.Validation() };

List<Error> errors = result.ErrorsOrEmptyList; // List<Error> { Error.Unexpected(), Error.Validation() }
```

```csharp
ErrorOr<int> result = ErrorOrFactory.From(5);

List<Error> errors = result.ErrorsOrEmptyList; // List<Error> { }
```

## Performing actions based on the `ErrorOr<result>` result

### `Match` / `MatchAsync`

Actions that return a value on the value or list of errors

```csharp
string foo = errorOrString.Match(
    value => value,
    errors => $"{errors.Count} errors occurred.");
```

```csharp
string foo = await errorOrString.MatchAsync(
    value => Task.FromResult(value),
    errors => Task.FromResult($"{errors.Count} errors occurred."));
```

### `MatchFirst` / `MatchFirstAsync`

Actions that return a value on the value or first error

```csharp
string foo = errorOrString.MatchFirst(
    value => value,
    firstError => firstError.Description);
```

```csharp
string foo = await errorOrString.MatchFirstAsync(
    value => Task.FromResult(value),
    firstError => Task.FromResult(firstError.Description));
```

### `Switch` / `SwitchAsync`

Actions that don't return a value on the value or list of errors

```csharp
errorOrString.Switch(
    value => Console.WriteLine(value),
    errors => Console.WriteLine($"{errors.Count} errors occurred."));
```

```csharp
await errorOrString.SwitchAsync(
    value => { Console.WriteLine(value); return Task.CompletedTask; },
    errors => { Console.WriteLine($"{errors.Count} errors occurred."); return Task.CompletedTask; });
```

### `SwitchFirst` / `SwitchFirstAsync`

Actions that don't return a value on the value or first error

```csharp
errorOrString.SwitchFirst(
    value => Console.WriteLine(value),
    firstError => Console.WriteLine(firstError.Description));
```

```csharp
await errorOrString.SwitchFirstAsync(
    value => { Console.WriteLine(value); return Task.CompletedTask; },
    firstError => { Console.WriteLine(firstError.Description); return Task.CompletedTask; });
```

### `Then` / `ThenAsync`

Multiple methods that return `ErrorOr<T>` can be chained as follows:

```csharp
static ErrorOr<string> ConvertToString(int num) => num.ToString();
static ErrorOr<int> ConvertToInt(string str) => int.Parse(str);

ErrorOr<string> errorOrString = "5";

ErrorOr<string> result = errorOrString
    .Then(str => ConvertToInt(str))
    .Then(num => ConvertToString(num))
    .Then(str => ConvertToInt(str))
    .Then(num => ConvertToString(num));
```

```csharp
static ErrorOr<string> ConvertToString(int num) => num.ToString();
static Task<ErrorOr<string>> ConvertToStringAsync(int num) => Task.FromResult(ErrorOrFactory.From(num.ToString()));
static Task<ErrorOr<int>> ConvertToIntAsync(string str) => Task.FromResult(ErrorOrFactory.From(int.Parse(str)));

ErrorOr<string> errorOrString = "5";

ErrorOr<string> result = await errorOrString
    .ThenAsync(str => ConvertToIntAsync(str))
    .ThenAsync(num => ConvertToStringAsync(num))
    .ThenAsync(str => ConvertToIntAsync(str))
    .ThenAsync(num => ConvertToStringAsync(num));

// mixing `ThenAsync` and `Then`
ErrorOr<string> result = await errorOrString
    .ThenAsync(str => ConvertToIntAsync(str))
    .Then(num => ConvertToString(num))
    .ThenAsync(str => ConvertToIntAsync(str))
    .Then(num => ConvertToString(num));
```

If any of the methods return an error, the chain will break and the errors will be returned.

### `Else` / `ElseAsync`

The `Else` / `ElseAsync` methods can be used to specify a fallback value in case the state is error anywhere in the chain.

```csharp
// ignoring the errors
string result = errorOrString
    .Then(str => ConvertToInt(str))
    .Then(num => ConvertToString(num))
    .Else("fallback value");

// using the errors
string result = errorOrString
    .Then(str => ConvertToInt(str))
    .Then(num => ConvertToString(num))
    .Else(errors => $"{errors.Count} errors occurred.");
```

```csharp
// ignoring the errors
string result = await errorOrString
    .ThenAsync(str => ConvertToInt(str))
    .ThenAsync(num => ConvertToString(num))
    .ElseAsync(Task.FromResult("fallback value"));

// using the errors
string result = await errorOrString
    .ThenAsync(str => ConvertToInt(str))
    .ThenAsync(num => ConvertToString(num))
    .ElseAsync(errors => Task.FromResult($"{errors.Count} errors occurred."));
```

## Error Types

### Built-in Error Types

Each error has a type out of the following options:

```csharp
public enum ErrorType
{
    Failure,
    Unexpected,
    Validation,
    Conflict,
    NotFound,
    Unauthorized,
}
```

Creating a new Error instance is done using one of the following static methods:

```csharp
public static Error Error.Failure(string code, string description, Dictionary<string, object>? metadata = null);
public static Error Error.Unexpected(string code, string description, Dictionary<string, object>? metadata = null);
public static Error Error.Validation(string code, string description, Dictionary<string, object>? metadata = null)
public static Error Error.Unexpected(string code, string description, Dictionary<string, object>? metadata = null);
public static Error Error.Conflict(string code, string description, Dictionary<string, object>? metadata = null);
public static Error Error.NotFound(string code, string description, Dictionary<string, object>? metadata = null);
public static Error Error.Unauthorized(string code, string description, Dictionary<string, object>? metadata = null);
```

The `ErrorType` enum is a good way to categorize errors.

### Custom error types

You can create your own error types if you would like to categorize your errors differently.

A custom error type can be created with the `Custom` static method

```csharp
public static class MyErrorTypes
{
    const int ShouldNeverHappen = 12;
}

var error = Error.Custom(
    type: MyErrorTypes.ShouldNeverHappen,
    code: "User.ShouldNeverHappen",
    description: "A user error that should never happen");
```

You can use the `Error.NumericType` method to retrieve the numeric type of the error.

```csharp
var errorMessage = Error.NumericType switch
{
    MyErrorType.ShouldNeverHappen => "Consider replacing dev team",
    _ => "An unknown error occurred.",
};
```

### Why would I want to categorize my errors?

If you are developing a web API, it can be useful to be able to associate the type of error that occurred to the HTTP status code that should be returned.

If you don't want to categorize your errors, simply use the `Error.Failure` static method.

## Built in result types

There are a few built in result types:

```csharp
ErrorOr<Success> result = Result.Success;
ErrorOr<Created> result = Result.Created;
ErrorOr<Updated> result = Result.Updated;
ErrorOr<Deleted> result = Result.Deleted;
```

Which can be used as following

```csharp
ErrorOr<Deleted> DeleteUser(Guid id)
{
    var user = await _userRepository.GetByIdAsync(id);
    if (user is null)
    {
        return Error.NotFound(code: "User.NotFound", description: "User not found.");
    }

    await _userRepository.DeleteAsync(user);
    return Result.Deleted;
}
```

# How Is This Different From `OneOf<T0, T1>` or `FluentResults`? 🤔

It's similar to the others, just aims to be more intuitive and fluent.
If you find yourself typing `OneOf<User, DomainError>` or `Result.Fail<User>("failure")` again and again, you might enjoy the fluent API of `ErrorOr<User>` (and it's also faster).

# Contribution 🤲

If you have any questions, comments, or suggestions, please open an issue or create a pull request 🙂

# Credits 🙏

- [OneOf](https://github.com/mcintyre321/OneOf/tree/master/OneOf) - An awesome library which provides F# style discriminated unions behavior for C#

# License 🪪

This project is licensed under the terms of the [MIT](https://github.com/mantinband/error-or/blob/main/LICENSE) license.
