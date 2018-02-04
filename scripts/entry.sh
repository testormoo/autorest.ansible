
# delete all old temporary files
rm -rf /ansible-hatchery-tmp/*

cd /autorest.ansible
git pull
cd /autorest.ansible/scripts
chmod 777 generate*
./generate-$1.sh

cp /ansible-hatchery-tmp/python/all/modules/* /ansible-hatchery/library/
cp /ansible-hatchery-tmp/python/all/tests/* /ansible-hatchery/tests/integration/targets
cp /ansible-hatchery-tmp/all/modules/* /ansible-hatchery/library/
cp /ansible-hatchery-tmp/all/tests/* /ansible-hatchery/tests/integration/targets
