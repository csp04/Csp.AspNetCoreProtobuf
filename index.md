# Protobuf for ASP.NET Core | Web APIs
https://github.com/protobuf-net/protobuf-net

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

<table>
    <tr>
        <td>Package</td>
        <td>Version</td>
        <td>Downloads</td>
    </tr>
    <tr>
        <td> <a href="https://www.nuget.org/packages/Csp.AspNetCore.Mvc.Protobuf" >Csp.AspNetCore.Mvc.Protobuf</a> </td>
        <td> <img src="https://img.shields.io/nuget/v/Csp.AspNetCore.Mvc.Protobuf" /> </td>
        <td> <img src="https://img.shields.io/nuget/dt/Csp.AspNetCore.Mvc.Protobuf" /> </td>
    </tr>
    <tr>
        <td> <a href="https://www.nuget.org/packages/Csp.Net.Http.Protobuf.Extensions" >Csp.Net.Http.Protobuf.Extensions</a> </td>
        <td> <img src="https://img.shields.io/nuget/v/Csp.Net.Http.Protobuf.Extensions" /> </td>
        <td> <img src="https://img.shields.io/nuget/dt/Csp.Net.Http.Protobuf.Extensions" /> </td>
    </tr>
</table>

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
