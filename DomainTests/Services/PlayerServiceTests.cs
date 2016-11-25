﻿using Domain.Entities;
using Domain.Interfaces;
using Domain.Value_Objects;
using DomainTests.Test_Dummies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Services.Tests
{
    [TestClass]
    public class PlayerServiceTests
    {
        private PlayerService playerService;
        private IEnumerable<Player> allPlayers;
        private Guid zlatanPlayerId;
        private Series series;
        private Team team1;
        private Team team2;
        private Team team3;
        private Team team4;

        [TestInitialize]
        public void Init()
        {
            this.playerService = new PlayerService();
            this.allPlayers = playerService.GetAll();
            this.zlatanPlayerId = allPlayers.ElementAt(0).Id;
            this.series = new Series(new MatchDuration(new TimeSpan(0, 90, 0)), new NumberOfTeams(4), "TestSerie");
        }

        [TestMethod]
        public void GetAllPlayersNotNull()
        {
            var getAllPlayers = this.playerService.GetAll();

            Assert.IsNotNull(getAllPlayers);
        }

        [TestMethod]
        public void FindPlayerByIdIsWorking()
        {
            var player = new Player(new Name("John", "Doe"), new DateOfBirth("1985-05-20"), PlayerPosition.Forward,
                PlayerStatus.Absent);
            Assert.IsFalse(playerService.FindById(player.Id) == player);
            playerService.Add(player);
            Assert.IsTrue(playerService.FindById(player.Id) == player);
        }

        #region PlayerService, FindPlayer metod tests

        [TestMethod]
        public void FindPlayerFullName()
        {
            var expectedPlayer =
                (Player)playerService.FindPlayer("Sergio Ramos", StringComparison.InvariantCultureIgnoreCase).First();
            var actualPlayerId = allPlayers.First(x => x.Name.ToString() == "Sergio Ramos").Id;

            Assert.AreEqual(expectedPlayer.Id, actualPlayerId);
        }

        [TestMethod]
        public void FindPlayerCaseSensitive()
        {
            var expectedPlayer =
                (Player)playerService.FindPlayer("SeRGio RaMos", StringComparison.InvariantCultureIgnoreCase).First();

            var actualPlayerId = allPlayers.First(x => x.Name.ToString() == "Sergio Ramos").Id;

            Assert.AreEqual(expectedPlayer.Id, actualPlayerId);
        }

        [TestMethod]
        public void FindPlayerPartOfFirstName()
        {
            var expectedPlayer =
                (Player)playerService.FindPlayer("ZLat", StringComparison.InvariantCultureIgnoreCase).First();

            var actualPlayerId = allPlayers.First(x => x.Name.ToString() == "Zlatan Ibrahimovic").Id;

            Assert.AreEqual(expectedPlayer.Id, actualPlayerId);
        }

        [TestMethod]
        public void FindPlayerPartOfLastName()
        {
            var expectedPlayer =
                (Player)playerService.FindPlayer("Ibra", StringComparison.InvariantCultureIgnoreCase).First();

            var actualPlayerId = allPlayers.First(x => x.Name.ToString() == "Zlatan Ibrahimovic").Id;

            Assert.AreEqual(expectedPlayer.Id, actualPlayerId);
        }

        [TestMethod]
        public void FindPlayerSpecialCharactersNotAllowed()
        {
            IPresentablePlayer expectedPlayerObj =
                this.playerService.FindPlayer("Ibra@%", StringComparison.InvariantCultureIgnoreCase).FirstOrDefault();

            Assert.IsNull(expectedPlayerObj);
        }

        #endregion PlayerService, FindPlayer metod tests

        [TestMethod]
        public void GetPlayerNameNotNull()
        {
            string expectedPlayerName = this.playerService.GetPlayerName(this.zlatanPlayerId);

            Assert.IsNotNull(expectedPlayerName);
        }

        [TestMethod]
        public void GetPlayerNameNotEmpty()
        {
            string expectedPlayerName = this.playerService.GetPlayerName(this.zlatanPlayerId);

            Assert.AreNotEqual("", expectedPlayerName);
        }

        [TestMethod]
        public void GetPlayerTeamIdNotNull()
        {
            Guid expectedTeamId = this.playerService.GetPlayerTeamId(this.zlatanPlayerId);

            Assert.IsNotNull(expectedTeamId);
        }

        [TestMethod]
        public void GetTopScorersTest()
        {
            var series = new DummySeries();
            var topScorers = playerService.GetTopScorersForSeries(series.SeriesDummy.Id);

            var allTeamsInSeries = series.SeriesDummy.TeamIds.Select(id => DomainService.FindTeamById(id)).ToList();
            var allPlayerInSeries = allTeamsInSeries.SelectMany(team => team.Players).ToList();
            var allPlayerStats = allPlayerInSeries.Select(player => player.PresentableSeriesStats[series.SeriesDummy.Id]).ToList();
            var allPlayerStatsSorted = allPlayerStats.OrderByDescending(ps => ps.GoalCount).Take(15);
            for (int i = 0; i < topScorers.Count(); i++)
            {
                Assert.IsTrue(allPlayerStatsSorted.ElementAt(i).GoalCount == topScorers.ElementAt(i).GoalCount);
            }
        }

        [TestMethod]
        public void GetTopAssistTest()
        {
            var series = new DummySeries();
            var topAssists = playerService.GetTopAssistsForSeries(series.SeriesDummy.Id);

            var allTeamsInSeries = series.SeriesDummy.TeamIds.Select(id => DomainService.FindTeamById(id)).ToList();
            var allPlayerInSeries = allTeamsInSeries.SelectMany(team => team.Players).ToList();
            var allPlayerStats = allPlayerInSeries.Select(player => player.PresentableSeriesStats[series.SeriesDummy.Id]).ToList();
            var allPlayerStatsSorted = allPlayerStats.OrderByDescending(ps => ps.AssistCount).Take(15);
            for (int i = 0; i < topAssists.Count(); i++)
            {
                Assert.IsTrue(allPlayerStatsSorted.ElementAt(i).AssistCount == topAssists.ElementAt(i).AssistCount);
            }
        }

        [TestMethod]
        public void GetTopRedCardsTest()
        {
            var series = new DummySeries();
            var topReds = playerService.GetTopRedCardsForSeries(series.SeriesDummy.Id);

            var allTeamsInSeries = series.SeriesDummy.TeamIds.Select(id => DomainService.FindTeamById(id)).ToList();
            var allPlayerInSeries = allTeamsInSeries.SelectMany(team => team.Players).ToList();
            var allPlayerStats = allPlayerInSeries.Select(player => player.PresentableSeriesStats[series.SeriesDummy.Id]).ToList();
            var allPlayerStatsSorted = allPlayerStats.OrderByDescending(ps => ps.RedCardCount).Take(5);
            for (int i = 0; i < topReds.Count(); i++)
            {
                Assert.IsTrue(allPlayerStatsSorted.ElementAt(i).RedCardCount == topReds.ElementAt(i).RedCardCount);
            }
        }

        [TestMethod]
        public void GetTopYellowCardsTest()
        {
            var series = new DummySeries();
            var topYellow = playerService.GetTopYellowCardsForSeries(series.SeriesDummy.Id);

            var allTeamsInSeries = series.SeriesDummy.TeamIds.Select(id => DomainService.FindTeamById(id)).ToList();
            var allPlayerInSeries = allTeamsInSeries.SelectMany(team => team.Players).ToList();
            var allPlayerStats = allPlayerInSeries.Select(player => player.PresentableSeriesStats[series.SeriesDummy.Id]).ToList();
            var allPlayerStatsSorted = allPlayerStats.OrderByDescending(ps => ps.YellowCardCount).Take(5);
            for (int i = 0; i < topYellow.Count(); i++)
            {
                Assert.IsTrue(allPlayerStatsSorted.ElementAt(i).YellowCardCount == topYellow.ElementAt(i).YellowCardCount);
            }
        }

        [TestMethod]
        public void AddListOfPlayerTest()
        {
            var series = new DummySeries();
            var playerOne = new Player(new Name("Kalle", "Kuling"), new DateOfBirth("2012-06-12"), PlayerPosition.Forward, PlayerStatus.Available);
            var playerTwo = new Player(new Name("Kalle", "Kuling"), new DateOfBirth("2012-06-12"), PlayerPosition.Forward, PlayerStatus.Available);
            var playerThree = new Player(new Name("Kalle", "Kuling"), new DateOfBirth("2012-06-12"), PlayerPosition.Forward, PlayerStatus.Available);

            var players = new List<Player>
            {
                playerOne,
                playerTwo
            };
            playerService.Add(players);
            var allPlayers = DomainService.GetAllPlayers();
            Assert.IsTrue(allPlayers.Contains(playerOne));
            Assert.IsTrue(allPlayers.Contains(playerTwo));
            Assert.IsFalse(allPlayers.Contains(playerThree));
        }
    }
}