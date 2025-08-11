using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CustomSignalReceiver : MonoBehaviour, INotificationReceiver
{
    [Serializable]
    public class StringSignalEntry
    {
        public StringSignalEmitter signal;
        public UnityEvent<string> onSignalReceived;
    }

    [Serializable]
    public class SignalEntry
    {
        public SignalAsset signal;
        public UnityEvent onSignalReceived;
    }

    public StringSignalEntry[] stringSignals;
    public SignalEntry[] signals;

    public void OnNotify(Playable origin, INotification notification, object context)
    {
        if (notification is StringSignalEmitter stringEmitter)
        {
            foreach (StringSignalEntry signalEntry in stringSignals)
            {
                if (signalEntry.signal.asset == stringEmitter.asset)
                {
                    signalEntry.onSignalReceived?.Invoke(signalEntry.signal.parameter);
                    return;
                }
            }
        }

        if (notification is SignalEmitter emitter)
        {
            foreach (SignalEntry signalEntry in signals)
            {
                if (signalEntry.signal == emitter.asset)
                {
                    signalEntry.onSignalReceived?.Invoke();
                    return;
                }
            }
        }
    }
}
