cd ~/ansible-hatchery
git pull

cd ~/ansible-hatchery/python/lib/ansible/modules/cloud/azure

for f in $1*
do
  echo "Processing $f"
  name=$(echo $f | cut -f 1 -d '.')
  cp -v ~/azure_preview_modules/library/$name.py ~/ansible/lib/ansible/modules/cloud/azure/$name.py
  cp -v -r ~/azure_preview_modules/tests/integration/targets/$name ~/ansible/test/integration/targets 

  ansible-test sanity -v --local --python 2.7 $name

done

cd ~
