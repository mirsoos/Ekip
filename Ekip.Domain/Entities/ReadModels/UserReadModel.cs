using Ekip.Domain.Entities.Base.Entities;
using Ekip.Domain.Enums.Identity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ekip.Domain.Entities.ReadModels
{
    public class UserReadModel
    {
        public Guid Id { get; set; }
        public Guid ProfileRef { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public GenderType Gender { get; set; }
        public int Age { get; set; }
        public string PhoneNumber { get; set; }
    }
}
