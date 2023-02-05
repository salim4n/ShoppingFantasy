using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShoppingFantasy.Data;
using ShoppingFantasy.Models;

namespace ShoppingFantasy.Pages.CrudPicture
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
        ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public Picture Picture { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.Pictures == null || Picture == null)
            {
                return Page();
            }

            _context.Pictures.Add(Picture);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
