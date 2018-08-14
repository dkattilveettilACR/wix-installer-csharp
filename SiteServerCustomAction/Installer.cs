
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using Acr.Delivery.Commons;
using Acr.Delivery.Commons.Logging;
using Acr.Delivery.Packaging;
using Acr.Delivery.UpdateEngine.Flows;

namespace Acr.SiteServer.InstallUtil
{
	public class Installer
	{
		public bool Install(
			string rootFolder, string packagePath, string version, string executableName, string componentDisplayName, string webApiUrl, string certificateSubject,
			ILogger logger = null)
		{
			var transactionStore = new NoTransactionStore();
			var selfControl = new NoSelfControl();

			var workingDirectory = Path.Combine(rootFolder, "temp");
			try
			{
				Directory2.CreateIfNotExists(workingDirectory);

				var environment = new InstallationEnvironment(
					workingDirectory,
					Path.Combine(rootFolder, "wwwroot"),
					new CertificateSubjectMatcher(certificateSubject));

				var componentPath = Path.Combine(rootFolder, $"Shell_{version}");
				var packageData = new Package { Id = 0, Path = packagePath };
				var componentData = new Component
				{
					Myself = false,
					InitialInstallation = true,
					CurrentVersion = null,
					ServiceName = null,
					NewServiceName = $"Acr_SiteServer_Shell_{version.Replace(".", "_")}",
					DistrPath = null,
					NewDistrPath = componentPath,
					Executable = null,
					NewExecutable = executableName,
					WwwPath = $"Shell_{version}",
					NewWwwPath = $"Shell_{version}",
					DisplayName = $"ACR Site Server: {componentDisplayName} (version {version})",
					Args = new Dictionary<string, string>
					{
						{ "webapi", webApiUrl },
					},
					AdditionalWebFiles = null,
				};

				return FlowBuilder.Install(componentData, packageData, environment, selfControl)
					.WithDefaultManifestHashAlgorithm("SHA1")
					.Build()
					.Execute(logger, transactionStore);
			}
			finally
			{
				Directory2.DeleteIfExists(workingDirectory);
			}
		}

		public bool Uninstall(string rootFolder, string currentVersion, string executableName, string webApiUrl,
			ILogger logger = null)
		{
			var transactionStore = new NoTransactionStore();

			var workingDirectory = Path.Combine(rootFolder, "temp");
			try
			{
				Directory2.CreateIfNotExists(workingDirectory);
				var environment = new InstallationEnvironment(
					Directory.GetCurrentDirectory(),
					Path.Combine(rootFolder, "wwwroot"),
					new CertificateSubjectMatcher(""));

				var componentPath = Path.Combine(rootFolder, $"Shell_{currentVersion}");
				var serviceName = $"Acr_SiteServer_Shell_{currentVersion.Replace(".", "_")}";

				var serviceController = GetService(serviceName);

				var componentData = new Component
				{
					CurrentVersion = null,
					ServiceName = serviceName,
					DistrPath = componentPath,
					Executable = executableName,
					WwwPath = $"Shell_{currentVersion}",
					DisplayName = serviceController != null ? serviceController.DisplayName : $"ACR Site Server: Shell (version {currentVersion})",
					Args = new Dictionary<string, string>
					{
						{ "webapi", webApiUrl },
					},
					AdditionalWebFiles = null,
				};

				var uninstalled = FlowBuilder.Uninstall(componentData, environment)
					.Build()
					.Execute(logger, transactionStore);
				if (uninstalled)
				{
					Directory2.DeleteIfExists(Path.Combine(rootFolder, "wwwroot"));
					Directory2.DeleteIfExists(Path.Combine(rootFolder, "Acr_SiteServer_ShellDb"));
					Directory2.DeleteIfExists(Path.Combine(rootFolder, "ShellDb"));
				}
				return uninstalled;
			}
			finally
			{
				Directory2.DeleteIfExists(workingDirectory);
			}
		}

		private static ServiceController GetService(string serviceName)
		{
			return ServiceController.GetServices().FirstOrDefault(x => string.Equals(x.ServiceName, serviceName));
		}
	}
}
