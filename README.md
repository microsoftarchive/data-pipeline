#Data Pipeline Guidance

[Microsoft patterns & practices](http://aka.ms/mspnp)

[![Build status](https://ci.appveyor.com/api/projects/status/vffa0di2sdg8nqkg/branch/master?svg=true)](https://ci.appveyor.com/project/mspnp/data-pipeline/branch/master)

[![Gitter](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/mspnp/data-pipeline?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

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
