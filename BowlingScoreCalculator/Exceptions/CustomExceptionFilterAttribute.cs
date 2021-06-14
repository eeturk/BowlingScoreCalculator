using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BowlingScoreCalculator.Models;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace BowlingScoreCalculator.Exceptions
{
    public class CustomExceptionFilterAttribute :
        ExceptionFilterAttribute, IExceptionFilter
    {
        public override void OnException(ExceptionContext context)
        {
            if(context.Exception is CustomException)
            {
                var ex = context.Exception as CustomException;

                context.HttpContext.Response.StatusCode = ex.StatusCode;

                ScoreCalculatorResponse res = new ScoreCalculatorResponse();
                res.status.Code = (HttpStatusCode)ex.StatusCode;
                res.status.Message = ex.StatusMessage;

                context.Result = new JsonResult(res);
            }          
        }
    }
}
