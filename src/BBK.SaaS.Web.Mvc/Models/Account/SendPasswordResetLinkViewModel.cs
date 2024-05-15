using System.ComponentModel.DataAnnotations;

namespace BBK.SaaS.Web.Models.Account
{
    public class SendPasswordResetLinkViewModel
    {
        [Required]
        public string EmailAddress { get; set; }
    }
}