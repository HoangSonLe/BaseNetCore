using System.Collections.Generic;

namespace Core.CommonModels
{
    public class Permission
    {
        public List<int> ListPermission { get; set; } = new List<int>();
        public bool Redirect { get; set; } = false;
    }
}
