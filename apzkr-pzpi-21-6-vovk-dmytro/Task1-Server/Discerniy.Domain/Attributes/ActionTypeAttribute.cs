namespace Discerniy.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ActionTypeAttribute : Attribute
    {
        public string ActionType { get; }
        public ActionTypeAttribute(string actionType)
        {
            ActionType = actionType;
        }
    }
}
