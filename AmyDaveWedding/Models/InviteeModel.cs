using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AmyDaveWedding.Models
{
    public class Invitee
    {
        public Invitee()
        {
            SignificantOtherKnown = false;
            Attending = false;
            AdditionalAdultCount = 0;
            ChildCount = 0;
            InterestedInChildCare = false;
        }

        public string Name { get; set; }

        public string ZipCode { get; set; }

        public string? Group { get; set; }

        public Boolean SignificantOtherKnown { get; set; }

        public bool Attending { get; set; }

        public int AdditionalAdultCount { get; set; }

        public int ChildCount { get; set; }

        public Boolean InterestedInChildCare { get; set; }
    }
}