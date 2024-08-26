using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }
        public List<int> RoleIds { get; set; }
        public string RoleName { get; set; }
        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }
        public bool IsActived { get; set; }




    }
}
