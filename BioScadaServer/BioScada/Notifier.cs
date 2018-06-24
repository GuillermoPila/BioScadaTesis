using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace BioScadaServer.BioScada
{
    [Serializable]
    public class Notifier
    {
//        public IChangeNotifier NotificationReceiver;

        public void NotifyChange(ChangeNotification notification, INotifierReceiverChange NotificationReceiver)
        {
            lock (lckPendantNotifications)

                pendantNotifications.Enqueue(new ParChangeNotification_Receiver()
                    {ChangeNotificationReceiver = NotificationReceiver,
                     ChangeNotification = notification
                    });
            waitPendantNotifications.Set();
        }
        [NonSerialized]
        private Thread notifierThread;
        private static Notifier notif = null;
        private Notifier()
        {
            notifierThread = new Thread(ThreadBody);
        }

        public static Notifier GetInstance()
        {
            if (notif == null)
               notif = new Notifier();
            return notif;
        }
        public void Start()
        {
            notifierThread.Start(this);
        }

        public void Stop()
        {
            lock (lckTerminate)
                Terminate = true;
            waitPendantNotifications.Set();
            notifierThread.Join(TimeSpan.FromSeconds(3));

            notif.waitPendantNotifications.Close();
            notifierThread.Abort();
        }

        private Queue<ParChangeNotification_Receiver> pendantNotifications = new Queue<ParChangeNotification_Receiver>();
        [NonSerialized]
        private ManualResetEvent waitPendantNotifications = new ManualResetEvent(false);
        private object lckPendantNotifications = new object();
        private bool Terminate = false;
        private object lckTerminate = new object();

        private static void ThreadBody (object Data)
        {
            Notifier notifier = (Notifier)Data;
            while (true)
            {
                notifier.waitPendantNotifications.WaitOne();
                lock (notifier.lckTerminate)
                    if (notifier.Terminate)
                        break;
                while (true)
                {
                    ParChangeNotification_Receiver notification;
                    lock (notifier.lckPendantNotifications)
                        notification = notifier.pendantNotifications.Dequeue();
                    SendNotification(notification.ChangeNotificationReceiver, notification.ChangeNotification);
                    lock (notifier.lckPendantNotifications)
                        if (notifier.pendantNotifications.Count == 0)
                        {
                            notifier.waitPendantNotifications.Reset();
                            break;
                        }
                }
            }
        }

        private static void SendNotification(INotifierReceiverChange receiver, ChangeNotification notification)
        {
            try
            {
                receiver.ReceiverChange(notification);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }

        

        public void Dispose()
        {
            Stop();
        }
    }
    [Serializable]
    public class ParChangeNotification_Receiver
    {
        public INotifierReceiverChange ChangeNotificationReceiver;
        public ChangeNotification ChangeNotification;

    }
}
