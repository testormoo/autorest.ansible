echo "----------------------------- Generating $1"
rm -rf /ansible-hatchery-tmp/*
cd /azure-rest-api-specs/specification/$1/resource-manager
autorest --output-folder=/ansible-hatchery-tmp/ --python-sdks-folder=/ansible-hatchery-tmp/ --use=/autorest.ansible --python --tag=$2
cp -r /ansible-hatchery-tmp/$3/* /ansible-hatchery-tmpx/
echo "---"
cd /ansible-hatchery-tmp
find -name azure_rm*.py
find -name azure_rm*.yml
