//------------------------------------------------------------------------------
// <auto-generated>
//     Der Code wurde von einer Vorlage generiert.
//
//     Manuelle Änderungen an dieser Datei führen möglicherweise zu unerwartetem Verhalten der Anwendung.
//     Manuelle Änderungen an dieser Datei werden überschrieben, wenn der Code neu generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Zutrittkontrolle_Zeppelin_Rental.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Event
    {
        public int Id { get; set; }
        public System.DateTime eventTime { get; set; }
        public string eventSource { get; set; }
        public string eventType { get; set; }
        public int personId { get; set; }
    
        public virtual Person Person { get; set; }
    }
}
