FROM dockiot/autorest-ansible
MAINTAINER zikalino


RUN git config --global user.email "zikalino@microsoft.com"
RUN git config --global user.name "zikalino"
RUN cd /autorest.ansible && git stash && git pull
RUN cd autorest.ansible && npm install && npm run build
