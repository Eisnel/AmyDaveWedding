using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
            LockedFromUserAssignment = false;
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string ZipCode { get; set; }

        public string Group { get; set; }

        public bool SignificantOtherKnown { get; set; }

        public bool Attending { get; set; }

        public int AdditionalAdultCount { get; set; }

        public int ChildCount { get; set; }

        public bool InterestedInChildCare { get; set; }

        public bool LockedFromUserAssignment { get; set; }
    }
}