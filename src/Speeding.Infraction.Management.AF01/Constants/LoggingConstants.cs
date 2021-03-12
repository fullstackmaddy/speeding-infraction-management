namespace  Speeding.Infraction.Management.AF01.Constants
{
    public class LoggingConstants
    {
        public const string Template
            = "{EventDescription}{CorrelationId}{ProcessingFunction}{ProcessStatus}{LogMessage}";
        
        public enum ProcessingFunction
        {
            DetectFaces,
            CreateSpeedingTicket,
            NotifyVehicleOwner,
            HandleFailures

        }

        public enum EventId
        {
            Exceptioned = 001,

            DetectFacesStarted = 101,
            DetectFacesFinished = 102,

            CreateSpeedingTicketStarted = 201,
            CreateSpeedingTicketFinished = 202,

            NotifyVehicleOwnerStarted = 301,
            NotifyVehicleOwnerFinished = 302,

            HandleFailuresStarted = 401,
            HandleFailuresFinished = 402


        }

        public enum ProcessStatus
        {
            Started,
            Finished,
            Failed
        }

    }
    
}