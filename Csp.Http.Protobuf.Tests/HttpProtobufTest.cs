using Csp.Http.Protobuf.Tests.Api;
using Csp.Net.Http.Protobuf.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using ProtoBuf;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Csp.Http.Protobuf.Tests
{
    // I'm having issue regarding post/put/delete method. Nothing has been delivered when using TestServer.
    // In ProtobufInputFormatter, request body is being nulled.
    // I got this code here https://github.com/dotnet/aspnetcore/issues/18463#issuecomment-613189489
    // You can remove this code in WebHostBuilder configuration if you want to see the bug.
    public class UnitTestStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                app.Use(async (context, next) =>
                {
                    context.Request.EnableBuffering();
                    using (var ms = new MemoryStream())
                    {
                        await context.Request.Body.CopyToAsync(ms);
                        context.Request.ContentLength = ms.Length;
                    }
                    context.Request.Body.Seek(0, SeekOrigin.Begin);
                    await next.Invoke();
                });
                next(app);
            };
        }
    }

    public class HttpProtobufTest
    {
        private readonly TestServer server;
        private readonly HttpClient client;

        public HttpProtobufTest()
        {
            server = new TestServer(
                    new WebHostBuilder()
                    .ConfigureServices(services =>
                    {
                        services.AddSingleton<IStartupFilter, UnitTestStartupFilter>();
                    })
                    .UseStartup<Startup>());
            client = server.CreateClient();
        }

        [Fact]
        public async Task Protobuf_ShouldReturn5()
        {
            var forecasts = await client.GetFromProtoAsync<WeatherForecast[]>("/weatherforecast");

            Assert.Equal(5, forecasts.Where(f => f.Summary != null).Count());
        }

        [Fact]
        public async Task Protobuf_ShouldReturnAnyCount()
        {
            int anyCount = 20;

            var forecasts = await client.GetFromProtoAsync<WeatherForecast[]>("/weatherforecast/" + anyCount.ToString());

            Assert.Equal(anyCount, forecasts.Where(f => f.Summary != null).Count());
        }

        [Fact]
        public async Task Protobuf_CanPostAndReceive()
        {
            using var response = await client.PostAsProtoAsync("/weatherforecast", new WeatherForecast { Summary = "Test" });

            using var stream = await response.Content.ReadAsStreamAsync();

            var forecast = Serializer.Deserialize<WeatherForecast>(stream);

            Assert.Equal("Test", forecast.Summary);
        }

        [Fact]
        public async Task Protobuf_CanPostJsonAndReceiveProtobuf()
        {
            using var response = await client.PostAsJsonAsync("/weatherforecast/json", new WeatherForecast { Summary = "Test" });

            using var stream = await response.Content.ReadAsStreamAsync();

            var forecast = Serializer.Deserialize<WeatherForecast>(stream);

            Assert.Equal("Test", forecast.Summary);
        }

        [Fact]
        public async Task Protobuf_CanPutAndReceive()
        {
            using var response = await client.PutAsProtoAsync("/weatherforecast", new WeatherForecast { Summary = "Test" });

            using var stream = await response.Content.ReadAsStreamAsync();

            var forecast = Serializer.Deserialize<WeatherForecast>(stream);

            Assert.Equal("Test", forecast.Summary);
        }

        [Fact]
        public async Task Protobuf_CanDeleteAndReceive()
        {
            using var response = await client.DeleteAsync("/weatherforecast/1");

            using var stream = await response.Content.ReadAsStreamAsync();

            var forecast = Serializer.Deserialize<WeatherForecast>(stream);

            Assert.Equal("1", forecast.Summary);
        }

        [Fact]
        public async Task Protobuf_ShouldReturnError_If_Object_IsMalformed()
        {
            //malformed data
            var data = new byte[] { 1, 2, 3 };

            HttpRequestMessage message = new HttpRequestMessage
            {
                RequestUri = new Uri("/weatherforecast", UriKind.Relative),
                Method = HttpMethod.Post
            };

            using var ms = new MemoryStream(data);
            var content = new StreamContent(ms);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-protobuf");
            message.Content = content;

            using var response = await client.SendAsync(message);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Protobuf_ShouldReturnError_If_Object_IsNotProtobuf()
        {
            HttpRequestMessage message = new HttpRequestMessage
            {
                RequestUri = new Uri("/weatherforecast", UriKind.Relative),
                Method = HttpMethod.Post
            };

            var content = new StringContent("{ \"a\": \"b\" }", encoding: Encoding.UTF8, "application/json");
            message.Content = content;

            using var response = await client.SendAsync(message);
            Assert.Equal(System.Net.HttpStatusCode.UnsupportedMediaType, response.StatusCode);
        }
    }
}