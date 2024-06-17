namespace Discerniy.Domain.Responses
{
    public class ActionConfirmationResponse
    {
        public string ActionType { get; set; }
        public object? Data { get; set; }

        public ActionConfirmationResponse(string actionType, object? data)
        {
            ActionType = actionType;
            Data = data;
        }

        public ActionConfirmationResponse(string actionType)
        {
            ActionType = actionType;
        }
    }
}
