using System.Runtime.Serialization;

namespace Blogsphere.Api.Gateway.Extensions;

public static class EnumValueExtensions
{
    public static string GetEnumMemberValue(this Enum value)
    {
        Type enumType = value.GetType();
        string enumName = Enum.GetName(enumType, value) ?? string.Empty;

        var memberInfo = enumType.GetMember(enumName);
        if(memberInfo.Length > 0)
        {
            var enumMemberAttribute = memberInfo[0].GetCustomAttributes(typeof(EnumMemberAttribute), false);
            if(enumMemberAttribute.Length > 0)
            {
                if (enumMemberAttribute[0] is EnumMemberAttribute enumMember && enumMember.Value != null)
                {
                    return enumMember.Value;
                }
            }
        }

        return enumName;
    }
}
