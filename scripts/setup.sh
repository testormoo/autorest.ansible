curl -sL https://deb.nodesource.com/setup_8.x | sudo -E bash -
sudo apt-get install -y nodejs

sudo npm -g install autorest@2.0.4215

# installing dotnet
curl https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.gpg
sudo mv microsoft.gpg /etc/apt/trusted.gpg.d/microsoft.gpg
sudo sh -c 'echo "deb [arch=amd64] https://packages.microsoft.com/repos/microsoft-ubuntu-xenial-prod xenial main" > /etc/apt/sources.list.d/dotnetdev.list'
sudo apt-get update
sudo apt-get install -y apt-transport-https
sudo apt-get install -y dotnet-sdk-2.1.4

cd ~
git clone https://github.com/zikalino/autorest.ansible.git
git clone https://github.com/zikalino/azure-rest-api-specs.git
cd azure-rest-api-specs/
git checkout ansible-generator-branch
cd ~
cd autorest.ansible
sudo npm install



sudo npm run build
cd ~
