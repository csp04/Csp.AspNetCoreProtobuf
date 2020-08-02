using Csp.AspNetCore.Mvc.Protobuf.Internal;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using ProtoBuf;
using ProtoBuf.Meta;
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

        private readonly TypeModel _model = RuntimeTypeModel.Default;

        public ProtobufOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(protoMediaType));
        }

        protected override bool CanWriteType(Type type)
        {
            return _model.CanSerializeContractType(type);
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

            var tempFilePath = AspNetCoreFileTemp.CreateTempFile();

            using (var fw = new FileStream(tempFilePath, FileMode.Create))
            {
                Serializer.Serialize(fw, context.Object);
            }

            using (var fr = new FileStream(tempFilePath, FileMode.Open))
            {
                await fr.CopyToAsync(res.Body);
            }
            File.Delete(tempFilePath);
        }
    }
}