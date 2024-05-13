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

        // 500 Internal Server Error
        public static AppException InternalServerError(string message = "Internal Server Error") =>
            new AppException(HttpStatusCode.InternalServerError, message);

        // 400 Bad Request
        public static AppException InvalidLoginCredentialsException(string message = "Invalid Login Credentials") =>
            new AppException(HttpStatusCode.BadRequest, message);

        // 400 Bad Request
        public static AppException NotLoginException(string message = "User is not logged in") =>
            new AppException(HttpStatusCode.BadRequest, message);

        // 409 Conflict
        public static AppException DuplicateEmailException(string message = "Email is already in use, please choose another email") =>
            new AppException(HttpStatusCode.Conflict, message);

        // 409 Conflict
        public static AppException DuplicateProductTitleException(string message = "Product title is already in use, please choose another title") =>
            new AppException(HttpStatusCode.Conflict, message);

        // 409 Conflict
        public static AppException DuplicateCategoryNameException(string message = "Category name is already in use, please choose another name") =>
            new AppException(HttpStatusCode.Conflict, message);

        // 400 Bad Request
        public static AppException ReviewRatingException(string message = "Rating must be between 1 and 5") =>
            new AppException(HttpStatusCode.BadRequest, message);

        // 400 Bad Request
        public static AppException InvalidInputException(string message = "Invalid input") =>
            new AppException(HttpStatusCode.BadRequest, message);
        #endregion
    }

}
