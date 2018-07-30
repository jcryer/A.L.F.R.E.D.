using System.Collections.Generic;

namespace RPBot
{
    class TagObject {
        public class RootObject
        {
            public string Name { get; set; }
            public string Content { get; set; }
            public ulong OwnerId { get; set; }

            public RootObject(string name, string content, ulong ownerId)
            {
                this.Name = name;
                this.Content = content;
                OwnerId = ownerId;
            }
        }
    }
}
