using MassTransit;
using System;
using System.Threading.Tasks;

public class PromoCodeReceivedConsumer : IConsumer<PromoCodeReceivedEvent>
{
    
    public PromoCodeReceivedConsumer()
    {
    }

   
    public async Task Consume(ConsumeContext<PromoCodeReceivedEvent> context)
    {
        var promoCode = context.Message.PromoCode;
        var userId = context.Message.UserId;

        await ProcessPromoCode(promoCode, userId);
    }

    private Task ProcessPromoCode(string promoCode, int userId)
    {
        Console.WriteLine($"Промокод {promoCode} получен для пользователя {userId}.");

        return Task.CompletedTask;
    }
}
