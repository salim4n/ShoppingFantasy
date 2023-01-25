using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoppingFantasy.Data;
using ShoppingFantasy.Models;

namespace ShoppingFantasy.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;
        private readonly ApplicationDbContext _db;

        public PrivacyModel(ILogger<PrivacyModel> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        [BindProperty]
        public ContactUs Contact { get; set; } = default!;

        public async Task OnGet()
        {
            var contactUs = await _db.ContactUs.FirstOrDefaultAsync();
            Contact = contactUs;
        }

        public async Task<IActionResult> OnPost()
        {
            var ctus = await _db.ContactUs.Where(c => c.Id == 1).FirstOrDefaultAsync();
            ctus.Message = Contact.Message;

            _db.ContactUs.Update(ctus);
           await  _db.SaveChangesAsync();

            return RedirectToPage("Privacy");
        }
    }
}