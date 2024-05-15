using Abp.Application.Services.Dto;
using Abp.Webhooks;
using BBK.SaaS.WebHooks.Dto;

namespace BBK.SaaS.Web.Areas.App.Models.Webhooks
{
    public class CreateOrEditWebhookSubscriptionViewModel
    {
        public WebhookSubscription WebhookSubscription { get; set; }

        public ListResultDto<GetAllAvailableWebhooksOutput> AvailableWebhookEvents { get; set; }
    }
}
