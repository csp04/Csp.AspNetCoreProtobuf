using ProtoBuf;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Csp.Net.Http.Protobuf.Extensions
{
    public static class HttpClientProtobufExtensions
    {
        public static Task<T> GetFromProtoAsync<T>(this HttpClient client, string requestUri)
            => GetFromProtoAsync<T>(client, requestUri, CancellationToken.None);

        public static Task<T> GetFromProtoAsync<T>(this HttpClient client, string requestUri, CancellationToken cancellationToken = default)
        {
            var taskResponse = client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            return DeserializeResponse<T>(taskResponse);
        }

        public static Task<HttpResponseMessage> PostAsProtoAsync<T>(this HttpClient client, string requestUri, T value)
            => PostAsProtoAsync(client, requestUri, value, CancellationToken.None);

        public static Task<HttpResponseMessage> PostAsProtoAsync<T>(this HttpClient client, string requestUri, T value, CancellationToken cancellationToken = default)
        {
            return client.PostAsync(requestUri, ProtobufContent.Create(value), cancellationToken);
        }

        public static Task<HttpResponseMessage> PutAsProtoAsync<T>(this HttpClient client, string requestUri, T value)
            => PutAsProtoAsync(client, requestUri, value, CancellationToken.None);

        public static Task<HttpResponseMessage> PutAsProtoAsync<T>(this HttpClient client, string requestUri, T value, CancellationToken cancellationToken = default)
        {
            return client.PutAsync(requestUri, ProtobufContent.Create(value), cancellationToken);
        }

        private async static Task<T> DeserializeResponse<T>(Task<HttpResponseMessage> taskResponse)
        {
            using HttpResponseMessage response = await taskResponse.ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            using var stream = await response.Content!.ReadAsStreamAsync();
            return await Task.FromResult(Serializer.Deserialize<T>(stream)).ConfigureAwait(false);
        }
    }
}