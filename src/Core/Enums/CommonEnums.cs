namespace Core.Enums
{
    /// Rule for naming : E + PascalCase

    /// <summary>
    /// Encode password type
    /// </summary>
    public enum EEncodeType
    {
        Sha256
    }
    /// <summary>
    /// State of row of database
    /// </summary>
    public enum EState
    {
        Active = 1,
        Delete = 2,
    }
    /// <summary>
    /// Enum file type
    /// </summary>
    public enum EFileType
    {
        Excel,
        //Word,...
    }
}
