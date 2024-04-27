using Quartz;

namespace AnyRivals.Application.Common.Helpers;
public static class TriggerKeyHelper
{
    public static TriggerKey CreateRevealAnswerKey(int gameId)
    {
        return new TriggerKey($"{gameId}-areveal");
    }

    public static TriggerKey CreateDistributeAnswerKey(int gameId)
    {
        return new TriggerKey($"{gameId}-qdist");
    }
}
