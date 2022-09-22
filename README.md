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
- [Getting Started](#getting-started)
  - [Single Error](#single-error)
    - [This 👇🏽](#this-)
    - [Turns into this 👇🏽](#turns-into-this-)
    - [This 👇🏽](#this--1)
    - [Turns into this 👇🏽](#turns-into-this--1)
  - [Multiple Errors](#multiple-errors)
- [A more practical example](#a-more-practical-example)
- [Dropping the exceptions throwing logic](#dropping-the-exceptions-throwing-logic)
- [Usage](#usage)
  - [Creating an `ErrorOr<result>`](#creating-an-errororresult)
    - [From Value, using implicit conversion](#from-value-using-implicit-conversion)
    - [From Value, using `From`](#from-value-using-from)
    - [From Single Error](#from-single-error)
    - [From List of Errors, using implicit conversion](#from-list-of-errors-using-implicit-conversion)
    - [From List of Errors, using `From`](#from-list-of-errors-using-from)
  - [Checking if the `ErrorOr<result>` is an error](#checking-if-the-errororresult-is-an-error)
  - [Accessing the `ErrorOr<result>` result](#accessing-the-errororresult-result)
    - [Accessing the Value](#accessing-the-value)
    - [Accessing the List of Errors](#accessing-the-list-of-errors)
    - [Accessing the First Error](#accessing-the-first-error)
  - [Performing actions based on the `ErrorOr<result>` result](#performing-actions-based-on-the-errororresult-result)
    - [`Match`](#match)
    - [`MatchFirst`](#matchfirst)
    - [`Switch`](#switch)
    - [`SwitchFirst`](#switchfirst)
  - [Error Types](#error-types)
    - [Built-in Error Types](#built-in-error-types)
    - [Custom error types](#custom-error-types)
    - [Why would I want to categorize my errors?](#why-would-i-want-to-categorize-my-errors)
  - [Built in result types](#built-in-result-types)
- [How Is This Different From `OneOf<T0, T1>` or `FluentResults`?](#how-is-this-different-from-oneoft0-t1-or-fluentresults)
- [Contribution](#contribution)
- [Credits](#credits)
- [License](#license)

# Give it a star ⭐!

Loving it? Show your support by giving this project a star!

# Getting Started

## Single Error

### This 👇🏽

```csharp
User GetUser(Guid id = default)
{
    if (id == default)
    {
        throw new ValidationException("Id is required");
    }

    return new User(Name: "Amichai");
}
```

```csharp
try
{
    var user = GetUser();
    Console.WriteLine(user.Name);
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}
```

### Turns into this 👇🏽

```csharp
ErrorOr<User> GetUser(Guid id = default)
{
    if (id == default)
    {
        return Error.Validation("Id is required");
    }

    return new User(Name: "Amichai");
}
```

```csharp
errorOrUser.SwitchFirst(
    user => Console.WriteLine(user.Name),
    error => Console.WriteLine(error.Description));
```

### This 👇🏽

```csharp
void AddUser(User user)
{
    if (!_users.TryAdd(user))
    {
        throw new Exception("Failed to add user");
    }
}
```

### Turns into this 👇🏽

```csharp
ErrorOr<Created> AddUser(User user)
{
    if (!_users.TryAdd(user))
    {
        return Error.Failure(description: "Failed to add user");
    }

    return Result.Created;
}
```

## Multiple Errors

Internally, the `ErrorOr` object has a list of `Error`s, so if you have multiple errors, you don't need to compromise and have only the first one.

```csharp
public class User
{
    public string Name { get; }

    private User(string name)
    {
        Name = name;
    }

    public static ErrorOr<User> Create(string name)
    {
        List<Error> errors = new();

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

        return new User(firstName, lastName);
    }
}
```

```csharp
public async Task<ErrorOr<User>> CreateUserAsync(string name)
{
    if (await _userRepository.GetAsync(name) is User user)
    {
        return Error.Conflict("User already exists");
    }

    var errorOrUser = User.Create("Amichai");

    if (errorOrUser.IsError)
    {
        return errorOrUser.Errors;
    }

    await _userRepository.AddAsync(errorOrUser.Value);
    return errorOrUser.Value;
}
```

# A more practical example

```csharp
[HttpGet("{id:guid}")]
public async Task<IActionResult> GetUser(Guid Id)
{
    var getUserQuery = new GetUserQuery(Id);

    ErrorOr<User> getUserResponse = await _mediator.Send(getUserQuery);

    return getUserResponse.Match(
        user => Ok(_mapper.Map<UserResponse>(user)),
        errors => ValidationProblem(errors.ToModelStateDictionary()));
}
```
A nice approach, is creating a static class with the expected errors. For example:

```csharp
public static partial class Errors
{
    public static class User
    {
        public static Error NotFound = Error.NotFound("User.NotFound", "User not found.");
        public static Error DuplicateEmail = Error.Conflict("User.DuplicateEmail", "User with given email already exists.");
    }
}
```

Which can later be used as following

```csharp

User newUser = ..;
if (await _userRepository.GetByEmailAsync(newUser.email) is not null)
{
    return Errors.User.DuplicateEmail;
}

await _userRepository.AddAsync(newUser);
return newUser;
```

Then, in an outer layer, you can use the `Error.Match` method to return the appropriate HTTP status code.

```csharp
return createUserResult.MatchFirst(
    user => CreatedAtRoute("GetUser", new { id = user.Id }, user),
    error => error is Errors.User.DuplicateEmail ? Conflict() : InternalServerError());
```

# Dropping the exceptions throwing logic

You have validation logic such as `MediatR` behaviors, you can drop the exceptions throwing logic and simply return a list of errors from the pipeline behavior

```csharp
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    private readonly IValidator<TRequest>? _validator;

    public ValidationBehavior(IValidator<TRequest>? validator = null)
    {
        _validator = validator;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        if (_validator == null)
        {
            return await next();
        }

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (validationResult.IsValid)
        {
            return await next();
        }

        return TryCreateResponseFromErrors(validationResult.Errors, out var response)
            ? response
            : throw new ValidationException(validationResult.Errors);
    }

    private static bool TryCreateResponseFromErrors(List<ValidationFailure> validationFailures, out TResponse response)
    {
        List<Error> errors = validationFailures.ConvertAll(x => Error.Validation(
                code: x.PropertyName,
                description: x.ErrorMessage));

        response = (TResponse?)typeof(TResponse)
            .GetMethod(
                name: nameof(ErrorOr<object>.From),
                bindingAttr: BindingFlags.Static | BindingFlags.Public,
                types: new[] { typeof(List<Error>) })?
            .Invoke(null, new[] { errors })!;

        return response is not null;
    }
}
```

# Usage

## Creating an `ErrorOr<result>`

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

### From Value, using `From`

```csharp
ErrorOr<int> result = ErrorOr.From(5);
```

```csharp
public ErrorOr<int> GetValue()
{
    return ErrorOr.From(5);
}
```

### From Single Error

```csharp
ErrorOr<int> result = Error.Unexpected();
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

### From List of Errors, using `From`

```csharp
ErrorOr<int> result = ErrorOr<int>.From(new List<Error> { Error.Unexpected(), Error.Validation() });
```

```csharp
public ErrorOr<int> GetValue()
{
    return ErrorOr<int>.From(List<Error>
    {
        Error.Unexpected(),
        Error.Validation()
    };
}
```

## Checking if the `ErrorOr<result>` is an error

```csharp
if (errorOrResult.IsError)
{
    // errorOrResult is an error
}
```

## Accessing the `ErrorOr<result>` result

### Accessing the Value

```csharp
ErrorOr<int> result = 5;

var value = result.Value;
```

### Accessing the List of Errors

```csharp
ErrorOr<int> result = new List<Error> { Error.Unexpected(), Error.Validation() };

List<Error> value = result.Errors; // List<Error> { Error.Unexpected(), Error.Validation() }
```

```csharp
ErrorOr<int> result = Error.Unexpected();

List<Error> value = result.Errors; // List<Error> { Error.Unexpected() }
```

### Accessing the First Error

```csharp
ErrorOr<int> result = new List<Error> { Error.Unexpected(), Error.Validation() };

Error value = result.FirstError; // Error.Unexpected()
```

```csharp
ErrorOr<int> result = Error.Unexpected();

Error value = result.FirstError; // Error.Unexpected()
```

## Performing actions based on the `ErrorOr<result>` result

### `Match`

Actions that return a value on the value or list of errors

```csharp
string foo = errorOrString.Match(
    value => value,
    errors => $"{errors.Count} errors occurred.");
```

### `MatchFirst`

Actions that return a value on the value or first error

```csharp
string foo = errorOrString.MatchFirst(
    value => value,
    firstError => firstError.Description);
```

### `Switch`

Actions that don't return a value on the value or list of errors

```csharp
errorOrString.Switch(
    value => Console.WriteLine(value),
    errors => Console.WriteLine($"{errors.Count} errors occurred."));
```

### `SwitchFirst`

Actions that don't return a value on the value or first error

```csharp
errorOrString.SwitchFirst(
    value => Console.WriteLine(value),
    firstError => Console.WriteLine(firstError.Description));
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
}
```

Creating a new Error instance is done using one of the following static methods:

```csharp
public static Error Error.Failure(string code, string description);
public static Error Error.Unexpected(string code, string description);
public static Error Error.Validation(string code, string description);
public static Error Error.Conflict(string code, string description);
public static Error Error.NotFound(string code, string description);
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

# How Is This Different From `OneOf<T0, T1>` or `FluentResults`?

It's similar to the others, just aims to be more intuitive and fluent.
If you find yourself typing `OneOf<User, DomainError>` or `Result.Fail<User>("failure")` again and again, you might enjoy the fluent API of `ErrorOr<User>` (and it's also faster).

# Contribution

If you have any questions, comments, or suggestions, please open an issue or create a pull request 🙂

# Credits

- [OneOf](https://github.com/mcintyre321/OneOf/tree/master/OneOf) - An awesome library which provides F# style discriminated unions behavior for C#

# License

This project is licensed under the terms of the [MIT](https://github.com/mantinband/error-or/blob/main/LICENSE) license.
