using backend.Models;
using backend.Models.ColorScheme;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LegendRowController : ControllerBase
    {
        private readonly CIRDevContext _context;

        public LegendRowController(CIRDevContext context)
        {
            _context = context;
        }

        // GET: api/<LegendController>
        [HttpGet]
        public IEnumerable<LegendRow> Get()
        {
            return _context.LegendRow.ToList();
        }

        // GET api/<LegendController>/5
        [HttpGet("{id}")]
        public ActionResult<IEnumerable<LegendRow>> Get(int id)
        {
            var legendRows = _context.LegendRow.Where(t => t.LegendId == id).ToList();

            if (legendRows.Count == 0)
            {
                return NotFound(); // Retourne une réponse 404 si aucun élément avec cet ID n'est trouvé
            }

            return legendRows;
        }


        // PUT api/<ColorSetController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ColorSetController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
