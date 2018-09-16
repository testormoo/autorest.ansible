FROM dockiot/autorest-ansible
MAINTAINER zikalino

RUN cd /autorest.ansible && git stash && git pull
RUN cd autorest.ansible && npm install && npm run build
