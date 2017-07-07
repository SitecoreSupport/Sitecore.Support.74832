namespace Sitecore.Support.Modules.EmailCampaign.Core.Pipelines.RedirectUrl
{
  using Sitecore.Analytics;
  using Sitecore.Analytics.Model;
  using Sitecore.Diagnostics;
  using Sitecore.Modules.EmailCampaign.Core.Pipelines.RedirectUrl;
  using Sitecore.Modules.EmailCampaign.Recipients;
  using Sitecore.Modules.EmailCampaign.Xdb;
  using System;

  public class IdentifyContact
  {
    public void Process(RedirectUrlPipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      if (!Tracker.IsActive)
      {
        try
        {
          Tracker.StartTracking();
        }
        catch (Exception)
        {
          return;
        }
      }
      if ((Tracker.Current != null) && (Tracker.Current.Session != null))
      {
        try
        {
          if (args.RecipientId != null)
          {
            Type[] requestedPropertyTypes = new Type[] { typeof(XdbRelation) };
            Recipient recipientSpecific = RecipientRepository.GetDefaultInstance().GetRecipientSpecific(args.RecipientId, requestedPropertyTypes);
            if (recipientSpecific != null)
            {
              XdbRelation defaultProperty = recipientSpecific.GetProperties<XdbRelation>().DefaultProperty;
              if (((defaultProperty != null) && !string.IsNullOrEmpty(defaultProperty.Identifier)) && ((Tracker.Current.Contact == null) || (Tracker.Current.Contact.Identifiers.Identifier != defaultProperty.Identifier)))
              {
                Tracker.Current.Session.Identify(defaultProperty.Identifier);
              }
            }
          }
          if (Tracker.Current.Contact != null)
          {
            Tracker.Current.Contact.Identifiers.IdentificationLevel = ContactIdentificationLevel.Known;
            Tracker.Current.Contact.ContactSaveMode = ContactSaveMode.AlwaysSave;
          }
        }
        catch (Exception)
        {
        }
      }
    }
  }
}