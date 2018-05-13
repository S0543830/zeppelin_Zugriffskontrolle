using System;

namespace Zutrittkontrolle_Zeppelin_Rental.Models
{
    public class CRFID
    {
        public int RFID
        {
            get;
            set;
        }

        public DateTime EventTime
        {
            get;
            set;
        }

        public string EventSource
        {
            get;
            set;
        }

        public string EventType
        {
            get;
            set;
        }
    }
}