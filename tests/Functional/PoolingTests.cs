using Extensions.ML;
using System;
using Xunit;

namespace Functional
{
    /// <summary>
    /// This is a loose coverage of pooling behaviour so that if we switch out the underlying pool
    /// we have a couple of safe-guards. I'm not trying to re-test all the behaviour of the underlying pool
    /// since it has its own tests.
    /// </summary>
    public class PoolingTests
    {
        [Fact]
        public void single_consumer_reuses_engine()
        {
        }
    }
}
