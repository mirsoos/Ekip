using Ekip.Domain.Entities.Base.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ekip.Domain.Entities.ReadModels
{
    public class ProfileReadModel 
    {
        public long Id { get; set; }
        public long UserRef { get; set; }
        public string AvatarUrl { get; set; }
        public double? Score { get; set; }
        public int Experience { get; set; }
    }
}
