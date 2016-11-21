﻿using Domain.Entities;
using Domain.Services;
using DomainTests.Test_Dummies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Domain.Value_Objects.Tests
{
    [TestClass]
    public class TeamSeriesEventsAndStatsTests
    {
        private DummySeries dummySeries;
        private Team teamTwo;

        [TestInitialize]
        public void Init()
        {
            this.dummySeries = new DummySeries();
            this.teamTwo = this.dummySeries.DummyTeams.DummyTeamTwo;
        }

        [TestMethod]
        public void TeamSeriesEventGamesIsNotNull()
        {
            Assert.IsNotNull(this.teamTwo.SeriesEvents[this.dummySeries.SeriesDummy.Id].Games);
        }

        [TestMethod]
        public void TeamSeriesStatsCanShowCorrectTeamWinCountBasedOnEvents()
        {
            var games = this.teamTwo.SeriesEvents[this.dummySeries.SeriesDummy.Id].Games;
            Assert.IsNotNull(games);
            var gamesWon = 0;
            foreach (var game in games)
            {
                if (game.Protocol.HomeTeamId == this.teamTwo.Id)
                {
                    if (game.Protocol.GameResult.HomeTeam_Score > game.Protocol.GameResult.AwayTeam_Score)
                    {
                        gamesWon++;
                    }
                }
                else if (game.Protocol.AwayTeamId == this.teamTwo.Id)
                {
                    if (game.Protocol.GameResult.HomeTeam_Score < game.Protocol.GameResult.AwayTeam_Score)
                    {
                        gamesWon++;
                    }
                }
            }
            var teamWins = this.teamTwo.PresentableSeriesStats[this.dummySeries.SeriesDummy.Id].Wins;
            Assert.IsTrue(teamWins != 0);
            Assert.IsTrue(gamesWon == teamWins);
        }

        [TestMethod]
        public void TeamSeriesStatsCanShowCorrectTeamDrawCountBasedOnEvents()
        {
            var games = this.teamTwo.SeriesEvents[this.dummySeries.SeriesDummy.Id].Games;
            Assert.IsNotNull(games);
            var gamesDraw = 0;
            foreach (var game in games)
            {
                if (game.Protocol.HomeTeamId == this.teamTwo.Id)
                {
                    if (game.Protocol.GameResult.HomeTeam_Score == game.Protocol.GameResult.AwayTeam_Score)
                    {
                        gamesDraw++;
                    }
                }
                else if (game.Protocol.AwayTeamId == this.teamTwo.Id)
                {
                    if (game.Protocol.GameResult.HomeTeam_Score == game.Protocol.GameResult.AwayTeam_Score)
                    {
                        gamesDraw++;
                    }
                }
            }
            var teamDraws = this.teamTwo.PresentableSeriesStats[this.dummySeries.SeriesDummy.Id].Draws;
            Assert.IsTrue(teamDraws != 0);
            Assert.IsTrue(gamesDraw == teamDraws);
        }

        [TestMethod]
        public void TeamSeriesStatsCanShowCorrectTeamLossCountBasedOnEvents()
        {
            var games = this.teamTwo.SeriesEvents[this.dummySeries.SeriesDummy.Id].Games;
            Assert.IsNotNull(games);
            var gamesLost = 0;
            foreach (var game in games)
            {
                if (game.Protocol.HomeTeamId == this.teamTwo.Id)
                {
                    if (game.Protocol.GameResult.HomeTeam_Score < game.Protocol.GameResult.AwayTeam_Score)
                    {
                        gamesLost++;
                    }
                }
                else if (game.Protocol.AwayTeamId == this.teamTwo.Id)
                {
                    if (game.Protocol.GameResult.HomeTeam_Score > game.Protocol.GameResult.AwayTeam_Score)
                    {
                        gamesLost++;
                    }
                }
            }
            var teamLosses = this.teamTwo.PresentableSeriesStats[this.dummySeries.SeriesDummy.Id].Losses;
            Assert.IsTrue(teamLosses != 0);
            Assert.IsTrue(gamesLost == teamLosses);
        }

        [TestMethod]
        public void TeamStatsCanShowCorrectTeamScoreBasedOnEvents()
        {
            var teamWins = this.teamTwo.PresentableSeriesStats[this.dummySeries.SeriesDummy.Id].Wins;
            var teamDraws = this.teamTwo.PresentableSeriesStats[this.dummySeries.SeriesDummy.Id].Draws;
            var teamLosses = this.teamTwo.PresentableSeriesStats[this.dummySeries.SeriesDummy.Id].Losses;
            var teamScore = this.teamTwo.PresentableSeriesStats[this.dummySeries.SeriesDummy.Id].Points;
            Assert.IsTrue(teamScore == (3 * teamWins) + (1 * teamDraws) + (0 * teamLosses));
        }

        [TestMethod]
        public void TeamSeriesStatsGoalsForReflectsChangesInEvents()
        {
            var goalsFor = DomainService.GetAllGames().Where(x => (x.SeriesId == this.dummySeries.SeriesDummy.Id)
            && x.Protocol.HomeTeamId == this.teamTwo.Id
            || x.Protocol.AwayTeamId == this.teamTwo.Id)
            .SelectMany(y => y.Protocol.Goals.Where(z => z.TeamId == this.teamTwo.Id));

            var preAddGoalsForCount = goalsFor.Count();
            Assert.IsTrue(this.teamTwo.PresentableSeriesStats[this.dummySeries.SeriesDummy.Id].GoalsFor
                == preAddGoalsForCount);

            this.dummySeries.DummyGames.GameThree.Protocol.Goals.Add(new Goal(new MatchMinute(35), this.teamTwo.Id,
                this.teamTwo.Players.ElementAt(0).Id));
            var postAddGoalsForCount = this.teamTwo.PresentableSeriesStats[this.dummySeries.SeriesDummy.Id].GoalsFor;
            Assert.IsTrue(postAddGoalsForCount - preAddGoalsForCount == 1);
        }

        [TestMethod]
        public void TeamSeriesStatsGoalsAgainstReflectsChangesInEvents()
        {
            var teamStats = this.teamTwo.PresentableSeriesStats[this.dummySeries.SeriesDummy.Id];
            var goalsAgainst = DomainService.GetAllGames().Where(x => (x.SeriesId == this.dummySeries.SeriesDummy.Id)
            && x.Protocol.HomeTeamId == this.teamTwo.Id
            || x.Protocol.AwayTeamId == this.teamTwo.Id)
            .SelectMany(y => y.Protocol.Goals.Where(z => z.TeamId != this.teamTwo.Id));

            var preAddGoalsAgainstCount = goalsAgainst.Count();
            Assert.IsTrue(teamStats.GoalsAgainst == preAddGoalsAgainstCount);
            this.dummySeries.DummyGames.GameThree.Protocol.Goals.Add(new Goal(new MatchMinute(60),
                this.dummySeries.DummyGames.GameThree.AwayTeamId, this.teamTwo.Players.ElementAt(0).Id));
            var postAddGoalAgainstCount = teamStats.GoalsAgainst;
            Assert.IsTrue(postAddGoalAgainstCount - preAddGoalsAgainstCount == 1);
        }

        [TestMethod]
        public void TeamSeriesStatsGoalDifferenceReflectsChangesInEvents()
        {
            var teamStats = this.teamTwo.PresentableSeriesStats[this.dummySeries.SeriesDummy.Id];
            var preAddGoalDiffernce = teamStats.GoalDifference;
            this.dummySeries.DummyGames.GameThree.Protocol.Goals.Add(new Goal(new MatchMinute(60),
                this.dummySeries.DummyGames.GameThree.AwayTeamId, this.teamTwo.Players.ElementAt(0).Id));
            var postAddGoalDifference = teamStats.GoalDifference;
            Assert.IsTrue(postAddGoalDifference - preAddGoalDiffernce == -1);
        }
    }
}