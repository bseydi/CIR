using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public partial class ChartRuleResponse
    {
        public ChartRuleResponse(ChartRule c)
        {
            ChartRuleId = c.ChartRuleId;
            TopicId = c.TopicId;
            TopicCode = c.Topic.Code;
            TopicName = c.Topic.Name;
            HtmlDescription = c.HtmlDescription;
            HtmlNote = c.HtmlNote;
            Generality = c.Generality;
            IsGeneric = c.IsGeneric;
            Level = c.Level;
            Value = c.Value;
            LastUpdatedDate = c.LastUpdatedDate;
            var categories = new List<int>();
            foreach (var category in c.Categories)
            {
                categories.Add(category.CategoryId);
            }
            Categories = categories;
        }

        public int ChartRuleId { get; set; }
        public int TopicId { get; set; }
        public string TopicCode { get; set; }
        public string TopicName { get; set; }
        public string HtmlDescription { get; set; } = null!;
        public string? HtmlNote { get; set; }
        public bool Generality { get; set; }
        public bool? IsGeneric { get; set; }
        public byte? Level { get; set; }
        public string? Value { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public virtual ICollection<int> Categories { get; set; }
    }
}
