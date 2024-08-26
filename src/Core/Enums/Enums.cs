using System.ComponentModel;

namespace Core.Enums
{
    /// Rule for naming : E + Noun  

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
        CRUD_TENANT = 1,

        CREATE_URN,
        READ_URN,
        UPDATE_URN,
        DELETE_URN,

        CREATE_USER,
        READ_USER,
        UPDATE_USER,
        DELETE_USER,

        CREATE_TENT,
        READ_TENT,
        UPDATE_TENT,
        DELETE_TENT,

        READ_CONFIG,
        UPDATE_CONFIG,

        CREATE_STORAGE,
        READ_STORAGE,
        UPDATE_STORAGE,
        DELETE_STORAGE,


        CREATE_REMINDER,
        READ_REMINDER,
        UPDATE_REMINDER,
        DELETE_REMINDER,
    }
    public enum EGender
    {
        [Description("Không xác định")]
        UNDEFINE,
        [Description("Nam")]
        MALE,
        [Description("Nữ")]
        FAMALE
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
        UNDEFINE,
        [Description("Gửi thông báo giỗ")]
        Anniversary,
        [Description("Gửi thông báo hạn ký gửi")]
        Expired
    }

}
