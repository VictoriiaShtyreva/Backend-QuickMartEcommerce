using Ardalis.GuardClauses;

namespace Ecommerce.Core.src.Common
{
    public class PasswordChange
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }

        public PasswordChange(string currentPassword, string newPassword)
        {
            CurrentPassword = Guard.Against.NullOrEmpty(currentPassword, nameof(CurrentPassword));
            NewPassword = Guard.Against.NullOrEmpty(newPassword, nameof(NewPassword));
        }
    }
}