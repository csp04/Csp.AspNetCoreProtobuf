using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using ProtoBuf;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Csp.AspNetCore.Mvc.Protobuf.Formatters
{
    /// <summary>
    /// Writes a protobuf object to the output stream.
    /// </summary>
    public sealed class ProtobufOutputFormatter : OutputFormatter
    {
        private const string protoMediaType = "application/x-protobuf";

        public ProtobufOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(protoMediaType));
        }

        protected override bool CanWriteType(Type type)
        {
            return true;
        }

        public override void WriteResponseHeaders(OutputFormatterWriteContext context)
        {
            base.WriteResponseHeaders(context);
            context.ContentType = protoMediaType;
        }

        public async override Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            var httpContext = context.HttpContext;
            var res = httpContext.Response;
            if (context.Object != null)
            {
                using var ms = new MemoryStream();
                Serializer.Serialize(ms, context.Object);
                ms.Position = 0;
                await ms.CopyToAsync(res.Body);
            }
            else
            {
                await Task.CompletedTask;
            }
        }
    }
}