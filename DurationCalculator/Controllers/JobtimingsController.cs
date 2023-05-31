using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DurationCalculator.Data;
using DurationCalculator.Models;
using DurationCalculator.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DurationCalculator.Controllers
{
    [ApiController]
    [Route("jobtiming")]
    public class JobtimingsController : Controller
    {
        private JobtimingsService jobtimingsService;

        public JobtimingsController(DataContext context)
        {
            //_context = context;
            jobtimingsService = new JobtimingsService(context);
        }

        [HttpGet("view")]
        public List<jobtiming> GetJobtiming()
        {
            return jobtimingsService.GetJobtimings("All");
            //return jobtimingsService.ViewJobtimings();
        }

        [HttpGet("calc")]
        public List<jobtiming> CalculateDuration()
        {
            return jobtimingsService.CalculateDuration();
        }
    }
}
