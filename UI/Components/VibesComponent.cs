using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;

namespace LiveSplit.UI.Components
{
    public class VibesComponent : IComponent
    {
        protected InfoTextComponent InternalComponent { get; set; }
        public VibesSettings Settings { get; set; }
        protected LiveSplitState CurrentState { get; set; }
        public string ComponentName => "Vibes";

        public float HorizontalWidth => InternalComponent.HorizontalWidth;
        public float MinimumWidth => InternalComponent.MinimumWidth;
        public float VerticalHeight => InternalComponent.VerticalHeight;
        public float MinimumHeight => InternalComponent.MinimumHeight;

        public float PaddingTop => InternalComponent.PaddingTop;
        public float PaddingLeft => InternalComponent.PaddingLeft;
        public float PaddingBottom => InternalComponent.PaddingBottom;
        public float PaddingRight => InternalComponent.PaddingRight;

        public IDictionary<string, Action> ContextMenuControls => null;

        protected bool isVibeValid { get; set; }

        public VibesComponent(LiveSplitState state)
        {
            Settings = new VibesSettings();
            InternalComponent = new InfoTextComponent("Vibes", "Immaculate");

            state.OnStart += state_OnStart;
            state.OnSplit += state_OnSplitChange;
            state.OnSkipSplit += state_OnSplitChange;
            state.OnUndoSplit += state_OnSplitChange;
            state.OnReset += state_OnReset;
            
            CurrentState = state;

            isVibeValid = false;
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {
            InternalComponent.NameLabel.HasShadow
                = InternalComponent.ValueLabel.HasShadow
                = state.LayoutSettings.DropShadows;

            InternalComponent.NameLabel.ForeColor = state.LayoutSettings.TextColor;
            InternalComponent.ValueLabel.ForeColor = state.LayoutSettings.TextColor;

            InternalComponent.DrawHorizontal(g, state, height, clipRegion);
        }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            InternalComponent.DisplayTwoRows = Settings.Display2Rows;

            InternalComponent.NameLabel.HasShadow
                = InternalComponent.ValueLabel.HasShadow
                = state.LayoutSettings.DropShadows;

            InternalComponent.NameLabel.ForeColor = state.LayoutSettings.TextColor;
            InternalComponent.ValueLabel.ForeColor = state.LayoutSettings.TextColor;

            InternalComponent.DrawVertical(g, state, width, clipRegion);
        }

        public Control GetSettingsControl(LayoutMode mode)
        {
            Settings.Mode = mode;
            return Settings;
        }

        public System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
        {
            return Settings.GetSettings(document);
        }

        public void SetSettings(System.Xml.XmlNode settings)
        {
            Settings.SetSettings(settings);
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            if (!isVibeValid)
            {
                isVibeValid = true;
                InternalComponent.InformationValue = SetVibe(state);
            }

            InternalComponent.Update(invalidator, state, width, height, mode);
        }
        
        void state_OnStart(object sender, EventArgs e)
        {
            isVibeValid = false;
        }

        void State_OnSkipSplit(object sender, EventArgs e)
        {
            isVibeValid = false;
        }

        void State_OnUndoSplit(object sender, EventArgs e)
        {
            isVibeValid = false;
        }

        void state_OnSplitChange(object sender, EventArgs e)
        {
            isVibeValid = false;
        }

        void state_OnReset(object sender, TimerPhase e)
        {
            isVibeValid = false;
        }

        string SetVibe(LiveSplitState state)
        {
            var comparison = Settings.Comparison;

            var DefaultVibe = Settings.TextDefault;

            double SplitDelta;

            // I only care if the run is going, I'm setting a default message to display
            if (state.CurrentPhase == TimerPhase.Running || state.CurrentPhase == TimerPhase.Paused)
            {
                TimeSpan? delta = LiveSplitStateHelper.GetLastDelta(state, state.CurrentSplitIndex, comparison, state.CurrentTimingMethod);
                SplitDelta = delta.Value.TotalSeconds;

                if (SplitDelta > 1 && SplitDelta < 30)
                {
                    // losing time
                    DefaultVibe = Settings.TextBehindLow;

                }

                else if (SplitDelta > 30)
                {
                    // losing a lot
                    DefaultVibe = Settings.TextBehindLarge;
                }

                else if (SplitDelta < 1 && SplitDelta > -30)
                {
                    // gaining a lil
                    DefaultVibe = Settings.TextAheadLow;
                }

                else if (SplitDelta < -30)
                {
                    // gaining a lot
                    DefaultVibe = Settings.TextAheadLarge;
                }
            }
            else if (state.CurrentPhase == TimerPhase.Ended)
            {
                // PB'd
                if (state.Run.Last().PersonalBestSplitTime[state.CurrentTimingMethod] == null ||
                    state.Run.Last().SplitTime[state.CurrentTimingMethod] < state.Run.Last().PersonalBestSplitTime[state.CurrentTimingMethod])
                {
                    //DefaultVibe = "MOM GET THE CAMERA!";
                    DefaultVibe = Settings.TextPB;
                }
                else {
                    // we tried
                    //DefaultVibe = "MISSION FAILED!";
                    DefaultVibe = Settings.TextNotPB;
                }
            }

            return DefaultVibe;
        }

        public void Dispose()
        {
            CurrentState.OnStart -= state_OnStart;
            CurrentState.OnSplit -= state_OnSplitChange;
            CurrentState.OnSkipSplit -= state_OnSplitChange;
            CurrentState.OnUndoSplit -= state_OnSplitChange;
            CurrentState.OnReset -= state_OnReset;
        }

        public int GetSettingsHashCode() => Settings.GetSettingsHashCode();
    }
}