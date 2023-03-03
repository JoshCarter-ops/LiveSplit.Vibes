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
        public string ComponentName => "Vibes";

        protected InfoTextComponent InternalComponent { get; set; }
        protected LiveSplitState CurrentState { get; set; }
        protected bool isVibeValid { get; set; }

        public VibesSettings Settings { get; set; }

        public float HorizontalWidth => InternalComponent.HorizontalWidth;
        public float MinimumWidth => InternalComponent.MinimumWidth;
        public float VerticalHeight => InternalComponent.VerticalHeight;
        public float MinimumHeight => InternalComponent.MinimumHeight;

        public float PaddingTop => InternalComponent.PaddingTop;
        public float PaddingLeft => InternalComponent.PaddingLeft;
        public float PaddingBottom => InternalComponent.PaddingBottom;
        public float PaddingRight => InternalComponent.PaddingRight;

        public IDictionary<string, Action> ContextMenuControls => null;

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

            DrawBackground(g, state, HorizontalWidth, height);
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

            DrawBackground(g, state, width, VerticalHeight);
            InternalComponent.DrawVertical(g, state, width, clipRegion);
        }

        private void DrawBackground(Graphics g, LiveSplitState state, float width, float height)
        {
            if (Settings.BackgroundColor.A > 0
                || Settings.BackgroundGradient != GradientType.Plain
                && Settings.BackgroundColor2.A > 0)
            {
                var gradientBrush = new LinearGradientBrush(
                            new PointF(0, 0),
                            Settings.BackgroundGradient == GradientType.Horizontal
                            ? new PointF(width, 0)
                            : new PointF(0, height),
                            Settings.BackgroundColor,
                            Settings.BackgroundGradient == GradientType.Plain
                            ? Settings.BackgroundColor
                            : Settings.BackgroundColor2);
                g.FillRectangle(gradientBrush, 0, 0, width, height);
            }
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
                var liveDelta = state.CurrentTime[state.CurrentTimingMethod] - state.CurrentSplit.Comparisons[comparison][state.CurrentTimingMethod];

                if (liveDelta > delta || (delta == null && liveDelta > TimeSpan.Zero))
                {
                    delta = liveDelta;
                }

                SplitDelta = delta.Value.TotalSeconds;

                if (SplitDelta > (int) Settings.NumBehindLow && SplitDelta < (int)Settings.NumBehindHigh)
                {
                    DefaultVibe = Settings.TextBehindLow;
                }
                else if (SplitDelta > (int)Settings.NumBehindHigh)
                {
                    DefaultVibe = Settings.TextBehindLarge;
                }

                if (SplitDelta < (int)Settings.NumAheadLow && SplitDelta > -(int)Settings.NumAheadHigh)
                {
                    DefaultVibe = Settings.TextAheadLow;
                }
                else if (SplitDelta < -(int)Settings.NumAheadHigh)
                {
                    DefaultVibe = Settings.TextAheadLarge;
                }
            }
            else if (state.CurrentPhase == TimerPhase.Ended)
            {
                if (state.Run.Last().PersonalBestSplitTime[state.CurrentTimingMethod] == null ||
                    state.Run.Last().SplitTime[state.CurrentTimingMethod] < state.Run.Last().PersonalBestSplitTime[state.CurrentTimingMethod])
                {
                    DefaultVibe = Settings.TextPB;
                }
                else {
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