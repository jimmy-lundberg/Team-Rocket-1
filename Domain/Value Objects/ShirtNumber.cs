﻿using Domain.Value_Objects;
using System;

namespace Domain.Entities
{
    public class ShirtNumber : ValueObject<ShirtNumber>
    {
        public Guid PlayerTeamId { get; }

        public int Value { get; set; }

        public ShirtNumber(Guid teamId, int number)
        {
            this.PlayerTeamId = teamId;
            this.Value = number;
        }
    }
}