using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoppingFantasy.Data;
using ShoppingFantasy.Models;
using ShoppingFantasy.Utility;
using Stripe.Checkout;
using System.Security.Claims;

namespace ShoppingFantasy.Pages
{
    public class OrderConfirmationModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;

        public OrderConfirmationModel(ApplicationDbContext db, IEmailSender emailSender)
        {
            _db = db;
            _emailSender = emailSender;
        }


        public int Id { get; set; } = default(int);

        public async Task OnGet(int id)
        {
            Id = id;
            OrderHeader orderHeader = await _db.OrderHeaders.Include(oh => oh.AppUser).FirstOrDefaultAsync(oh => oh.Id == id);
            
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                //check the stripe status
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    await UpdateStripePayment(id, orderHeader.SessionId, session.PaymentIntentId);
                    await UpdateStatus(id, SD.StatusApprouve, SD.PaiementStatusApprouve);
                    await _db.SaveChangesAsync();

                }

            await _emailSender
                .SendEmailAsync(orderHeader.AppUser.Email,
                "Mille et une Création - Votre commande",
                "<p>Merci pour votre achat ! Nous traitons au plus vite votre commande.</p>");

            List<ShoppingCart> shoppingCarts = await _db.ShoppingCarts
                .Where(sp => sp.ApplicationUserId == orderHeader.AppUserId)
                .ToListAsync();
            _db.RemoveRange(shoppingCarts);
            _db.SaveChanges();
            
        }

        private async Task UpdateStripePayment(int id, string sessionId, string paymentIntentId)
        {
            var orderFromDb = await _db.OrderHeaders.FirstOrDefaultAsync(o => o.Id == id);
            orderFromDb.PaymentDate = DateTime.Now;
            orderFromDb.SessionId = sessionId;
            orderFromDb.PaymentIntentId = paymentIntentId;
        }

        private async Task UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderFromDb = await _db.OrderHeaders.FirstOrDefaultAsync(o => o.Id == id);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if (paymentStatus != null)
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
        }
    }
}
