﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Dto.MoralStat
{
    public class StudentInfo:Student.Dto.Student.Info
    {

        public bool IsStar{ get; set; }
    }
}