using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BowlingScoreCalculator.Interfaces;
using BowlingScoreCalculator.Models;
using Microsoft.Extensions.Logging;
using System.Net;
using BowlingScoreCalculator.Exceptions;

namespace BowlingScoreCalculator.Controllers
{
    [Route("scores")]
    [ApiController]
    public class ScoreCalculatorController : ControllerBase
    {
        private IScoreCalculator _scoreCalculator;
        private readonly ILogger<ScoreCalculatorController> _logger;

        public ScoreCalculatorController(IScoreCalculator scoreCalculator, ILogger<ScoreCalculatorController> logger)
        {
            _scoreCalculator = scoreCalculator;
            _logger = logger;
        }

        // POST /scores
        /// <summary>
        /// Gets the frame progress scores with a status flag indicating whether or not game is completed
        /// </summary>
        /// <param name="request">ScoreCalculatorRequest</param>
        /// <returns>ScoreCalculatorResponse</returns>
        [HttpPost]
        [CustomExceptionFilter]
        public ActionResult<ScoreCalculatorResponse> GetScores(ScoreCalculatorRequest request)
        {
            _logger.LogTrace("Bowling game score calculation started..");
            var response = _scoreCalculator.GetProgressScore(request);

            if (response.status.Code == HttpStatusCode.InternalServerError)
            {
                _logger.LogCritical("[ERROR] Internal Server Error");
                throw new CustomException("Internal Server Error. Please contact Admin.", 500);
            }
            else if(response.status.Code == HttpStatusCode.BadRequest)
            {
                _logger.LogError("[ERROR] Bad Request ");
                return BadRequest(response);
            }

            _logger.LogTrace("Bowling game score calculation completed..");
            return Ok(response);
            
        }
    }
}
