namespace KaneKan.Backend;

public static class Checker
{
    public static bool IsPassedPayment(Models.Payment payment)
    {
        bool result = true;
        result &= IsPassedPaymentName(payment.Name);
        return result;
    }

    public static bool IsPassedPaymentName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        return true;
    }
}