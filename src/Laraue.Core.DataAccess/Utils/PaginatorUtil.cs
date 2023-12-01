using System;

namespace Laraue.Core.DataAccess.Utils;

/// <summary>
/// Utils to build the correct pagination.
/// </summary>
public static class PaginatorUtil
{
    /// <summary>
    /// Validate that pagination values are correct.
    /// </summary>
    /// <param name="page"></param>
    /// <param name="perPage"></param>
    /// <exception cref="ArgumentException"></exception>
    public static void ValidatePagination(long page, int perPage)
    {
        if (page < 0)
        {
            throw new ArgumentException("Page should be grater or equal to 0", nameof(page));
        }
            
        if (perPage < 1)
        {
            throw new ArgumentException("Per page should be grater or equal to 1", nameof(perPage));
        }
    }
}