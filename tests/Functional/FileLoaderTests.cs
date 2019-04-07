using Extensions.ML;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Extensions.ML
{
    public class FileLoaderTests
    {
        [Fact]
        public void throw_until_started()
        {
            var services = new ServiceCollection()
                .AddOptions()
                .AddLogging();
            var sp = services.BuildServiceProvider();

            var loaderUnderTest = ActivatorUtilities.CreateInstance<FileModelLoader>(sp);
            Assert.Throws<InvalidOperationException>(() => loaderUnderTest.GetModel());
            Assert.Throws<InvalidOperationException>(() => loaderUnderTest.GetReloadToken());
        }

        [Fact]
        public void can_load_model()
        {
            var services = new ServiceCollection()
                .AddOptions()
                .AddLogging();
            var sp = services.BuildServiceProvider();

            var loaderUnderTest = ActivatorUtilities.CreateInstance<FileModelLoader>(sp);
            loaderUnderTest.Start("TestModel.zip", false);

            var model = loaderUnderTest.GetModel();
        }

        //TODO: This is a quick test to give coverage of the main scenarios. Refactoring and re-implementing of tests should happen.
        //Right now this screams of probably flakeyness
        [Fact]
        public async Task can_reload_model()
        {
            var services = new ServiceCollection()
                .AddOptions()
                .AddLogging();
            var sp = services.BuildServiceProvider();

            var loaderUnderTest = ActivatorUtilities.CreateInstance<FileLoaderDouble>(sp);
            loaderUnderTest.Start("testdata.txt", true);

            var changed = false;
            var changeTokenRegistration = ChangeToken.OnChange(
                        () => loaderUnderTest.GetReloadToken(),
                        () => changed = true);

            await File.WriteAllTextAsync("testdata.txt", "test");

            await Task.Delay(1000);

            Assert.True(changed);
        }


        private class FileLoaderDouble : FileModelLoader
        {
            public FileLoaderDouble(IOptions<MLContextOptions> contextOptions, ILogger<FileModelLoader> logger) : base(contextOptions, logger)
            {
            }

            internal override void LoadModel()
            {
            }
        }
    }
}
