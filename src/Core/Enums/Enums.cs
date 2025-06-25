using System.ComponentModel;

namespace Core.Enums
{
    /// Rule for naming : E + PascalCase

    /// <summary>
    /// Define enums for specific models
    /// </summary>
    public enum EReactType
    {
        Like = 0,
        Dislike = 1,
        Haha = 2
    }
    public enum ERoleType
    {
        [Description("Admin hệ thống")]
        SystemAdmin = 1,
        [Description("Admin")]
        Admin,
        [Description("Reporter")]
        Reporter,
        [Description("Người dùng")]
        User
    }
    public enum ELevelRole
    {
        SystemAdmin = 1,
        Admin,
        Reporter,
        User
    }
    public enum EActionRole
    {
        CrudTenant = 1,

        CreateUrn,
        ReadUrn,
        UpdateUrn,
        DeleteUrn,

        CreateUser,
        ReadUser,
        UpdateUser,
        DeleteUser,

        CreateTent,
        ReadTent,
        UpdateTent,
        DeleteTent,

        ReadConfig,
        UpdateConfig,

        CreateStorage,
        ReadStorage,
        UpdateStorage,
        DeleteStorage,

        CreateReminder,
        ReadReminder,
        UpdateReminder,
        DeleteReminder,
    }
    public enum EGender
    {
        [Description("Không xác định")]
        Undefined,
        [Description("Nam")]
        Male,
        [Description("Nữ")]
        Female
    }
    public enum EUrnType
    {
        [Description("Linh")]
        Soul,
        [Description("Cốt")]
        Gauss
    }
    public enum ETelegramNotiType
    {
        [Description("Không xác định")]
        Undefined,
        [Description("Gửi thông báo giỗ")]
        Anniversary,
        [Description("Gửi thông báo hạn ký gửi")]
        Expired
    }

}
