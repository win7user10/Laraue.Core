using System;
using System.IO;
using Keras.PreProcessing.Image;
using Numpy;

namespace Laraue.Core.Keras.Utils;

/// <summary>
/// Util to create arrays for predictions.
/// </summary>
// ReSharper disable once InconsistentNaming
public static class NDArrayCreator
{
    /// <summary>
    /// Loads NDArray from the passed image bytes array.
    /// </summary>
    /// <param name="imagesBytes"></param>
    /// <param name="imageWidth"></param>
    /// <param name="imageHeight"></param>
    /// <param name="colorsCount"></param>
    /// <param name="maxPixelValue"></param>
    /// <returns></returns>
    public static NDarray ForImageBatch(
        byte[][] imagesBytes,
        int imageWidth,
        int imageHeight,
        int colorsCount = 3,
        int maxPixelValue = 255)
    {
        var resultArray = np.zeros((imagesBytes.Length, imageWidth, imageHeight, colorsCount), np.float16);

        var i = 0;
        foreach (var fileBytes in imagesBytes)
        {
            resultArray[i++] = ForImage(
                fileBytes,
                imageWidth,
                imageHeight, 
                colorsCount,
                maxPixelValue);
        }

        return resultArray;
    }

    /// <summary>
    /// Loads NDArray from the passed image bytes.
    /// </summary>
    /// <param name="imageBytes"></param>
    /// <param name="imageWidth"></param>
    /// <param name="imageHeight"></param>
    /// <param name="colorsCount"></param>
    /// <param name="maxPixelValue"></param>
    /// <returns></returns>
    public static NDarray ForImage(
        byte[] imageBytes,
        int imageWidth,
        int imageHeight,
        int colorsCount = 3,
        int maxPixelValue = 255)
    {
        var tempFileName = $"{Guid.NewGuid()}.temp";
        File.WriteAllBytes(tempFileName, imageBytes);
        
        var image = ImageUtil.LoadImg(tempFileName, target_size: (imageWidth, imageHeight, colorsCount));
        File.Delete(tempFileName);
        
        return ImageUtil.ImageToArray(image) / maxPixelValue;
    }
}