using Xunit;

namespace Toyana.E2E.Tests;

[CollectionDefinition("ToyanaE2E")]
public class ToyanaTestCollection : ICollectionFixture<ToyanaApiFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
