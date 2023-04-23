using System;
using Laraue.Core.Keras.Models;
using Laraue.Core.Keras.Prediction;
using Laraue.Core.Keras.Utils;
using Microsoft.Extensions.Logging;
using Moq;
using Numpy;

namespace Laraue.Core.Tests.Keras.Utils;

public class BinaryClassifierPredictor : BasePredictor<bool>
{
    private readonly BinaryKerasModel _isRenovationExistsModel = new (
        PathUtil.GetFullPath("Keras", "Utils", "binary_model.h5"));
    
    public BinaryClassifierPredictor(ILogger<BinaryClassifierPredictor> logger)
        : base(
            new PredictorOptions(),
            logger)
    {
    }

    protected override NDarray GetNdArray(byte[][] byteArrays)
    {
        return NDArrayCreator.ForImageBatch(
            byteArrays,
            150,
            150);
    }

    protected override bool[] PredictBatch(NDarray inputDataBatch)
    {
        return _isRenovationExistsModel.Predict(inputDataBatch);
    }
}