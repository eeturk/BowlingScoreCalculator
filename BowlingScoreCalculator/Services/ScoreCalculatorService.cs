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
using BowlingScoreCalculator.Exceptions;

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
                    throw new CustomException("Please provide valid number of pins downed.", 400);

                if (pinsDowned.Count > Constants.MaxRollsCount)
                    throw new CustomException("Total number of rolls exceeds the maximum value.", 400);

                if (pinsDowned.Any(p => p > Constants.MaxPinValue || p < Constants.MinPinValue))
                    throw new CustomException("A pin value is outside of allowable values.", 400);

                int frameProgressScore = 0;
                int framesCompleted = 0;
                bool bonusRollProvided = false;
                int totalRolls = pinsDowned.Count;

                for (int i = 0; i < totalRolls ; i++)
                {
                    int currentPinDowned = pinsDowned[i];
                    var rollsLeft = pinsDowned.GetRange(i, totalRolls - i);

                    bool isStrike = currentPinDowned == Constants.MaxPinValue;
                    bool isSpare = false;
                    bool invalidFrameScore = false;

                    if (i < totalRolls - 1)
                    {
                        isSpare = currentPinDowned + pinsDowned[i + 1] == Constants.MaxPinValue;
                        invalidFrameScore = (isStrike == false) && (currentPinDowned + pinsDowned[i + 1] > Constants.MaxPinValue);
                    }

                    if (invalidFrameScore)
                        throw new CustomException("Invalid frame score.", 400);

                    if (bonusRollProvided)
                    {
                        continue;
                    }

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

                    if (bonusRollProvided && rollsLeft.Count > 3)
                        throw new CustomException("Too many pins downed.", 400);
                }

                gameCompleted = framesCompleted == Constants.MaxFramesCount;

                response.FrameProgressScores = frameProgressScores;
                response.GameCompleted = gameCompleted;
                response.status = new ScoreCalculatorResponse.Status()
                {
                    Code = HttpStatusCode.OK,
                    Message = "Success."
                };
            }
            catch (CustomException ex)
            {
                response.FrameProgressScores = frameProgressScores;
                response.GameCompleted = gameCompleted;
                response.status = new ScoreCalculatorResponse.Status()
                {
                    Code = (HttpStatusCode)ex.StatusCode,
                    Message = ex.StatusMessage
                };
            }
            catch (Exception ex)
            {
                response.FrameProgressScores = frameProgressScores;
                response.GameCompleted = gameCompleted;
                response.status = new ScoreCalculatorResponse.Status()
                {
                    Code = HttpStatusCode.InternalServerError,
                    Message = "Internal Server Error. Please contact Admin"
                };
            }

            return response;
        }


    }
}

