using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Microsoft.Extensions.Logging;

namespace CcgWorks.Lambda
{
    public class ResponseCreator
    {
        private readonly Func<object> _handler;
        private readonly List<(Type Type, Func<Exception, APIGatewayProxyResponse> GetResponse)> _exceptionResponses;
        private string _resultPropertyName;
        private Dictionary<string, object> _metaData;

        private ResponseCreator(Func<object> handler)
        {
            _handler = handler;
            _exceptionResponses = new List<(Type Type, Func<Exception, APIGatewayProxyResponse> GetResponse)>();
            _resultPropertyName = string.Empty;
            _metaData = new Dictionary<string, object>();
        }

        public static ResponseCreator From(Func<object> action)
        {
            return new ResponseCreator(action);
        }        

        public ResponseCreator OnException<TException>(int statusCode)
            where TException : Exception
        {
            _exceptionResponses.Add(
                (typeof(TException),
                (ex) => CreateErrorResponse(statusCode, ex.Message, ex.InnerException?.Message)));

            return this;
        }

        public ResponseCreator OnException<TException>(int statusCode, string errorMessage)
            where TException : Exception
        {
            _exceptionResponses.Add(
                (typeof(TException),
                (ex) => CreateErrorResponse(statusCode, errorMessage, ex.InnerException?.Message)));

            return this;
        }

        public ResponseCreator OnException<TException>(int statusCode, Func<Exception, object> mapException)
            where TException : Exception
        {
            _exceptionResponses.Add(
                (typeof(TException),
                (ex) => CreateResponse(statusCode, mapException(ex))));

            return this;
        }

        public ResponseCreator WithMetaData(string key, object value)
        {
            if (string.IsNullOrEmpty(_resultPropertyName))
                _resultPropertyName = "Result";

            _metaData.Add(key, value);
            return this;
        }

        public ResponseCreator WithResultProperty(string resultPropertyName)
        {
            _resultPropertyName = resultPropertyName;
            return this;
        }

        public APIGatewayProxyResponse GetResponse()
        {
            return GetResponseAsync().GetAwaiter().GetResult();
        }

        public async Task<APIGatewayProxyResponse> GetResponseAsync()
        {
            try
            {
                var result = _handler();
                if (result is Task t)
                {
                    await t;
                    result = t.GetType().GetProperty("Result").GetValue(t);
                }

                return CreateResponse(200, result);
            }
            catch (Exception ex)
            {                
                var response = _exceptionResponses.FirstOrDefault(x => x.Type.IsAssignableFrom(ex.GetType()));
                if (response.GetResponse == null)
                    return CreateErrorResponse(500, "An unexpected error occurred.");
                else
                    return response.GetResponse(ex);
            }
        }

        private APIGatewayProxyResponse CreateErrorResponse(int statusCode, string message, string description = null)
        {
            _resultPropertyName = "Error";
            return string.IsNullOrEmpty(description)
                ? CreateResponse(statusCode, new { Message = message })
                : CreateResponse(statusCode, new { Message = message, Description = description });
        }

        private APIGatewayProxyResponse CreateResponse(int statusCode, object body)
        {
            if (string.IsNullOrEmpty(_resultPropertyName) == false)
            {
                body = new Dictionary<string, object>
                {
                    { "MetaData", _metaData },
                    { _resultPropertyName, body }
                };
            }

            return new APIGatewayProxyResponse
            {
                StatusCode = statusCode,
                Body = body is string stringBody
                    ? stringBody
                    : Newtonsoft.Json.JsonConvert.SerializeObject(body)
            };
        }
    }
}
