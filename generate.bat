docker kill autorest-ansible
docker rm autorest-ansible
docker pull dockiot/autorest-ansible-inc
docker run --name autorest-ansible -v c:/dev-current/ansible-hatchery:/ansible-hatchery -v c:/dev-current/tmp:/ansible-hatchery-tmp dockiot/autorest-ansible-inc %1
