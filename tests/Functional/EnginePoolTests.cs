using Extensions.ML;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.ML;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Functional
{
    public class EnginePoolTests
    {
        public EnginePoolTests()
        {
           
        }

        private IServiceProvider BuildNoDefaultModelServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddOptions();
            services.AddPredictionEngine<Input, Output>("model", ctx => new Mock<PredictionEnginePoolOptions<Input, Output>>());
            return services.BuildServiceProvider();
        }

        [Fact]
        public void must_have_default_to_use_default()
        {
            var poolUnderTest = BuildNoDefaultModelServiceProvider().GetRequiredService<PredictionEnginePool<Input, Output>>();
            Assert.Throws<ArgumentException>(() => poolUnderTest.GetPredictionEngine());
        }

        [Fact]
        public void can_create_with_no_default()
        {
            BuildNoDefaultModelServiceProvider().GetRequiredService<PredictionEnginePool<Input, Output>>();
        }
    }


    public class Input
    {
    }

    public class Output
    {
    }
}
