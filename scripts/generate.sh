
rm -rf /ansible-hatchery/prs
rm -rf /ansible-hatchery/role
rm -rf /ansible-hatchery/all
rm -rf /ansible-hatchery/template

bash /ansible-hatchery/generate-sql.sh &
bash /ansible-hatchery/generate-mysql.sh &

#cd /azure-rest-api-specs/specification/sql/resource-manager && autorest --output-folder=/ansible-hatchery --use=/autorest.ansible --python --tag=package-2017-03-preview &
#cd /azure-rest-api-specs/specification/mysql/resource-manager && autorest --output-folder=/ansible-hatchery/ --use=/autorest.ansible --python --tag=package-2017-04-preview &
#cd /azure-rest-api-specs/specification/postgresql/resource-manager && autorest --output-folder=/ansible-hatchery/ --use=/autorest.ansible --python --tag=package-2017-04-preview &
#cd /azure-rest-api-specs/specification/authorization/resource-manager && autorest --output-folder=/ansible-hatchery/ --use=/autorest.ansible --python --tag=package-2015-07 &
#cd /azure-rest-api-specs/specification/web/resource-manager && autorest --output-folder=/ansible-hatchery/ --use=/autorest.ansible --python --tag=package-2016-09 &
#cd /azure-rest-api-specs/specification/network/resource-manager && autorest --output-folder=/ansible-hatchery/ --use=/autorest.ansible --python --tag=package-2017-10 &
#cd /azure-rest-api-specs/specification/containerinstance/resource-manager && autorest --output-folder=/ansible-hatchery/ --use=/autorest.ansible --python --tag=package-2017-10-preview &
#cd /azure-rest-api-specs/specification/containerregistry/resource-manager && autorest --output-folder=/ansible-hatchery/ --use=/autorest.ansible --python --tag=package-2017-10 &
#cd /azure-rest-api-specs/specification/keyvault/resource-manager && autorest --output-folder=/ansible-hatchery/ --use=/autorest.ansible --python --tag=package-2016-10 &

wait

cp /ansible-hatchery/python/prs/* /ansible-hatchery/prs/
cp /ansible-hatchery/python/role/* /ansible-hatchery/role/
cp /ansible-hatchery/python/all/* /ansible-hatchery/all/
cp /ansible-hatchery/python/template/* /ansible-hatchery/template/
rm -rf /ansible-hatchery/python
