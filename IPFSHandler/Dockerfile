FROM node:8 as builder

ADD . app

WORKDIR /app

RUN npm install

FROM node:8-alpine

WORKDIR /App

COPY --from=builder /app/handlers ./handlers
COPY --from=builder /app/node_modules ./node_modules
COPY --from=builder /app/index.js ./

ENTRYPOINT [ "node", "index.js" ]
