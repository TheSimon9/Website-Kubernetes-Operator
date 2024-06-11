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
- feat n.4: When the pod starts, update the default nginx index.html and put inside a short description from the custom resource
- feat n.5: Show time: 🎉 Create a NodePort service for that deployment and retreive the public ip address.

## How to generate manifests

You could use the [KubeOps.Cli](https://buehler.github.io/dotnet-operator-sdk/src/KubeOps.Cli/README.html) tool. It allows you to generate the needed files for your operator as well as managing resources in your Kubernetes cluster.

    dotnet new tool-manifest
    dotnet tool install KubeOps.Cli
    dotnet kubeops gen op website --out ./manifests
    //update the image in kustomization.yaml with the operator docker image you'll use
    //set the imagePullPolicy: IfNotPresent property in the deployment

## How to run in local env
If you want to test the operator locally you could easily run it in a local minikube instance.

Build operator image:

    docker buildx build . -f manifests/Dockerfile -t operator

Load the image in your local minikube cluster

    minikube image load operator:latest

Apply it:

    kubectl apply -k manifests

## Notes

(*) Reconciles: it stands for the act of the operator to edit
(reconcile) the status of the kubernetes cluster as we want (looking at the custom resource)
# Website-Kubernetes-Operator
