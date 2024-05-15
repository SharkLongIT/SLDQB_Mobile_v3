using System.Collections.Generic;
using Abp.Notifications;

namespace BBK.SaaS.Web.Areas.App.Models.Notifications
{
    public class CreateMassNotificationViewModel
    {
        public List<string> TargetNotifiers { get; set; }
    
        public NotificationSeverity Severity { get; set; }
    }
}