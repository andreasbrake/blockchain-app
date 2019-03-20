##Redis##
1. `docker pull redis`
2. `docker run --name app-cache -d redis`
`docker run --name <name> -d redis`

##RabbitMQ##
1. `docker pull rabbitmq`
2. `docker run -d --publish 5672:5672 --hostname app-rabbit-1 --name app-rabbit rabbitmq:3`
`docker run -d --publish <host port>:<container port> --hostname <node name> --name <container name> rabbitmq:3`


##Networking on windows##
`route /P add 172.17.0.0 MASK 255.255.0.0 10.0.75.2`
`route /P add <container subdomain> MASK 255.255.0.0 <docker host ip>`