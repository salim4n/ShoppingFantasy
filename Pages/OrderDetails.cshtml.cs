using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoppingFantasy.Data;
using ShoppingFantasy.Utility;
using ShoppingFantasy.ViewModels;
using Stripe;
using System.Text.Encodings.Web;

namespace ShoppingFantasy.Pages
{
	public class OrderDetailsModel : PageModel
	{
		public readonly ApplicationDbContext _db;
		public readonly IEmailSender _emailSender;

		public OrderDetailsModel(ApplicationDbContext db
								, IEmailSender emailSender)
		{
			_db = db;
			_emailSender = emailSender;
		}

		[BindProperty]
		public OrderVM OrderVM { get; set; } = default!;

		public async Task<IActionResult> OnGet(int orderId)
		{
			OrderVM order = new()
			{
				OrderHeader = await _db.OrderHeaders.Include(oh => oh.AppUser).FirstOrDefaultAsync(oh => oh.Id == orderId),
				OrderDetails = await _db.OrderDetails.Include(oh => oh.Product).Where(oh => oh.OrderId == orderId).ToListAsync()
			};

			OrderVM = order;
			return Page();
		}



		public async Task<IActionResult> OnPostUpdate()
		{
			try
			{
				var orderDb = await _db.OrderHeaders.FirstOrDefaultAsync(o => o.Id == OrderVM.OrderHeader.Id);
				orderDb.Name = OrderVM.OrderHeader.Name;
				orderDb.SurName = OrderVM.OrderHeader.SurName;
				orderDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
				orderDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
				orderDb.City = OrderVM.OrderHeader.City;
				orderDb.AddressComplement = OrderVM.OrderHeader.AddressComplement;
				orderDb.PostalCode = OrderVM.OrderHeader.PostalCode;
				//orderDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
				//orderDb.Carrier = OrderVM.OrderHeader.Carrier;
				if (OrderVM.OrderHeader.Carrier != null)
				{
					orderDb.Carrier = OrderVM.OrderHeader.Carrier;
				}
				if (OrderVM.OrderHeader.TrackingNumber != null)
				{
					orderDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
				}

				_db.Update(orderDb);
				_db.SaveChanges();
				TempData["Success"] = "Modification r�ussi";

			}
			catch (Exception ex)
			{
				TempData["Error"] = "Une erreur est survenue lors de l'�dition du formulaire, v�rifier � bien remplir les champs, merci.";


			}

			return RedirectToPage("OrderDetails", new { orderId = OrderVM.OrderHeader.Id });
		}

		public async Task<IActionResult> OnGetStartProcessing(int orderId)
		{

			await UpdateStatus(orderId, SD.StatusEnCours);
			_db.SaveChanges();
			TempData["Success"] = $"Statut modifi� pour la commande {orderId}";

			return RedirectToPage("OrderIndex");
		}

		public async Task<IActionResult> OnGetShipOrder(int orderId)
		{
			var order = await _db.OrderHeaders
				.Include(o => o.AppUser)
				.FirstOrDefaultAsync(o => o.Id == orderId);
			order.OrderStatus = SD.StatusEnvoye;
			order.ShippingDate = DateTime.Now;

			_db.Update(order);
			await _db.SaveChangesAsync();

			await _emailSender.SendEmailAsync(order.AppUser.Email, $"Livraison de votre commande #{order.Id}",
	$"Bonjour {order.Name} {order.SurName},\r\n\r\nNous vous informons que votre commande a �t� exp�di�e aujourd'hui.Voici le num�ro de suivie de votre commande:\r\n\r\n{order.TrackingNumber}\r\nTransporteur : {order.Carrier}\r\nDate d'exp�dition : {order.ShippingDate}\r\n\r\nMerci d'avoir choisi notre application e-commerce pour vos achats en ligne. Nous esp�rons que vous appr�cierez votre commande.\r\n\r\nCordialement,\r\nMille et une Cr�ation");

			TempData["Success"] = $"statut pass� � : Envoy� pour la commande {order.Id}";

			return RedirectToPage("OrderIndex");
		}

		public async Task<IActionResult> OnGetCancelOrder(int orderId)
		{
			var order = await _db.OrderHeaders.FindAsync(orderId);
			if (order.PaymentStatus == SD.PaiementStatusApprouve)
			{
				var options = new RefundCreateOptions
				{
					Reason = RefundReasons.RequestedByCustomer,
					PaymentIntent = order.PaymentIntentId,
				};

				var service = new RefundService();
				Refund refund = service.Create(options);

				order.OrderStatus = SD.StatusRembourse;
				if (order.PaymentStatus != null)
				{
					order.PaymentStatus = SD.StatusRembourse;
				}

			}
			else
			{
				order.OrderStatus = SD.StatusAnnule;
				order.PaymentStatus = SD.StatusAnnule;

			}

			_db.Update(order);
			await _db.SaveChangesAsync();
			TempData["Success"] = $"Statut  Annul� pour la commande {order.Id}";
			return RedirectToPage("OrderIndex");

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
