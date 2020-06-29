using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntroProject.Data;
using IntroProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IntroProject.Views.Home
{
    public class Index1Model : PageModel
    {
        private readonly ApplicationDbContext _db;

        public Index1Model(ApplicationDbContext db)
        {
            _db = db;
        }

        public IEnumerable<Table> Tables { get; set; }
        public async Task OnGet()
        {
            Tables = await _db.Table.ToListAsync();
        }
    }
}
