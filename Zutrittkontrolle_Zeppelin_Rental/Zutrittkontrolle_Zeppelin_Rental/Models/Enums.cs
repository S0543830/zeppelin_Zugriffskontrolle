using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zutrittkontrolle_Zeppelin_Rental.Models
{
    public class Enums
    {
        public enum Source
        {
            DEV = 0,
            SYS = 1,
            MAN = 2
        }

        public enum Type
        {
            LEAVE,
            ENTER
        }
    }
}