cd /ansible-hatchery
git pull
cd /autorest.ansible
git pull
npm run build
cd /ansible-hatchery
chmod 777 generate.sh
./generate.sh
