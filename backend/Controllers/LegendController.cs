using backend.Models;
using backend.Models.ColorScheme;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LegendController : ControllerBase
    {
        private readonly CIRDevContext _context;

        public LegendController(CIRDevContext context)
        {
            _context = context;
        }

        // GET: api/<LegendController>
        [HttpGet]
        public IEnumerable<Legend> Get()
        {
            return _context.Legend.ToList();
        }

        // GET api/<LegendController>/5
        [HttpGet("{id}")]
        public ActionResult<IEnumerable<Legend>> Get(int id)
        {
            var activeLegend = _context.ActiveLegend.FirstOrDefault(a => a.ActiveLegendId == id);

            if (activeLegend == null)
            {
                return NotFound(); // Retourne une réponse 404 si aucun élément avec cet ID n'est trouvé dans ActiveLegend
            }

            var legend = _context.Legend.Where(t => t.legendId == activeLegend.LegendId).ToList();

            if (legend.Count == 0)
            {
                return NotFound(); // Retourne une réponse 404 si aucun élément avec cet ID n'est trouvé dans Legend
            }

            return legend;
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
