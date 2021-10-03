using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Extensions
{
    public static class DateTimeExtension
    {
        public static int CalculateAge(this DateTime dateTime){
            var today = DateTime.Today;
            var age = today.Year - dateTime.Year;
            if (dateTime.Date > today.AddYears(-age))
            {
                return age--;
            }
            return age;
        }
    }
}