using backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartRulesController : ControllerBase
    {
        private readonly CIRDevContext _context;

        public ChartRulesController(CIRDevContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChartRuleResponse>>> GetChartRules([FromQuery] string? topicCode = null, [FromQuery] bool? isGeneric = null, [FromQuery] int? level = -1, [FromQuery] string? categoryName = null)
        {

            if (_context.ChartRules == null)
            {
                return NotFound();
            }

            var list = await _context.ChartRules.Include(x => x.Topic).Include(x => x.Categories).ToListAsync();
            if (topicCode != null)
            {
                list = list.Where(x => x.Topic.Code == topicCode).ToList();
            }
            if (isGeneric != null)
            {
                list = list.Where(x => x.IsGeneric == isGeneric).ToList();
            }
            if (level != -1)
            {
                list = list.Where(x => x.Level == level).ToList();
            }
            if (categoryName != null)
            {
                list = list.Where(x => x.Categories.Where(c => c.Name == categoryName).Count() != 0).ToList();
            }
            var listResponse = new List<ChartRuleResponse>();
            foreach (var item in list)
            {
                listResponse.Add(new ChartRuleResponse(item));
            }
           
            return listResponse;
        }


        // GET: api/ChartRules/5
        [HttpGet("byTopic/{code}")]
        public IEnumerable<ChartRuleResponse> GetChartRulesByTopicCode(string code)
        {
            var topic = _context.Topics.ToList().Where(t => t.Code == code).FirstOrDefault();
            List<ChartRuleResponse> list = new List<ChartRuleResponse>();
            TopicTraverse(list, topic);

            return list;
        }
        private void TopicTraverse(List<ChartRuleResponse> list, Topic topic)
        {
            var c = _context.ChartRules.Where(c => c.TopicId == topic.TopicId).FirstOrDefault();
            if (c != null)
            {
                list.Add(new ChartRuleResponse(c));
            }
            foreach (Topic t in topic.InverseParentTopic)
            {
                TopicTraverse(list, t);
            }
        }
        

        [HttpGet("byCategories/{code}")]
        public IEnumerable<ChartRuleResponse> GetChartRulesByCategoriesCode(int code)
        {
            var category = _context.Categories.ToList().Where(t => t.CategoryId == code).FirstOrDefault();
            List<ChartRuleResponse> list = new List<ChartRuleResponse>();
            CategoriesTraverse(list, category);

            return list;
        }
        private void CategoriesTraverse(List<ChartRuleResponse> list, Category category)
        {
            var categoryId = category?.CategoryId;
            var chartRules = _context.ChartRules
                .Include(c => c.Topic)
                .Where(rule => rule.Categories.Any(c => c.CategoryId == categoryId))
                .ToList();

            foreach (var c in chartRules)
            {
                ChartRuleResponse chartRuleResponse = new ChartRuleResponse(c);
                list.Add(chartRuleResponse);
            }
        }

        // PUT: api/ChartRules/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> UpdateChartRule()
        {
            var key = Convert.ToInt32(Request.Form["key"]);
            var values = Request.Form["values"][0];
            var CC = JObject.Parse(values);
            if (!ChartRuleExists(key))
            {
                return BadRequest();
            }
            var chartRule = _context.ChartRules.Include(c => c.Categories).Where(c => c.ChartRuleId == key).FirstOrDefault();
            chartRule.LastUpdatedDate = DateTime.Now;
            var serializerSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
            JsonConvert.PopulateObject(values, chartRule, serializerSettings);
            if (chartRule.HtmlNote == null) chartRule.HtmlNote = "";
            if (chartRule.Level == null) chartRule.Level = 0;
            if (CC.ContainsKey("categories"))
            {
                chartRule.Categories.Clear();
                var tmp = CC["categories"].ToObject<ICollection<Category>>();
                foreach (var c in tmp)
                {
                    chartRule.Categories.Add(_context.Categories.Find(c.CategoryId));
                }
            }
            _context.Entry(chartRule).State = EntityState.Modified;
            try
            {
                if (!validate(chartRule))
                {
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChartRuleExists(key))
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


        // POST: api/ChartRules
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("Insert")]
        public async Task<IActionResult> InsertChartRule()
        {
            var values = Request.Form["values"];
            var CC = JObject.Parse(values);
            var chartRule = new ChartRule();
            JsonConvert.PopulateObject(values, chartRule);
            chartRule.LastUpdatedDate = DateTime.Now;
            if (chartRule.HtmlNote == null) chartRule.HtmlNote = "";
            if (chartRule.Level == null) chartRule.Level = 0;
            if (CC.ContainsKey("categories"))
            {
                chartRule.Categories.Clear();
                var tmp = CC["categories"].ToObject<ICollection<Category>>();
                foreach (var c in tmp)
                {
                    chartRule.Categories.Add(_context.Categories.Find(c.CategoryId));
                }
            }
            try
            {
                if (!validate(chartRule))
                {
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            _context.ChartRules.Add(chartRule);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/ChartRules/5
        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> DeleteChartRule()
        {
            var key = Convert.ToInt32(Request.Form["key"]);
            if (_context.ChartRules == null)
            {
                return NotFound();
            }
            var chartRule = _context.ChartRules.Include(x => x.Categories).Where(x => x.ChartRuleId == key).FirstOrDefault();
            foreach (var c in chartRule.Categories)
                if (chartRule == null)
                {
                    return NotFound();
                }
            foreach (var c in chartRule.Categories.ToList())
            {
                chartRule.Categories.Remove(c);
            }
            _context.ChangeTracker.DetectChanges();
            Console.WriteLine(_context.ChangeTracker.DebugView.LongView);
            _context.ChartRules.Remove(chartRule);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool validate(ChartRule chartRule)
        {
            var list = _context.ChartRules.ToList();
            list.Remove(chartRule);
            if (chartRule == null) { return false; }
            if (chartRule.TopicId < 0) { return false; }
            if (chartRule.Level < 0 || chartRule.Level > 3) { throw new Exception("Level must be  between 0 and 3"); }
            foreach (var c in chartRule.Categories)
            {
                if (!_context.Categories.Contains(c)) { return false; }
            }
            foreach (var c in list)
            {
                if (c.TopicId == chartRule.TopicId) throw new Exception("Topic is already use");
            }
            return true;
        }

        private bool ChartRuleExists(int id)
        {
            return (_context.ChartRules?.Any(e => e.ChartRuleId == id)).GetValueOrDefault();
        }

    }
}
