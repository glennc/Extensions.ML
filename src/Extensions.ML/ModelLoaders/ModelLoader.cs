using Microsoft.Extensions.Primitives;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Text;

namespace Extensions.ML
{
    public abstract class ModelLoader
    {
        public abstract IChangeToken GetReloadToken();

        public abstract ITransformer GetModel();
    }
}
