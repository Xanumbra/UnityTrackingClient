﻿using Assets.Tracking_Framework.TransmissionFramework.TracklinkTransmission;
using System;

namespace Assets.Tracking_Framework.TransmissionFramework.UnityPharusFramework
{
    /// <summary>
    /// The UnityPharusEventProcessor picks up all the PharusEvents from the queue provided by the UnityPharusListener and informs its subscribers.
    /// </summary>
    public class UnityPharusEventProcessor
    {
        #region event args
        public class PharusEventTrackArgs : EventArgs
        {
            public readonly TrackRecord trackRecord;
            public PharusEventTrackArgs(TrackRecord thetrackRecord)
            {
                trackRecord = thetrackRecord;
            }
        }
        #endregion

        #region event handlers
        public static event EventHandler<PharusEventTrackArgs> TrackAdded;
        public static event EventHandler<PharusEventTrackArgs> TrackUpdated;
        public static event EventHandler<PharusEventTrackArgs> TrackRemoved;
        #endregion

        private UnityPharusListener m_listener;

        #region constructor
        public UnityPharusEventProcessor(UnityPharusListener theUnityPharusListener)
        {
            m_listener = theUnityPharusListener;
        }
        #endregion

        #region finalizer
        ~UnityPharusEventProcessor()
        {
            ClearAllSubscribers();
        }
        #endregion

        #region public methods
        public void Process()
        {
            while (m_listener.EventQueue.Count > 0)
            {
                UnityPharusListener.PharusEvent aEvent;
                lock (m_listener.LockObj)
                {
                    aEvent = m_listener.EventQueue.Dequeue();
                }
                switch (aEvent.PharusEventType)
                {
                    case ETrackState.TS_NEW:
                        if (TrackAdded != null) TrackAdded(this, new PharusEventTrackArgs(aEvent.TrackRecord));
                        break;
                    case ETrackState.TS_CONT:
                        if (TrackUpdated != null) TrackUpdated(this, new PharusEventTrackArgs(aEvent.TrackRecord));
                        break;
                    case ETrackState.TS_OFF:
                        if (TrackRemoved != null) TrackRemoved(this, new PharusEventTrackArgs(aEvent.TrackRecord));
                        break;
                }
            }
        }

        public void ClearAllSubscribers()
        {
            TrackAdded = null;
            TrackUpdated = null;
            TrackRemoved = null;
        }
        #endregion

    }
}
