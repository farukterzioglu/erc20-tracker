## A Minimalist ERC-20 Tracker  

Scans Ethererum blockchain for Erc20 transactions and sends them to RabbitMq queue for further processing.  
Filtered with specified Erc20 contracts and receiver addresses.  

Build Docker image  
`
docker build -t erc20-tracker .
`  

Run on full node, listen Tether for all txs  
(Tether contract address used as sample: [0xdAC17F958D2ee523a2206206994597C13D831ec7](https://etherscan.io/address/0xdac17f958d2ee523a2206206994597c13d831ec7)  
  
`
$ docker run -e NODEURL=http://52.208.46.161:8546 -e CONTRACTADDRESSES:0=0xdAC17F958D2ee523a2206206994597C13D831ec7 -e LASTPROCESSEDBLOCK=8363860 erc20-tracker
`

Run on full node, listen Tether for txs sent to the address: 0xde769dda17eD7d2178a5646D279382d0CaEC8079  
`
$ docker run -e NODEURL=http://52.208.46.161:8546 -e CONTRACTADDRESSES:0=0xdac17f958d2ee523a2206206994597c13d831ec7 -e TRACKEDADDRESSES:0=0xde769dda17eD7d2178a5646D279382d0CaEC8079 -e LASTPROCESSEDBLOCK=8363860 erc20-tracker
`

Run on full node, listen Tether for txs sent to first 100 addresses of a seed    
`$ SEED="carry fix final shield cruel learn pave during habit adapt alter habit"`  
`    
$ docker run -e NODEURL=http://52.208.46.161:8546 -e CONTRACTADDRESSES:0=0xdAC17F958D2ee523a2206206994597C13D831ec7 -e SEED=$SEED -e HDADDRESSCOUNT=100 -e LASTPROCESSEDBLOCK=8363860 erc20-tracker
`

All options;  
```
$ docker run \
    -e NodeUrl=http://localhost:7545 \
    -e CONTRACTADDRESSES:0=0xdac17f958d2ee523a2206206994597c13d831ec7 \
    -e CONTRACTADDRESSES:1=0xB8c77482e45F1F44dE1745F52C74426C631bDD52 \
    -e TRACKEDADDRESSES:0=0xde769dda17eD7d2178a5646D279382d0CaEC8079 \
    -e SEED="carry fix final shield cruel learn pave during habit adapt alter habit" \
    -e HDADDRESSCOUNT=100 \
    -e RABBITMQHOSTNAME=localhost \
    -e RABBITMQUSERNAME=guest \
    -e RABBITMQPASSWORD=guest \
    -e RABBITMQEXCHANGENAME=Erc20Transactions \
    erc20-tracker
```

Start a RabbitMq instance with Docker  
`docker run -d --hostname my-rabbit --name some-rabbit -p 4369:4369 -p 5671:5671 -p 5672:5672 -p 15672:15672 rabbitmq:3-management`    
Navigate to 'http://localhost:15672/#/'  
