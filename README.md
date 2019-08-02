## A Minimalist ERC-20 Tracker  

`docker run -d --hostname my-rabbit --name some-rabbit -p 4369:4369 -p 5671:5671 -p 5672:5672 -p 15672:15672 rabbitmq:3-management`
Navigate to 'http://localhost:15672/#/'  

Samples;   
```
docker build -t erc20-tracker .
```

```
$ docker run -e CONTRACTADDRESS=0xdac17f958d2ee523a2206206994597c13d831ec7 erc20-tracker 

$ docker run \
    -e CONTRACTADDRESS:0=0xdac17f958d2ee523a2206206994597c13d831ec7 \
    -e TRACKEDADDRESSES:0=0xde769dda17eD7d2178a5646D279382d0CaEC8079 \
    erc20-tracker

$ docker run \
    -e NodeUrl=http://localhost:7545 \
    -e CONTRACTADDRESS:0=0xdac17f958d2ee523a2206206994597c13d831ec7 \
    -e TRACKEDADDRESSES:0=0xde769dda17eD7d2178a5646D279382d0CaEC8079 \
    -e TRACKEDADDRESSES:1=0xD5eaddD9462D2138389d3Ca281c3a10c952267E5 \
    -e RABBITMQHOSTNAME=localhost \
    -e RABBITMQUSERNAME=guest \
    -e RABBITMQPASSWORD=guest \
    -e RABBITMQEXCHANGENAME=Erc20Transactions \
    erc20-tracker
```