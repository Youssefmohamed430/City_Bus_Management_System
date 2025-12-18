namespace City_Bus_Management_System.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CacheableAttribute : Attribute
    {
        public string Key { get; }

        public CacheableAttribute(string key)
        {
            Key = key;
        }
    }
}
