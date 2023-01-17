using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoppingFantasy.Data;
using ShoppingFantasy.Utility;
using ShoppingFantasy.ViewModels;
using Stripe;

namespace ShoppingFantasy.Pages
{
	public class OrderDetailsModel : PageModel
	{
		public readonly ApplicationDbContext _db;

		public OrderDetailsModel(ApplicationDbContext db
			)
		{
			_db = db;
		}

		[BindProperty]
		public OrderVM OrderVM { get; set; }

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



		public async Task<IActionResult> OnPost(int orderId)
		{
			try
			{
				var orderDb = await _db.OrderHeaders.FirstOrDefaultAsync(o => o.Id == orderId);
				orderDb.Name = OrderVM.OrderHeader.Name;
				orderDb.SurName = OrderVM.OrderHeader.SurName;
				orderDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
				orderDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
				orderDb.City = OrderVM.OrderHeader.City;
				orderDb.AddressComplement = OrderVM.OrderHeader.AddressComplement;
				orderDb.PostalCode = OrderVM.OrderHeader.PostalCode;
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
				TempData["Success"] = "Modification réussi";

			}
			catch (Exception ex)
			{
				TempData["Error"] = "Une erreur est survenue lors de l'édition du formulaire, vérifier à bien remplir les champs, merci.";


			}

			return RedirectToPage("OrderDetails", new { orderId = OrderVM.OrderHeader.Id });
		}

		public async Task<IActionResult> OnPostStartProcessing(int orderId)
		{

			var orderDb = await _db.OrderHeaders.FirstOrDefaultAsync(o => o.Id == orderId);
			if (orderDb != null)
			{
				orderDb.OrderStatus = SD.StatusEnCours;
				if (OrderVM.OrderHeader.PaymentStatus != null)
				{
					orderDb.PaymentStatus = OrderVM.OrderHeader.PaymentStatus;
				}
			}

			_db.SaveChanges();
			TempData["Success"] = "Statut modifié";

			return RedirectToPage("OrderDetails", new { orderId = OrderVM.OrderHeader.Id });
		}

		public async Task<IActionResult> OnPostShipOrder(int orderId)
		{
			var order = await _db.OrderHeaders.FirstOrDefaultAsync(o => o.Id == orderId);
			order.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
			order.Carrier = OrderVM.OrderHeader.Carrier;
			order.OrderStatus = SD.StatusEnvoye;
			order.ShippingDate = DateTime.Now;

			 _db.Update(order);
			await _db.SaveChangesAsync();
			TempData["Success"] = "Statut modifié, statut = envoyé !";

			return RedirectToPage("OrderDetails", new { orderId = OrderVM.OrderHeader.Id });
		}

		public async Task<IActionResult> OnPostCancelOrder(int orderId)
		{
			var order = await _db.OrderHeaders.FindAsync(orderId);
			if(order.PaymentStatus == SD.PaiementStatusApprouve)
			{
				var options = new RefundCreateOptions
				{
					Reason = RefundReasons.RequestedByCustomer,
					PaymentIntent = order.PaymentIntentId,
				};

				var service = new RefundService();
				Refund refund = service.Create(options);

				order.OrderStatus = SD.StatusRembourse;
				if (OrderVM.OrderHeader.PaymentStatus != null)
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
			TempData["Success"] = "Statut Remboursé ou Annulé réussi";
			return RedirectToPage("OrderDetails", new { orderId = OrderVM.OrderHeader.Id });
				
		}
	}
}
