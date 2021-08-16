using System;
using System.Collections.Generic;
using System.Text;
using Telegram.WebAPI.Domain.Entities;

namespace Telegram.WebAPI.Domain.Entities
{
    public class RiverLevel : Entity
    {
        public DateTime ReadDateTime { get; private set; }
        public double Level { get; private set; }
        public double Variation { get; private set; }
    }
}
