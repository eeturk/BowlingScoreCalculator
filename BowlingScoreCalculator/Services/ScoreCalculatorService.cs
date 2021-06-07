using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BowlingScoreCalculator.Interfaces;
using BowlingScoreCalculator.Models;
using Microsoft.Extensions.Configuration;

namespace BowlingScoreCalculator.Services
{
    public class ScoreCalculatorService : IScoreCalculator
    {
        public const int MaxPinValue = 10;
        public const int MinPinValue = 0;
        public const int MaxFramesCount = 10;
        public const int MaxRollsCount = 21;
        public const string PendingRoll = "*";

        public ScoreCalculatorResponse GetProgressScore(ScoreCalculatorRequest request)
        {
            List<int> pinsDowned = request?.PinsDowned?.ToList();

            if (pinsDowned == null || !pinsDowned.Any()) 
                throw new ArgumentException("Please provide valid number of pins downed.");

            if (pinsDowned.Count > MaxRollsCount) 
                throw new ArgumentException("Total number of rolls exceeds the maximum value.");

            if (pinsDowned.Any(p => p > MaxPinValue || p < MinPinValue))
                throw new ArgumentException("A pin value is outside of allowable values.");
            
            int frameProgressScore = 0;
            int framesCompleted = 0;
            bool bonusRollProvided = false;

            List<string> frameProgressScores = new List<string>();
            bool gameCompleted = false;

            for (int i = 0; i < pinsDowned.Count; i++)
            {
                int currentPinDowned = pinsDowned[i];
                bool isStrike = currentPinDowned == MaxPinValue;
                bool isSpare = false;

                if (isStrike == false && (i + 1) < pinsDowned.Count)
                {
                    isSpare = currentPinDowned + pinsDowned[i + 1] == MaxPinValue;
                }

                if (bonusRollProvided)
                {
                    continue;
                }

                if (isStrike)
                {
                    bool canCompleteStrike = (i + 2) < pinsDowned.Count;
                    if (canCompleteStrike)
                    {
                        int score = MaxPinValue + pinsDowned[i + 1] + pinsDowned[i + 2];
                        frameProgressScore += score;
                        frameProgressScores.Add(frameProgressScore.ToString());
                        framesCompleted++;
                    }
                    else
                    {
                       frameProgressScores.Add(PendingRoll);
                    }
                }
                else if (isSpare)
                {
                    bool canCompleteSpare = (i + 2) < pinsDowned.Count;
                    if (canCompleteSpare)
                    {
                        int score = currentPinDowned + pinsDowned[i + 1] + pinsDowned[i + 2];
                        frameProgressScore += score;
                        frameProgressScores.Add(frameProgressScore.ToString());
                        framesCompleted++;
                        i++;
                    }
                    else
                    {
                        frameProgressScores.Add(PendingRoll);
                    }
                }
                else
                {
                    if ((i + 1) < pinsDowned.Count)
                    {
                        int score = currentPinDowned + pinsDowned[i + 1];
                        frameProgressScore += score;
                        frameProgressScores.Add(frameProgressScore.ToString());
                        framesCompleted++;
                        i++;
                    }
                    else
                    {
                        int score = currentPinDowned;
                        frameProgressScore += score;
                        frameProgressScores.Add(frameProgressScore.ToString());
                    }
                }
                bonusRollProvided = framesCompleted == MaxFramesCount && (isSpare || isStrike);
            }

            gameCompleted = framesCompleted == MaxFramesCount;

            var response = new ScoreCalculatorResponse(){ 
                FrameProgressScores = frameProgressScores, 
                GameCompleted = gameCompleted 
            };

            return response;
        }
    }
}
