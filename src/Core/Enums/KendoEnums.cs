using System.ComponentModel;

namespace Core.Enums
{
    /// <summary>
    /// Enum type notification for kendoUI
    /// </summary>
    public enum EKendoNotificationType
    {
        [Description("info")]
        Info,
        [Description("success")]
        Success,
        [Description("warning")]
        Warning,
        [Description("error")]
        Error
    }
}
