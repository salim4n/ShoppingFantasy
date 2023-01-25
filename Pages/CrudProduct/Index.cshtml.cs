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
    public class IndexModel : PageModel
    {
        private readonly ShoppingFantasy.Data.ApplicationDbContext _context;

        public IndexModel(ShoppingFantasy.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Product> Product { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Products != null)
            {
                Product = await _context.Products
                .Include(p => p.Category).ToListAsync();
            }
        }
    }
}
