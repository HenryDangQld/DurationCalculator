using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DurationCalculator.Data;
using DurationCalculator.Library;
using DurationCalculator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DurationCalculator.Services
{
    /// <summary>
    /// Calculating duration for 10000 records without loop
    /// </summary>
    public class JobtimingsService
    {
        public DataContext _context;

        /// <summary>
        /// Initialize variable
        /// </summary>
        /// <param name="context"></param>
        public JobtimingsService(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all data from jobtiming table
        /// </summary>
        /// <param name="jobday">Day selection</param>
        /// <returns></returns>
        public List<jobtiming> GetJobtimings(string jobday)
        {
            List<jobtiming> listRequest = new List<jobtiming>();

            //listRequest = await _context.jobtimings.ToListAsync();

            if (jobday == "All")
            {
                var result = _context.jobtimings.ToList();

                listRequest.AddRange(result);
            }
            else
            {
                listRequest = (from item in _context.jobtimings
                               where item.jobday == jobday
                               select new jobtiming
                               {
                                   jobno = item.jobno,
                                   operatorID = item.operatorID,
                                   jobday = item.jobday,
                                   jobtime = item.jobtime,
                                   id = item.id,
                                   stationNo = item.stationNo,
                                   duration = item.duration,
                                   filename = item.filename,
                                   handle = item.handle,
                                   itemno = item.itemno,
                                   storageInfo = item.storageInfo,
                                   packingID = item.packingID,
                                   resetDay = item.resetDay,
                                   resetTime = item.resetTime
                               }).ToList();
            }

            return listRequest;
        }

        /// <summary>
        /// Calculating duration in this function
        /// </summary>
        /// <returns></returns>
        public List<jobtiming> CalculateDuration()
        {
            List<jobtiming> jobtimingOriginal = GetJobtimings("All");

            List<jobtiming> orderedJobtiming = new List<jobtiming>();
            List<jobtiming> orderedJobtimingExtra = new List<jobtiming>();

            var resultLogout = jobtimingOriginal.FindAll(x => x.jobno.Contains("logout")).Select(y => {
                y.operatorID = y.jobno.Split(" - ")[0];
                y.duration = "00:00:00";
                return y;
            });
            var resultNoLogout = jobtimingOriginal.FindAll(x => !x.jobno.Contains("logout"));

            jobtimingOriginal.Clear();
            jobtimingOriginal.AddRange(resultLogout);
            jobtimingOriginal.AddRange(resultNoLogout);

            orderedJobtiming = jobtimingOriginal.
                OrderBy(x => x.operatorID).
                ThenBy(y => DateTime.ParseExact(y.jobday, "d/M/yyyy", CultureInfo.InvariantCulture)).
                ThenBy(z => DateTime.ParseExact(z.jobtime, "HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

            orderedJobtimingExtra.AddRange(orderedJobtiming);
            orderedJobtimingExtra.Add(new jobtiming {
                jobno = "",
                operatorID = "",
                jobday = "",
                jobtime = "00:00:00",
                id = 0,
                stationNo = 0,
                duration = "00:00:00",
                filename = "",
                handle = "",
                itemno = "",
                storageInfo = "",
                packingID = "",
                resetDay = "",
                resetTime = ""
            });
            orderedJobtimingExtra.RemoveAt(0);

            var newResult = orderedJobtiming.Select((x, i) => {
                string durationString = "00:00:00";

                if (IsValidTimeFormat(orderedJobtimingExtra[i].jobtime) &&
                IsValidTimeFormat(x.jobtime) &&
                (orderedJobtimingExtra[i].operatorID == x.operatorID) &&
                (orderedJobtimingExtra[i].jobday == x.jobday))
                {
                    double duration = StringToSeconds(orderedJobtimingExtra[i].jobtime) - StringToSeconds(x.jobtime);
                    if (duration > 0)
                    {
                        durationString = SecondsToString(duration);
                        x.duration = durationString;
                    }
                }

                return x;
            }).ToList();

            var sortNewResult = newResult.OrderBy(x => x.id).ToList();

            ExportCSV.StartExport(sortNewResult, "jobtiming");

            return newResult;
        }

        /// <summary>
        /// Convert hh:mm:ss to seconds
        /// </summary>
        /// <param name="time">Time stamp in hh:mm:ss</param>
        /// <returns></returns>
        private double StringToSeconds(string time)
        {
            return TimeSpan.Parse(time).TotalSeconds;
        }

        private string SecondsToString(double secs)
        {
            TimeSpan t = TimeSpan.FromSeconds(secs);

            return string.Format("{0:D2}:{1:D2}:{2:D2}",
                            t.Hours,
                            t.Minutes,
                            t.Seconds);
        }

        /// <summary>
        /// Check if the time is in correct format
        /// </summary>
        /// <param name="input">Time stamp in hh:mm:ss</param>
        /// <returns></returns>
        private bool IsValidTimeFormat(string input)
        {
            TimeSpan dummyOutput;
            return TimeSpan.TryParse(input, out dummyOutput);
        }
    }
}
