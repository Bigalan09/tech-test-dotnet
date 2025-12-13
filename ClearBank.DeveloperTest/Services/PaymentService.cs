using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services
{
    internal class PaymentService : IPaymentService
    {
        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            if (request == null || request.Amount <= 0)
                return MakePaymentResult.Rejected("Invalid amount.");

            return MakePaymentResult.Success();
        }
    }
}
