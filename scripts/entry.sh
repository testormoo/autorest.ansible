
# delete all old temporary files
rm -rf /ansible-hatchery-tmp/*
mkdir /ansible-hatchery-tmpx

cd /autorest.ansible
git stash
git pull
cd /autorest.ansible/scripts
chmod 777 generate*
./generate-$1.sh

echo "----------- template"
ls -al /ansible-hatchery-tmpx/template
echo "----------- python/all/modules"
ls -al /ansible-hatchery-tmp/python/all/modules
echo "----------- python/modules"
ls -al /ansible-hatchery-tmp/all/modules

cd /
find -name azure_rm*.py

cp -R /ansible-hatchery-tmpx/all/modules/* /ansible-hatchery/library/
cp -R /ansible-hatchery-tmpx/all/tests/* /ansible-hatchery/tests/integration/targets
cp -R /ansible-hatchery-tmpx/template/* /ansible-hatchery/__template
cp -R /ansible-hatchery-tmpx/examples/* /ansible-hatchery/examples
