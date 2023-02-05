using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShoppingFantasy.Data;
using ShoppingFantasy.Models;

namespace ShoppingFantasy.Pages.CrudProduct
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public Product Product { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!Decimal.TryParse(Product.Price.ToString(), out decimal price))
            {
                ModelState.AddModelError(nameof(Product.Price), "Le prix doit être un nombre décimal");
            }

            if (!Decimal.TryParse(Product.PromoPrice.ToString(), out decimal promoPrice))
            {
                ModelState.AddModelError(nameof(Product.PromoPrice), "Le prix promotionnel doit être un nombre décimal");
            }
            if (!ModelState.IsValid || _context.Products == null || Product == null)
            {
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
                return Page();
            }

            _context.Products.Add(Product);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

    }
}
