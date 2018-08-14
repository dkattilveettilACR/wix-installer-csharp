using Acr.Delivery.UpdateEngine.Flows;

namespace Acr.SiteServer.InstallUtil
{
	internal sealed class NoTransactionStore : IFlowTransactionStore
	{
		public FlowTransaction Read()
		{
			return null;
		}

		public void Write(FlowTransaction transaction)
		{}
	}
}
