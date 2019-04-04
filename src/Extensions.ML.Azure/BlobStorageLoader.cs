using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Primitives;
using Microsoft.ML;

namespace Extensions.ML.Azure
{
    internal class BlobStorageLoader : ModelLoader
    {
        public override ITransformer GetModel()
        {
            throw new NotImplementedException();
        }

        public override IChangeToken GetReloadToken()
        {
            throw new NotImplementedException();
        }
    }
}
