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

namespace ShoppingFantasy.Pages.CrudProduct
{
    [Authorize(Roles = "Admin")]
    public class DetailsModel : PageModel
    {
        private readonly ShoppingFantasy.Data.ApplicationDbContext _context;

        public DetailsModel(ShoppingFantasy.Data.ApplicationDbContext context)
        {
            _context = context;
        }

      public Product Product { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.Include(p => p.Category)
                                                 .Include(p => p.Picture)
                                                  .FirstOrDefaultAsync(p => p.Id == id);
                                                  
            if (product == null)
            {
                return NotFound();
            }
            else 
            {
                Product = product;
            }
            return Page();
        }
    }
}
