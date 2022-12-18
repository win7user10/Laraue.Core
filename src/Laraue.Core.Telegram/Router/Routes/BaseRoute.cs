using System.Text.RegularExpressions;
using Laraue.Core.Telegram.Extensions;
using Laraue.Core.Telegram.Router.Request;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Laraue.Core.Telegram.Router.Routes;

public abstract class BaseRoute<T> : IRoute
{
    private readonly UpdateType _updateType;
    public string Pattern { get; }
    private readonly Type _routeAttributeType;
    private readonly Regex _isRouteMatchRegex;
    private readonly ExecuteRouteAsync<T> _executeRouteAsyncDelegate;

    protected BaseRoute(UpdateType updateType, string routePattern, Type routeAttributeType, ExecuteRouteAsync<T> executeRouteAsyncDelegate)
    {
        _updateType = updateType;
        _executeRouteAsyncDelegate = executeRouteAsyncDelegate;
        Pattern = routePattern;
        _routeAttributeType = routeAttributeType;
        _isRouteMatchRegex = RouteRegexCreator.ForRoute(routePattern);
    }

    public Task<object?> ExecuteAsync(
        Update update,
        PathParameters pathParameters,
        string userId)
    {
        var entity = GetEntity(update);
        var content = GetContent(entity);

        var parameters = new RequestParameters(content.ParseQueryParts());
        
        return _executeRouteAsyncDelegate.Invoke(
            new RouteData<T>(
                entity,
                new RequestContext
                {
                    UserId = userId
                }, parameters, pathParameters));
    }

    public override string ToString()
    {
        return $"{_routeAttributeType} {Pattern}";
    }

    public bool TryMatch(UpdateType updateType, string? route, Type routeAttributeType)
    {
        if (_updateType != updateType || _routeAttributeType != routeAttributeType)
        {
            return false;
        }
        
        if (route is null)
        {
            return false;
        }

        var match = _isRouteMatchRegex.Match(route);
        return match.Success;
    }

    public Task<object?> ExecuteAsync(Update update, string userId)
    {
        var entity = GetEntity(update);
        var content = GetContent(entity);

        var match = _isRouteMatchRegex.Match(content!);
        var routeValues = match.Groups.Values.Skip(1)
            .Select(x => x.Value)
            .ToArray();

        var pathParameters = new PathParameters(routeValues);
        
        var parameters = new RequestParameters(content.ParseQueryParts());
        
        return _executeRouteAsyncDelegate.Invoke(
            new RouteData<T>(
                entity,
                new RequestContext
                {
                    UserId = userId
                }, parameters, pathParameters));
    }

    public string? GetContent(Update update)
    {
        var entity = GetEntity(update);
        return GetContent(entity);
    }

    protected abstract string? GetContent(T data);

    protected abstract T GetEntity(Update update);
}