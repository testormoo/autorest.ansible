FROM dockiot/autorest-ansible
MAINTAINER zikalino


RUN git config --global user.email "zikalino@microsoft.com"
RUN git config --global user.name "zikalino"
RUN  rm -rf azure-rest-api-specs && git clone https://github.com/Azure/azure-rest-api-specs.git
RUN cd /autorest.ansible && git stash && git pull
RUN cd autorest.ansible && npm install && npm run build
