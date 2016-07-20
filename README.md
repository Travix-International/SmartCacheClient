# SmartCacheClient.Dotnet

This serves as a base implementation for any client to our internal services.

It caches results per called url based on the max-age header outputted by the original service call. 
And it wraps the result so it can store null values.

[![Build Status .NET](https://ci.appveyor.com/api/projects/status/github/Travix-International/SmartCacheClient?svg=true)](https://ci.appveyor.com/project/JSalverda/smartcacheclient/)
[![Version](https://img.shields.io/nuget/v/SmartCacheClient.svg)](https://www.nuget.org/packages/SmartCacheClient)
[![License](https://img.shields.io/github/license/Travix-International/SmartCacheClient.svg)](https://github.com/Travix-International/SmartCacheClient/blob/master/LICENSE)

Usage
--------------------------------

The SmartCacheClient can be used by adding a dependency on ISmartCacheClient to your specific client implementation as below

```csharp
public class SpecificClient
{
  private readonly ISmartCacheClient smartCacheClient;

  public SpecificClient(ISmartCacheClient smartCacheClient)
  {
    this.smartCacheClient = smartCacheClient;
  }

  public async Task<SomeModel> GetSomeModelAsync(string key)
  {
    Uri uri = new Uri(string.Format("http://baseurl/{0}", key), UriKind.Absolute);

    return await smartCacheClient.GetAsync<SomeModel>(uri);
  }
}
```

To do
--------------------------------

In order to reduce customer requests waiting for internal subsystems the cached items should be refreshed before they expire if the cached value has been accessed in the mean time. 

For frequently used values this will make sure it always comes from cache but is refreshed regularly as well.

Until this is implemented it's actually not very smart :)