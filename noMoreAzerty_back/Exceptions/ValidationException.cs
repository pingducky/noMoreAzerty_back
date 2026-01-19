namespace noMoreAzerty_back.Exceptions
{
    public class ValidationException : ApiException
    {
        public Dictionary<string, string[]> Errors { get; }

        public ValidationException(Dictionary<string, string[]> errors)
            : base("One or more validation errors occurred", StatusCodes.Status400BadRequest)
        {
            Errors = errors;
        }

        public ValidationException(string field, string errorMessage)
            : base("Validation error", StatusCodes.Status400BadRequest)
        {
            Errors = new Dictionary<string, string[]>
            {
                { field, new[] { errorMessage } }
            };
        }
    }

}
