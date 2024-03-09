---
Title: Azure login in Github Actions via Service Principal
Link: Azure login in Github Actions via Service Principal
Summary: In this blog, I show you how we can login to Azure from GitHub actions and make a service principal using Azure CLI, then add its credentials to GitHub actions. Besides, look over a workflow file in a repository, plus scopes and roles of service principals.
Keywords: Azure, GitHub Actions, Service Principal, dependency injection
CreationDate: Friday, April 25, 2023
Author: Kaywan Salimi
ArticleId: 48er5dfb-1r48-48bd-b8c1-5a71b7q1w2t6
DisplayPriority: 8
---
<div align="center">

## Azure login in Github Actions via Service Principal

</div>

<div align="center">

  ![inversion-of-control](/data/Images/sp%26githubactions/azure-cloud-.png)
  
</div>


As it is obvious, deployment is one of the most crucial processes of the software development life cycle (SDLC). As a developer, we are not only responsible for developing software but also for delivering our product on different platforms, such as cloud platforms.

Thanks to IaC or Infrastructure as Code, we can manage, configure, and provision resources on cloud platforms through code rather than time-consuming and error-prone manual processes.
		
In this blog, I will show you how to make a connection between GitHub Actions and Azure Cloud with which we can deploy resources on Azure. Although, the latter part is not covered this time because it makes the blog very long and less efficient.

