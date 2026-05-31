<p align="center">
  <img src="https://signals.emitknowledge.com/assets/temp-logo.png" alt="Signals" />
</p>

# Signals

**Signals** is a .NET 10 framework focused on improving software quality and development productivity by providing reusable tools, application aspects, and development processes.

Signals helps teams standardize common infrastructure concerns such as dependency injection, configuration, logging, auditing, caching, localization, storage, communication, authentication, scheduling, error handling, and benchmarking.

## Overview

Project overview: https://signals.emitknowledge.com

Documentation: [Signals Wiki](https://github.com/EmitKnowledge/Signals/wiki)

Boilerplate project: [Signals Boilerplate](https://github.com/EmitKnowledge/Signals-Boilerplate)

## Getting Started

The fastest way to start using Signals is through the preconfigured boilerplate project:

https://github.com/EmitKnowledge/Signals-Boilerplate

The boilerplate provides a ready-to-use project structure with Signals already configured.

## High-Level Aspects

Most applications require a common set of infrastructure capabilities during the initial project setup phase. Signals provides standardized, extensible implementations for these concerns so teams can focus more on business logic and less on repetitive setup work.

## Dependency Injection

Signals allows you to use your dependency injection framework of choice while adding support for attribute-based injection.

Out of the box, Signals provides integrations with:

* Autofac
* Microsoft .NET Dependency Injection
* Simple Injector

Additional dependency injection frameworks can be integrated by following the Signals extension conventions.

## Configuration

Signals treats configuration as strongly typed objects, independent of the underlying storage mechanism.

The configuration aspect allows you to:

* Define configuration models as objects
* Maintain different configurations per environment
* Select the active environment
* Map configuration values directly to strongly typed objects

Out of the box, Signals provides configuration providers for:

* File-based configuration
* Microsoft SQL Server

## Logging

Signals provides a standardized logging abstraction that can be integrated with existing logging libraries.

Out of the box, Signals integrates with:

* NLog
* Serilog

The logging aspect is extensible, allowing additional logging providers to be added when needed.

## Auditing

Enterprise applications often need to track who performed an action, what was changed, and when it happened.

Signals supports auditing through integrations with:

* Audit.NET
* Microsoft SQL Server

Like other Signals aspects, auditing can be extended with additional providers or custom implementations.

## Caching

Signals provides caching support for storing frequently accessed data and reducing repeated computation or data access.

Out of the box, Signals supports:

* In-memory caching
* Redis

Other caching systems, such as Redis, can be integrated by extending the caching aspect.

## Localization

Signals provides localization support for applications that need to support multiple languages or regional formats.

Out of the box, Signals supports localization through:

* File-based resources
* Microsoft SQL Server
* In-memory localization

## Storage

Signals provides file storage capabilities, including file upload and encryption.

Out of the box, Signals supports storage through:

* File system
* Azure Blob Storage
* Microsoft SQL Server

Additional storage providers can be added by extending the storage aspect.

## Communication Channels

Signals supports distributed processes across different application boundaries.

For example, a process can start in a web application and complete in a background service. This is enabled through communication channels.

Out of the box, Signals supports:

* Azure Event Grid
* MSMQ
* Microsoft SQL Server
* Azure Service Bus

## Authentication and Authorization

Signals provides support for configuring authentication and authorization in both ASP.NET MVC and ASP.NET Core applications.

The authentication and authorization aspect supports:

* Authentication management
* Authorization management
* Permission management
* Attribute-based access control
* Direct usage of authentication, authorization, and permission managers

## Scheduled Tasks

Signals supports task scheduling with multiple recurrence options, including:

* Daily
* Weekly
* Monthly
* Weekend-based
* Workday-based
* Pattern-based
* Time-part-based recurrence

Out of the box, scheduled tasks are implemented through:

* FluentScheduler
* Hangfire

Additional scheduling libraries can be integrated through extension.

## Error Handling

Signals provides centralized error handling and retry policy support to help applications handle failures consistently and prevent internal implementation details from leaking outside system boundaries.

This aspect is built around:

* Centralized exception handling
* Retry policies
* Error abstraction
* Meaningful error extraction

Signals supports this through integration with Polly.NET.

## Benchmarking

Signals includes benchmarking capabilities for tracking and improving long-running or performance-sensitive processes.

The benchmarking aspect allows you to monitor:

* Processes
* Execution chains
* Checkpoints
* Bottlenecks

This helps teams understand system behavior and identify areas that need performance improvements.
