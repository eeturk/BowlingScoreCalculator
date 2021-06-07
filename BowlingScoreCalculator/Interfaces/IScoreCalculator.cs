using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BowlingScoreCalculator.Models;

namespace BowlingScoreCalculator.Interfaces
{
    public interface IScoreCalculator
    {
        ScoreCalculatorResponse GetProgressScore(ScoreCalculatorRequest request);
    }
}
