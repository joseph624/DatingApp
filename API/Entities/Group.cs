using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class Group
    {
        // default constructor for entity framework to create table
        public Group()
        {
        }

        public Group(string name)
        {
            Name = name;
        }

        [Key] // name is primary key - Name of group
        public string Name { get; set; }
        public ICollection<Connection> Connections { get; set; } = new List<Connection>();

    }
}