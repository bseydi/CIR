using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace backend.Models
{
    public partial class Topic
    {
        public Topic()
        {
            ChartRules = new HashSet<ChartRule>();
            InverseParentTopic = new HashSet<Topic>();
        }
        public int TopicId { get; set; }
        public int? ParentTopicId { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public virtual Topic? ParentTopic { get; set; }
        [JsonIgnore]
        public virtual ICollection<ChartRule> ChartRules { get; set; }
        public virtual ICollection<Topic> InverseParentTopic { get; set; }
    }
}
