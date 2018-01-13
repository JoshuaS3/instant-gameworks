using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstantGameworks.Services
{
    class SysTime
    {

        public static string GetTime()
        {
            DateTime datetime = DateTime.Now;
            string CurrentTime = "";
            CurrentTime += string.Format("{0:0}:{1:00}:{2:00}.{3:000}", datetime.Hour, datetime.Minute, datetime.Second, datetime.Millisecond);
            return CurrentTime;
        }

    }
}
