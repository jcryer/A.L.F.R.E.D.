﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPBot
{
    public class InstanceObject
    {
        public class ChannelTemplate
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public List<string> Content { get; set; }

            public ChannelTemplate(int id, string name, List<string> content)
            {
                this.Id = id;
                this.Name = name;
                this.Content = content;
            }
        }
    }
}