using System;

namespace ClearBank.DeveloperTest.Types
{
    public sealed class MakePaymentResult
    {
        private MakePaymentResult(bool isSuccess, string reason)
        {
            IsSuccess = isSuccess;
            Reason = reason;
        }

        public bool IsSuccess { get; }

        public string Reason { get; }

        public static MakePaymentResult Success()
            => new(true, null);

        public static MakePaymentResult Rejected(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Reason is required for a rejected payment");

            return new (false, reason);
        }
    }
}
