using Microsoft.Extensions.Logging;

namespace Laraue.Core.Tests.Keras.Utils;

public sealed class BinaryClassifierFixture
{
    public readonly BinaryClassifierPredictor Predictor;

    public BinaryClassifierFixture()
    {
        Predictor = new(LoggerFactory.Create(builder => builder.AddConsole())
            .CreateLogger<BinaryClassifierPredictor>());
    }
}