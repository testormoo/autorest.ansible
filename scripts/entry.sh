
# delete all old temporary files
rm -rf /ansible-hatchery-tmp/*

cd /autorest.ansible
git stash
git pull
cd /autorest.ansible/scripts
chmod 777 generate*
./generate-$1.sh

echo "----------- template"
ls -al /ansible-hatchery-tmp/template
echo "----------- python/all/modules"
ls -al /ansible-hatchery-tmp/python/all/modules
echo "----------- python/modules"
ls -al /ansible-hatchery-tmp/all/modules

cd /ansible-hatchery-tmp
find .

cp -R /ansible-hatchery-tmp/python/all/modules/* /ansible-hatchery/library/
cp -R /ansible-hatchery-tmp/python/all/tests/* /ansible-hatchery/tests/integration/targets
cp -R /ansible-hatchery-tmp/all/modules/* /ansible-hatchery/library/
cp -R /ansible-hatchery-tmp/all/tests/* /ansible-hatchery/tests/integration/targets
cp -R /ansible-hatchery-tmp/template/* /ansible-hatchery/__template
cp -R /ansible-hatchery-tmp/python/template/* /ansible-hatchery/__template
