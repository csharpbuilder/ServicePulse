﻿namespace ServicePulse.Host.Commands
{
    using System;
    using System.Configuration.Install;
    using System.IO;
    using System.Reflection;
    using System.ServiceProcess;
    using Hosting;

    internal abstract class ServiceCommand : AbstractCommand
    {
        readonly Action<TransactedInstaller> action;

        protected ServiceCommand(Action<TransactedInstaller> action)
        {
            this.action = action;
        }

        public override void Execute(HostArguments args)
        {
            var serviceInstaller = new ServiceInstaller
            {
                ServiceName = args.ServiceName,
                Description = args.Description,
                DisplayName = args.DisplayName,
            };
            SetStartMode(serviceInstaller, args.StartMode);

            var serviceProcessInstaller = new ServiceProcessInstaller
            {
                Username = args.Username,
                Password = args.Password,
                Account = args.ServiceAccount,
            };
            var installers = new Installer[]
            {
                serviceInstaller,
                serviceProcessInstaller
            };

            var arguments = String.Empty;

            if (!String.IsNullOrEmpty(args.DisplayName))
            {
                arguments += string.Format(" -displayname \"{0}\"", args.DisplayName);
            }

            if (!String.IsNullOrEmpty(args.ServiceName))
            {
                arguments += string.Format(" -servicename \"{0}\"", args.ServiceName);
            }
            
            using (var e = new HostInstaller(args, arguments, installers))
            using (var transactedInstaller = new TransactedInstaller())
            {
                transactedInstaller.Installers.Add(e);

                var assembly = Assembly.GetEntryAssembly();

                var path = String.Format("/assemblypath={0}", assembly.Location);
                string[] commandLine = {path};

                var context = new InstallContext(null, commandLine);
                transactedInstaller.Context = context;

                Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

                action(transactedInstaller);
            }
        }

        static void SetStartMode(ServiceInstaller installer, StartMode startMode)
        {
            switch (startMode)
            {
                case StartMode.Automatic:
                    installer.StartType = ServiceStartMode.Automatic;
                    break;

                case StartMode.Manual:
                    installer.StartType = ServiceStartMode.Manual;
                    break;

                case StartMode.Disabled:
                    installer.StartType = ServiceStartMode.Disabled;
                    break;

                case StartMode.Delay:
                    installer.StartType = ServiceStartMode.Automatic;
                    installer.DelayedAutoStart = true;
                    break;
            }
        }
    }
}