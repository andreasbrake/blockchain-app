FROM node:8 as builder

ADD . app

WORKDIR /app

RUN npm install
RUN npm install -g solc

WORKDIR /app/Contracts

RUN solcjs --bin --abi -o out Messenger.sol

FROM node:8-alpine

WORKDIR /App

COPY --from=builder /app/Contracts/out Contracts/out
COPY --from=builder /app/node_modules node_modules
COPY --from=builder /app/ipfs_connect.js ./
COPY --from=builder /app/mq_connect.js ./
COPY --from=builder /app/methods.js ./
COPY --from=builder /app/contract.js ./
COPY --from=builder /app/index.js ./

ENTRYPOINT [ "node", "index.js" ]
