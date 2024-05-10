using System.Text.RegularExpressions;
using Ardalis.GuardClauses;

namespace Ecommerce.Core.src.Extensions
{
    public static partial class GuardClauseExtensions
    {
        [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
        private static partial Regex EmailRegex();
        public static string InvalidEmail(this IGuardClause guardClause, string input, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(input) || !EmailRegex().IsMatch(input))
            {
                throw new ArgumentException("Invalid email format.", parameterName);
            }
            return input;
        }
    }
}