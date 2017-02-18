using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Code
{
    class ConvertHelper
    {
    }
}

public static class Extend
{
    public static int ConvertToInt(this string str)
    {
        int iresult;
        if (int.TryParse(str, out iresult))
        {
            return iresult;
        }
        else
        {
            return 0;
        }
    }

    public static int ConvertToInt(this object obj, int defaultValue)
    {
        int iresult;
        if (int.TryParse(obj.ToString(), out iresult)
            && iresult != 0)
        {
            return iresult;
        }

        return defaultValue;
    }

    public static int? ConvertToIntWithNull(this string str)
    {
        int iresult;
        if (int.TryParse(str, out iresult))
        {
            return iresult;
        }
        else
        {
            return null;
        }
    }

    public static bool IsInt(this string str)
    {
        int iresult;
        return int.TryParse(str, out iresult);
    }

    public static decimal ConvertToDecimal(this object str)
    {
        decimal iresult;
        if (decimal.TryParse(Convert.ToString(str), out iresult))
        {
            return iresult;
        }
        else
        {
            return decimal.Zero;
        }
    }

    public static decimal? ConvertToDecimalWithNull(this object str)
    {
        decimal iresult;
        if (decimal.TryParse(Convert.ToString(str), out iresult))
        {
            return iresult;
        }
        else
        {
            return null;
        }
    }

    public static decimal ConvertToDecimal(this object obj, decimal defaultValue)
    {
        decimal iresult;
        if (decimal.TryParse(obj.ToString(), out iresult)
            && iresult != 0m)
        {
            return iresult;
        }

        return defaultValue;
    }

    public static bool IsDecimal(this string str)
    {
        decimal iresult;
        return decimal.TryParse(str, out iresult);
    }

    public static bool ToBoolean(this string str)
    {
        bool iresult;
        if (bool.TryParse(str, out iresult))
        {
            return iresult;
        }
        else
        {
            return false;
        }
    }

    public static DateTime ConvertToDateTime(this string str)
    {
        DateTime iresult;
        if (DateTime.TryParse(str, out iresult))
        {
            return iresult;
        }
        else
        {
            return XkSystem.Code.DateHelper.MinDate;
        }
    }

    public static DateTime? ConvertToDateTimeWithNull(this string str)
    {
        DateTime iresult;
        if (DateTime.TryParse(str, out iresult))
        {
            return iresult;
        }
        else
        {
            return null;
        }
    }

    public static string ConvertToString(this object obj)
    {
        if (obj == null)
        {
            return string.Empty;
        }
        else
        {
            return obj.ToString().Trim();
        }
    }

    public static string ConvertDateTimeToString(this DateTime? obj, string format = "")
    {
        if (obj == null)
        {
            return string.Empty;
        }
        else
        {
            if (string.IsNullOrEmpty(format))
            {
                format = "yyyy-MM-dd HH:mm:ss";
            }
            return ((DateTime)obj).ToString(format);
        }
    }
}

