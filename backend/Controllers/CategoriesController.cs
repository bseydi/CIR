using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using backend.Models;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly CIRDevContext _context;

        public CategoriesController(CIRDevContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            if (_context.Categories == null)
            {
                return NotFound();
            }
            return await _context.Categories.ToListAsync();
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> UpdateCategory()
        {
            var key = Convert.ToInt32(Request.Form["key"]);
            var values = Request.Form["values"];
            if (!CategoryExists(key))
            {
                return BadRequest();
            }
            var category = _context.Categories.Find(key);
            JsonConvert.PopulateObject(values, category);
            if (!ValidateCategory(category))
            {
                return BadRequest("Update not possible because the name or code already exists");
            }
            _context.Entry(category).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("Insert")]
        public async Task<IActionResult> InsertCategory()
        {
            var values = Request.Form["values"];
            var category = new Category();
            JsonConvert.PopulateObject(values, category);
            if (!ValidateCategory(category))
            {
                return BadRequest("Insert not possible because the name or code already exists");
            }
            _context.Categories.Add(category);
            _context.SaveChanges();

            return NoContent();
        }

        // DELETE: api/Categories/5
        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> DeleteCategory()

        {
            var key = Convert.ToInt32(Request.Form["key"]);
            if (_context.Categories == null)
            {
                return NotFound();
            }
            var category = _context.Categories.Include(c => c.ChartRules).Where(c => c.CategoryId == key).FirstOrDefault();
            if (category == null)
            {
                return BadRequest("Category not found");
            }
            if (category.ChartRules.Count != 0)
            {
                return BadRequest("We cannot delete it because this element is linked to a chart rule");
            }

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return (_context.Categories?.Any(e => e.CategoryId == id)).GetValueOrDefault();
        }
        private bool ValidateCategory(Category c)
        {
            var list = _context.Categories.ToList();
            foreach (var category in list)
            {
                if (c.CategoryId != category.CategoryId && (c.Code == category.Code || c.Name == category.Name))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
