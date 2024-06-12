using k8s.Models;
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
        
        var deployment = new V1Deployment
        {
            Metadata = new V1ObjectMeta
            {
                Name = entity.Metadata.Name,
                NamespaceProperty = entity.Metadata.NamespaceProperty
            },
            Spec = new V1DeploymentSpec()
            {
                Replicas = entity.Spec.Replicas,
                Selector = new V1LabelSelector()
                {
                    MatchLabels = new Dictionary<string, string>()
                    {
                        {"app", "website"}
                    }
                },
                Template = new V1PodTemplateSpec()
                {
                    Metadata = new V1ObjectMeta()
                    {
                        Labels = new Dictionary<string, string>()
                        {
                            {"app", "website"}
                        },
                    },
                    Spec = new V1PodSpec()
                    {
                        Containers = new List<V1Container>
                        {
                            new()
                            {
                                Name = "container",
                                Image = "nginx",
                                Env = new List<V1EnvVar>()
                                {
                                    new()
                                    {
                                        Name = "NAME",
                                        Value = entity.Spec.Name
                                    },
                                }
                            }
                        }
                    }
                }
            }
        };
        
        await _client.CreateAsync(deployment, cancellationToken);
        
        entity.Status.Status = "Reconciled";
        await _client.UpdateStatusAsync(entity, cancellationToken);
    }
    
    public Task DeletedAsync(WebsiteEntity entity, CancellationToken cancellationToken)
    {
        _client.Delete<V1Pod>($"{entity.Metadata.Name}", entity.Metadata.NamespaceProperty);
        return Task.CompletedTask;
    }
}