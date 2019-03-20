# Blockchain App

This app is a proof of concept for a scalable data API based on a Blockchain and an IPFS server.

This repository contains a collection of services that can be connected using Rabbit-MQ to transmit requests and data throughout the network.

The individual services are stateless and can be scaled horizontally as desired and Rabbit-MQ can be used to balance requests between them.

Data is stored on IPFS and a directory structure is saved to the Blockchain to ensure reliable and distributed retrieval of data.