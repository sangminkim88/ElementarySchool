using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public enum EAttendance
    {
        결석,
        지각,
        조퇴,
        현장학습,          
    }

    public enum EConfigSection
    {
        Students,
        Attendance,
    }
    public enum EConfigKey
    {
        FilePath,
    }
}
