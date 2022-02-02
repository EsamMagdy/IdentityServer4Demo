using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Identity.Quickstart
{
    public class NiceInternalServerExceptionResponse: IHttpActionResult
    {
        public string Message { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }

        public NiceInternalServerExceptionResponse(
            string message,
            HttpStatusCode code)
        {
            Message = message;
            StatusCode = code;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(StatusCode);
            response.Content = new StringContent(Message);
            return Task.FromResult(response);
        }
    }
}
