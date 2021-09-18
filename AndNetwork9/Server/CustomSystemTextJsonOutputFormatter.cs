using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;

#nullable enable
namespace AndNetwork9.Server
{
    public class CustomSystemTextJsonOutputFormatter : TextOutputFormatter
    {
        public CustomSystemTextJsonOutputFormatter(JsonSerializerOptions jsonSerializerOptions)
        {
            SerializerOptions = jsonSerializerOptions;
            SupportedEncodings.Add(Encoding.ASCII);
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
            SupportedEncodings.Add(Encoding.UTF32);
            SupportedMediaTypes.Add("application/json");
            SupportedMediaTypes.Add("text/json");
            SupportedMediaTypes.Add("application/*+xml");
        }

        public JsonSerializerOptions SerializerOptions { get; }

        public sealed override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context,
            Encoding selectedEncoding)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));
            if (selectedEncoding is null)
                throw new ArgumentNullException(nameof(selectedEncoding));

            HttpContext httpContext = context.HttpContext;
            string? fullName = context.ObjectType?.FullName;
            if (context is null || context.ObjectType is null) throw new ArgumentNullException(nameof(context));
            Type type = context.ObjectType;
            if (fullName is not null && fullName.StartsWith("Castle.Proxies.")) type = type.BaseType!;
            Stream responseStream = httpContext.Response.Body;
            if (selectedEncoding.CodePage == Encoding.UTF8.CodePage)
            {
                await JsonSerializer.SerializeAsync(responseStream, context.Object, type, SerializerOptions).ConfigureAwait(false);
                await responseStream.FlushAsync().ConfigureAwait(false);
            }
            else
            {
                Stream stream = Encoding.CreateTranscodingStream(httpContext.Response.Body,
                    selectedEncoding,
                    Encoding.UTF8,
                    true);
                await using ConfiguredAsyncDisposable _ = stream.ConfigureAwait(false);
                await JsonSerializer.SerializeAsync(stream, context.Object, type, SerializerOptions).ConfigureAwait(false);
                await stream.FlushAsync().ConfigureAwait(false);
            }
        }
    }
}