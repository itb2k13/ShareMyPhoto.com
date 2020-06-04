namespace ShareMyPhoto.models
{
    public static class Extensions
    {
        public static bool Exists(this string s)
        {
            return s != null && s != string.Empty;
        }
    }
}
