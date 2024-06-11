namespace ErrorOr.IntegrationTests.Mvc;

// ReSharper disable once UnusedType.Global -- test methods are in base class
public sealed class MvcApiTests : BaseApiTests<MvcApiFixture>, IClassFixture<MvcApiFixture>
{
    public MvcApiTests(MvcApiFixture apiFixture)
        : base(apiFixture)
    {
    }
}
