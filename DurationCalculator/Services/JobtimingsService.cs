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
    public class JobtimingsService
    {
        public DataContext _context;

        public JobtimingsService(DataContext context)
        {
            _context = context;
        }

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

                //var entity = _context.jobtimings.FirstOrDefault(item => item.id == x.id);
                //entity.duration = durationString;
                //_context.Entry(entity).State = EntityState.Modified;
                //_context.Update(entity);
                //_context.SaveChanges();

                return x;
            }).ToList();

            var sortNewResult = newResult.OrderBy(x => x.id).ToList();

            ExportCSV.StartExport(sortNewResult, "jobtiming");

            return newResult;
        }

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

        private bool IsValidTimeFormat(string input)
        {
            TimeSpan dummyOutput;
            return TimeSpan.TryParse(input, out dummyOutput);
        }
    }
}
