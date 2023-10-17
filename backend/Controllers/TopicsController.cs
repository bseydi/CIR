using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicsController : ControllerBase
    {
        private readonly CIRDevContext _context;

        public TopicsController(CIRDevContext context)
        {
            _context = context;
        }

        // GET: api/Topics
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Topic>>> GetTopics(
        [FromQuery] string? topicCode = null, 
        [FromQuery] bool? isGeneric = null, 
        [FromQuery] int? level = -1, 
        [FromQuery] string? categoryName = null)
        {

            if (_context.ChartRules == null)
            {
                return NotFound();
            }
            var listTopic = _context.Topics.ToList();
            var list = await _context.ChartRules.ToListAsync();
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
            //listTopic = list.ConvertAll(x => x.Topic).ToList();
            return listTopic;
        }

        [HttpGet]
        [Route("GetTopicsForEditChartRule")]
        public async Task<ActionResult<IEnumerable<Topic>>> GetTopicsForEditChartRule([FromQuery] int? chartRuleId = -1)
        {

            if (_context.ChartRules == null)
            {
                return NotFound();
            }
            List<Topic> listTopic = null;
            /*            var list = await _context.ChartRules.ToListAsync();*/
            if (chartRuleId != -1)
            {
                listTopic = _context.Topics.Include(x => x.ChartRules).Where(x => x.ChartRules.Count == 0 || x.ChartRules.First().ChartRuleId == chartRuleId).ToList();
            }
            else
            {
                listTopic = _context.Topics.Include(x => x.ChartRules).Where(x => x.ChartRules.Count == 0).ToList();
            }
            return listTopic;
        }

        // GET: api/Topics/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Topic>> GetTopic(int id)
        {
            if (_context.Topics == null)
            {
                return NotFound();
            }
            var topic = await _context.Topics.FindAsync(id);

            if (topic == null)
            {
                return NotFound();
            }

            return topic;
        }

        // PUT: api/Topics/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> UpdateTopic()
        {
            var key = Convert.ToInt32(Request.Form["key"]);
            var values = Request.Form["values"];
            if (!TopicExists(key))
            {
                return BadRequest();
            }


            var topic = _context.Topics.Include(t => t.ParentTopic).Include(t => t.InverseParentTopic).Where(t => t.TopicId == key).FirstOrDefault();
            JsonConvert.PopulateObject(values, topic);

            try
            {

                if (!ValidateTopic(topic))
                {
                    return BadRequest("Insert not possible because the code already exists or parentTopicId = topicId");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            _context.Entry(topic).State = EntityState.Modified;

            var list = _context.Topics.ToList();
            list = list.Where(x => x.ParentTopicId == null).ToList();

            try
            {

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TopicExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (StackOverflowException)
            {
                return BadRequest();
            }

            return NoContent();
        }

        //POST: api/Topics
        //To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("Insert")]
        public async Task<IActionResult> InsertCategory()
        {
            var values = Request.Form["values"];
            var topic = new Topic();
            JsonConvert.PopulateObject(values, topic);
            _context.Topics.Update(topic);
            try
            {

                if (!ValidateTopic(topic))
                {
                    return BadRequest("Insert not possible because the code already exists");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            _context.Topics.Add(topic);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTopic", new { id = topic.TopicId }, topic);
        }



        // DELETE: api/Topics/5
        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> DeleteTopic()
        {
            var key = Convert.ToInt32(Request.Form["key"]);
            if (_context.Topics == null)
            {
                return NotFound();
            }
            var topic = _context.Topics
                .Include(t => t.ParentTopic)
                .Include(t => t.InverseParentTopic).Include(t => t.ChartRules)
                .Where(t => t.TopicId == key).FirstOrDefault();
            if (topic == null)
            {
                return BadRequest("Topic not found");
            }
            if (topic.InverseParentTopic.Count != 0)
            {
                return BadRequest("We cannot delete it because this element has children");
            }
            if (topic.ChartRules.Count != 0)
            {
                return BadRequest("We cannot delete it because this element has an associated rule");
            }
            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TopicExists(int id)
        {
            return (_context.Topics?.Any(e => e.TopicId == id)).GetValueOrDefault();
        }
        private bool ValidateTopic(Topic topic)
        {
            var list = _context.Topics.ToList();
            foreach (var t in list)
            {
                if (t.TopicId != topic.TopicId && t.Code == topic.Code)
                {
                    return false;
                }
            }
            if (topic.ParentTopicId != -1)
            {
                if (topic.ParentTopicId == topic.TopicId)
                {
                    return false;
                }
                DFS(topic, topic);
            }
            return true;
        }
        private void DFS(Topic t, Topic initialTopic)
        {
            foreach (var topic in t.InverseParentTopic)
            {
                if (initialTopic.ParentTopicId == topic.TopicId)
                {
                    throw new Exception("Infinite loop in TreeList");
                }
                DFS(topic, initialTopic);
            }
        }
    }
}
