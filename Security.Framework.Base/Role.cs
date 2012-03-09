using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace SecurityFramework
{
    public class Role
    {
        public Role()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
    }
}
