using IHI.Server.Console;

namespace IHI.Server.Plugins.IHIStandardConsoleLayout
{
    public class IHIStandardConsoleLayout : Plugin
    {
        public override string Id
        {
            get { return "cecer:ihistandardconsolelayout"; }
        }

        public override string Name
        {
            get
            {
                return "IHIStandardConsoleLayout";
            }
        }

        internal EventFirer EventFirer
        {
            get;
            private set;
        }

        public ConsoleLayout ConsoleLayout
        {
            get;
            private set;
        }

        /// <summary>
        ///   Called when the plugin is started.
        /// </summary>
        public override void Start(EventFirer eventFirer)
        {
            EventFirer = eventFirer;
            ConsoleLayout = new Layout(this).ConsoleLayout;

        }
    }
}