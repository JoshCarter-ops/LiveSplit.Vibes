using LiveSplit.Model;
using System;

namespace LiveSplit.UI.Components
{
    public class VibesFactory : IComponentFactory
    {
        public string ComponentName => "Vibes";

        public string Description => "Displays the vibe of the run.";

        public ComponentCategory Category => ComponentCategory.Information;

        public IComponent Create(LiveSplitState state) => new VibesComponent(state);

        public string UpdateName => ComponentName;

        public string UpdateURL => "https://raw.githubusercontent.com/cartersoft/LiveSplit.Vibes/master/";

        public string XMLURL => UpdateURL + "Components/update.LiveSplit.Vibes.xml";

        public Version Version => Version.Parse("1.1.2");
    }
}