namespace TaskManagement.Domain.Common.ExtensionMethods
{
    public static class ConversionExtension
    {
        public static TKey ToKey<TKey>(this string value)
        {
            if (typeof(TKey) == typeof(Guid))
            {
                return (TKey)(object)Guid.Parse(value);
            }

            return (TKey)Convert.ChangeType(value, typeof(TKey));
        }

    }
}
