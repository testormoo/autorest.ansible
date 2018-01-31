cd ~/ansible-hatchery
git checkout master
git pull

cd ~/azure_preview_modules
git checkout hatchery-pr-branch 2>/dev/null || git checkout -b hatchery-pr-branch;
git pull

while IFS='' read -r line || [[ -n "$line" ]]; do
    echo "--- Updating Module: $line"
    cp -rf ~/ansible-hatchery/all/modules/$line.py ~/azure_preview_modules/library
    cp -rf ~/ansible-hatchery/all/tests/$line ~/azure_preview_modules/tests/integration/targets
done < ~/ansible-hatchery/current-role.txt

git add -A
git commit -m "updated modules"
git push https://$GIT_USER:$GIT_PASSWORD@github.com/Azure/azure_preview_modules.git refs/heads/hatchery-pr-branch:refs/heads/hatchery-pr-branch
