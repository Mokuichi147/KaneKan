using Kanekan.Models;

namespace Kanekan.Backend;

public static class AppDatabase
{
    public static List<Payment> GetPayments()
    {
        using var context = new AppDbContext(isReadOnly: true);
        return [.. context.Payments.OrderBy(o => o.CreatedAt)];
    }

    public static List<Payment> GetPayments(DateTime startDate, DateTime endDate, bool isContainNullPaymentAt)
    {
        using var context = new AppDbContext(isReadOnly: true);
        IQueryable<Payment>? query;
        if (isContainNullPaymentAt)
        {
            query = context.Payments.Where(p => p.PaymentAt == null || (p.PaymentAt >= startDate && p.PaymentAt <= endDate));
        }
        else
        {
            query = context.Payments.Where(p => p.CreatedAt >= startDate && p.CreatedAt <= endDate);
        }

        return [.. query.OrderBy(o => o.CreatedAt)];
    }

    public static async Task SavePayment(Payment payment)
    {
        using var context = new AppDbContext(isReadOnly: false);
        using (var transaction = await context.Database.BeginTransactionAsync())
        {
            try
            {
                context.Payments.Add(payment);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}