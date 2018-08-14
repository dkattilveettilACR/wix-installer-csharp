using Acr.Delivery.UpdateEngine.Flows;

namespace Acr.SiteServer.InstallUtil
{
	internal sealed class NoSelfControl : ISelfControl
	{
		public void StopActivities()
		{}

		public void StartActivities()
		{}
	}
}
