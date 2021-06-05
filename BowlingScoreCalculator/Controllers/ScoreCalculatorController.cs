using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BowlingScoreCalculator.Interfaces;

namespace BowlingScoreCalculator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScoreCalculatorController : ControllerBase
    {
        private IScoreCalculator _scoreCalculator;
        public ScoreCalculatorController(IScoreCalculator scoreCalculator)
        {
            _scoreCalculator = scoreCalculator;
        }
    }
}
