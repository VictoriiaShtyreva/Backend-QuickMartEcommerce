using System.Net;

namespace Ecommerce.Core.src.Common
{
    public class AppException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }

        public AppException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        #region Custom Exceptions
        // 400 Bad Request
        public static AppException BadRequest(string message = "Bad Request") =>
            new AppException(HttpStatusCode.BadRequest, message);

        // 404 Not Found
        public static AppException NotFound(string message = "Not Found") =>
            new AppException(HttpStatusCode.NotFound, message);

        // 401 Unauthorized
        public static AppException Unauthorized(string message = "Unauthorized") =>
            new AppException(HttpStatusCode.Unauthorized, message);

        // 400 Bad Request
        public static AppException InvalidLoginCredentialsException(string message = "Invalid Login Credentials") =>
            new AppException(HttpStatusCode.BadRequest, message);

        // 400 Bad Request
        public static AppException NotLoginException(string message = "User is not logged in") =>
            new AppException(HttpStatusCode.BadRequest, message);

        // 409 Conflict
        public static AppException DuplicateException(string message = "Duplicate entry detected.") =>
            new AppException(HttpStatusCode.Conflict, message);

        // 400 Bad Request
        public static AppException InvalidInputException(string message = "Invalid input") =>
            new AppException(HttpStatusCode.BadRequest, message);
        #endregion
    }

}
