using KubeOps.Abstractions.Controller;
using KubeOps.Abstractions.Finalizer;
using KubeOps.Abstractions.Rbac;
using KubeOps.KubernetesClient;

namespace WebsiteOperator;

// A controller is the element that reconciles a specific entity. 

[EntityRbac(typeof(WebsiteEntity), Verbs = RbacVerb.All)]
public class Controller : IEntityController<WebsiteEntity>
{
    private readonly IKubernetesClient _client;
    private readonly EntityFinalizerAttacher<Finalizer, WebsiteEntity> _finalizer;

    public Controller(
        IKubernetesClient client,
        EntityFinalizerAttacher<Finalizer, WebsiteEntity> finalizer)
    {
        _client = client;
        _finalizer = finalizer;
    }

    public async Task ReconcileAsync(WebsiteEntity entity, CancellationToken cancellationToken)
    {
        entity = await _finalizer(entity, cancellationToken);

        entity.Status.Status = "Reconciling";
        entity = await _client.UpdateStatusAsync(entity, cancellationToken);
        entity.Status.Status = "Reconciled";
        await _client.UpdateStatusAsync(entity, cancellationToken);
    }
    
    public Task DeletedAsync(WebsiteEntity entity, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}