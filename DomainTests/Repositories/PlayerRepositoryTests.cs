﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Domain.Services;
using Domain.Value_Objects;

namespace DomainTests.Repositories
{
    [TestClass]
    public class PlayerRepositoryTests
    {



        [TestMethod]
        public void RepoStateIsTheSame()
        {
            var instance = TeamRepository.instance;
            var instance2 = TeamRepository.instance;

            Assert.AreEqual(instance, instance2);
        }

        [TestMethod]
        public void RepoGetAllReturnsIEnumerable()
        {
            Assert.IsInstanceOfType(PlayerRepository.instance.GetAll(), typeof(IEnumerable<Player>));
        }

        [TestMethod]
        public void AddPlayerWorking()
        {
            var newPlayer = new Player(new Name("Manuel", "Neuer"), new DateOfBirth("1986-03-27"), PlayerPosition.GoalKeeper, PlayerStatus.Available);
            PlayerRepository.instance.Add(newPlayer);
            var allPlayers = PlayerRepository.instance.GetAll();
            Assert.IsTrue(allPlayers.Contains(newPlayer));
        }

        [TestMethod]
        public void GetAllNotNull()
        {

            Assert.IsNotNull(PlayerRepository.instance.GetAll());
        }
    }
}
