# The Event Hub service

This chapter provides a brief overview of the capabilities of the Event Hub service and explains some of the terminology associated with the Event Hub service before describing the role played by the Event Hub service in the RI and some of the key configuration options we you configure an Event Hub service for use with the RI.

## Overview of the Azure Service Bus Event Hub service

Microsoft Azure Service Bus Event Hubs is an event ingestor service that provides event and telemetry ingress to the cloud at massive scale, with low latency and high reliability. It sits between event producers and event consumers to decouple the devices generating the event stream from the downstream services that consume the event stream. 

{{Diagram from http://msdn.microsoft.com/en-us/library/dn836025.aspx}}

An Event Hub service can ingest events from multiple event producers and deliver them to multiple event consumers. In this way it is similar to a Service Bus *topic* with publish/subscribe semantics, but the Event Hub service is designed for high throughput scenarios which it supports at the expense of some of the capabilities of topics. This chapter does provide an in depth discussion of Event Hub, but does provide some definitions of Event Hub terminology that will help you to understand the RI.

### Partitions


### Partition Keys


### Consumer groups

### Stream offsets

### Checkpointing



## Context


## Solution