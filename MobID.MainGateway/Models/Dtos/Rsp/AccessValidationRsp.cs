namespace MobID.MainGateway.Models.Dtos.Rsp
{
    public class AccessValidationRsp
    {
        public bool IsValid { get; }
        public string Message { get; }

        public AccessValidationRsp(bool isValid, string message)
        {
            IsValid = isValid;
            Message = message;
        }
    }
}
