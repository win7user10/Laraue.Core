using Laraue.Core.DataAccess.Contracts;
using Laraue.Core.DataAccess.Utils;

namespace Laraue.Core.DataAccess.Extensions;

public static class PaginatedRequestExtensions
{
    public static void Validate(this IPaginatedRequest request)
    {
        PaginatorUtil.ValidatePagination(request.Page, request.PerPage);
    }
}