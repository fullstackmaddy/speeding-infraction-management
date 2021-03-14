using System;
using System.Collections.Generic;
using System.Text;

namespace Speeding.Infraction.Management.AF01.Models
{
    public class OwnerNotificationMessage
    {
        public string InfractionDate { get; set; }

        public string InfractionDistrict { get; set; }

        public string TicketNumber { get; set; }

        public VehicleOwnerInfo VehicleOwnerInfo { get; set; }

        public List<OwnerNotificationMessageAttachment> Attachments { get; set; }


    }
}
