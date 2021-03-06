using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonApiDotNetCoreExampleTests
{
    public static class HttpResponseMessageExtensions
    {
        public static HttpResponseMessageAssertions Should(this HttpResponseMessage instance)
        {
            return new HttpResponseMessageAssertions(instance);
        }

        public sealed class HttpResponseMessageAssertions
            : ReferenceTypeAssertions<HttpResponseMessage, HttpResponseMessageAssertions>
        {
            protected override string Identifier => "response";

            public HttpResponseMessageAssertions(HttpResponseMessage instance)
            {
                Subject = instance;
            }

            public AndConstraint<HttpResponseMessageAssertions> HaveStatusCode(HttpStatusCode statusCode)
            {
                if (Subject.StatusCode != statusCode)
                {
                    string responseText = GetFormattedContentAsync(Subject).Result;
                    Subject.StatusCode.Should().Be(statusCode, "response body returned was:\n" + responseText);
                }

                return new AndConstraint<HttpResponseMessageAssertions>(this);
            }

            private static async Task<string> GetFormattedContentAsync(HttpResponseMessage responseMessage)
            {
                string text = await responseMessage.Content.ReadAsStringAsync();

                try
                {
                    if (text.Length > 0)
                    {
                        return JsonConvert.DeserializeObject<JObject>(text).ToString();
                    }
                }
                catch
                {
                    // ignored
                }

                return text;
            }
        }
    }
}
