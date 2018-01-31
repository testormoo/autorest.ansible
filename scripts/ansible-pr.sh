if [ ! -d "~/$1" ]; then
    echo "CLONING ansible repo"
    git clone --recursive https://github.com/VSChina/ansible.git ~/$1
    cd ~/$1
    git checkout $1 2>/dev/null || git checkout -b $1;
fi

cd ~/$1
git pull
cp ~/ansible-hatchery/all/modules/$1.py ~/$1/lib/ansible/modules/cloud/azure/$1.py
cp -v -r ~/ansible-hatchery/all/tests/$1 ~/$1/test/integration/targets
git add -A
git commit -m "updates to $1"
git push https://$GIT_USER:$GIT_PASSWORD@github.com/VSChina/ansible.git refs/heads/$1:refs/heads/$1
echo curl -v -u $GIT_USER:$GIT_PASSWORD -H "Content-Type:application/json" -X POST https://api.github.com/repos/ansible/ansible/pulls -d '{"title":"[new module] '$1'", "body": "##### SUMMARY\nAdding support for '$1'\n\n##### ISSUE TYPE\n - New Module Pull Request\n\n##### COMPONENT NAME\n\n'$1'\n\n##### ANSIBLE VERSION\n\n 2.4\n\n##### ADDITIONAL INFORMATION\n\n", "head": "VSChina:'$1'", "base": "devel"}' 
curl -v -u $GIT_USER:$GIT_PASSWORD -H "Content-Type:application/json" -X POST https://api.github.com/repos/ansible/ansible/pulls -d '{"title":"[new module] '$1'", "body": "##### SUMMARY\nAdding support for '$1'\n\n##### ISSUE TYPE\n - New Module Pull Request\n\n##### COMPONENT NAME\n\n'$1'\n\n##### ANSIBLE VERSION\n\n 2.4\n\n##### ADDITIONAL INFORMATION\n\n", "head": "VSChina:'$1'", "base": "devel"}'
