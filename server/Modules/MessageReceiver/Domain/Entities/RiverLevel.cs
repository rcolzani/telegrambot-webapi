using System;

namespace Telegram.WebAPI.Domain.Entities;

public class RiverLevel
{
    public int Id { get; protected set; }
    public DateTime ReadDateTime { get; private set; }
    public double Level { get; private set; }
    public double Variation { get; private set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime UpdatedAt { get; protected set; }
}

