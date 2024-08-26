using Core.Enums;

namespace Core.Helper
{
    public static class FileHelper
    {
        private static List<string> GetExtensionExcelFile()
        {
            return new List<string>()
            {
                ".xlsx", ".xls", ".xlsm", ".csv"
            };
        }
        /// <summary>
        /// Validate file upload (Triển khai thêm type nếu chưa có)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="validateFileType"></param>
        /// <returns></returns>
        public static bool ValidateFileExt(EFileType type, string validateFileType)
        {
            //Declare file"s ext
            List<string> excelExt = GetExtensionExcelFile();

            //Switch return allow ext List

            List<string> allowedExtensions = new List<string>();

            switch (type)
            {
                case EFileType.Excel:
                    allowedExtensions = excelExt;
                    break;
                default: break;
            }
            //Validate file type

            if (!string.IsNullOrEmpty(validateFileType))
            {
                // Compare the file extension with the allowed extensions
                foreach (var allowedExtension in allowedExtensions)
                {
                    if (validateFileType.Equals(allowedExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        return true; // Valid extension
                    }
                }
            }
            return false;
        }
    }
}
