using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using ProtoBuf;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Csp.AspNetCore.Mvc.Protobuf.Formatters
{
    /// <summary>
    /// Reads a protobuf object from the request body.
    /// </summary>
    public sealed class ProtobufInputFormatter : InputFormatter
    {
        private const string protoMediaType = "application/x-protobuf";

        public ProtobufInputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(protoMediaType));
        }

        protected override bool CanReadType(Type type)
        {
            return type.IsClass;
        }

        public async override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            try
            {
                var req = context.HttpContext.Request;

                using var body = new MemoryStream();
                await req.Body.CopyToAsync(body);
                body.Position = 0;
                var parsed = Serializer.Deserialize(context.ModelType, body);
                
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