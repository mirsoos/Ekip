using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ekip.Domain.Entities.Base.Entities
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid RowVersion { get; private set; }
        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            CreateDate = DateTime.UtcNow;
            IsDeleted = false;
            RowVersion = Guid.NewGuid();
        }
        public void IncrementVersion()
        {
            RowVersion = Guid.NewGuid();
        }
    }
}
