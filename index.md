# Protobuf for ASP.NET Core | Web APIs

[protobuf-net](https://github.com/protobuf-net/protobuf-net)

A lighter way to transfer data through http. 

---
### Supported Runtimes
- .Net Standard 2.1+

### Media Type
- application/x-protobuf


## Runtime Installation

You can use the following command in the Package Manager Console:

###### For Server Side 

```ps
Install-Package Csp.AspNetCore.Mvc.Protobuf
```

###### For Client Side

```ps
Install-Package Csp.Net.Http.Protobuf.Extensions
```

| Package | Version | Downloads |
| ------- | ------- | --------- |
| [Csp.AspNetCore.Mvc.Protobuf](https://www.nuget.org/packages/Csp.AspNetCore.Mvc.Protobuf/) | ![Csp.AspNetCore.Mvc.Protobuf](https://img.shields.io/nuget/v/Csp.AspNetCore.Mvc.Protobuf) | ![Csp.AspNetCore.Mvc.Protobuf](https://img.shields.io/nuget/dt/Csp.AspNetCore.Mvc.Protobuf) |
| [Csp.Net.Http.Protobuf.Extensions](https://www.nuget.org/packages/Csp.Net.Http.Protobuf.Extensions/) | ![Csp.Net.Http.Protobuf.Extensions](https://img.shields.io/nuget/v/Csp.Net.Http.Protobuf.Extensions) | ![Csp.Net.Http.Protobuf.Extensions](https://img.shields.io/nuget/dt/Csp.Net.Http.Protobuf.Extensions) |

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
