using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoppingFantasy.Data;
using ShoppingFantasy.Models;

namespace ShoppingFantasy.Pages.CrudShippingService
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly ShoppingFantasy.Data.ApplicationDbContext _context;

        public DeleteModel(ShoppingFantasy.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
      public ShippingService ShippingService { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.ShippingServices == null)
            {
                return NotFound();
            }

            var shippingservice = await _context.ShippingServices.FirstOrDefaultAsync(m => m.Id == id);

            if (shippingservice == null)
            {
                return NotFound();
            }
            else 
            {
                ShippingService = shippingservice;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.ShippingServices == null)
            {
                return NotFound();
            }
            var shippingservice = await _context.ShippingServices.FindAsync(id);

            if (shippingservice != null)
            {
                ShippingService = shippingservice;
                _context.ShippingServices.Remove(ShippingService);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
