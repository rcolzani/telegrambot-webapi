using System;
using System.Collections.Generic;
using System.Text;
using Telegram.WebAPI.Domain.Entities;

namespace Telegram.WebAPI.Domain.Entities
{
    public class RiverLevel : Entity
    {
        public DateTime ReadDateTime { get; set; }
        public double Level { get; set; }
        public double Variation { get; set; }
    }
}
