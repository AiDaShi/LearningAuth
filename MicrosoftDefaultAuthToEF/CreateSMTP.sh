#/bin/bash

name="smtp"
docker rm -f $name
docker run --restart=always -d \
    -e "RELAY_NETWORKS=:0.0.0.0/0" \
    --name $name \
    -p 25:25 \
    namshi/smtp