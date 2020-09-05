using Enterwell.Clients.Wpf.Notifications;

namespace Nebula.Core.Notifications
{
    public class NebulaNotifications
    {
        public NebulaNotifications()
        {
        }

        public NotificationMessageManager NotificationsManager { get; } = new NotificationMessageManager();

        public void NotifyOk(string badgeKey, string contentKey, string accentColor = "#1751C3", params object[] args)
        {
            NotificationsManager.CreateMessage()
                                .Animates(true)
                                .AnimationInDuration(0.75)
                                .AnimationOutDuration(0.25)
                                .Accent(accentColor)
                                .Background("#333")
                                .HasBadge(string.IsNullOrWhiteSpace(badgeKey) ? NebulaClient.GetLocString("NotificationsBadgeInfo") : badgeKey)
                                .HasMessage(NebulaClient.GetLocString(contentKey, args))
                                .Dismiss().WithButton(NebulaClient.GetLocString("ButtonOk"), button => { })
                                .Queue();
        }
    }
}