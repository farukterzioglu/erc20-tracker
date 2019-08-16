## A Minimalist ERC-20 Tracker  

Scans Ethererum blockchain for Erc20 transactions and sends them to RabbitMq queue for further processing.  
Filtered with specified Erc20 contracts and receiver addresses.  

### Build Docker image     
```
docker build -t erc20-tracker .
```

### Run on full node, listen Tether for txs sent to 0xde769dda17eD7d2178a5646D279382d0CaEC8079 
```
$ docker run -e NODEURL=http://52.208.46.161:8546 -e CONTRACTADDRESS:0=0xdac17f958d2ee523a2206206994597c13d831ec7 -e TRACKEDADDRESSES:0=0xde769dda17eD7d2178a5646D279382d0CaEC8079 -e LASTPROCESSEDBLOCK=8278800 erc20-tracker
```

### Start a RabbitMq instance with Docker
`docker run -d --hostname my-rabbit --name some-rabbit -p 4369:4369 -p 5671:5671 -p 5672:5672 -p 15672:15672 rabbitmq:3-management`
Navigate to 'http://localhost:15672/#/'  



```
$ docker run -e CONTRACTADDRESS=0xdac17f958d2ee523a2206206994597c13d831ec7 erc20-tracker 


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