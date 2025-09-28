using System;
using Laraue.Core.DataAccess.Contracts;
using Laraue.Core.DataAccess.Utils;

namespace Laraue.Core.DataAccess.Extensions;

public static class PaginatedRequestExtensions
{
    /// <summary>
    /// Validate that pagination object is configured correctly or throws.
    /// </summary>
    /// <param name="request"></param>
    public static void Validate(this IPaginatedRequest? request)
    {
        if (request?.Pagination is null)
        {
            throw new ArgumentNullException(nameof(request));
        }
        
        PaginatorUtil.ValidatePagination(request.Pagination.Page, request.Pagination.PerPage);
    }
}