#### Project Description
The purpose of this project is to distribute the PFE Dynamics Core Library for Dynamics CRM source code to the customer/partner community. We developed this library in response to requests from customers and partners for whom we've delivered sample/prototype solutions built on Dynamics CRM that serve an educational purpose.  In delivering those solutions, we repeatedly encountered the need to implement known best-practices when authenticating and interacting with CRM services regardless of the solution context.  Since consolidating those techniques into a reusable library, we've seen customer and partners' solutions that reference our library simultaneously increase performance and reduce code. 

###### [Release History](Release-History)
###### [Known Issues](Known-Issues)

#### Library Contents

We are providing four separate branches of the library, one that targets Dynamics CRM 2011 (V5), another that targets Dynamics CRM 2013 (V6), a third that targets Dynamics CRM 2015 (V7), and a fourth that targets Dynamics CRM 2016 (V8). While the code is virtually identical today due to the overlap in programming models, we do see the potential for them to diverge as new capabilities and API's for Dynamics CRM are released.  This library contains the following key components:

* [XrmServiceUriFactory Class](XrmServiceUriFactory-Class) Provide a common pattern for constructing Uri instances that target Dyanmics CRM endpoints
* [XrmServiceManager Classes](XrmServiceManager-Classes) Provide a simplified and efficient connection pattern to Dynamics CRM endpoints regardless of the authentication method.
* [ParallelServiceProxy Classes](ParallelServiceProxy-Classes) Provide abstracted parallelized processing of Dynamics CRM service operations.  
* [Query Extensions](Query-Extensions) Methods for retrieving all pages and performing operations on each page of results.
* [Security Extensions](Security-Extensions) Methods for encrypting/decrypting values and converting between String and SecurityString types.
* [BatchRequestExtensions](BatchRequestExtensions)  Methods for converting request collections into batched ExecuteMultipleRequest collections for parallel batch processing. Specify batch size as method argument.
#### Sample Code

* Examples
	* [XrmServiceManager](https://pfexrmcore.codeplex.com/SourceControl/latest#Samples/Microsoft.Pfe.Xrm.Core.Samples/XrmServiceManagerSamples.cs)
	* [Parallel Common Request](https://pfexrmcore.codeplex.com/SourceControl/latest#Samples/Microsoft.Pfe.Xrm.Core.Samples/ParallelRequestSamples.cs)
	* [Parallel RetrieveMultiple Requests](https://pfexrmcore.codeplex.com/SourceControl/latest#Samples/Microsoft.Pfe.Xrm.Core.Samples/ParallelRetrieveMultipleSamples.cs)
	* [Parallel Execute Requests](https://pfexrmcore.codeplex.com/SourceControl/latest#Samples/Microsoft.Pfe.Xrm.Core.Samples/ParallelExecuteSamples.cs)
	* [Parallel Discovery Requests](https://pfexrmcore.codeplex.com/SourceControl/latest#Samples/Microsoft.Pfe.Xrm.Core.Samples/ParallelDiscoverySamples.cs)
* Solutions
	* [PfeXrmCoreWalkthroughApp](PfeXrmCoreWalkthroughApp)