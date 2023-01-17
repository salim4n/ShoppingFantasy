using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoppingFantasy.Data;
using ShoppingFantasy.Utility;
using ShoppingFantasy.ViewModels;

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

			return RedirectToPage("OrderDetails", new { orderId =  OrderVM.OrderHeader.Id});
        }
    }
}
