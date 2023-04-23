using Castle.Core.Logging;
using Laraue.Core.Testing.Logging;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Laraue.Core.Tests.Keras.Utils;

public class BinaryClassifierFixture
{
    public readonly BinaryClassifierPredictor Predictor;

    public BinaryClassifierFixture()
    {
        Predictor = new(LoggerFactory.Create(builder => builder.AddConsole())
            .CreateLogger<BinaryClassifierPredictor>());
    }
}