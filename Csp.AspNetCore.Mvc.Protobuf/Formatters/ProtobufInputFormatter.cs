using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using ProtoBuf;
using ProtoBuf.Meta;
using System;
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

                using var fbr = new FileBufferingReadStream(req.Body, 32768);
                await fbr.DrainAsync(CancellationToken.None);
                fbr.Position = 0;
                var parsed = Serializer.Deserialize(context.ModelType, fbr);

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