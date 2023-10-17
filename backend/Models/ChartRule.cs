using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public partial class ChartRule
    {
        public ChartRule()
        {
            Categories = new HashSet<Category>();
        }
        public int ChartRuleId { get; set; }
        public int TopicId { get; set; }
        public string HtmlDescription { get; set; } = null!;
        public string? HtmlNote { get; set; }
        public bool Generality { get; set; }
        public bool IsGeneric { get; set; }
        public byte? Level { get; set; }
        public string? Value { get; set; }
        public DateTime LastUpdatedDate { get; set; }

        public virtual Topic Topic { get; set; } = null!;

        public virtual ICollection<Category> Categories { get; set; }
    }
}
