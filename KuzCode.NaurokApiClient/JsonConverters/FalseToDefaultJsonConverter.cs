namespace KuzCode.NaurokApiClient.JsonConverters
{
    internal class FalseToDefaultJsonConverter<T> : ValueOrDefaultJsonConverter<T>
    {
        public FalseToDefaultJsonConverter() : base(false) { }
    }
}