>The GitHub repository for this blog is available [here](https://github.com/kevinsalimi/azure-login-via-serviceprincipal).

>This post is already published on the [Techspire Co](https://techspire.nl/cloud/cloud-platforms/azure/azure-login-in-github-actions-via-service-principal/) blog.

## Table of contents:

* [Service Principal](#service-principal)
    * [Create a Service Principal](#create-a-service-principal)
    * [Role and Scope](#role-and-scope)
    * [Client secret](#client-secret)
* [Add credentials to GitHub](#add-credentials-to-github)
    * [Github Actions and Workflow](#github-actions-and-workflow)
* [Conclusion](#conclusion)

## Service Principal
Simply put, the Service Principal is nothing more than a security identifier that is used by applications, services, or other clients that want to access an existing resource or create one on Azure.

The whole story is like this; an Azure administrator would create a Service Principal and assign it a secret key(application key) and a set of permissions(role assignment), then clients can use these credentials to access resources on Azure.

### Create a Service Principal
Creating a Service Principal is pretty simple. You can choose either Azure CLI or Azure Portal. I go with Azure CLI because it is way easier than Azure Portal.

But first, we need to create a resource group:

1. Navigate to the [Azure Portal](https://portal.azure.com/) home page, click on the hamburger menu, and select **Resource groups**. If it does not exist there, you can search for it in the search bar.
2. From the top of the resource group table, click **+Create**. The **Create a resource group** appears.
3. Type a resource group name, for example: **sampleResourceGroup**.
4. Click **Review + create**.
5. Click **Create**.


It is time to create a service principal, but before using Azure CLI commands, sign in with [`az login`](https://learn.microsoft.com/en-us/cli/azure/reference-index?view=azure-cli-latest#az-login).
Then, you can easily create a service principal and configure its access to Azure resources using the below command.

> To install Azure CLI on your local machine, go [here](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli).

```
# Create a service principal for a resource group using a preferred name and role
az ad sp create-for-rbac --name "sampleServicePrincipal" \
--role contributor \
--scopes /{subscription-id}/subscriptionID/resourceGroups/sampleResourceGroup \
--sdk-auth
```
The preceding command has three parameters:

* `--name`: It is the name of the service principal. If you do not specify this, the default value would be a name containing a time stamp. (e.g. `azure-cli-%Y-%m-%d-%H-%M-%S`)
* `--role `: It is the role of the service principal, such as the `reader` or `contributor`, etc. We discuss the role down here.
* `--scopes`: It narrows the scope to a resource or resource group. We discuss the scope down here.
* `{subscription-id}`: Do not forget to replace your actual Azure subscription id.

The `az ad sp create-for-rbac` output is a JSON that includes credentials the client needs to get a token from Azure so that you must protect. It would be like below:

```JSON
{
"clientId": "<GUID>",
"clientSecret": "<GUID>",
"subscriptionId": "<GUID>",
"tenantId": "<GUID>",
(...)
}
```

#### Role and Scope
In our case, I assigned the Contributor role to the service principal at a resource group level(scope). Be aware that you should assign proper roles at proper scopes based on your needs, and avoid broader roles at broader scopes.

When you assign a role at a scope, the client with that role can access all resources that exist under a specified scope. In our example, the client with a Contributor role can access resources in the sampleResourceGroup resource group.

> To learn more about assigning an Azure role, you can read [this](https://learn.microsoft.com/en-us/azure/role-based-access-control/role-assignments-steps) blog.

#### Client secret
If you look at the `az ad sp create-for-rbac` output, there is a client secret already generated for you. The client secret is a key that clients use to prove their identity when requesting a token. In our case, Github sends the secret to Azure to get a token with which, Github can deploy resources on Azure.

Now, you can navigate to the Azure portal, and find your service principal. From the left panel, click **Cerificates & secrets**, then the **Client secrets** tab; you will be able to see a record in the client secrets table like below:
![secret-service-principal](/data/Images/sp%26githubactions/secret-service-principal.png)


Awesome! We are done with the Azure part; let's do the Github stuff.

## Add credentials to GitHub
We want to add the azure credentials to our GitHub project to be able to login to Azure. I have already created a new project on GitHub for the demo purpose. The project is pretty fresh, so let's add what we needed to it.

First, I am going to add Azure credentials to my repository secrets. Secrets are encrypted and are used for sensitive data. Be aware that those people who have collaborator access to your repository can use secrets for actions.

1. Navigate to your project on GitHub.
2. Click **Settings** from the left panel.
3. Expand the **Secrets and variables**.
4. Click **Actions**.
5. Click **New repository secret**.
![github-add-action](/data/Images/sp%26githubactions/github-add-action.png)

6. Give a meaningful name to your secret. We will use this name in the workflow. I used `AZURE_CREDENTIALS`. Then copy the output of the `az ad sp create-for-rbac` command and paste it in the **Secret** input box.
7. Click **Add secret**.
![github-add-secret](/data/Images/sp%26githubactions/github-add-secret.png)

**Bear in mind that after adding the secret you are no longer able to reveal the secret value although the only possible action is editing.**

Now, it is time to add an action to your repository but before that let's review GitHub actions and workflows.

### Github Actions and Workflow
GitHub Actions is a platform with which you can automatically build, test, and deploy your projects on GitHub.

Also, it's a convenient way to create workflows in your repository. Workflow is nothing more than a bunch of jobs and steps that are supposed to run when a specific activity on GitHub happens or is triggered manually.

To add an action to your project:

1. Click **Actions** tab and under the **Suggested for this repository**, find the **Simple workflow** box; click **Configure** to add a simple workflow to your project.
![github-add-default-action](/data/Images/sp%26githubactions/github-add-default-action.png)
2. Give a name to the workflow file name. I used `azurelogindeploy`.
![github-add-workflow](/data/Images/sp%26githubactions/github-add-workflow.png)
3. Replace the content of the workflow file with the below one then click **Start commit**. Add a comment for the commit and click **Commit new file**.

```
name: "Azure Login Workflow"

on:
push:
branches:
- main

jobs:
azureLogin:
runs-on: ubuntu-latest

steps:
- name: Azure Login
uses: Azure/login@v1
with:
creds: ${{secrets.AZURE_CREDENTIALS}}
```


* `name`: specified the name of the workflow.
* `on`: specified events that cause the workflow to run. In our case, when a push action happens on the main branch, the workflow will be triggered. As a result, as soon as we commit the new workflow file in step 3, the action starts running.
* `jobs`: a workflow comprises one or more `jobs`. You can use `runs-on` to define the type of machine to run the job on.
* `steps`: A job contains a sequence of tasks called `steps`. Inside a step, you can select an action to run as a part of a step in your job via `uses`. Some actions require inputs that you must set using the `with`.

In order to login to Azure, I used [`Azure/Login@v1`](https://github.com/marketplace/actions/azure-login) action and provided the `creds` input with `secrets.AZURE_CREDENTIALS` which we already defined in the secrets of the repository.

>To learn more about workflow syntax, you can read [here](https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions).

Let's check out the workflow run by navigating to the **Actions** tab in your repository. As you can see in the below photo, the workflow runs successfully and is able to login to Azure. You can also visit the workflow runs [here](https://github.com/kevinsalimi/azure-login-via-serviceprincipal/actions/workflows/azurelogindeploy.yml).
![github-workflow-run](/data/Images/sp%26githubactions/github-workflow-run.png)
Click on the run action then go through the job detail by clicking on `azureLogin` job. Expand the **Azure Login** step to see its detail.
![github-workflow-run-details1](/data/Images/sp%26githubactions/github-workflow-run-details1.png)
![github-workflow-run-details2](/data/Images/sp%26githubactions/github-workflow-run-details2.png)


### Conclusion
In this blog, I showed you how we can login to Azure from GitHub actions. We made a service principal using Azure CLI, then added its credentials to GitHub actions. Besides, looked over a workflow file in a repository, plus scopes and roles of service principals.