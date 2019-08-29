using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceWeb.Model.ViewModel
{
    public partial class InsertViewModel
    {
        [Required(ErrorMessage = "請填入客戶ID")]
        public string ApplicantId { get; set; }
        public string ActionContent { get; set; }
        [Required(ErrorMessage = "請填入點數")]
        public int Point { get; set; }
    }
}
