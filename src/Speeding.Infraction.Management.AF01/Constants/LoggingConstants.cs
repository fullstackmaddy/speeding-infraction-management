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

            DetectAndBlurFacesStarted = 301,
            DetectAndBlurFacesFinished = 302,

            CreateSpeedingTicketStarted = 201,
            CreateSpeedingTicketFinished = 202,

            NotifyVehicleOwnerStarted = 401,
            NotifyVehicleOwnerFinished = 402,

            ManageExeceptionsStarted = 501,
            ManageExeceptionsFinished = 502


        }

        public enum ProcessStatus
        {
            Started,
            Finished,
            Failed
        }

    }
    
}