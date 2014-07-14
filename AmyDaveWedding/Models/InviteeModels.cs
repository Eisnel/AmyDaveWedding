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
            LockedFromUserAssignment = false;
            Group = "";
            IsKnown = false;
            ChildCount = 0;
            InvitedToRehearsal = false;
            InternalNotes = "";
            Note = "";
        }

        public int Id { get; set; }

        public bool LockedFromUserAssignment { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string ZipCode { get; set; }

        public string Group { get; set; }

        public bool IsKnown { get; set; }

        public bool? Attending { get; set; }

        public int ChildCount { get; set; }

        public bool? InterestedInChildCare { get; set; }

        public bool InvitedToRehearsal { get; set; }

        public bool? AttendingRehearsal { get; set; }

        public string InternalNotes { get; set; }

        public string Note { get; set; }

        public DateTime? RsvpDate { get; set; }
    }

    public class RsvpModel
    {
        [Display(Name = "Attending")]
        [Required]
        public bool? Attending { get; set; }

        [Display(Name = "Plus One Attending")]
        public bool? PlusOneAttending { get; set; }

        [Display(Name = "Child Count")]
        public int ChildCount { get; set; }

        [Display(Name = "Interested in Child Care")]
        public bool? InterestedInChildCare { get; set; }

        [Display(Name = "Rehearsal Dinner & Family Picnic")]
        public bool? AttendingRehearsal { get; set; }

        [Display(Name = "Note to the Bride and Groom")]
        [StringLength(5000, ErrorMessage = "Please limit your note to 5,000 characters")]
        public string Note { get; set; }

        // public DateTime? RsvpDate { get; set; }
    }
}