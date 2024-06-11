namespace ErrorOr.IntegrationTests.MinimalApi;

// ReSharper disable once UnusedType.Global -- test methods are in base class
public sealed class MinimalApiTests : BaseApiTests<MinimalApiFixture>, IClassFixture<MinimalApiFixture>
{
    public MinimalApiTests(MinimalApiFixture apiFixture)
        : base(apiFixture)
    {
    }
}
