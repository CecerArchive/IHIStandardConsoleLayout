using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cecer.ConsoleSplit;
using IHI.Server.Console;
using IHI.Server.Events;

namespace IHI.Server.Plugins.IHIStandardConsoleLayout
{
    internal class Layout
    {
        #region Console Layout
        public ConsoleLayout ConsoleLayout
        {
            get;
            private set;
        }

        private OutLogicalConsole _standardOutputConsole;
        private InLogicalConsole _inputConsole;
        private OutLogicalConsole _inputReplyConsole; // TODO: Make a system for this
        private OutLogicalConsole _statsOutputConsole; // TODO: Make stats a thing

        private IHIStandardConsoleLayout _pluginInstance;

        internal Layout(IHIStandardConsoleLayout pluginInstance)
        {
            _pluginInstance = pluginInstance;
            ConsoleLayout = new ConsoleLayout();

            _standardOutputConsole = new OutLogicalConsole(65, 37, 53, 1);
            _inputConsole = new InLogicalConsole(50, 3, 2, 35);
            _inputReplyConsole = new OutLogicalConsole(50, 26, 2, 8);
            _statsOutputConsole = new OutLogicalConsole(21, 5, 31, 2);

            ConsoleLayout
                .AddLogicalConsole(_standardOutputConsole)
                .AddLogicalConsole(_inputConsole)
                .AddLogicalConsole(_inputReplyConsole)
                .AddLogicalConsole(_statsOutputConsole)
                .BackgroundDrawer = DrawBackground;

            CoreManager.ServerCore.ConsoleManager.ConsoleLayout = ConsoleLayout;

            CoreManager.ServerCore.EventManager.StrongBind <ConsoleOutputEventArgs>("stdout:after", PrintStandardOutEvents);

            new Thread(ReadInput)
            {
                Name = "IHIPLUGIN:IHIStandardConsoleLayout-InputReaderThread",
                IsBackground = true
            }.Start();
        }

        private void PrintStandardOutEvents(ConsoleOutputEventArgs eventArgs)
        {
            lock (_standardOutputConsole)
            {
                switch (eventArgs.Level)
                {
                    case ConsoleOutputLevel.Debug:
                    {
                        _standardOutputConsole.ForegroundColor = ConsoleColor.DarkGray;
                        break;
                    }
                    case ConsoleOutputLevel.Notice:
                    {
                        _standardOutputConsole.ForegroundColor = ConsoleColor.Gray;
                        break;
                    }
                    case ConsoleOutputLevel.Important:
                    {
                        _standardOutputConsole.ForegroundColor = ConsoleColor.Green;
                        break;
                    }
                    case ConsoleOutputLevel.Warning:
                    {
                        _standardOutputConsole.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    }
                    case ConsoleOutputLevel.Error:
                    {
                        _standardOutputConsole.ForegroundColor = ConsoleColor.Red;
                        break;
                    }
                }
                _standardOutputConsole.WriteLine("[" + eventArgs.Channel + "]" + eventArgs.Message);
            }
        }

        private void DrawBackground()
        {
            System.Console.Write(   @"                                                    /-----------------------------------------------------------------\ " +
                                    @"    ######  ##  ##  ######    /---------------------<                                                                 | " +
                                    @"      ##    ##  ##    ##      |                     |                                                                 | " +
                                    @"      ##    ######    ##      |                     |                                                                 | " +
                                    @"      ##    ##  ##    ##      |                     |                                                                 | " +
                                    @"    ######  ##  ##  ######    |                     |                                                                 | " +
                                    @"                              |                     |                                                                 | " +
                                    @" /----------------------------^---------------------<                                                                 | " +
                                    @" |                                                  |                                                                 | " +
                                    @" |                                                  |                                                                 | " +
                                    @" |                                                  |                                                                 | " +
                                    @" |                                                  |                                                                 | " +
                                    @" |                                                  |                                                                 | " +
                                    @" |                                                  |                                                                 | " +
                                    @" |                                                  |                                                                 | " +
                                    @" |                                                  |                                                                 | " +
                                    @" |                                                  |                                                                 | " +
                                    @" |                                                  |                                                                 | " +
                                    @" |                                                  |                                                                 | " +
                                    @" |                                                  |                                                                 | " +
                                    @" |                                                  |                                                                 | " +
                                    @" |                                                  |                                                                 | " +
                                    @" |                                                  |                                                                 | " +
                                    @" |                                                  |                                                                 | " +
                                    @" |                                                  |                                                                 | " +
                                    @" |                                                  |                                                                 | " +
                                    @" |                                                  |                                                                 | " +
                                    @" |                                                  |                                                                 | " +
                                    @" |                                                  |                                                                 | " +
                                    @" |                                                  |                                                                 | " +
                                    @" |                                                  |                                                                 | " +
                                    @" |                                                  |                                                                 | " +
                                    @" |                                                  |                                                                 | " +
                                    @" |                                                  |                                                                 | " +
                                    @" >--------------------------------------------------<                                                                 | " +
                                    @" |                                                  |                                                                 | " +
                                    @" |                                                  |                                                                 | " +
                                    @" |                                                  |                                                                 | " +
                                    @" \--------------------------------------------------^-----------------------------------------------------------------/ " +
                                    @"                                                                                                                       ");
        }

        private void ReadInput()
        {
            while (true)
            {
                string inputLine = _inputConsole.ReadLine();

                ConsoleInputEventArgs eventArgs = new ConsoleInputEventArgs(inputLine);

                _pluginInstance.EventFirer.Fire("stdin:before", eventArgs);
                if (!eventArgs.IsCancelled)
                    _pluginInstance.EventFirer.Fire("stdin:after", eventArgs);
            }
        }

        #endregion
    }
}
