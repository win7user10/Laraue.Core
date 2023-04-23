using System.IO;

namespace Laraue.Core.Tests.Keras.Utils;

public static class PathUtil
{
    public static string GetFullPath(params string[] pathSegments)
    {
        var dirName = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!;

        return Path.Combine(dirName, Path.Combine(pathSegments));
    }
}