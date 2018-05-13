using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zutrittkontrolle_Zeppelin_Rental.Models
{
    public class CompareViewModel
    {
        public List<CcorrectModel> _oldModel { get; set; }
        public List<CcorrectModel> _newModel { get; set; }
    }
}