using System.ComponentModel;

namespace Core.Enums
{
    /// <summary>
    /// Enum type notification for kendoUI
    /// </summary>
    public enum EKendoNotificationType
    {
        [Description("info")]
        INFO,
        [Description("success")]
        SUCCESS,
        [Description("warning")]
        WARNING,
        [Description("error")]
        ERROR
    }
}
