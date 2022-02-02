
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Net.Http;

namespace IdentityServerHost.Quickstart.UI
{
    public class ResponseDto<T> : HttpResponseMessage
    {
        //public ResponseDto(T response, HttpStatusCode statusCode, string msg = "")
        //{
           
        //    if (statusCode != HttpStatusCode.OK)
        //    {
        //        if (statusCode == HttpStatusCode.BadRequest || statusCode == HttpStatusCode.InternalServerError)
        //            throw new BadHttpRequestException(msg);
        //        if (statusCode == HttpStatusCode.NotFound)
        //            throw new InvalidOperationException(msg);
        //    }
        //    else
        //    {

        //        Data = response;
        //    }

        //}
        //public HttpStatusCode Status { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
    }
}