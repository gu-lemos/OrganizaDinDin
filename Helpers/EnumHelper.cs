using System.ComponentModel;
using System.Reflection;

namespace OrganizaDinDin.Helpers
{
    public static class EnumHelper
    {
        public static string GetDescription(this System.Enum value)
        {
            FieldInfo? field = value.GetType().GetField(value.ToString());
            if (field == null) return value.ToString();

            DescriptionAttribute? attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? value.ToString();
        }
    }
}
