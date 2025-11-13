using System.Text;

namespace Livestock.Auth.Database;

public static class StringExtensions
{
    public static string ToSnakeCase(this string text)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(char.ToLowerInvariant(text[0]));
        for (int index = 1; index < text.Length; ++index)
        {
            var c = text[index];
            if (char.IsUpper(c))
            {
                stringBuilder.Append('_');
                stringBuilder.Append(char.ToLowerInvariant(c));
            }
            else
                stringBuilder.Append(c);
        }
        return stringBuilder.ToString();
    }
}