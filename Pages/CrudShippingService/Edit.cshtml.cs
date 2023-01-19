using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoppingFantasy.Data;
using ShoppingFantasy.Models;

namespace ShoppingFantasy.Pages.CrudShippingService
{
    public class EditModel : PageModel
    {
        private readonly ShoppingFantasy.Data.ApplicationDbContext _context;

        public EditModel(ShoppingFantasy.Data.ApplicationDbContext context)
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

            var shippingservice =  await _context.ShippingServices.FirstOrDefaultAsync(m => m.Id == id);
            if (shippingservice == null)
            {
                return NotFound();
            }
            ShippingService = shippingservice;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(ShippingService).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShippingServiceExists(ShippingService.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ShippingServiceExists(int id)
        {
          return (_context.ShippingServices?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
