using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.ML;
using System;
using System.Threading;

namespace Extensions.ML
{
    internal class PoolLoader<TData, TPrediction>: IDisposable
                        where TData : class
                        where TPrediction : class, new()
    {
        private DefaultObjectPool<PredictionEngine<TData, TPrediction>> _pool;
        private IDisposable _changeTokenRegistration;

        public PoolLoader(IServiceProvider sp, PredictionEnginePoolOptions<TData, TPrediction> poolOptions)
        {
            var contextOptions = sp.GetRequiredService<IOptions<MLContextOptions>>();
            Context = contextOptions.Value.MLContext ?? throw new ArgumentNullException(nameof(contextOptions));
            Loader = poolOptions.ModelLoader ?? throw new ArgumentNullException(nameof(poolOptions));

            LoadPool();

            _changeTokenRegistration = ChangeToken.OnChange(
                () => Loader.GetReloadToken(),
                () => LoadPool());
        }

        public ModelLoader Loader { get; private set; }
        public MLContext Context { get; private set; }
        public ObjectPool<PredictionEngine<TData, TPrediction>> PredictionEnginePool { get { return _pool; } }

        public void LoadPool()
        {
            var predictionEnginePolicy = new PredictionEnginePoolPolicy<TData, TPrediction>(Context, Loader.GetModel());
            Interlocked.Exchange(ref _pool, new DefaultObjectPool<PredictionEngine<TData, TPrediction>>(predictionEnginePolicy));
        }

        public void Dispose()
        {
            _changeTokenRegistration?.Dispose();
        }
    }
}
