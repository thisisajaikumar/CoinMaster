using System.Text.RegularExpressions;

public static class InputValidator
{
    public static ValidationResult ValidatePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
        {
            return new ValidationResult(false, "Phone number is required");
        }

        // Remove any non-digit characters
        string cleanNumber = Regex.Replace(phoneNumber, @"[^\d]", "");

        if (cleanNumber.Length != 10)
        {
            return new ValidationResult(false, "Phone number must be exactly 10 digits");
        }

        // Check if it starts with valid digits (not 0 or 1)
        if (cleanNumber[0] == '0' || cleanNumber[0] == '1')
        {
            return new ValidationResult(false, "Phone number cannot start with 0 or 1");
        }

        return new ValidationResult(true, "Valid phone number");
    }

    public static ValidationResult ValidatePassword(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return new ValidationResult(false, "Password is required");
        }

        if (password.Length < 6)
        {
            return new ValidationResult(false, "Password must be at least 6 characters");
        }

        if (password.Length > 20)
        {
            return new ValidationResult(false, "Password cannot exceed 20 characters");
        }

        return new ValidationResult(true, "Valid password");
    }

    public static string FormatPhoneNumber(string phoneNumber)
    {
        string cleanNumber = Regex.Replace(phoneNumber, @"[^\d]", "");

        if (cleanNumber.Length == 10)
        {
            return $"({cleanNumber.Substring(0, 3)}) {cleanNumber.Substring(3, 3)}-{cleanNumber.Substring(6, 4)}";
        }

        return phoneNumber;
    }
}

public class ValidationResult
{
    public bool IsValid { get; private set; }
    public string Message { get; private set; }

    public ValidationResult(bool isValid, string message)
    {
        IsValid = isValid;
        Message = message;
    }
}