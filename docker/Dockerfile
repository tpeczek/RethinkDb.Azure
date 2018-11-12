FROM ubuntu:xenial
RUN \
  apt-get update && \
  apt-get install -y wget && \
  rm -rf /var/lib/apt/lists/*
RUN \
  echo "deb http://download.rethinkdb.com/apt xenial main" > /etc/apt/sources.list.d/rethinkdb.list && \
  wget -O- http://download.rethinkdb.com/apt/pubkey.gpg | apt-key add - && \
  apt-get update && \
  apt-get install -y rethinkdb && \
  rm -rf /var/lib/apt/lists/*
VOLUME ["/data"]
WORKDIR /data
CMD ["rethinkdb", "--bind", "all"]
# WEB UI
EXPOSE 8080
# PROCESS
EXPOSE 28015
# CLUSTER
EXPOSE 29015