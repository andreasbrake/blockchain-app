FROM node:8 as builder

ADD . app

WORKDIR /app

RUN npm install
RUN npm install -g solc

WORKDIR /app/Contracts

RUN solcjs --bin --abi -o out Vault.sol

FROM node:8-alpine

WORKDIR /App

COPY --from=builder /app/Contracts/out Contracts/out
COPY --from=builder /app/node_modules node_modules
COPY --from=builder /app/deployer.js ./
COPY --from=builder /app/index.js ./

ENTRYPOINT [ "node", "index.js" ]
