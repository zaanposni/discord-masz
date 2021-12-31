namespace MASZ.Extensions
{
    public static class GuildIcon
    {
        public static string GetAnimatedOrDefaultAvatar(this string iconUrl)
        {
            if (iconUrl != null)
            {
                if (iconUrl.Split("/").Last().StartsWith("a_"))
                    iconUrl = iconUrl.Replace(".jpg", ".gif");
                else
                    iconUrl = iconUrl.Replace(".jpg", ".png");

                if (!iconUrl.Contains('?'))
                    iconUrl += "?size=512";
            }
            return iconUrl;
        }
    }
}
