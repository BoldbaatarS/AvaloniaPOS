namespace CloudApi.Utils
{
    public static class StringHelpers
    {
        /// <summary>
        /// Таслалаар салгасан утасны дугааруудыг List болгон буцаана.
        /// </summary>
        public static List<string> SplitPhones(string? phoneString)
        {
            if (string.IsNullOrWhiteSpace(phoneString))
                return new List<string>();

            return phoneString
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .ToList();
        }
    }
}
