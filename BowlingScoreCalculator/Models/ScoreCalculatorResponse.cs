using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BowlingScoreCalculator.Models
{
    public class ScoreCalculatorResponse
    {
        public List<string> FrameProgressScores { get; set; }
        public bool GameCompleted { get; set; }
    }
}
