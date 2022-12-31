using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoppingFantasy.Data;
using ShoppingFantasy.Models;

namespace ShoppingFantasy.Pages.CrudPicture
{
    public class IndexModel : PageModel
    {
        private readonly ShoppingFantasy.Data.ApplicationDbContext _context;

        public IndexModel(ShoppingFantasy.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Picture> Picture { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Pictures != null)
            {
                Picture = await _context.Pictures
                .Include(p => p.Product).ToListAsync();
            }
        }
    }
}
