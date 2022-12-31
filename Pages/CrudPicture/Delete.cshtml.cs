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
    public class DeleteModel : PageModel
    {
        private readonly ShoppingFantasy.Data.ApplicationDbContext _context;

        public DeleteModel(ShoppingFantasy.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Picture Picture { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Pictures == null)
            {
                return NotFound();
            }

            var picture = await _context.Pictures.FirstOrDefaultAsync(m => m.Id == id);

            if (picture == null)
            {
                return NotFound();
            }
            else 
            {
                Picture = picture;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Pictures == null)
            {
                return NotFound();
            }
            var picture = await _context.Pictures.FindAsync(id);

            if (picture != null)
            {
                Picture = picture;
                _context.Pictures.Remove(Picture);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
