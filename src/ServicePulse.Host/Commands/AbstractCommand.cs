﻿namespace ServicePulse.Host.Commands
{
    using Hosting;

    internal abstract class AbstractCommand
    {
        public abstract void Execute(HostArguments args);
    }
}