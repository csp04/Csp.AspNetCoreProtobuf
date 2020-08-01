# Protobuf for ASP.NET Core
https://github.com/protobuf-net/protobuf-net

---
- ### Supported Runtimes
  - .Net Standard 2.1+
- ### Media Type
  - application/x-protobuf
---
## Sample Usage
### Server Side
```cs
using Csp.AspNetCore.Mvc.Protobuf;
```
```cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers()
            .AddProtobufSerializer(true); //true if use as default
}
```

### Client Side
```cs
using Csp.Net.Http.Protobuf.Extensions;
...
var client = new HttpClient() { ... };
```

##### Get
```cs
var forecasts = await client.GetFromProtoAsync<WeatherForecast[]>("/weatherforecast");
```
##### Post
```cs
using var response = await client.PostAsProtoAsync("/weatherforecast", new WeatherForecast { ... });

using var stream = await response.Content.ReadAsStreamAsync();

var forecast = Serializer.Deserialize<WeatherForecast>(stream);
```
