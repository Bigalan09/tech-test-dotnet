using System;

namespace ClearBank.DeveloperTest.Types
{
    public class MakePaymentResult
    {
        public MakePaymentResult()
        {
        }
        
        private MakePaymentResult(bool isSuccess, string? reason)
        {
            IsSuccess = isSuccess;
            Reason = reason;
        }

        public bool IsSuccess { get; }
        
        public bool Success => IsSuccess;

        public string? Reason { get; } = null;

        public static MakePaymentResult Accepted() 
            => new(true, null);

        public static MakePaymentResult Rejected(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Reason is required for a rejected payment");

            return new (false, reason);
        }
    }
}
