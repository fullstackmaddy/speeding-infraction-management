using System;
using System.Collections.Generic;
using System.Text;

namespace Speeding.Infraction.Management.AF01.ConfigOptions
{
    public class SendGridOptions
    {
        public string EmailSubject { get; set; }

        public string EmailFromAddress { get; set; }

        public string EmailBodyTemplate { get; set; }
    }
}
