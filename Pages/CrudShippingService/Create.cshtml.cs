using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShoppingFantasy.Data;
using ShoppingFantasy.Models;

namespace ShoppingFantasy.Pages.CrudShippingService
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly ShoppingFantasy.Data.ApplicationDbContext _context;

        public CreateModel(ShoppingFantasy.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public ShippingService ShippingService { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.ShippingServices == null || ShippingService == null)
            {
                return Page();
            }

            _context.ShippingServices.Add(ShippingService);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
