﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TabloidCLI.Models
{
    public class Post
    {
        internal string URL;

        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public DateTime PublishDateTime { get; set; }
        public List<Tag> Tags { get; set; } = new List<Tag>();
        public Author Author { get; set; }
        public Blog Blog { get; set; }

        public override string ToString()
        {
            return $"{Title} ({PublishDateTime})";
        }

    }
}
