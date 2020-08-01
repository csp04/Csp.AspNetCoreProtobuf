using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using ProtoBuf;
using ProtoBuf.Meta;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Csp.AspNetCore.Mvc.Protobuf.Formatters
{
    /// <summary>
    /// Reads a protobuf object from the request body.
    /// </summary>
    public sealed class ProtobufInputFormatter : InputFormatter
    {
        private const string protoMediaType = "application/x-protobuf";

        private readonly TypeModel _model = RuntimeTypeModel.Default;

        public ProtobufInputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(protoMediaType));
        }

        protected override bool CanReadType(Type type)
        {
            return _model.CanSerializeContractType(type);
        }

        public async override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            try
            {
                var req = context.HttpContext.Request;

                if (!req.Body.CanSeek)
                {
                    req.EnableBuffering();
                    await req.Body.DrainAsync(CancellationToken.None);
                    req.Body.Seek(0L, SeekOrigin.Begin);
                }

                var parsed = Serializer.Deserialize(context.ModelType, req.Body);

                return await InputFormatterResult.SuccessAsync(parsed);
            }
            catch
            {
                context.ModelState.AddModelError(context.ModelName, $"Malformed input for {context.ModelType}.");
                return await InputFormatterResult.FailureAsync();
            }
        }
    }
}