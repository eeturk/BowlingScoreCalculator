using System;
using Xunit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using BowlingScoreCalculator.Services;
using BowlingScoreCalculator.Models;
using System.Collections;


namespace BowlingScoreCalculator.Tests
{
    [TestClass]
    public class BowlingScoreCalculatorTest
    {
        [Fact]
        public void TestForEmptyRolls()
        {
            var NullRequest = new ScoreCalculatorRequest() { PinsDowned = null };
            var service = new ScoreCalculatorService();
            var response = service.GetProgressScore(NullRequest);
            Xunit.Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.status.Code);
        }

        [Fact]
        public void TestForPinsDownedCountGreaterThanMaxValue()
        {
            var PinsDownedCountGreaterThanMaxValueRequest = new ScoreCalculatorRequest()
            {
                PinsDowned = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2 }
            };
            var service = new ScoreCalculatorService();
            var response = service.GetProgressScore(PinsDownedCountGreaterThanMaxValueRequest);
            Xunit.Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.status.Code);
        }

        [Fact]
        public void TestForNegativePinValue()
        {
            var NegativePinValueRequest = new ScoreCalculatorRequest()
            {
                PinsDowned = new List<int> { 1, 2, -3, 4, 5 }
            };
            var service = new ScoreCalculatorService();
            var response = service.GetProgressScore(NegativePinValueRequest);
            Xunit.Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.status.Code);
        }

        [Fact]
        public void TestForPinValueGreaterThanMaxValue()
        {
            var PinValueGreaterThanMaxValueRequest = new ScoreCalculatorRequest()
            {
                PinsDowned = new List<int> { 1, 2, 13, 4, 5, 6 }
            };

            var service = new ScoreCalculatorService();
            var response = service.GetProgressScore(PinValueGreaterThanMaxValueRequest);
            Xunit.Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.status.Code);
        }

        [Fact]
        public void TestForPerfectGame()
        {
            var PerfectGameRequest = new ScoreCalculatorRequest()
            {
                PinsDowned = new List<int> { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }
            };

            var ExpectedPerfectGameResponse = new ScoreCalculatorResponse()
            {
                FrameProgressScores = new List<string> { "30", "60", "90", "120", "150", "180", "210", "240", "270", "300" },
                GameCompleted = true
            };

            var service = new ScoreCalculatorService();
            var response = service.GetProgressScore(PerfectGameRequest);

            Xunit.Assert.Equal(System.Net.HttpStatusCode.OK, response.status.Code);
            Xunit.Assert.Equal(ExpectedPerfectGameResponse.GameCompleted, response.GameCompleted);
            Xunit.Assert.Equal((ICollection)ExpectedPerfectGameResponse.FrameProgressScores, (ICollection)response.FrameProgressScores);

        }

        [Fact]
        public void TestForGutterBallGame()
        {
            var GutterBallGameRequest = new ScoreCalculatorRequest()
            {
                PinsDowned = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            };

            var ExpectedGutterBallGameResponse = new ScoreCalculatorResponse()
            {
                FrameProgressScores = new List<string> { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0" },
                GameCompleted = true
            };

            var service = new ScoreCalculatorService();
            var response = service.GetProgressScore(GutterBallGameRequest);

            Xunit.Assert.Equal(System.Net.HttpStatusCode.OK, response.status.Code);
            Xunit.Assert.Equal(ExpectedGutterBallGameResponse.GameCompleted, response.GameCompleted);
            Xunit.Assert.Equal((ICollection)ExpectedGutterBallGameResponse.FrameProgressScores, (ICollection)response.FrameProgressScores);

        }

        [Fact]
        public void TestForNoSpareNoStrikeGame()
        {
            var NoSpareNoStrikeGameRequest = new ScoreCalculatorRequest()
            {
                PinsDowned = new List<int> { 2, 3, 2, 3, 2, 3, 2, 3, 2, 3, 2, 3, 2, 3, 2, 3, 2, 3, 2, 3 }
            };

            var ExpectedNoSpareNoStrikeGameResponse = new ScoreCalculatorResponse()
            {
                FrameProgressScores = new List<string> { "5", "10", "15", "20", "25", "30", "35", "40", "45", "50" },
                GameCompleted = true
            };

            var service = new ScoreCalculatorService();
            var response = service.GetProgressScore(NoSpareNoStrikeGameRequest);

            Xunit.Assert.Equal(System.Net.HttpStatusCode.OK, response.status.Code);
            Xunit.Assert.Equal(ExpectedNoSpareNoStrikeGameResponse.GameCompleted, response.GameCompleted);
            Xunit.Assert.Equal((ICollection)ExpectedNoSpareNoStrikeGameResponse.FrameProgressScores, (ICollection)response.FrameProgressScores);

        }

        [Fact]
        public void TestForSixFramesCompletedAllRollsOne()
        {
            var SixFramesCompletedAllRollsOneRequest = new ScoreCalculatorRequest()
            {
                PinsDowned = new List<int> { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
            };

            var ExpectedSixFramesCompletedAllRollsOneResponse = new ScoreCalculatorResponse()
            {
                FrameProgressScores = new List<string> { "2", "4", "6", "8", "10", "12" },
                GameCompleted = false
            };

            var service = new ScoreCalculatorService();
            var response = service.GetProgressScore(SixFramesCompletedAllRollsOneRequest);

            Xunit.Assert.Equal(System.Net.HttpStatusCode.OK, response.status.Code);
            Xunit.Assert.Equal(ExpectedSixFramesCompletedAllRollsOneResponse.GameCompleted, response.GameCompleted);
            Xunit.Assert.Equal((ICollection)ExpectedSixFramesCompletedAllRollsOneResponse.FrameProgressScores, (ICollection)response.FrameProgressScores);

        }

        [Fact]
        public void TestForSevenFramesCompletedWithSpareAndStrikes()
        {
            var SevenFramesCompletedWithSpareAndStrikesRequest = new ScoreCalculatorRequest()
            {
                PinsDowned = new List<int> { 1, 1, 1, 1, 9, 1, 2, 8, 9, 1, 10, 10 }
            };

            var ExpectedSevenFramesCompletedWithSpareAndStrikesResponse = new ScoreCalculatorResponse()
            {
                FrameProgressScores = new List<string> { "2", "4", "16", "35", "55", "*", "*" },
                GameCompleted = false
            };

            var service = new ScoreCalculatorService();
            var response = service.GetProgressScore(SevenFramesCompletedWithSpareAndStrikesRequest);

            Xunit.Assert.Equal(System.Net.HttpStatusCode.OK, response.status.Code);
            Xunit.Assert.Equal(ExpectedSevenFramesCompletedWithSpareAndStrikesResponse.GameCompleted, response.GameCompleted);
            Xunit.Assert.Equal((ICollection)ExpectedSevenFramesCompletedWithSpareAndStrikesResponse.FrameProgressScores, (ICollection)response.FrameProgressScores);
        }

        [Fact]
        public void TestForSpare()
        {
            var SparesRequest = new ScoreCalculatorRequest()
            {
                PinsDowned = new List<int> { 8, 2, 10 }
            };

            var ExpectedSparesResponse = new ScoreCalculatorResponse()
            {
                FrameProgressScores = new List<string> { "20", "*" },
                GameCompleted = false
            };

            var service = new ScoreCalculatorService();
            var response = service.GetProgressScore(SparesRequest);

            Xunit.Assert.Equal(System.Net.HttpStatusCode.OK, response.status.Code);
            Xunit.Assert.Equal(ExpectedSparesResponse.GameCompleted, response.GameCompleted);
            Xunit.Assert.Equal((ICollection)ExpectedSparesResponse.FrameProgressScores, (ICollection)response.FrameProgressScores);
        }

        [Fact]
        public void IncompleteFrameOnePriorStrike()
        {
            var PerfectGameRequest = new ScoreCalculatorRequest()
            {
                PinsDowned = new List<int> { 10, 10, 10, 9, 1, 5, 4, 9, 1, 10, 9 }
            };

            var ExpectedPerfectGameResponse = new ScoreCalculatorResponse()
            {
                FrameProgressScores = new List<string> { "30", "59", "79", "94", "103", "123", "*", "*" },
                GameCompleted = false
            };

            var service = new ScoreCalculatorService();
            var response = service.GetProgressScore(PerfectGameRequest);

            Xunit.Assert.Equal(System.Net.HttpStatusCode.OK, response.status.Code);
            Xunit.Assert.Equal(ExpectedPerfectGameResponse.GameCompleted, response.GameCompleted);
            Xunit.Assert.Equal((ICollection)ExpectedPerfectGameResponse.FrameProgressScores, (ICollection)response.FrameProgressScores);

        }

        [Fact]
        public void IncompleteFrameTwoPriorStrike()
        {
            var PerfectGameRequest = new ScoreCalculatorRequest()
            {
                PinsDowned = new List<int> { 9, 1, 10, 10, 10, 9 }
            };

            var ExpectedPerfectGameResponse = new ScoreCalculatorResponse()
            {
                FrameProgressScores = new List<string> { "20", "50", "79", "*", "*" },
                GameCompleted = false
            };

            var service = new ScoreCalculatorService();
            var response = service.GetProgressScore(PerfectGameRequest);

            Xunit.Assert.Equal(System.Net.HttpStatusCode.OK, response.status.Code);
            Xunit.Assert.Equal(ExpectedPerfectGameResponse.GameCompleted, response.GameCompleted);
            Xunit.Assert.Equal((ICollection)ExpectedPerfectGameResponse.FrameProgressScores, (ICollection)response.FrameProgressScores);

        }

        [Fact]
        public void IncompleteFramePriorSpare()
        {
            var PerfectGameRequest = new ScoreCalculatorRequest()
            {
                PinsDowned = new List<int> { 10, 10, 10, 9, 1, 5, 4, 9, 1, 9 }
            };

            var ExpectedPerfectGameResponse = new ScoreCalculatorResponse()
            {
                FrameProgressScores = new List<string> { "30", "59", "79", "94", "103", "122", "*" },
                GameCompleted = false
            };

            var service = new ScoreCalculatorService();
            var response = service.GetProgressScore(PerfectGameRequest);

            Xunit.Assert.Equal(System.Net.HttpStatusCode.OK, response.status.Code);
            Xunit.Assert.Equal(ExpectedPerfectGameResponse.GameCompleted, response.GameCompleted);
            Xunit.Assert.Equal((ICollection)ExpectedPerfectGameResponse.FrameProgressScores, (ICollection)response.FrameProgressScores);

        }

        [Fact]
        public void SingleNonStrikeRoll()
        {
            var PerfectGameRequest = new ScoreCalculatorRequest()
            {
                PinsDowned = new List<int> { 5 }
            };

            var ExpectedPerfectGameResponse = new ScoreCalculatorResponse()
            {
                FrameProgressScores = new List<string> { "*" },
                GameCompleted = false
            };

            var service = new ScoreCalculatorService();
            var response = service.GetProgressScore(PerfectGameRequest);

            Xunit.Assert.Equal(System.Net.HttpStatusCode.OK, response.status.Code);
            Xunit.Assert.Equal(ExpectedPerfectGameResponse.GameCompleted, response.GameCompleted);
            Xunit.Assert.Equal((ICollection)ExpectedPerfectGameResponse.FrameProgressScores, (ICollection)response.FrameProgressScores);

        }
    }
}
