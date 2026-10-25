using System;
using System.Collections.Generic;
using System.Text;

namespace JwtStore.Core.SharedContext.Entities
{
    public abstract class Entity : IEquatable<Guid>
    {
        protected Entity() => Id = Guid.NewGuid();      

        public Guid Id { get; }
        public bool Equals(Guid id) => Id.Equals(id);
        public override int GetHashCode() => Id.GetHashCode();
        
    }
}
