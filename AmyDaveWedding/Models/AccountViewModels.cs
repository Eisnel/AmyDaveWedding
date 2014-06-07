using System;
using System.ComponentModel.DataAnnotations;

namespace AmyDaveWedding.Models
{
    //public class BaseRegistrationModel
    //{
    //    [Required]
    //    [Display(Name = "User name")]
    //    public string UserName { get; set; }

    //    [Display(Name = "Email")]
    //    public string Email { get; set; }

    //    [Display(Name = "Full name")]
    //    public string Name { get; set; }

    //    [Display(Name = "Code on invite")]
    //    public string Code { get; set; }

    //    [Display(Name = "Attending")]
    //    public bool Attending { get; set; }

    //    [Display(Name = "Plus one")]
    //    public bool PlusOne { get; set; }

    //    [Display(Name = "Number of children attending")]
    //    public int ChildCount { get; set; }
    //}

    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 5, ErrorMessage = "This is too short. Please enter your first and last name (to verify that you're on our list)")]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        //[Display(Name = "Code on invite")]
        //public string Code { get; set; }

        //[Display(Name = "Attending")]
        //public bool Attending { get; set; }

        //[Display(Name = "Plus one")]
        //public bool PlusOne { get; set; }

        //[Display(Name = "Number of children attending")]
        //public int ChildCount { get; set; }

        [Required]
        [Display(Name = "Zip/Postal Code")]
        public string ZipCode { get; set; }

        public int? InviteeId { get; set; }
    }

    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
