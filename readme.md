# Laraue.Core 

Laraue.Core is the solution with often-using classes in other projects in Laraue.* namespace

## Laraue.Keras

The package allows to use Keras models in .NET.
To run the models, python 3.x version should be installed on the machine with packages numpy and tensorflow.
To work with images Pillow package also required.

Example of yaml to configure Linux env on github workflow to run unit tests
```yaml
jobs:
  - name: Install Python dependencies
    run: |
      alias python3.8="python"
      python -m pip install --upgrade pip
      pip install numpy
      pip install tensorflow
      pip install Pillow
```

To run the model, BasePredictor class can be inherited

```csharp
public sealed class BinaryClassifierPredictor : BasePredictor<bool>
{
    private readonly BinaryKerasModel _isDogOnPhotoModel = new ("C://binary_model.h5");
    
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
            width: 150,
            height: 150);
    }

    protected override bool[] PredictBatch(NDarray inputDataBatch)
    {
        return _isDogOnPhotoModel.Predict(inputDataBatch);
    }
}
```

Now predictions can be made:

```csharp
var image1 = await File.ReadAllBytesAsync("C://image1.jpg");
var image2 = await File.ReadAllBytesAsync("C://image2.jpg");

var predictor = new BinaryClassifierPredictor();
var predictions = predictor.PredictAsync(new[] { image1, image2 });
```

Now the predictions variable contains two predictions, for the first and second images.