using System;
using System.Collections.Generic;

namespace backend.Models
{
    public partial class File
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public byte[] Content { get; set; } = null!;
    }
}
