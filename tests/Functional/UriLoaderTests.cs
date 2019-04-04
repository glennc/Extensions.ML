using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Extensions.ML
{
    public class UriLoaderTests
    {
        [Fact]
        public void throw_until_started()
        {
            var services = new ServiceCollection()
                .AddOptions()
                .AddLogging();
            var sp = services.BuildServiceProvider();

            var loaderUnderTest = ActivatorUtilities.CreateInstance<UriModelLoader>(sp);
            Assert.Throws<InvalidOperationException>(() => loaderUnderTest.GetModel());
            Assert.Throws<InvalidOperationException>(() => loaderUnderTest.GetReloadToken());
        }
    }
}
