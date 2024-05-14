var builder = WebApplication.CreateBuilder(args);

builder.Services.AddErrorOr(options =>
{
    options.IncludeMetadataInProblemDetails = true;

    options.ErrorToResultMapper.Add(error =>
    {
        return error.NumericType == 421
            ? Results.Ok("this is actually not an error")
            : null;
    });

    options.ErrorToStatusCodeMapper.Add(error =>
    {
        return error.NumericType switch
        {
            421 => StatusCodes.Status307TemporaryRedirect,
            68 => StatusCodes.Status202Accepted,
            _ => null,
        };
    });
});

var app = builder.Build();

app.Run();
