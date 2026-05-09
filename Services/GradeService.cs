using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siemens.Internship2026.GradeBook.Interfaces;
using Siemens.Internship2026.GradeBook.Models;


namespace Siemens.Internship2026.GradeBook.Services
{
    public class GradeService : IGradeService
    {
        public double CalculateAverage(IEnumerable<Item> items)
        {
            return items.Any() ? (double)items.Average(i => i.Value) : 0;
        }

        public int GetTotalCount(IEnumerable<Item> items)
        {
            return items.Count();
        }
    }
}
