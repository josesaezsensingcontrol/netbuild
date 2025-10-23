using Google.FlatBuffers;
using openmeteo_sdk;
using System.Net;

namespace NetBuild.App.Core.Weather
{
    public class OpenMeteoClient
    {
        private readonly HttpClient Client;

        public OpenMeteoClient()
        {
            this.Client = new HttpClient(new RetryHandler(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            }));
            this.Client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
        }

        public async Task<WeatherApiResponse[]> GetWeather(Uri uri)
        {
            var response = await Client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            var bytes = await response.Content.ReadAsByteArrayAsync();
            return DecodeWeatherResponses(bytes);
        }

        public static WeatherApiResponse[] DecodeWeatherResponses(byte[] bytes)
        {
            var buffer = new ByteBuffer(bytes);

            var total = 0;
            while (buffer.Position < buffer.Length)
            {
                var length = buffer.GetInt(buffer.Position);
                buffer.Position += sizeof(int) + length;
                total++;
            }

            var results = new WeatherApiResponse[total];
            var i = 0;
            buffer.Position = 0;

            while (buffer.Position < buffer.Length)
            {
                var length = buffer.GetInt(buffer.Position);
                buffer.Position += 4;
                results[i] = WeatherApiResponse.GetRootAsWeatherApiResponse(buffer);
                buffer.Position += length;
                i += 1;
            }
            return results;
        }
    }

    public class RetryHandler : DelegatingHandler
    {
        private const int MaxRetries = 3;
        private const double BackoffFactor = 0.5;
        private const int BackoffMaxSeconds = 2;

        public RetryHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        { }


        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;
            for (int i = 0; i < MaxRetries; i++)
            {
                response = await base.SendAsync(request, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
                int waitMs = (int)Math.Min(BackoffFactor * Math.Pow(2, i), BackoffMaxSeconds) * 1000;
                await Task.Delay(waitMs);
            }
            return response;
        }
    }
}