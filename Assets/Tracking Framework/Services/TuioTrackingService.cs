﻿using Assets.Tracking_Framework.Interfaces;
using Assets.Tracking_Framework.TransmissionFramework;
using Assets.Tracking_Framework.TransmissionFramework.TuioTransmission.TUIO;
using Assets.Tracking_Framework.TransmissionFramework.UnityPharusFramework;
using Assets.Tracking_Framework.TransmissionFramework.UnityTuioFramwork;
using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.Tracking_Framework.Services
{
    /// <summary>
    /// The TUIO Tracking Service keeps control over the UnityTuioListener and the UnityTuioEventProcessor.
    /// </summary>
    public class TuioTrackingService : ITrackingService
    {
        public static event EventHandler<EventArgs> OnTrackingInitialized;
        private TrackingSettings settings;
        private UnityTuioListener listener;
        private UnityTuioEventProcessor eventProcessor;

        public int TrackingInterpolationX
        {
            get { return settings.TrackingResolutionX; }
        }
        public int TrackingInterpolationY
        {
            get { return settings.TrackingResolutionY; }
        }

        public float TrackingStageX
        {
            get { return settings.StageSizeX; }
        }
        public float TrackingStageY
        {
            get { return settings.StageSizeY; }
        }

        /// <summary>
        /// Used to calculate player position based on the tracking resolution.
        /// </summary>
        /// <param name="x">The x coordinate</param>
        /// <param name="y">The y coordinate</param>
        /// <returns></returns>
        public Vector2 GetScreenPositionFromRelativePosition(float x, float y)
        {
            return new Vector2((int)Mathf.Round(x * settings.TrackingResolutionX), settings.TrackingResolutionY - (int)Mathf.Round(y * settings.TrackingResolutionY));
        }

        /// <summary>
        /// Initializes the tracking service by initializing the TUIO listener and event processor.
        /// </summary>
        /// <param name="settings">The settings xml file, which can be edited externally in the Streaming Assets.</param>
        public void Initialize(TrackingSettings settings)
        {
            this.settings = settings;

            listener = new UnityTuioListener(this.settings.TuioUdpPort);
            eventProcessor = new UnityTuioEventProcessor(listener);

            if (OnTrackingInitialized != null)
            {
                OnTrackingInitialized(this, new EventArgs());
            }

            TrackingAdapter.InjectTrackingManager(this);
        }

        /// <summary>
        /// Updates the event processor.
        /// </summary>
        public void Update()
        {
            //Listen for tuio data if enabled
            if (eventProcessor != null)
            {
                eventProcessor.Process();
            }
        }

        /// <summary>
        /// Reconnects the tracking service with an optional delay.
        /// </summary>
        /// <param name="theDelay">The delay in milliseconds.</param>
        public void Reconnect(int theDelay = -1)
        {
            if (listener == null || listener.HasTuioContainers())
                return;

            if (theDelay <= 0)
            {
                listener.Reconnect();
            }
            else
            {
                this.ReconnectTuioListenerDelayed(theDelay);
            }
        }

        /// <summary>
        /// Shuts down the TUIO tracking service
        /// </summary>
        public void Shutdown()
        {
            if (listener != null)
            {
                listener.Shutdown();
            }
        }

        private async void ReconnectTuioListenerDelayed(int theDelay)
        {
            listener.Shutdown();
            await Task.Delay(theDelay);
            listener.Reconnect();
        }
    }
}
