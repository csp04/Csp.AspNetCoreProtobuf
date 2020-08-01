using Csp.AspNetCore.Mvc.Protobuf.Formatters;
using Microsoft.Extensions.DependencyInjection;

namespace Csp.AspNetCore.Mvc.Protobuf
{
    public static class MvcProtobufExtensions
    {
        /// <summary>
        /// Adds the protobuf serializer as one of the input/output formatters.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="default">Use protobuf as the default serializer.</param>
        /// <returns><inheritdoc/></returns>
        public static IMvcBuilder AddProtobufSerializer(this IMvcBuilder builder, bool @default = false)
        {
            var inputFormatter = new ProtobufInputFormatter();
            var outputFormatter = new ProtobufOutputFormatter();

            return builder.AddMvcOptions(opt =>
            {
                if (@default)
                {
                    opt.InputFormatters.Insert(0, inputFormatter);
                    opt.OutputFormatters.Insert(0, outputFormatter);
                }
                else
                {
                    opt.InputFormatters.Add(inputFormatter);
                    opt.OutputFormatters.Add(outputFormatter);
                }
            });
        }
    }
}