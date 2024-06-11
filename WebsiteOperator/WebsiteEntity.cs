using k8s.Models;
using KubeOps.Abstractions.Entities;

namespace WebsiteOperator;

/*
    Class that describers the entity.
    This includes its own properties, version and group.
    The Status property is mandatory and allows you to manipulate and get the status of the entity once deployed.
*/
[KubernetesEntity(Group = "intre.it", ApiVersion = "v1", Kind = "website")]
public class WebsiteEntity :
    CustomKubernetesEntity<WebsiteEntity.EntitySpec, WebsiteEntity.EntityStatus>
{
    public override string ToString()
        => $"Entity ({Metadata.Name}): {Spec.Name}";

    // Properties of the custom resource
    public class EntitySpec
    {
        public string Name { get; set; }
    }

    public class EntityStatus
    {
        public string Status { get; set; } = string.Empty;
    }
}