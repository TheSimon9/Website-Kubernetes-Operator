using System.Threading;
using System.Threading.Tasks;
using KubeOps.Abstractions.Finalizer;

namespace WebsiteOperator;

//A finalizer is an element for asynchronous cleanup in Kubernetes.
 
public class Finalizer : IEntityFinalizer<WebsiteEntity>
{
    public Task FinalizeAsync(WebsiteEntity entity, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}