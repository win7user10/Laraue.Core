# Laraue.Core 

Laraue.Core is the solution with often-using classes in other projects in Laraue.* Namespace.

## Laraue.Core.DataAccess

A list of contracts to work with a database.

## Laraue.Core.DataAccess.Linq2Db

Often-used methods to work with DB using EF Core with integrated Linq2DB provider.

## Laraue.Core.DataAccess.EfCore

Often-used methods to work with DB using EF Core.

## Laraue.Core.Testing

`Proxy<TController>` calls an ASP.NET Core controller action over a real `HttpClient` by
expressing the call as a strongly-typed expression against the controller, instead of hand-building
URLs and request payloads in integration tests. It resolves the route, HTTP method and parameter
binding (`[FromRoute]`/`[FromQuery]`/`[FromBody]`/`[FromForm]`, including `IFormFile`) from the same
attributes ASP.NET Core itself uses to bind the action.

Create one per controller from your `WebApplicationFactory`:
```csharp
public Proxy<TController> Controller<TController>() where TController : ControllerBase
{
    var client = CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    return new Proxy<TController>(client, Services);
}
```

Call an action the same way you'd call the controller method directly:
```csharp
var issue = await host.Controller<IssuesController>()
    .Execute(c => c.GetIssue(issueId));

await host.Controller<IssuesController>()
    .Execute(c => c.CreateIssue(new CreateIssueRequest { Title = "New issue" }));
```

Failed responses (non-2xx) throw an `HttpRequestException` whose `InnerException` is a
`BadRequestException`/`NotFoundException`/`ForbiddenException` from `Laraue.Core.Exceptions.Web`
when the status code maps to one, so tests can assert on the exception type instead of the raw
status code:
```csharp
await Assert.ThrowsAsync<HttpRequestException>(() => proxy.Execute(c => c.GetIssue(missingId)));
```

Authentication headers aren't app-specific in this package - use `WithAuthorizationToken`
directly, or add your own extension method on top of the exposed `Services` provider for
app-specific token minting:
```csharp
public static Proxy<TController> WithUserAuthorization<TController>(this Proxy<TController> proxy, Guid userId)
    where TController : ControllerBase
{
    var authService = proxy.Services.GetRequiredService<IAuthService>();
    return proxy.WithAuthorizationToken(authService.CreateUserToken(userId));
}
```