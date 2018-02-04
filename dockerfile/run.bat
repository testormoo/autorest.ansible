docker kill autorest-ansible
docker rm autorest-ansible
docker run -t -i --name autorest-ansible -v c:/dev/ansible-hatchery:/ansible-hatchery dockiot/autorest-ansible
