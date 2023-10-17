using backend.Models;
using backend.Models.ColorScheme;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActiveLegendController : ControllerBase
    {
        private readonly CIRDevContext _context;

        public ActiveLegendController(CIRDevContext context)
        {
            _context = context;
        }

        // GET: api/<ActiveLegendController>
        [HttpGet]
        public IEnumerable<ActiveLegend> Get()
        {
            return _context.ActiveLegend.ToList();
        }

        // GET api/<LegendController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // PUT api/<LegendController>/5
        [HttpPut("Update/{newLegendId}")]
        public ActionResult Put(int newLegendId)
        {
            var activeLegend = _context.ActiveLegend.FirstOrDefault(a => a.ActiveLegendId == 1);

            if (activeLegend == null)
            {
                return NotFound(); // Retourne une réponse 404 si aucun élément avec cet ID n'est trouvé dans ActiveLegend
            }

            var legend = _context.Legend.FirstOrDefault(t => t.legendId == newLegendId);

            if (legend == null)
            {
                return NotFound(); // Retourne une réponse 404 si aucun élément avec le nouvel ID n'est trouvé dans Legend
            }

            // Mettre à jour le champ legendId de l'élément ActiveLegend
            activeLegend.LegendId = newLegendId;
            _context.SaveChanges(); // Sauvegarder les modifications dans la base de données

            return Ok(); // Retourne une réponse 200 (OK) pour indiquer que la mise à jour a réussi
        }

        // DELETE api/<ColorSetController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
