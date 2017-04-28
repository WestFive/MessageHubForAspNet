using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Data.Common
{
    public class ApiResultModel
    {
        private HttpStatusCode statusCode;
        private object data;
        private string errorMessage;
        private bool isSucess;

        public HttpStatusCode StatusCode { get { return statusCode; } set { statusCode = value; } }

        public object Data { get { return data; } set { data = value; } }

        public string ErrorMessage { get { return errorMessage; } set { errorMessage = value; } }

        public bool IsSucess { get { return isSucess; } set { isSucess = value; } }
    }
}
