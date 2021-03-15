namespace  Speeding.Infraction.Management.AF01.Constants
{
    public class LoggingConstants
    {
        public const string Template
            = "{EventDescription}{CorrelationId}{ProcessingFunction}{ProcessStatus}{LogMessage}";
        
        public enum ProcessingFunction
        {
            ExtractRegistrationNumber,
            DetectAndBlurFaces,
            CreateSpeedingTicket,
            NotifyRegisteredOwner,
            ManageExeceptions

        }

        public enum EventId
        {
            

            ExtractRegistrationNumberStarted = 101,
            ExtractRegistrationNumberFinished = 102,

            DetectAndBlurFacesStarted = 201,
            DetectAndBlurFacesFinished = 202,

            CreateSpeedingTicketStarted = 301,
            CreateSpeedingTicketFinished = 302,

            NotifyVehicleOwnerStarted = 401,
            NotifyVehicleOwnerFinished = 402,

            ManageExeceptionsStarted = 401,
            ManageExeceptionsFinished = 402


        }

        public enum ProcessStatus
        {
            Started,
            Finished,
            Failed
        }

    }
    
}