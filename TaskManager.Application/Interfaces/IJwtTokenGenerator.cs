using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(Guid id, string email);
    }
}
