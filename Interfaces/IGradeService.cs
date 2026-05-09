using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Interfaces
{
    public interface IGradeService
    {
        double CalculateAverage(IEnumerable<Item> items);
        int GetTotalCount(IEnumerable<Item> items);
    }
}
