using ProtoBuf;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Csp.Net.Http.Protobuf.Extensions
{
    internal sealed class ProtobufContent : HttpContent
    {
        private const string protoMediaType = "application/x-protobuf";

        private readonly object inputValue;

        private ProtobufContent(object inputValue)
        {
            this.inputValue = inputValue;
            Headers.ContentType = new MediaTypeHeaderValue(protoMediaType);
        }

        public static ProtobufContent Create(object inputValue) => new ProtobufContent(inputValue);

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            Serializer.Serialize(stream, inputValue);
            return Task.CompletedTask;
        }

        protected override bool TryComputeLength(out long length)
        {
            length = 0;
            return false;
        }
    }
}