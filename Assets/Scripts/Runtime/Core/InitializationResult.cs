// ReSharper disable once CheckNamespace
namespace MP.Core
{
    public readonly struct InitializationResult
    {
        // Misc
        public readonly InitializationResultType Type;
        public bool WasSuccessful =>
            Type == InitializationResultType.Successful ||
            Type == InitializationResultType.SuccessfulWithErrors;
        public bool WasFailed =>
            Type == InitializationResultType.Failed;
        public bool HasResult => Type != InitializationResultType.None;
        public bool HasMessage => !string.IsNullOrEmpty(Message);
        public readonly string Message;
        
        // Defaults
        public static readonly InitializationResult None = new ();
        public static readonly InitializationResult Successful = new (InitializationResultType.Successful);
        public static readonly InitializationResult SuccessfulWithErrors = new (InitializationResultType.SuccessfulWithErrors);
        public static readonly InitializationResult Failed = new (InitializationResultType.Failed);

        public InitializationResult(InitializationResultType resultType)
            : this(resultType, string.Empty)
        {

        }

        public InitializationResult(InitializationResultType resultType, string message)
            : this()
        {
            Type = resultType;
            Message = message;
        }
    }
}
