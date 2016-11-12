﻿using Domain.Entities;
using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Value_Objects
{
    public class ShirtNumbers : ValueObject<ShirtNumbers>
    {
        private Dictionary<int, bool> availableNumbers;
        private Team Team { get; }

        public ShirtNumber this[int? number]
        {
            get
            {
                this.SetAvailableNumbersToTrue();
                this.UpdateIsAvailableShirtNumber();
                try
                {
                    if (number.HasValue)
                    {
                        if (this.availableNumbers[(int)number])
                        {
                            this.availableNumbers[(int)number] = false;
                            return new ShirtNumber(this.Team.Id, number);
                        }
                        else
                        {
                            throw new ShirtNumberAlreadyInUseException("The shirt number you tried to assign is " +
                                "already being used by another player on the team.");
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (KeyNotFoundException)
                {
                    throw new IndexOutOfRangeException("A shirt number must be an integer within the range "
                        + "0-99");
                }
                catch (ShirtNumberAlreadyInUseException ex)
                {
                    throw ex;
                }
            }
        }

        public int[] AvailableNumbers
        {
            get
            {
                this.SetAvailableNumbersToTrue();
                this.UpdateIsAvailableShirtNumber();
                return this.availableNumbers.Where(x => x.Value).Select(x => x.Key).ToArray();
            }
        }

        public ShirtNumbers(Team team)
        {
            this.Team = team;
            this.availableNumbers = new Dictionary<int, bool>();
            for (int i = 0; i < 99; i++)
            {
                this.availableNumbers.Add(i, true);
            }
        }

        private void SetAvailableNumbersToTrue()
        {
            for (int i = 0; i < 99; i++)
            {
                this.availableNumbers[i] = true;
            }
        }

        private void UpdateIsAvailableShirtNumber()
        {
            var playerService = new PlayerService();
            var players = playerService.GetAll().ToList();
            foreach (var playerId in this.Team.PlayerIds)
            {
                var player = players.Find(x => x.Id == playerId);
                if (player.ShirtNumber.Value.HasValue)
                {
                    this.availableNumbers[(int)player.ShirtNumber.Value] = false;
                }
            }
        }
    }
}