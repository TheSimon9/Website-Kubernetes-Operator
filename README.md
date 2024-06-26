# Website operator

This repository contains an example schema of an operator written in .Net. You could use it in order to start develop your first Kubernetes operator!

The schema contains three elements:
- Controller (a.k.a. Reconciler)
- Entity
- Finalizer

The **controller** is going to manage the "reconcile" step. It will watch over the observed resources and takes care of being sure that the k8s cluster state and the desired state are the same.

The **entity** described the state that should be observed by the controller.

The **finalizer** has the responsibility of dispose unused resources.

## 💪🏻 Working software exercises
You should start from this schema and implement the following feature:

- feat n.1: Create a Pod with image nginx everytime a new custom resource of type website has been created
- feat n.2: When a custom resource has been deleted, remove the linked resources (Pod)
- feat n.3: When a new custom resource has been created or updated, the operator should deploy a deployment manifest and take the number of replicas from the new custom resource
- feat n.4: Show time: 🎉 Create a service for that deployment and retrieve the public ip address.
- feat n.5: When the pod starts, update the default nginx index.html and put inside a short description from the custom resource

## How to generate manifests

You could use the [KubeOps.Cli](https://buehler.github.io/dotnet-operator-sdk/src/KubeOps.Cli/README.html) tool. It allows you to generate the needed files for your operator as well as managing resources in your Kubernetes cluster.

    dotnet new tool-manifest
    dotnet tool install KubeOps.Cli
    dotnet kubeops gen op website --out ./manifests
    //update the image in kustomization.yaml with the operator docker image you'll use

## How to run in local env
If you want to test the operator locally you could easily run it in a local minikube instance.

Build operator image:

    docker buildx build . -f manifests/Dockerfile -t operator

set the imagePullPolicy: IfNotPresent property in the generated deployment

      containers:
      - env:
        - name: POD_NAMESPACE
          valueFrom:
            fieldRef:
              fieldPath: metadata.namespace
        image: operator
        name: operator
        imagePullPolicy: IfNotPresent


Load the image in your local minikube cluster

    minikube image load operator:latest

Apply it:

    kubectl apply -k manifests

## Custom resource example

```yaml
    apiVersion: intre.it/v1
    kind: website
    metadata:
      name: my-website
    namespace: default
    spec:
    name: "My Website"
```

## Notes & FAQ

### Minikube images aren't updated
If you are using minikube and you are not able to see the updated image, you could try to run the following command:

    minikube image rm operator:latest

And upload the new image again with

    minikube image load operator:latest

### Common words
(*) Reconciles: it stands for the act of the operator to edit
(reconcile) the status of the kubernetes cluster as we want (looking at the custom resource)

 