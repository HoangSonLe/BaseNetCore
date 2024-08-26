using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Entitites
{
    public class Role : BaseAuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "varchar")]
        public required string NameNonUnicode { get; set; }
        public int Level { get; set; }

        public List<int> EnumActionList { get; set; }
    }
}
