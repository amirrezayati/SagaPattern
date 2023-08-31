# SagaPattern
Distributed transaction using SAGA pattern, RabbitMQ and asp.net core

Distributed transaction is one that spans multiple databases across the network while preserving ACID properties.
 It is very important in Microservices because of its distributed nature. To manage data consistency we may use SAGA design pattern.
 In this article, I will show you distributed transaction using SAGA pattern, RabbitMQ and asp.net core.

Saga Pattern

A saga is a sequence of local transactions. Each local transaction updates the local database and publishes a messages or event to message broker for updating next corresponding database.
 If next database transaction fails, a series of transactions will occur to undo the changes.

Saga is implemented in two ways -

Choreography
Orchestration

Choreography In choreography, participants exchange events without a centralized control.
Orchestration In orchestration, participants exchange events with a centralized control.

(((Implementation of Choregraphy Pattern)))

Tools and technology used

Visual studio 2019
SQLite
ASP.NET Core
RabbitMQ
Step 1: Run docker container for RabbitMQ

Run the following command to run rabbitmq in a container
docker run -d --hostname host-rabbit --name ecommerce-rabbit -p 15672:15672 -p 5672:5672 rabbitmq:3-management
