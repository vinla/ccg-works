using System.Collections.Generic;
using Amazon.Lambda.APIGatewayEvents;

namespace CcgWorks.Lambda
{
    public static class APIGatewayExtensions
    {
        public static void EnsureHeader(this APIGatewayProxyResponse response, string header, string value)
        {
            if (response.Headers == null)
                response.Headers = new Dictionary<string, string>();
            response.Headers.Add(header, value);
        }
    }
}