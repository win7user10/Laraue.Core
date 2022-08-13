using System.Text.RegularExpressions;
using Laraue.Core.Telegram.Extensions;
using Laraue.Core.Telegram.Router.Request;
using MediatR;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Laraue.Core.Telegram.Router.Routes;

public abstract class BaseRoute<T> : IRoute
{
    protected readonly UpdateType UpdateType;
    private readonly string _routePattern;
    private readonly Regex _isRouteMatchRegex;
    private readonly PerformRoute<T> _getRequest;

    protected BaseRoute(UpdateType updateType, string routePattern, PerformRoute<T> getRequest)
    {
        UpdateType = updateType;
        _getRequest = getRequest;
        _routePattern = routePattern;
        _isRouteMatchRegex = RouteRegexCreator.ForRoute(routePattern);
    }

    public virtual bool TryMatch(Update update, out PathParameters? pathParameters)
    {
        pathParameters = null;
        
        if (UpdateType != update.Type)
        {
            return false;
        }
        
        var content = GetContent(GetEntity(update));
        if (content is null)
        {
            return false;
        }

        var match = _isRouteMatchRegex.Match(content);
        if (!match.Success)
        {
            return false;
        }

        var routeValues = match.Groups.Values.Skip(1)
            .Select(x => x.Value)
            .ToArray();

        pathParameters = new PathParameters(routeValues);
        return true;
    }

    public IRequest<object?> GetRequest(
        Update update,
        PathParameters pathParameters,
        string userId)
    {
        var entity = GetEntity(update);
        var content = GetContent(entity);

        var parameters = new RequestParameters(content.ParseQueryParts());
        
        return _getRequest.Invoke(
            new RouteData<T>(
                entity,
                new RequestContext
                {
                    UserId = userId
                }, parameters, pathParameters));
    }

    protected abstract string? GetContent(T data);

    protected abstract T GetEntity(Update update);
}