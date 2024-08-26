using Core.Models.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Entitites
{
    public class User : BaseAuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public required string UserName { get; set; }
        public required string Password { get; set; }


        public required string Name { get; set; }
        public string? Email { get; set; }
        [Column(TypeName = "varchar")]
        public required string NameNonUnicode { get; set; }
        public required string Phone { get; set; } = "";
        public List<int> RoleIdList { get; set; }
        [NotMapped]
        public List<RoleViewModel> RoleList { get; set; } = new List<RoleViewModel>();
        public int TypeAccount { get; set; } // loại tk : admin - người dùng
    }
}
