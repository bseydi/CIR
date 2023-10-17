using System;
using System.Collections.Generic;

namespace backend.Models
{
    public partial class Category
    {
        public Category()
        {
            ChartRules = new HashSet<ChartRule>();
        }

        public int CategoryId { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;

        public virtual ICollection<ChartRule> ChartRules { get; set; }
    }
}
