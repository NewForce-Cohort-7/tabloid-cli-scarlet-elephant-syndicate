using System;
using System.Collections.Generic;

namespace TabloidCLI.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public static implicit operator List<object>(Tag v)
        {
            throw new NotImplementedException();
        }
    }
}