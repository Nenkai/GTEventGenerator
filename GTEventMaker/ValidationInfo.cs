using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTEventMaker
{
    public class ValidationInfo
    {
        public Severity Severity { get; set; }
        public string Message { get; set; }

        public ValidationInfo(Severity severity, string message)
        {
            this.Severity = severity;
            this.Message = message;
        }
    }

    public enum Severity
    {
        Info,
        Warning,
        Error,
    }
}
