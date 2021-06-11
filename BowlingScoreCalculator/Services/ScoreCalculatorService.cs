using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BowlingScoreCalculator.Interfaces;
using BowlingScoreCalculator.Models;
using Microsoft.Extensions.Configuration;
using BowlingScoreCalculator.Common;
using System.Net.Http;
using System.Net;

namespace BowlingScoreCalculator.Services
{
    public class ScoreCalculatorService : IScoreCalculator
    {
        public ScoreCalculatorResponse GetProgressScore(ScoreCalculatorRequest request)
        {
            var response = new ScoreCalculatorResponse();
            List<string> frameProgressScores = new List<string>();
            bool gameCompleted = false;

            try
            {
                List<int> pinsDowned = request?.PinsDowned?.ToList();

                if (pinsDowned == null || !pinsDowned.Any())
                    throw new ArgumentException("Please provide valid number of pins downed.");

                if (pinsDowned.Count > Constants.MaxRollsCount)
                    throw new ArgumentException("Total number of rolls exceeds the maximum value.");

                if (pinsDowned.Any(p => p > Constants.MaxPinValue || p < Constants.MinPinValue))
                    throw new ArgumentException("A pin value is outside of allowable values.");

                int frameProgressScore = 0;
                int framesCompleted = 0;
                bool bonusRollProvided = false;
                int totalRolls = pinsDowned.Count;

                for (int i = 0; i < totalRolls; i++)
                {
                    int currentPinDowned = pinsDowned[i];
                    var rollsLeft = pinsDowned.GetRange(i, totalRolls - i);

                    bool isStrike = currentPinDowned == Constants.MaxPinValue;
                    bool isSpare = false;                 

                    if (i < totalRolls - 1)
                    {
                        isSpare = currentPinDowned + pinsDowned[i + 1] == Constants.MaxPinValue;
                    }

                    if (bonusRollProvided)                   
                        continue;                   

                    if (isStrike)
                    {
                        if (rollsLeft.Count < 3)
                        {
                            frameProgressScores.Add(Constants.PendingRoll);
                        }
                        else
                        {
                            int score = Constants.MaxPinValue + pinsDowned[i + 1] + pinsDowned[i + 2];
                            frameProgressScore += score;
                            frameProgressScores.Add(frameProgressScore.ToString());
                        }

                    }
                    else if (isSpare)
                    {
                        if (rollsLeft.Count < 3)
                        {
                            frameProgressScores.Add(Constants.PendingRoll);
                        }
                        else
                        {
                            int score = currentPinDowned + pinsDowned[i + 1] + pinsDowned[i + 2];
                            frameProgressScore += score;
                            frameProgressScores.Add(frameProgressScore.ToString());
                        }
                        i++;
                    }
                    else
                    {
                        if (rollsLeft.Count < 2)
                        {
                            frameProgressScores.Add(Constants.PendingRoll);
                        }
                        else
                        {
                            int score = currentPinDowned + pinsDowned[i + 1];
                            frameProgressScore += score;
                            frameProgressScores.Add(frameProgressScore.ToString());
                        }
                        i++;
                    }

                    framesCompleted++;
                    bonusRollProvided = framesCompleted == Constants.MaxFramesCount && (isSpare || isStrike);
                }

                gameCompleted = framesCompleted == Constants.MaxFramesCount;


                response.FrameProgressScores = frameProgressScores;
                response.GameCompleted = gameCompleted;
                response.status = new ScoreCalculatorResponse.Status()
                {
                    Code = HttpStatusCode.OK,
                    Message = "Success"
                };
            }
            catch(Exception ex)
            {
                response.FrameProgressScores = frameProgressScores;
                response.GameCompleted = gameCompleted;
                response.status = new ScoreCalculatorResponse.Status()
                {
                    Code = HttpStatusCode.BadRequest,
                    Message = ex.Message
                };
            }

            return response;
        }
    }
}

