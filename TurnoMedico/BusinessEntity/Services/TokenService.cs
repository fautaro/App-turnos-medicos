using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntity.Services
{
    public class TokenService
    {
        public string GenerateGuidToken()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
