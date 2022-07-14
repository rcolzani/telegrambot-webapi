using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Telegram.WebAPI.Domain.Interfaces.Application
{
    public interface IRiverLevelApplication
    {
        Task SendRiverLevel();
    }
}
