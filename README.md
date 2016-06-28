#Data Pipeline Guidance

[Microsoft patterns & practices](http://aka.ms/mspnp)

:memo: 

> An updated version of the [Cold Storage Processor](https://github.com/mspnp/data-pipeline/tree/master/src/Implementation/ColdStorage) and the [Simulator](https://github.com/mspnp/data-pipeline/tree/master/src/Implementation/Simulator) are available as part of our [IoT Journey project](https://github.com/mspnp/iot-journey). The updated versions are:
* [Long-term Storage event processor](https://github.com/mspnp/iot-journey/tree/master/src/LongTermStorage/DotnetEventProcessor)
* [Scenario Simulator](https://github.com/mspnp/iot-journey/tree/master/src/Simulator/ScenarioSimulator)


[![Build status](https://ci.appveyor.com/api/projects/status/vffa0di2sdg8nqkg/branch/master?svg=true)](https://ci.appveyor.com/project/mspnp/data-pipeline/branch/master)

This reference implementation is a work-in-progress project. It is meant to demonstrate proven practices regarding the high-scale, high-volume ingestion of data in a typical event processing system.

The project makes heavy use of [Microsoft Azure Event Hubs](http://azure.microsoft.com/en-us/services/event-hubs/), a cloud-scale telemetry ingestion service. Familiarity with the [general concepts underlying Event Hubs](http://msdn.microsoft.com/en-us/library/azure/dn789972.aspx) is very useful for understanding the source in this reference implementation.

##Overview

The two primary concerns of this project are:

* Facilitating cold storage of data for later analytics. That is, translating the _chatty_ stream of events into _chunky_ blobs.

* Dispatching incoming events to specific handlers. That is, examining an incoming event and passing it along to an appropriate handler function. The emphasis of our dispatcher solution is on speed and overal throughput.

## Next Steps

* [Scenarios & Requirements](/docs/Introduction.md)
* [Architecture Overview](/docs/ArchitectureOverview.md)
* [Getting Started](/docs/GettingStarted.md)
* [Contribute](CONTRIBUTING.md) :smile:

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
