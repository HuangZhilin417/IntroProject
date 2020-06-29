using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IntroProject.Data;
using IntroProject.Models;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IntroProject.Controllers
{
    //The Station Controller
    public class TablesController : Controller
    {
        private readonly ApplicationDbContext _context;
     

        public TablesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Gets the list of the Station in the index view
        public IActionResult Index()
        {
            return View(_context.Table.AsEnumerable().GroupBy(r => r.Title).Select(g => g.OrderByDescending(x => x.StationCount).First()).ToList());

        }


     

       
        // GET: Tables/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tables/Create
        // Adds the vaild station to the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,StationCount,TimeStamp")] Table table)
        {
           
            if (ModelState.IsValid)
            {
                if (TableTitleExists(table.Title))
                {
                    ModelState.AddModelError("", "This Station Name Already Exist!");
                    return View(table);
                }
                table.StationCount = 0;
                table.TimeStamp = DateTime.Now.ToString();
                _context.Add(table);
                await _context.SaveChangesAsync();
            
                return RedirectToAction(nameof(Index));
            }
            return View(table);
        }

        // GET: Tables/Edit/5
        // Adding the count of a station
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var table = await _context.Table.FindAsync(id);
            int? new_id = id;
            if (table == null)
            {
                return NotFound();
            }
            while (await _context.Table.FindAsync(new_id) != null)
            {
                new_id += 1;
            }
            var newTable = table;
            newTable.Id = (int)new_id;
            newTable.StationCount++;
            newTable.TimeStamp = DateTime.Now.ToString();
            _context.Add(newTable);
            

            _context.Database.OpenConnection();
            try
            {
                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Table] ON");
               
                await _context.SaveChangesAsync();
                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Table] OFF");
            }
            finally
            {
                _context.Database.CloseConnection();
            }




            return RedirectToAction(nameof(Index));
        }

        // Loading the Data Table for the DevExtreme View
        public ActionResult GetOrders(DataSourceLoadOptions loadOptions)
        {
            var result = DataSourceLoader.Load(_context.Table, loadOptions);
            var resultJson = JsonConvert.SerializeObject(result);
            return Content(resultJson, "application/json");
        }


        // GET: Tables/Delete/5
        // Reverting a Station to the last add
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) 
            {
                return NotFound();
            }

            var table = await _context.Table
                .FirstOrDefaultAsync(m => m.Id == id);
            if (table == null)
            {
                return NotFound();
            }

            return View(table);
        }



        // POST: Tables/Delete/5
        // Reverting a Station to the last add
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var table = await _context.Table.FindAsync(id);
            _context.Table.Remove(table);
            await _context.SaveChangesAsync();
        
            return RedirectToAction(nameof(Index));
        }

        // Deleting a station from the data base
        public async Task<IActionResult> DeleteStation(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var table = await _context.Table
                .FirstOrDefaultAsync(m => m.Id == id);
            if (table == null)
            {
                return NotFound();
            }

            _context.Table.RemoveRange(_context.Table.AsEnumerable().Where(m => m.Title == table.Title));

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }


        //check if a station exists by id 
        private bool TableExists(int id)
        {
            return _context.Table.Any(e => e.Id == id);
        }

        //check if a station exists by title
        private bool TableTitleExists(String title)
        {
            return _context.Table.Any(e => e.Title == title);
        }
    }
}
