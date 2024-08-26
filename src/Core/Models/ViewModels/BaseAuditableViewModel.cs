using Core.Enums;

namespace Core.Models.ViewModels
{
    public abstract class BaseAuditableViewModel
    {
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int CreatedBy { get; set; } = 1;

        public DateTime? UpdatedDate { get; set; } = DateTime.Now;

        public int? UpdatedBy { get; set; } = 1;
        /// <summary>
        /// EState
        /// </summary>
        public int State { get; set; } = (int)EState.Active;

    }
}
