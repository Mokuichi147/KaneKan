using KaneKan.Models;

namespace KaneKan.Backend;

public static class AppDatabase
{
    public static List<Payment> GetPayments()
    {
        using var context = new AppDbContext(isReadOnly: true);
        return [.. context.Payments.OrderByDescending(x => x.PaymentAt ?? x.CreatedAt)];
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
        Payment? _payment = await context.Payments.FindAsync(payment.Id);
        using (var transaction = await context.Database.BeginTransactionAsync())
        {
            try
            {
                if (_payment != null)
                {
                    _payment.CreatedAt = payment.CreatedAt;
                    _payment.PaymentAt = payment.PaymentAt;
                    _payment.IsPaid = payment.IsPaid;
                    _payment.Name = payment.Name;
                    _payment.Amount = payment.Amount;
                    _payment.PaymentMethod = payment.PaymentMethod;
                    _payment.Description = payment.Description;
                    _payment.CategoriesJson = payment.CategoriesJson;
                }
                else
                {
                    context.Payments.Add(payment);
                }
                
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

    public static List<string> GetPaymentMethods(int loadCount = 100)
    {
        using var context = new AppDbContext(isReadOnly: true);
        return context.Payments.OrderByDescending(x => x.CreatedAt)
                               .Take(loadCount)
                               .AsEnumerable()
                               .Where(x => x.PaymentMethod != null)
                               .Select(x => x.PaymentMethod ?? throw new Exception("PaymentMethod is null"))
                               .GroupBy(x => x)
                               .ToDictionary(x => x.Key, x => x.Count())
                               .OrderByDescending(x => x.Value)
                               .Select(x => x.Key)
                               .ToList();
    }

    public static List<string> GetCategories(int loadCount = 100)
    {
        using var context = new AppDbContext(isReadOnly: true);
        return context.Payments.OrderByDescending(x => x.CreatedAt)
                               .Take(loadCount)
                               .AsEnumerable()
                               .SelectMany(x => x.Categories)
                               .GroupBy(x => x)
                               .ToDictionary(x => x.Key, x => x.Count())
                               .OrderByDescending(x => x.Value)
                               .Select(x => x.Key)
                               .ToList();
    }
}