namespace noMoreAzerty_back.Exceptions
{
    public class ForbiddenException : ApiException
    {
        public ForbiddenException(string message = "Accès interdit")
            : base(message, StatusCodes.Status403Forbidden) { }
    }
}
