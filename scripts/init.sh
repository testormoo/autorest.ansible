cd ~
git clone --recursive https://github.com/VSChina/ansible.git

git clone https://github.com/Azure/preview-modules.git
cd ~/preview-modules
git checkout hatchery-pr-branch 2>/dev/null || git checkout -b hatchery-pr-branch;


sudo apt-get update
sudo apt-get install python-pip
sudo pip install -r ~/ansible/requirements.txt
sudo pip install -r ~/ansible/packaging/requirements/requirements-azure.txt 
sudo pip install pycodestyle pylint voluptuous pytest
sudo pip install azure-mgmt-rdbms
cd ~/ansible
source ./hacking/env-setup
