using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

namespace BowlingScoreCalculator.Models
{
    public class ScoreCalculatorResponse
    {
        public List<string> FrameProgressScores { get; set; }
        public bool GameCompleted { get; set; }
        public Status status { get; set; }

        public class Status
        {
            public HttpStatusCode Code { get; set; }
            public string Message { get; set; }
        }
    }
}
