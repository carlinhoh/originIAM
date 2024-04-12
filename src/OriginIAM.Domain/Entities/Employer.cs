using System;

namespace OriginIAM.Domain.Entities
{
    public class Employer
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Employer()
        {
                
        }
        public Employer(string name)
        {
            Id = Guid.NewGuid().ToString();
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
        public Employer(string name, string id)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
