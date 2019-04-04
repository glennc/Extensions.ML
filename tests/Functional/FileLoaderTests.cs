using Extensions.ML;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
            loaderUnderTest.Start("TestModelA.zip", false);

            var model = loaderUnderTest.GetModel();
        }

        //TODO: This is a quick test to give coverage of the main scenarios. Refactoring and re-implementing of tests should happen.
        //Right now this screams of probably flakeyness
        [Fact]
        public void can_reload_model()
        {
            var services = new ServiceCollection()
                .AddOptions()
                .AddLogging();
            var sp = services.BuildServiceProvider();

            var loaderUnderTest = ActivatorUtilities.CreateInstance<FileModelLoader>(sp);
            loaderUnderTest.Start("TestModelA.zip", true);

            var changed = false;
            var changeTokenRegistration = ChangeToken.OnChange(
                        () => loaderUnderTest.GetReloadToken(),
                        () => changed = true);

            var modelA = loaderUnderTest.GetModel();
            File.Copy("TestModelB.zip", "TestModelA.zip", true);
            System.Threading.Thread.Sleep(1000);
            var modelB = loaderUnderTest.GetModel();

            Assert.True(changed);
            Assert.NotEqual(modelA, modelB);
        }

    }
}
