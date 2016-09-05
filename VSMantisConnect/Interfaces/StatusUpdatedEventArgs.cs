using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSMantisConnect.Interfaces
{
    public class StatusUpdatedEventArgs : EventArgs
    {
        public string StatusInfo { get; set; }
        public double  StatusPercentage { get; set; }
        public bool IsIndeterminate { get; set; }

        public StatusUpdatedEventArgs(string info, double percentage, bool isIndeterminate)
        {
            this.StatusInfo = info;
            this.StatusPercentage = percentage;
            this.IsIndeterminate = isIndeterminate;
        }
    }
}
