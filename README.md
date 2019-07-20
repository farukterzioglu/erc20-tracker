## A Minimalist ERC-20 Tracker  

```
docker build -t erc20-tracker .
```

```
$ docker run -e CONTRACTADDRESS=0xdac17f958d2ee523a2206206994597c13d831ec7 erc20-tracker 

$ docker run \
    -e CONTRACTADDRESS=0xdac17f958d2ee523a2206206994597c13d831ec7 \
    -e ADDRESS=0xe7483f9dca3b823e87a6e0d6c3c8ee15aa651799 \
    erc20-tracker

$ docker run \
    -e CONTRACTADDRESS=0xdac17f958d2ee523a2206206994597c13d831ec7 \
    -e ADDRESS=0xe7483f9dca3b823e87a6e0d6c3c8ee15aa651799 \
    -e ZEROMQURL=127.0.0.1:5556 \
    erc20-tracker
```