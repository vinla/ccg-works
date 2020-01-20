using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

namespace CcgWorks.Lambda
{
    public interface IRequestHandler
    {
        Task<APIGatewayProxyResponse> HandleRequest(APIGatewayProxyRequest request, ILambdaContext lambdaContext);
    }
}
