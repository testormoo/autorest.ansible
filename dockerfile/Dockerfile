FROM ubuntu
MAINTAINER zikalino

RUN apt-get update
RUN apt-get install -y git curl

RUN curl -sL https://deb.nodesource.com/setup_8.x | bash -
RUN apt-get install -y nodejs

RUN npm -g install autorest@2.0.4215

RUN curl https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.gpg
RUN mv microsoft.gpg /etc/apt/trusted.gpg.d/microsoft.gpg
RUN sh -c 'echo "deb [arch=amd64] https://packages.microsoft.com/repos/microsoft-ubuntu-xenial-prod xenial main" > /etc/apt/sources.list.d/dotnetdev.list'
RUN apt-get update
RUN apt-get install -y apt-transport-https
RUN apt-get install -y dotnet-sdk-2.1.4

ENV invalidatecache xxx
RUN git clone https://github.com/zikalino/autorest.ansible.git
RUN git clone https://github.com/zikalino/azure-rest-api-specs.git
RUN cd azure-rest-api-specs && git checkout ansible-generator-branch
RUN cd autorest.ansible && npm install && npm run build
RUN git clone https://github.com/zikalino/ansible-hatchery.git
RUN echo "chmod 777 /ansible-hatchery/generate* && cd /ansible-hatchery && ./generate-with-pull.sh" > /run.sh
RUN chmod 777 /run.sh

ENV invalidatecache eee
RUN cd /autorest.ansible && git pull
RUN chmod 777 /autorest.ansible/scripts/entry.sh

# generate sth once, first time autorest will fail
RUN if autorest --output-folder=/tmp --use=/autorest.ansible --python --tag=package-2017-04-preview; then echo "success"; else echo "failed"; fi
ENTRYPOINT [ "bash", "/autorest.ansible/scripts/entry.sh" ]
