#!/bin/bash

# Wait for redis-master to become consistently resolvable
success_counter=0
while [[ $success_counter -lt 5 ]]; do
  if ping -c1 redis-master &>/dev/null; then
    ((success_counter++))
    echo "Successfully resolved redis-master ${success_counter} times."
    sleep 1
  else
    success_counter=0
    echo "$(date) - waiting for redis-master to be resolvable..."
    sleep 1
  fi
done

echo "$(date) - redis-master resolved. Starting Sentinel."

# Get the IP address of redis-master
REDIS_MASTER_IP=$(getent hosts redis-master | awk '{ print $1 }')

# Replace the redis-master hostname with its IP address in sentinel.conf
sed -i "s/redis-master/$REDIS_MASTER_IP/g" /etc/redis/sentinel.conf

# Start Redis Sentinel
redis-sentinel /etc/redis/sentinel.conf
