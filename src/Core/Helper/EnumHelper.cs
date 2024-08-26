using Core.CommonModels;
using System.ComponentModel;

namespace Core.Helper
{
    public static class EnumHelper
    {
        public static string GetEnumDescription(this Enum enumValue)
        {
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : enumValue.ToString();
        }

        public static List<KendoDropdownListModel<string>> ToDropdownList<TEnum>()
        {
            var enumValues = Enum.GetValues(typeof(TEnum));
            var result = new List<KendoDropdownListModel<string>>();
            foreach (Enum e in enumValues)
            {
                var description = e.GetEnumDescription();
                result.Add(new KendoDropdownListModel<string>
                {
                    Text = description,
                    Value = Convert.ToInt32(e).ToString(),
                    //Data = e.ToString()
                });
            }
            return result;
        }

    }
}
