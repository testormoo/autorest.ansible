while IFS='' read -r line || [[ -n "$line" ]]; do
    echo "--- Updating Module: $line"
    ./ansible-pr.sh $line
done < ~/ansible-hatchery/current-prs.txt
