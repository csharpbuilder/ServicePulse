﻿namespace ServicePulse.Host.Commands
{
    using System.IO;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Hosting;

    internal class ExtractAndUpdateConstantsCommand : AbstractCommand
    {
        public override void Execute(HostArguments args)
        {
#if !DEBUG
            ExtractApp(args.OutputPath);
            UpdateVersion(args.OutputPath);
            UpdateConfig(args.OutputPath, args.ServiceControlUrl);
#endif
        }

        static void ExtractApp(string directoryPath)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (var resourceStream = assembly.GetManifestResourceStream(@"app\js\app.constants.js"))
            {
                var destinationPath = Path.Combine(directoryPath, "js\\app.constants.js");

                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

                if (File.Exists(destinationPath))
                {
                    File.Delete(destinationPath);
                }

                using (Stream file = File.OpenWrite(destinationPath))
                {
                    resourceStream.CopyTo(file);
                    file.Flush();
                }
            }
        }


        public static void UpdateConfig(string directoryPath, string serviceControlUrl)
        {
            var appJsPath = Path.Combine(directoryPath, "js/app.constants.js");
            var appJsCode = File.ReadAllText(appJsPath);

            File.WriteAllText(appJsPath,
                Regex.Replace(appJsCode, @"(service_control_url: ')([\w:/]*)(')", "$1" + serviceControlUrl + "$3"));
        }

        public static void UpdateVersion(string directoryPath)
        {
            var appJsPath = Path.Combine(directoryPath, "js/app.constants.js");
            var appJsCode = File.ReadAllText(appJsPath);

            var updatedContent = Regex.Replace(appJsCode, @"(constant\('version', ')([\w:/.-]*)(')", "${1}" + GetFileVersion() + "$3");

            File.WriteAllText(appJsPath, updatedContent);
        }

        static string GetFileVersion()
        {
            var customAttributes =
                typeof(AbstractCommand).Assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute),
                    false);

            if (customAttributes.Length >= 1)
            {
                var fileVersionAttribute = (AssemblyInformationalVersionAttribute)customAttributes[0];
                var informationalVersion = fileVersionAttribute.InformationalVersion;
                return informationalVersion.Split('+')[0];
            }

            return typeof(AbstractCommand).Assembly.GetName().Version.ToString(4);
        }
    }
}